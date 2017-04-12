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
    public class NormalScheduleTask : ScheduleTask
    {
        private TaskInternalLock _InternalLock = new TaskInternalLock();
        public TaskInternalLock InternalLock { get { return _InternalLock; } }

        private EventHandler TimerCallProc = null;

        public NormalScheduleTask(EventHandler timerCallProc)
        {
            TimerCallProc = timerCallProc;
        }

        public void run()
        {
            try
            {
                if (null != TimerCallProc)
                {
                    TimerCallProc(this, EventArgs.Empty);
                }
            }
            catch
            {

            }
        }
    }

    public class ScheduleExecutor2
    {
        /// <summary>
        /// 静态实例
        /// </summary>
        public static ScheduleExecutor2 Instance = new ScheduleExecutor2();

        Dictionary<ScheduleTask, Timer> TimerDict = new Dictionary<ScheduleTask, Timer>();

        public void start()
        {
        }

        public void stop()
        {
            lock (this)
            {
                foreach (var t in TimerDict)
                {
                    t.Value.Dispose();
                }
                TimerDict.Clear();
            }
        }

        public void scheduleCancle(ScheduleTask task)
        {
            lock (this)
            {
                Timer timer;
                if (TimerDict.TryGetValue(task, out timer))
                {
                    timer.Dispose();
                    TimerDict.Remove(task);
                }
            }
        }

        /// <summary>
        /// 周期性任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="delay">延迟开始时间（毫秒）</param>
        /// <param name="periodic">间隔周期时间（毫秒）</param>
        /// <returns></returns>
        public void scheduleExecute(ScheduleTask task, int delay, int periodic)
        {
            if (periodic < 15 || periodic > 86400 * 1000)
            {
                throw new Exception("不正确的调度时间间隔periodic = " + periodic);
            }

            if (delay <= 0)
            {
                delay = periodic;
            }
            lock (this)
            {
                Timer timer;
                if (!TimerDict.TryGetValue(task, out timer))
                {
                    timer = new Timer(OnTimedEvent, task, Global.GetRandomNumber(delay / 2, delay * 3 / 2), periodic);
                    TimerDict.Add(task, timer);
                }
                else
                {
                    timer.Change(periodic, periodic);
                }
            }
        }

        private static void OnTimedEvent(object source)
        {
            ScheduleTask task = source as ScheduleTask;
            if (task.InternalLock.TryEnter())
            {
                try
                {
                    task.run();
                }
                catch (System.Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("{0}执行时异常,{1}", task.ToString(), ex.ToString()));
                }
                finally
                {
                    task.InternalLock.Leave();
                }
            }
            else
            {
                try
                {
                    MonsterTask monsterTask = task as MonsterTask;
                    if (null != monsterTask)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("{0}自身执行时间超过设定的时间间隔,本次执行取消,mapCode:{1},subMapCode:{2}"
                                                , task.ToString(), monsterTask.mapCode, monsterTask.subMapCode));
                    }
                    else
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("{0}自身执行时间超过设定的时间间隔,本次执行取消", task.ToString()));
                    }
                }
                catch
                {
                    //写日志异常就不记了
                }
            }
        }
    }
}
