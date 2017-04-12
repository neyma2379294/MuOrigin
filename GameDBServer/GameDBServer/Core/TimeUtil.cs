using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.Core
{
    public class TimeUtil
    {
        /// <summary>
        /// millisecond
        /// </summary>
        public static readonly long MILLISECOND = 1;
        /// <summary>
        /// second
        /// </summary>
        public static readonly long SECOND = 1000 * MILLISECOND;
        /// <summary>
        /// minute
        /// </summary>
        public static readonly long MINITE = 60 * SECOND;
        /// <summary>
        /// hour
        /// </summary>
        public static readonly long HOUR = 60 * MINITE;
        /// <summary>
        /// Day
        /// </summary>
        public static readonly long DAY = 24 * HOUR;

        private static int CurrentTickCount = 0;

        private static long CurrentTicks = DateTime.Now.Ticks / 10000;

        private static DateTime _Now = DateTime.Now;

        /// <summary>
        /// The current system time in milliseconds
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
            if (GameDBManager.StatisticsMode == 0)
            {
                return CurrentTicks;
            }
            else if (GameDBManager.StatisticsMode == 1)
            {
                return CurrentTicks;
            }
            else if (GameDBManager.StatisticsMode == 2)
            {
                return NOW();
            }
            else if (GameDBManager.StatisticsMode == 3)
            {
                return DateTime.Now.Ticks / 10000;
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
            if (GameDBManager.StatisticsMode <= 3)
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
            if (GameDBManager.StatisticsMode <= 3)
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
}
