using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using System.Threading;

namespace GameDBServer.Core.Executor
{

    public interface ScheduleTask
    {
        void run();
    }

    /// <summary>
    /// Task wrapper class, encapsulates some of the parameters required by the Task implementation
    /// </summary>
    internal class TaskWrapper
    {
        // The task to be performed
        private ScheduleTask currentTask;
        //Start execution time (milliseconds)
        private long startTime = -1;
        //Execution cycle interval (in milliseconds)
        private long periodic = -1;
        //The number of executions
        private int executeCount = 0;

        public bool canExecute = true;

        public TaskWrapper(ScheduleTask task, long delay, long periodic)
        {
            this.currentTask = task;
            this.startTime = TimeUtil.NOW() + delay;
            this.periodic = periodic;
        }

        public ScheduleTask CurrentTask
        {
            get { return currentTask; }
        }

        public long StartTime
        {
            get { return startTime; }
        }

        /// <summary>
        /// Reset the execution time
        /// </summary>
        public void resetStartTime()
        {
            this.startTime = this.startTime + this.periodic;
        }

        public long Periodic
        {
            get { return periodic; }
        }

        public void release()
        {
            currentTask = null;
        }

        public void addExecuteCount()
        {
            executeCount++;
        }

        public int ExecuteCount
        {
            get { return this.executeCount; }
        }
    }

    public interface PeriodicTaskHandle
    {
        void cannel();
        bool isCanneled();
        int getExecuteCount();
        long getPeriodic();
    }

    internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
    {
        private TaskWrapper taskWrapper;
        private ScheduleExecutor executor;
        private bool canneled = false;

        public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, ScheduleExecutor executor)
        {
            this.taskWrapper = taskWrapper;
            this.executor = executor;
        }


        public void cannel()
        {
            if (canneled)
                return;

            if (null != executor && null != taskWrapper)
            {
                executor.removeTask(taskWrapper);
                executor = null;
            }

            canneled = true;
        }
        public bool isCanneled()
        {
            return canneled;
        }
        public int getExecuteCount()
        {
            return taskWrapper.ExecuteCount;
        }
        public long getPeriodic()
        {
            return taskWrapper.Periodic;
        }
    }

    /// <summary>
    /// Task Actuator
    /// </summary>
    internal class Worker
    {
        private ScheduleExecutor executor = null;

        private Thread currentThread = null;

        public Worker(ScheduleExecutor executor)
        {
            this.executor = executor;
        }

        public Thread CurrentThread
        {
            set { this.currentThread = value; }
        }

        private TaskWrapper getCanExecuteTask()
        {
            TaskWrapper TaskWrapper = null;

            int getNum = 0;

            while (null != (TaskWrapper = executor.getTask()))
            {
                //Have not yet begun to perform the time 
                if (TimeUtil.NOW() < TaskWrapper.StartTime)
                {
                    if (TaskWrapper.canExecute)
                        executor.addTask(TaskWrapper);
                    getNum++;
                    //Task queue on line 1000 / thread, if there is no executable task, stop the search
                    if (getNum >= 1000)
                    {
                        break;
                    }
                    continue;
                }

                return TaskWrapper;
            }


            return null;
        }

        public void work()
        {
            TaskWrapper TaskWrapper = null;

            while (true)
            {
                //Retrieves executable tasks
                TaskWrapper = getCanExecuteTask();

                if (null == TaskWrapper)
                {
                    try
                    {
                        Thread.Sleep(5);
                    }
                    catch (ThreadInterruptedException)
                    {
                        continue;
                    }
                    //Continue to retrieve executable tasks
                    continue;
                }

                try
                {
                    if (null == TaskWrapper || null == TaskWrapper.CurrentTask)
                        continue;

                    if (TaskWrapper.canExecute)
                        try
                        {
                            TaskWrapper.CurrentTask.run();
                        }
                        catch (System.Exception ex)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("Asynchronous scheduling task execution error: {0}", ex));
                        }


                    //If it is a cycle of the implementation of the task
                    if (TaskWrapper.Periodic > 0 && TaskWrapper.canExecute)
                    {
                        //Set the next execution time
                        TaskWrapper.resetStartTime();
                        executor.addTask(TaskWrapper);
                        TaskWrapper.addExecuteCount();
                    }

                }
                catch (System.Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "Asynchronous scheduling task execution exception", false);
                }

            }

        }

    }

    public class ScheduleExecutor
    {

        private List<Worker> workerQueue = null;
        private List<Thread> threadQueue = null;
        private LinkedList<TaskWrapper> TaskQueue = null;
        private int maxThreadNum = 0;

        /// <summary>
        /// Set the maximum value of the thread pool
        /// </summary>
        /// <param name="maxThreadNum"></param>
        public ScheduleExecutor(int maxThreadNum)
        {
            this.maxThreadNum = maxThreadNum;
            threadQueue = new List<Thread>();
            workerQueue = new List<Worker>();
            TaskQueue = new LinkedList<TaskWrapper>();
            for (int i = 0; i < maxThreadNum; i++)
            {
                Worker worker = new Worker(this);
                Thread thread = new Thread(new ThreadStart(worker.work));
                worker.CurrentThread = thread;
                workerQueue.Add(worker);
                threadQueue.Add(thread);
            }
        }

        public void start()
        {
            lock (threadQueue)
            {
                foreach (Thread thread in threadQueue)
                {
                    thread.Start();
                }
            }
        }

        public void stop()
        {
            lock (threadQueue)
            {
                foreach (Thread thread in threadQueue)
                {
                    thread.Abort();
                }

                threadQueue.Clear();
            }

            lock (workerQueue)
            {
                workerQueue.Clear();
            }


        }

        /// <summary>
        /// Asynchronously perform tasks
        /// </summary>
        /// <param name="task">task</param>
        /// <returns></returns>
        public bool execute(ScheduleTask task)
        {
            TaskWrapper wrapper = new TaskWrapper(task, -1, -1);

            addTask(wrapper);

            return true;

        }
        /// <summary>
        /// Delay the asynchronous execution of the task
        /// </summary>
        /// <param name="task">task</param>
        /// <param name="delay">Delay start time (ms)</param>
        /// <returns></returns>
        public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
        {
            return scheduleExecute(task, delay, -1);
        }

        /// <summary>
        /// Periodic tasks
        /// </summary>
        /// <param name="task">task</param>
        /// <param name="delay">Delay start time (ms)</param>
        /// <param name="periodic">Interval cycle time (ms)</param>
        /// <returns></returns>
        public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
        {
            TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);

            PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, this);

            addTask(wrapper);

            return handle;

        }

        internal TaskWrapper getTask()
        {
            lock (TaskQueue)
            {
                if (TaskQueue.Count == 0)
                {
                    return null;
                }
                else
                {
                    TaskWrapper currentTask = TaskQueue.First.Value;
                    TaskQueue.RemoveFirst();
                    if (currentTask.canExecute)
                    {
                        return currentTask;
                    }
                    else
                    {
                        return null;
                    }

                }
            }

        }

        internal void addTask(TaskWrapper taskWrapper)
        {
            lock (TaskQueue)
            {
                TaskQueue.AddLast(taskWrapper);
                taskWrapper.canExecute = true;
            }
        }

        internal void removeTask(TaskWrapper taskWrapper)
        {
            lock (TaskQueue)
            {
                TaskQueue.Remove(taskWrapper);
                taskWrapper.canExecute = false;
            }
        }
    }
}
