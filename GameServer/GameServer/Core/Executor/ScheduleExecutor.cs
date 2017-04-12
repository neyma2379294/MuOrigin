using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using System.ComponentModel;
using System.Text.RegularExpressions; // 引用正则的命名空间
using System.Windows;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;

using Server.Tools;
using GameServer.Tools;
using GameServer.Logic;

namespace GameServer.Core.Executor
{
    public class TaskInternalLock
    {
        private int _LockCount;

        public bool TryEnter()
        {
            if (Interlocked.CompareExchange(ref _LockCount, 1, 0) != 0)
            {
                return false;
            }
            return true;
        }

        public void Leave()
        {
            Interlocked.CompareExchange(ref _LockCount, 0, 1);
        }
    }

    public interface ScheduleTask
    {
        TaskInternalLock InternalLock { get; }
        void run();
    }

    /// <summary>
    /// Task包装类,封装Task执行需要的一些参数
    /// </summary>
    internal class TaskWrapper : IComparer<TaskWrapper>
    {
        //要执行的任务
        private ScheduleTask currentTask;
        //开始执行时间（毫秒）
        private long startTime = -1;
        //执行周期间隔时间（毫秒）
        private long periodic = -1;
        //执行次数
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
        /// 重置开始执行时间
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

        public int Compare(TaskWrapper x, TaskWrapper y)
        {
            long ret = x.startTime - y.startTime;
            if (ret == 0)
            {
                return 0;
            }
            else if (ret > 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
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
    /// Task执行器
    /// </summary>
    internal class Worker
    {
        private ScheduleExecutor executor = null;

        private Thread currentThread = null;
        private static int nThreadCount = 0;
        private int nThreadOrder = 0;

        // 对象锁
        private static Object _lock = new Object();

        public Worker(ScheduleExecutor executor)
        {
            this.executor = executor;
        }

        public Thread CurrentThread
        {
            set { this.currentThread = value; }
        }

        private TaskWrapper getCanExecuteTask(long ticks)
        {
            TaskWrapper taskWrapper = executor.GetPreiodictTask(ticks);

            if (null != taskWrapper)
            {
                return taskWrapper;
            }

            int getNum = 0;

            // 任务队列上线200/线程 ChenXiaojun
            int nMaxProcCount = 200;
            int nTaskCount = executor.GetTaskCount();

            // 处理条数不能超过队列中元素个数，避免之前只要队列中有一个元素，重复删除添加1000次 ChenXiaojun
            if (nTaskCount == 0)
            {
                return null;
            }
            else if (nTaskCount < nMaxProcCount)
            {
                nMaxProcCount = nTaskCount;
            }

            while (null != (taskWrapper = executor.getTask()))
            {
                //还没开始执行的时间 
                if (ticks < taskWrapper.StartTime)
                {
                    if (taskWrapper.canExecute)
                        executor.addTask(taskWrapper);
                    getNum++;

                    // 任务队列上线200/线程 ChenXiaojun
                    if (getNum >= nMaxProcCount)
                    {
                        break;
                    }
                    continue;
                }

                return taskWrapper;
            }


            return null;
        }

        public void work()
        {
            lock (_lock)
            {
                nThreadCount++;
                nThreadOrder = nThreadCount;
            }

            TaskWrapper taskWrapper = null;

            int lastTickCount = int.MinValue;
            while (!Program.NeedExitServer)
            {
                //检索可执行的任务
                int tickCount = Environment.TickCount;
                if (tickCount <= lastTickCount + 5)
                {
                    if (lastTickCount <= 0 || tickCount >= 0) //考虑当打到int最大值时的情况
                    {
                        Thread.Sleep(5);
                        continue;
                    }
                }
                lastTickCount = tickCount;

                long ticks = TimeUtil.NOW();
                while (true)
                {
                    try
                    {
                        taskWrapper = getCanExecuteTask(ticks);
                        if (null == taskWrapper || null == taskWrapper.CurrentTask)
                            break;

                        if (taskWrapper.canExecute)
                        {
                            try
                            {
                                taskWrapper.CurrentTask.run();
                            }
                            catch (System.Exception ex)
                            {
                                DataHelper.WriteFormatExceptionLog(ex, "异步调度任务执行异常", false);
                            }
                        
                        }

                        //如果是周期执行的任务
                        if (taskWrapper.Periodic > 0 && taskWrapper.canExecute)
                        {
                            //设置下一次执行的时间
                            taskWrapper.resetStartTime();
                            executor.addTask(taskWrapper);
                            taskWrapper.addExecuteCount();
                        }
                    }
                    catch (System.Exception/* ex*/)
                    {
                        //LogManager.WriteLog(LogTypes.Error, string.Format("异步调度任务执行错误: {0}", ex));
                    }
                }
            }

            System.Console.WriteLine(string.Format("ScheduleTask Worker{0}退出...", nThreadOrder));
        }
    }

    public class ScheduleExecutor
    {
        private List<Worker> workerQueue = null;
        private List<Thread> threadQueue = null;
        private LinkedList<TaskWrapper> TaskQueue = null;
        private List<TaskWrapper> PreiodictTaskList = new List<TaskWrapper>();
        private int maxThreadNum = 0;

        /// <summary>
        /// 设置线程池最大值
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
        /// 异步执行任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <returns></returns>
        public bool execute(ScheduleTask task)
        {
            TaskWrapper wrapper = new TaskWrapper(task, -1, -1);

            addTask(wrapper);

            return true;

        }
        /// <summary>
        /// 延迟异步执行任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延迟开始时间（毫秒）</param>
        /// <returns></returns>
        public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
        {
            return scheduleExecute(task, delay, -1);
        }

        /// <summary>
        /// 周期性任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延迟开始时间（毫秒）</param>
        /// <param name="periodic">间隔周期时间（毫秒）</param>
        /// <returns></returns>
        public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
        {
            TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);

            PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, this);

            addTask(wrapper);

            return handle;

        }

        internal TaskWrapper GetPreiodictTask(long ticks)
        {
            lock (PreiodictTaskList)
            {
                if (PreiodictTaskList.Count == 0)
                {
                    return null;
                }
                else if (PreiodictTaskList[0].StartTime > ticks)
                {
                    return null;
                }

                TaskWrapper taskWrapper = PreiodictTaskList[0];
                PreiodictTaskList.RemoveAt(0);
                return taskWrapper;
            }
        }

        internal TaskWrapper getTask()
        {
            lock (TaskQueue) //使用TryEnter 导致了cpu的占用不稳定，升高，恢复原来的代码
            //if (Monitor.TryEnter(TaskQueue))
            {
                try
                {
                    if (TaskQueue.Count <= 0)
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
                catch (System.Exception/* ex*/)
                {
                }
                //finally
                //{
                //    Monitor.Exit(TaskQueue);
                //}
            }
            //else
            //{
            //    return null;
            //}

            return null;
        }

        internal int GetTaskCount()
        {
            lock (TaskQueue)
            {
                return TaskQueue.Count;
            }
        }

        internal void addTask(TaskWrapper taskWrapper)
        {
            if (taskWrapper.Periodic > 0)
            {
                lock (PreiodictTaskList)
                {
                    ListExt.BinaryInsertAsc(PreiodictTaskList, taskWrapper, taskWrapper);
                }
            }
            else
            {
                lock (TaskQueue)
                {
                    TaskQueue.AddLast(taskWrapper);
                    taskWrapper.canExecute = true;
                }
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

    public class TimeUtil
    {
        /// <summary>
        /// 毫秒
        /// </summary>
        public const int MILLISECOND = 1;
        /// <summary>
        /// 秒
        /// </summary>
        public const int SECOND = 1000 * MILLISECOND;
        /// <summary>
        /// 分钟
        /// </summary>
        public const int MINITE = 60 * SECOND;
        /// <summary>
        /// 小时
        /// </summary>
        public const int HOUR = 60 * MINITE;
        /// <summary>
        /// 天
        /// </summary>
        public const int DAY = 24 * HOUR;

        private static int CurrentTickCount = 0;

        private static long CurrentTicks = DateTime.Now.Ticks / 10000;

        private static DateTime _Now = DateTime.Now;

        /// <summary>
        /// 当前系统时间毫秒数
        /// </summary>
        public static long NOW()
        {
            int tickCount = Environment.TickCount;
            if (tickCount != CurrentTickCount)
            {
                _Now = DateTime.Now;
                CurrentTickCount = tickCount;
                CurrentTicks = _Now.Ticks / 10000;
            }
            return CurrentTicks;
        }

        public static DateTime NowDateTime()
        {
            int tickCount = Environment.TickCount;
            if (tickCount != CurrentTickCount)
            {
                _Now = DateTime.Now;
                CurrentTickCount = tickCount;
                CurrentTicks = _Now.Ticks / 10000;
            }
            return _Now;
        }

        [DllImport("kernel32.dll")]
        extern static bool QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static bool QueryPerformanceFrequency(ref long x);

        private static long _StartCounter = 0;
        private static long _CounterPerSecs = 0;
        private static bool _EnabelPerformaceCounter = false;
        private static long _StartTicks = 0;

        public static long CounterPerSecs
        {
            get
            {
                return _CounterPerSecs;
            }
        }

        public static long Init()
        {
            _EnabelPerformaceCounter = QueryPerformanceFrequency(ref _CounterPerSecs);
            QueryPerformanceCounter(ref _StartCounter);
            _EnabelPerformaceCounter = (_EnabelPerformaceCounter && _CounterPerSecs > 0 && _StartCounter > 0);
            _StartTicks = DateTime.Now.Ticks;
            return _StartTicks;
        }

        public static long NowEx()
        {
            if (GameManager.StatisticsMode == 0)
            {
                return CurrentTicks;
            }
            else if (GameManager.StatisticsMode == 1)
            {
                return CurrentTicks;
            }
            else
            {
                if (_EnabelPerformaceCounter)
                {
                    long counter = 0;
                    QueryPerformanceCounter(ref counter);
                    return counter;
                }
                else
                {
                    return DateTime.Now.Ticks;
                }
            }
        }

        public static double TimeMS(long time, int round = 2)
        {
            if (GameManager.StatisticsMode <= 1)
            {
                return time;
            }
            else
            {
                long timeDiff = TimeDiff(time, 0);
                return Math.Round(timeDiff / 10000.0, round);
            }
        }

        public static long TimeDiff(long timeEnd, long timeStart = 0)
        {
            if (GameManager.StatisticsMode <= 1)
            {
                return timeEnd - timeStart;
            }
            else if (_EnabelPerformaceCounter)
            {
                long counter = timeEnd - timeStart;
                long count1;
                long secs = Math.DivRem(counter, _CounterPerSecs, out count1);
                return secs * 10000000 + count1 * 10000000 / _CounterPerSecs;
            }
            else
            {
                return timeEnd - timeStart;
            }
        }
    }

//     public interface ScheduleTask
//     {
//         void run();
//     }
// 
//     /// <summary>
//     /// Task包装类,封装Task执行需要的一些参数
//     /// </summary>
//     internal class TaskWrapper
//     {
//         //要执行的任务
//         private ScheduleTask currentTask;
//         //开始执行时间（毫秒）
//         private long startTime = -1;
//         //执行周期间隔时间（毫秒）
//         private long periodic = -1;
//         //执行次数
//         private int executeCount = 0;
// 
//         public TaskWrapper(ScheduleTask task, long delay, long periodic)
//         {
//             this.currentTask = task;
//             this.startTime = TimeUtil.NOW() + delay;
//             this.periodic = periodic;
//         }
// 
//         public ScheduleTask CurrentTask
//         {
//             get { return currentTask; }
//         }
// 
//         public long StartTime
//         {
//             get { return startTime; }
//         }
// 
//         /// <summary>
//         /// 重置开始执行时间
//         /// </summary>
//         public void resetStartTime()
//         {
//             this.startTime = this.startTime + this.periodic;
//         }
// 
//         public long Periodic
//         {
//             get { return periodic; }
//         }
// 
//         public void release()
//         {
//             currentTask = null;
//         }
// 
//         public void addExecuteCount()
//         {
//             executeCount++;
//         }
// 
//         public int ExecuteCount
//         {
//             get { return this.executeCount; }
//         }
//     }
// 
//     public interface PeriodicTaskHandle
//     {
//         void cannel();
//         bool isCanneled();
//         int getExecuteCount();
//         long getPeriodic();
//     }
// 
//     internal class PeriodicTaskHandleImpl : PeriodicTaskHandle
//     {
//         private TaskWrapper taskWrapper;
//         private Worker worker;
//         private bool canneled = false;
// 
//         public PeriodicTaskHandleImpl(TaskWrapper taskWrapper, Worker worker)
//         {
//             this.taskWrapper = taskWrapper;
//             this.worker = worker;
//         }
// 
// 
//         public void cannel()
//         {
//             if (canneled)
//                 return;
// 
//             if (null != worker && null != taskWrapper)
//             {
//                 worker.removeTask(taskWrapper);
//                 canneled = true;
//             }
// 
//             taskWrapper.release();
//             taskWrapper = null;
//             worker = null;
// 
//         }
//         public bool isCanneled()
//         {
//             return canneled;
//         }
//         public int getExecuteCount()
//         {
//             return taskWrapper.ExecuteCount;
//         }
//         public long getPeriodic()
//         {
//             return taskWrapper.Periodic;
//         }
//     }
// 
//     /// <summary>
//     /// Task执行器
//     /// </summary>
//     internal class Worker
//     {
//         private ScheduleExecutor executor = null;
// 
//         private Thread currentThread = null;
// 
//         private LinkedListNode<TaskWrapper> currentNode = null;
// 
//         //待执行任务队列
//         private LinkedList<TaskWrapper> TaskQueue = null;
// 
//         public Worker(ScheduleExecutor executor)
//         {
//             this.TaskQueue = new LinkedList<TaskWrapper>();
//             this.executor = executor;
//         }
// 
//         /// <summary>
//         /// 删除任务
//         /// </summary>
//         /// <param name="wrapper"></param>
//         public void removeTask(TaskWrapper wrapper)
//         {
//             lock (TaskQueue)
//             {
//                 TaskQueue.Remove(wrapper);
//             }
//         }
// 
//         public Thread CurrentThread
//         {
//             set { this.currentThread = value; }
//         }
// 
//         public void addTask(TaskWrapper Task)
//         {
//             lock (TaskQueue)
//             {
//                 TaskQueue.AddLast(Task);
//             }
// 
//         }
// 
//         private TaskWrapper getTaskWrapper()
//         {
//             lock (TaskQueue)
//             {
//                 if (null == TaskQueue || TaskQueue.Count == 0)
//                 {
//                     return null;
//                 }
// 
//                 if (null == currentNode || null == currentNode.Next)
//                 {
//                     currentNode = TaskQueue.First;
//                     return currentNode.Value;
//                 }
//                 else
//                 {
//                     currentNode = currentNode.Next;
//                     return currentNode.Value;
//                 }
// 
//             }
//         }
// 
//         private TaskWrapper getCanExecuteTask()
//         {
//             TaskWrapper TaskWrapper = null;
// 
//             int getNum = 0;
// 
//             while (null != (TaskWrapper = getTaskWrapper()))
//             {
//                 //还没开始执行的时间 
//                 if (TimeUtil.NOW() < TaskWrapper.StartTime)
//                 {
//                     getNum++;
//                     //任务队列上线1000/线程,如果当先没有可执行的任务，停止检索
//                     if (getNum >= 1000)
//                     {
//                         break;
//                     }
//                     continue;
//                 }
// 
//                 return TaskWrapper;
//             }
// 
// 
//             return null;
//         }
// 
//         public void work()
//         {
//             TaskWrapper TaskWrapper = null;
// 
//             while (true)
//             {
//                 //检索可执行的任务
//                 TaskWrapper = getCanExecuteTask();
// 
//                 if (null == TaskWrapper)
//                 {
//                     try
//                     {
//                         Thread.Sleep(5);
//                     }
//                     catch (ThreadInterruptedException)
//                     {
//                         continue;
//                     }
//                     //继续检索可执行的任务
//                     continue;
//                 }
// 
// 
// 
//                 try
//                 {
//                     if (null == TaskWrapper || null == TaskWrapper.CurrentTask)
//                         continue;
//                     TaskWrapper.CurrentTask.run();
//                     TaskWrapper.addExecuteCount();
//                     //如果是周期执行的任务
//                     if (TaskWrapper.Periodic > 0)
//                     {
//                         //设置下一次执行的时间
//                         TaskWrapper.resetStartTime();
// 
//                     }
//                     else
//                     {
//                         //删除任务
//                         this.removeTask(TaskWrapper);
//                     }
//                 }
//                 catch (System.Exception ex)
//                 {
//                     LogManager.WriteLog(LogTypes.Error, string.Format("异步调度任务执行错误: {0}", ex));
//                 }
// 
//             }
// 
//         }
// 
//     }
// 
// 
//     public class ScheduleExecutor
//     {
// 
//         private List<Worker> workerQueue = null;
//         private List<Thread> threadQueue = null;
//         private int maxThreadNum = 0;
//         private int currentWorkerIndex = 0;
// 
//         /// <summary>
//         /// 设置线程池最大值
//         /// </summary>
//         /// <param name="maxThreadNum"></param>
//         public ScheduleExecutor(int maxThreadNum)
//         {
//             this.maxThreadNum = maxThreadNum;
//             threadQueue = new List<Thread>();
//             workerQueue = new List<Worker>();
//             for (int i = 0; i < maxThreadNum; i++)
//             {
//                 Worker worker = new Worker(this);
//                 Thread thread = new Thread(new ThreadStart(worker.work));
//                 worker.CurrentThread = thread;
//                 workerQueue.Add(worker);
//                 threadQueue.Add(thread);
//             }
//         }
// 
//         public void start()
//         {
//             lock (threadQueue)
//             {
//                 foreach (Thread thread in threadQueue)
//                 {
//                     thread.Start();
//                 }
//             }
//         }
// 
//         public void stop()
//         {
//             lock (threadQueue)
//             {
//                 foreach (Thread thread in threadQueue)
//                 {
//                     thread.Abort();
//                 }
// 
//                 threadQueue.Clear();
//             }
// 
//             lock (workerQueue)
//             {
//                 workerQueue.Clear();
//             }
// 
// 
//         }
// 
//         /// <summary>
//         /// 异步执行任务
//         /// </summary>
//         /// <param name="task">任务</param>
//         /// <returns></returns>
//         public bool execute(ScheduleTask task)
//         {
//             TaskWrapper wrapper = new TaskWrapper(task, -1, -1);
// 
//             //逐个接受Task 保证负载均衡
//             if (currentWorkerIndex >= workerQueue.Count)
//             {
//                 currentWorkerIndex = 0;
//             }
//             //选择当前任务队列最小的worker，各个线程负载均衡
//             Worker worker = workerQueue[currentWorkerIndex];
//             ;
//             if (null == worker)
//             {
//                 //一般不会出现这种情况，但需要制定处理策略，暂定遗弃任务
//                 worker = workerQueue[0];
//             }
//             worker.addTask(wrapper);
// 
//             currentWorkerIndex++;
// 
//             return true;
//         }
//         /// <summary>
//         /// 延迟异步执行任务
//         /// </summary>
//         /// <param name="task">任务</param>
//         /// <param name="delay">延迟开始时间（毫秒）</param>
//         /// <returns></returns>
//         public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay)
//         {
//             return scheduleExecute(task, delay, -1);
//         }
// 
//         /// <summary>
//         /// 周期性任务
//         /// </summary>
//         /// <param name="task">任务</param>
//         /// <param name="delay">延迟开始时间（毫秒）</param>
//         /// <param name="periodic">间隔周期时间（毫秒）</param>
//         /// <returns></returns>
//         public PeriodicTaskHandle scheduleExecute(ScheduleTask task, long delay, long periodic)
//         {
//             TaskWrapper wrapper = new TaskWrapper(task, delay, periodic);
// 
//             //逐个接受Task 保证负载均衡
//             if (currentWorkerIndex >= workerQueue.Count)
//             {
//                 currentWorkerIndex = 0;
//             }
//             //选择当前任务队列最小的worker，各个线程负载均衡
//             Worker worker = workerQueue[currentWorkerIndex];
//             ;
//             if (null == worker)
//             {
//                 //一般不会出现这种情况，但需要制定处理策略，暂定遗弃任务
//                 worker = workerQueue[0];
//             }
//             worker.addTask(wrapper);
// 
//             currentWorkerIndex++;
// 
//             PeriodicTaskHandle handle = new PeriodicTaskHandleImpl(wrapper, worker);
// 
//             return handle;
// 
//         }
// 
//     }
// 
//     public class TimeUtil
//     {
//         /// <summary>
//         /// 毫秒
//         /// </summary>
//         public static readonly long MILLISECOND = 1;
//         /// <summary>
//         /// 秒
//         /// </summary>
//         public static readonly long SECOND = 1000 * MILLISECOND;
//         /// <summary>
//         /// 分钟
//         /// </summary>
//         public static readonly long MINITE = 60 * SECOND;
//         /// <summary>
//         /// 小时
//         /// </summary>
//         public static readonly long HOUR = 60 * MINITE;
//         /// <summary>
//         /// 天
//         /// </summary>
//         public static readonly long DAY = 24 * HOUR;
// 
//         /// <summary>
//         /// 当前系统时间毫秒数
//         /// </summary>
//         public static long NOW()
//         {
//             return DateTime.Now.Ticks / 10000;
//         }
//     }
}
