using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameServer.Core.Executor;

namespace Server.Tools
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogTypes
    {
        Ignore = -1, //忽略
        Info = 0,
        Warning = 1,
        Error = 2,
        SQL = 3,
    }

    public class LogFilePoolItem
    {
        public FileStream _FileStream;
        public StreamWriter _StreamWriter;
        public long OpenTimeOnHours; //打开的时间
        public long OpenTimeOnDayOfYear; //打开的时间

        ~LogFilePoolItem()
        {
            if (null != _StreamWriter)
            {
                try
                {
                    _StreamWriter.Close();
                }catch{}
                _StreamWriter = null;
            }
        }
    }

    /// <summary>
    /// 日志管理类
    /// </summary>
    public class LogManager
    {
        public LogManager()
        {
        }

        public static Dictionary<LogTypes, LogFilePoolItem> LogType2FileDict = new Dictionary<LogTypes, LogFilePoolItem>();

        /// <summary>
        /// 允许实际写的日志级别
        /// </summary>
        public static LogTypes LogTypeToWrite
        {
            get;
            set;
        }

        /// <summary>
        /// 是否允许输出到dbgView窗口
        /// </summary>
        public static bool EnableDbgView = false;

        /// <summary>
        /// 日志输出目录
        /// </summary>
        private static string _LogPath = string.Empty;

        /// <summary>
        /// 日志输出目录
        /// </summary>
        public static string LogPath
        {
            get
            {
                lock (mutex)
                {
                    if (_LogPath == string.Empty)
                    {
                        _LogPath = AppDomain.CurrentDomain.BaseDirectory + @"log/";
                        if (!System.IO.Directory.Exists(_LogPath))
                        {
                            System.IO.Directory.CreateDirectory(_LogPath);
                        }
                    }
                }

                return _LogPath;
            }
            set
            {
                lock (mutex)
                {
                    _LogPath = value;
                }

                if (!System.IO.Directory.Exists(_LogPath))
                {
                    System.IO.Directory.CreateDirectory(_LogPath);
                }
            }
        }

        /// <summary>
        /// 异常输出目录
        /// </summary>
        private static string _ExceptionPath = string.Empty;

        /// <summary>
        /// 异常输出目录
        /// </summary>
        public static string ExceptionPath
        {
            get
            {
                lock (mutex)
                {
                    if (_ExceptionPath == string.Empty)
                    {
                        _ExceptionPath = AppDomain.CurrentDomain.BaseDirectory + @"Exception/";
                        if (!System.IO.Directory.Exists(_ExceptionPath))
                        {
                            System.IO.Directory.CreateDirectory(_ExceptionPath);
                        }
                    }
                }

                return _ExceptionPath;
            }
            set
            {
                lock (mutex)
                {
                    _ExceptionPath = value;
                }

                if (!System.IO.Directory.Exists(_ExceptionPath))
                {
                    System.IO.Directory.CreateDirectory(_ExceptionPath);
                }
            }
        }

        /// <summary>
        /// 将日志写入指定的文件
        /// </summary>
        private static void WriteLogEx(LogTypes logType, string logMsg)
        {
            try
            {
                StreamWriter sw = GetStreamWriter(logType);

                string text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss: ") + logMsg;
                if (EnableDbgView)
                {
                    System.Diagnostics.Debug.WriteLine(text);
                }

                sw.WriteLine(text);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 将异常写入指定的文件
        /// </summary>
        private static void _WriteException(string exceptionMsg)
        {
            try
            {
                StreamWriter sw = File.CreateText(
                    ExceptionPath + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
                sw.WriteLine(exceptionMsg);
                sw.Close();
                sw = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 写日志的锁对象
        /// </summary>
        private static object mutex = new object();

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="logMsg">日志信息</param>

        public static void WriteLog(LogTypes logType, string logMsg)
        {
            if ((int)logType < (int)LogTypeToWrite) //不必记录
            {
                return;
            }

            lock (mutex)
            {
                WriteLogEx(logType, logMsg);
            }
        }

        /// <summary>
        /// 写异常
        /// </summary>
        /// <param name="exceptionMsg">异常信息</param>
        public static void WriteException(string exceptionMsg)
        {
            lock (mutex)
            {
                _WriteException(exceptionMsg);
            }
        }

        private static StreamWriter GetStreamWriter(LogTypes logType)
        {
            LogFilePoolItem item;
            DateTime now = TimeUtil.NowDateTime();
            lock (mutex)
            {
                if (!LogType2FileDict.TryGetValue(logType, out item))
                {
                    item = new LogFilePoolItem();
                    LogType2FileDict.Add(logType, item);
                }
                if (now.Hour != item.OpenTimeOnHours || now.DayOfYear != item.OpenTimeOnDayOfYear || item._StreamWriter == null)
                {
                    if (null != item._StreamWriter)
                    {
                        item._StreamWriter.Close();
                        item._StreamWriter = null;
                    }
                    item._StreamWriter = File.AppendText(LogPath + logType.ToString() + "_" + now.ToString("yyyyMMdd") + ".log");
                    item.OpenTimeOnHours = now.Hour;
                    item.OpenTimeOnDayOfYear = now.DayOfYear;
                    item._StreamWriter.AutoFlush = true;
                }
                return item._StreamWriter;
            }
        }
    }
}
