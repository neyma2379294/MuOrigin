using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using MySQLDriverCS;
using Server.Protocol;
using System.Xml.Linq;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Logic;
using Server.Data;
using ProtoBuf;
using System.Threading;
using System.ComponentModel;
using Server.Tools;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using GameDBServer.Core;

namespace GameDBServer
{
    public class Program
    {
        /// <summary>
        /// File version information
        /// </summary>
        public static FileVersionInfo AssemblyFileVersion;

#if Windows
        #region console closes control windows
        public delegate bool ControlCtrlDelegate(int CtrlType);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);

        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("The tool is forced to close (ctrl + c) "); // Ctrl + C is closed   
                    break;
                case 2:
                    //Console.WriteLine("The tool is forced off (interface close button) "); // press the console close button to close   
                    break;
            }

            //After the shutdown event is captured, no shutdown processing is required
            return true;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// Disable the close button
        /// </summary>
        static void HideCloseBtn()
        {
            Console.Title = "Game database server";
            IntPtr windowHandle = FindWindow(null, Console.Title);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        #endregion Console off control
#endif
        /// <summary>
        /// Global console instance
        /// </summary>
        public static Program ServerConsole = new Program();

        /// Command callback
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void CmdCallback(String cmd);

        /// <summary>
        /// Command dictionary
        /// </summary>
        private static Dictionary<String, CmdCallback> CmdDict = new Dictionary<string, CmdCallback>();

        private static Boolean NeedExitServer = false;

        #region Global uncaught exception handling

        /// <summary>
        /// Intercept the unhandled exception code in the thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = e.ExceptionObject as Exception;

                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(exception, "CurrentDomain_UnhandledException", UnhandedException.ShowErrMsgBox, true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Hanging thread exception handling hook
        /// </summary>
        static void ExceptionHook()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        #endregion Global uncaught exception handling

        #region Start close the program when the file operation part
        /// <summary>
        /// Delete a specified file
        /// </summary>
        public static void DeleteFile(String strFileName)
        {
            String strFullFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + strFileName;
            if (File.Exists(strFullFileName))
            {
                FileInfo fi = new FileInfo(strFullFileName);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    fi.Attributes = FileAttributes.Normal;
                }

                File.Delete(strFullFileName);
            }
        }


        /// <summary>
        /// Writes the process ID to a file
        /// </summary>
        public static void WritePIDToFile(String strFile)
        {
            String strFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + strFile;

            Process processes = Process.GetCurrentProcess();
            int nPID = processes.Id;
            File.WriteAllText(strFileName, "" + nPID);
        }

        /// <summary>
        /// Get the process ID from the file
        /// </summary>
        public static int GetServerPIDFromFile()
        {
            String strFileName = System.IO.Directory.GetCurrentDirectory() + "\\GameServerStop.txt";
            if (File.Exists(strFileName))
            {
                string str = File.ReadAllText(strFileName);
                return int.Parse(str);
            }

            return 0;
        }
        #endregion Start, close the program when the file operation part

        #region The main part of the console program
        static string[] cmdLineARGS = null;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                cmdLineARGS = args;
            }

            // Delete the corresponding file at startup
            DeleteFile("Start.txt");
            DeleteFile("Stop.txt");
            DeleteFile("GameServerStop.txt");

#if Windows
            #region Console closes control windows

            HideCloseBtn();

            SetConsoleCtrlHandler(newDelegate, true);

            if (Console.WindowWidth < 88)
            {
                Console.BufferWidth = 88;
                Console.WindowWidth = 88;
            }

            #endregion
#endif
            ///Hanging thread exception handling hook
            ExceptionHook();

            InitCommonCmd();

            //Start the server
            OnStartServer();

            ShowCmdHelpInfo();

            // The process ID is written to the file when it starts successfully
            WritePIDToFile("Start.txt");

            String cmd = System.Console.ReadLine();

            while (!NeedExitServer)
            {
                //ctrl + c Will get null
                if (null == cmd || 0 == cmd.CompareTo("exit"))
                {
                    //There may be a game server or sub-line server connected, can not quit
                    if (ServerConsole.CanExit())
                    {
                        System.Console.WriteLine("Are you sure to exit (enter y will exit immediately))？");
                        cmd = System.Console.ReadLine();
                        if (0 == cmd.CompareTo("y"))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //Not an exit command, an additional resolution is performed
                    ParseInputCmd(cmd);
                }

                cmd = System.Console.ReadLine();
            }

            // Exit the server
            OnExitServer();
        }

        /// <summary>
        /// Parse the input command
        /// </summary>
        /// <param name="cmd"></param>
        private static void ParseInputCmd(String cmd)
        {
            CmdCallback cb = null;

            if (CmdDict.TryGetValue(cmd, out cb) && null != cb)
            {
                cb(cmd);
            }
            else
            {
                System.Console.WriteLine("Unknown command, enter help to view the specific command information");
            }
        }
        /// <summary>
        /// Start the server
        /// </summary>
        private static void OnStartServer()
        {
            ServerConsole.InitServer();

            Console.Title = string.Format("Game database server {0} zone @ {1} @ {2}", GameDBManager.ZoneID, GetVersionDateTime(), ProgramExtName);
        }
        /// <summary>
        ///The process exits
        /// </summary>
        private static void OnExitServer()
        {
            ServerConsole.ExitServer();
        }

        public static void Exit()
        {
            NeedExitServer = true;
            //The main thread in the receiving input state, how to wake up?
        }

        #endregion The main part of the console program

        #region Command function

        /// <summary>
        /// Initialize the public command, go out of the exit other than the command
        /// </summary>
        private static void InitCommonCmd()
        {
            CmdDict.Add("help", ShowCmdHelpInfo);
            CmdDict.Add("gc", GarbageCollect);
            CmdDict.Add("append lipinma", AppendLiPinMaCmd);
            CmdDict.Add("load names", LoadNamesCmd);
            CmdDict.Add("show baseinfo", ShowServerBaseInfo);
            CmdDict.Add("show tcpinfo", ServerConsole.ShowServerTCPInfo);
        }

        /// <summary>
        /// Display help information
        /// </summary>
        private static void ShowCmdHelpInfo(String cmd = null)
        {
            System.Console.WriteLine(string.Format("Game database server"));
            System.Console.WriteLine("enter help， Display help information");
            System.Console.WriteLine("enter exit， Then enter y to exit？");
            System.Console.WriteLine("enter gc， Perform garbage collection");
            System.Console.WriteLine("enter append lipinma， Add a gift code from the file");
            System.Console.WriteLine("enter load names， Load the name from the directory");
            System.Console.WriteLine("enter show baseinfo， View the underlying run information");
            System.Console.WriteLine("enter show tcpinfo， View the instruction consumption information");
        }

        /// <summary>
        /// Garbage collection
        /// </summary>
        private static void GarbageCollect(String cmd = null)
        {
            try
            {
                //Release memory
                GC.Collect();
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "GarbageCollect()", false);
            }
        }

        /// <summary>
        /// Add a gift code from the file
        /// </summary>
        private static void AppendLiPinMaCmd(String cmd = null)
        {
            try
            {
                ServerConsole.AppendLiPinMa();
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShowDBConnectInfo()", false);
            }
        }

        /// <summary>
        /// Add a gift code
        /// </summary>
        private void AppendLiPinMa()
        {
            if (!updateLiPinMaWorker.IsBusy)
            {
                //Whether it is an additional gift code
                toAppendLiPinMa = true;

                updateLiPinMaWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Load the name from the directory
        /// </summary>
        private static void LoadNamesCmd(String cmd = null)
        {
            try
            {
                ServerConsole.LoadNames();
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShowDBConnectInfo()", false);
            }
        }

        /// <summary>
        /// Load the name from the directory
        /// </summary>
        private void LoadNames()
        {
            if (!updatePreNamesWorker.IsBusy)
            {
                updatePreNamesWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Display public information
        /// </summary>
        private static void ShowServerBaseInfo(String cmd = null)
        {
            string info = string.Format("Number of logon server connections that have been connected:{0}", ServerConsole.TotalConnections);

            System.Console.WriteLine(info);

            info = string.Format("Number of database connection pools: {0}", ServerConsole._DBManger.GetMaxConnsCount());

            System.Console.WriteLine(info);

            info = string.Format("User cache: {0}, role cache: {1}",
                ServerConsole._DBManger.dbUserMgr.GetUserInfoCount(),
                ServerConsole._DBManger.DBRoleMgr.GetRoleInfoCount());

            System.Console.WriteLine(info);
        }

        /// <summary>
        /// Display information about Tcp
        /// </summary>
        private void ShowServerTCPInfo(String cmd = null)
        {
            bool clear = cmd.Contains("/c");
            bool detail = cmd.Contains("/d");

            string info = string.Format("Total receive bytes: {0:0.00} MB", _TCPManager.MySocketListener.TotalBytesReadSize / (1024.0 * 1024.0));
            System.Console.WriteLine(info);
            info = string.Format("Always send bytes: {0:0.00} MB", _TCPManager.MySocketListener.TotalBytesWriteSize / (1024.0 * 1024.0));
            System.Console.WriteLine(info);

            //info = string.Format("Total number of processing instructions {0}", TCPCmdHandler.TotalHandledCmdsNum);
            //System.Console.WriteLine(info);
            //info = string.Format("The number of threads that are currently processing instructions {0}", TCPCmdHandler.GetHandlingCmdCount());
            //System.Console.WriteLine(info);
            //info = string.Format("The maximum time a single instruction consumes {0}", TCPCmdHandler.MaxUsedTicksByCmdID);
            //System.Console.WriteLine(info);
            //info = string.Format("The maximum time instruction ID consumed {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID);
            //System.Console.WriteLine(info);
            //info = string.Format("Send the total number of calls {0}", Global._TCPManager.MySocketListener.GTotalSendCount);
            //System.Console.WriteLine(info);
            //info = string.Format("The maximum size of the packet sent {0}", Global._SendBufferManager.MaxOutPacketSize);
            //System.Console.WriteLine(info);
            //info = string.Format("The maximum number of packets sent by the ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID);
            //System.Console.WriteLine(info);

            //////////////////////////////////////////
            info = string.Format("Instruction processing average time-consuming (milliseconds) {0}", TCPManager.processCmdNum != 0 ? TimeUtil.TimeMS(TCPManager.processTotalTime / TCPManager.processCmdNum) : 0);
            System.Console.WriteLine(info);
            info = string.Format("Instruction processing time-consuming details");
            System.Console.WriteLine(info);

            int count = 0;
            lock (TCPManager.cmdMoniter)
            {
                foreach (PorcessCmdMoniter m in TCPManager.cmdMoniter.Values)
                {
                    Console.ForegroundColor = (ConsoleColor)(count % 5 + ConsoleColor.Green); //Set the font color by line
                    if (detail)
                    {
                        if (count++ == 0)
                        {
                            info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}{4, 7}{5, 7}", "Message", "processed number", "average processing time", "total consumption time", "maximum processing time", "maximum waiting time");
                            System.Console.WriteLine(info);
                        }
                        info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}{4, 13:0.##}{5, 13:0.##}", (TCPGameServerCmds)m.cmd, m.processNum, TimeUtil.TimeMS(m.avgProcessTime()), TimeUtil.TimeMS(m.processTotalTime), TimeUtil.TimeMS(m.processMaxTime), TimeUtil.TimeMS(m.maxWaitProcessTime));
                        System.Console.WriteLine(info);
                    }
                    else
                    {
                        if (count++ == 0)
                        {
                            info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", "Message", "processed number", "average processing time", "total consumption time");
                            System.Console.WriteLine(info);
                        }
                        info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", (TCPGameServerCmds)m.cmd, m.processNum, TimeUtil.TimeMS(m.avgProcessTime()), TimeUtil.TimeMS(m.processTotalTime));
                        System.Console.WriteLine(info);
                    }
                    if (clear)
                    {
                        m.maxWaitProcessTime = 0;
                        m.processMaxTime = 0;
                        m.processNum = 0;
                        m.processTotalTime = 0;
                        m.waitProcessTotalTime = 0;
                    }
                }
                Console.ForegroundColor = ConsoleColor.White; //Restore the font color
            }
        }

        #endregion Command function

        #region External call interface

        /// <summary>
        /// Data server connection quantity information
        /// </summary>
        private int _TotalConnections = 0;

        /// <summary>
        /// Set the data server connection quantity information
        /// </summary>
        /// <param name="index"></param>
        /// <param name="info"></param>
        public int TotalConnections
        {
            set { _TotalConnections = value; }
            get { return _TotalConnections; }
        }

        #endregion

        #region Game database server specific function part
        /// <summary>
        /// Database operation object
        /// </summary>
        private DBManager _DBManger = DBManager.getInstance();

        /// <summary>
        /// Communication management object
        /// </summary>
        private TCPManager _TCPManager = null;

        /// <summary>
        /// Whether it is to be closed immediately
        /// </summary>
        private bool MustCloseNow = false;

        /// <summary>
        /// Whether it has entered the shutdown mode
        /// </summary>
        private bool EnterClosingMode = false;

        /// <summary>
        /// 60 seconds of the countdown
        /// </summary>
        private int ClosingCounter = 30 * 200;

        /// <summary>
        /// The last time to write the database log
        /// </summary>
        private long LastWriteDBLogTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// Displays the timer that closes the message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string title = "";

                //Turn off countdown
                ClosingCounter -= 200;

                //Determine whether the DB command queue has been executed?
                if (ClosingCounter <= 0)
                {
                    //Whether it is closed immediately
                    MustCloseNow = true;
                }
                else
                {
                    int counter = (ClosingCounter / 200);
                    title = string.Format("Game database server {0} off, countdown: {1}", GameDBManager.ZoneID, counter);
                }

                //Set the title
                Console.Title = title;
            }
            catch (Exception ex)
            {
                ////System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "closingTimer_Tick", false);
                    //throw ex;
                //}//);
            }
        }

        /// <summary>
        /// Background master scheduling thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainDispatcherWorker_DoWork(object sender, EventArgs e)
        {
            long startTicks = DateTime.Now.Ticks / 10000;
            long endTicks = DateTime.Now.Ticks / 10000;

            //sleeping time
            int maxSleepMs = 1000;
            int sleepMs = 1000;

            while (true)
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000;

                    //Execute the background thread object once every second
                    ExecuteBackgroundWorkers(null, EventArgs.Empty);

                    if (NeedExitServer)
                    {
                        //Enter the shutdown mode, sleep time becomes 200 milliseconds
                        maxSleepMs = 200;
                        sleepMs = 200;

                        //Dispatch operation ---> every 200 milliseconds to implement once, no need to be so frequent, anyway, have to quit
                        closingTimer_Tick(null, null);

                        //Closed, they should also quit
                        if (MustCloseNow)
                        {
                            break;
                        }
                    }

                    endTicks = DateTime.Now.Ticks / 10000;

                    //Sleep up to 1000 milliseconds, sleep at least 1 millisecond
                    sleepMs = (int)Math.Max(1, maxSleepMs - (endTicks - startTicks));

                    Thread.Sleep(sleepMs);

                    // Found that when the GameServer exits, it automatically exits
                    if (0 != GetServerPIDFromFile())
                    {
                        OnExitServer();
                    }
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "MainDispatcherWorker_DoWork", false);
                }
            }

            System.Console.WriteLine("The main loop thread exits and the carriage returns to the system");
            if (0 != GetServerPIDFromFile())
            {
                // The process ID is written to the file at the end
                WritePIDToFile("Stop.txt");
            }
        }

        /// <summary>
        /// A timer function that displays information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowInfoTicks(object sender, EventArgs e)
        {
            try
            {
                // Execute the background thread object
                ExecuteBackgroundWorkers(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ////System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "ShowInfoTicks", false);
                    //throw ex;
                //}//);
            }
        }

        /// <summary>
        /// Additional name of the program
        /// </summary>
        private static string ProgramExtName = "";

        /// <summary>
        /// Initialize the application name
        /// </summary>
        /// <returns></returns>
        private static void InitProgramExtName()
        {
            ProgramExtName = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Initialize the game resource information
        /// The original Window_Loaded(object sender, RoutedEventArgs e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InitServer()
        {
            InitProgramExtName();

            System.Console.WriteLine("The language file is being initialized");

            Global.LoadLangDict();

            XElement xml = null;

            System.Console.WriteLine("The system configuration file is being initialized");

            try
            {
                xml = XElement.Load(@"AppConfig.xml");
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Load xml file at startup: {0} failed", @"AppConfig.xml"));
            }

            // Program log level
            LogManager.LogTypeToWrite = (LogTypes)(int)Global.GetSafeAttributeLong(xml, "Server", "LogType");

            // Event log level
            GameDBManager.SystemServerSQLEvents.EventLevel = (EventLevels)(int)Global.GetSafeAttributeLong(xml, "Server", "EventLevel");

            int dbLog = Math.Max(0, (int)Global.GetSafeAttributeLong(xml, "DBLog", "DBLogEnable"));

            //Write the log in the cache to the file
            //GameDBManager.DBEventsWriter.Enable = (dbLog > 0);
            //GameDBManager.DBEventsWriter.EventDiskWriter.EventRootPath = Global.GetSafeAttributeStr(xml, "DBLog", "Path");
            //GameDBManager.DBEventsWriter.MaxCacheCount = 10000 * 10;

            GameDBManager.ZoneID = (int)Global.GetSafeAttributeLong(xml, "Zone", "ID");

            string uname = StringEncrypt.Decrypt(Global.GetSafeAttributeStr(xml, "Database", "uname"), "eabcix675u49,/", "3&3i4x4^+-0");
            string upasswd = StringEncrypt.Decrypt(Global.GetSafeAttributeStr(xml, "Database", "upasswd"), "eabcix675u49,/", "3&3i4x4^+-0");
			
            System.Console.WriteLine("The number of data connection pools the server is building: {0}", (int)Global.GetSafeAttributeLong(xml, "Database", "maxConns"));
            System.Console.WriteLine("Database address: {0}", Global.GetSafeAttributeStr(xml, "Database", "ip"));
            System.Console.WriteLine("Name database: {0}", Global.GetSafeAttributeStr(xml, "Database", "dname"));
			System.Console.WriteLine("Database character set: {0}", Global.GetSafeAttributeStr(xml, "Database", "names"));						

            DBConnections.dbNames = Global.GetSafeAttributeStr(xml, "Database", "names");

            System.Console.WriteLine("The database connection is being initialized");

            //long ticks = DateTime.Now.Ticks;
            _DBManger.LoadDatabase(new MySQLConnectionString(
                Global.GetSafeAttributeStr(xml, "Database", "ip"),
                Global.GetSafeAttributeStr(xml, "Database", "dname"),
                uname,
                upasswd),
                (int)Global.GetSafeAttributeLong(xml, "Database", "maxConns"),
			    (int)Global.GetSafeAttributeLong(xml, "Database", "codePage"));

            //Verify area code
            ValidateZoneID();

            //Prepare the necessary data sheet
            GameDBManager.DBName = Global.GetSafeAttributeStr(xml, "Database", "dname");
            DBWriter.ValidateDatabase(_DBManger, GameDBManager.DBName);

            //Initialize the database since the growth value
            if (!Global.InitDBAutoIncrementValues(_DBManger))
            {
                System.Console.WriteLine("There is a fatal error. Please enter exit and y to exit");
                return;
            }

            //DBWriter.ClearUnusedGoodsData(_DBManger, true);
            //MessageBox.Show(string.Format("Add a total cost: {0}", (DateTime.Now.Ticks - ticks) / 10000));

            //Line management
            LineManager.LoadConfig(xml);

            System.Console.WriteLine("The network is being initialized");

//             _TCPManager = new TCPManager((int)Global.GetSafeAttributeLong(xml, "Socket", "capacity"));

            _TCPManager = TCPManager.getInstance();
            _TCPManager.initialize((int)Global.GetSafeAttributeLong(xml, "Socket", "capacity"));

            //Start the communication management object
            _TCPManager.DBMgr = _DBManger;
            _TCPManager.RootWindow = this;
            _TCPManager.Start(Global.GetSafeAttributeStr(xml, "Socket", "ip"),
                (int)Global.GetSafeAttributeLong(xml, "Socket", "port"));

            System.Console.WriteLine("Configuring background threads");

            //Set the background worker thread
            eventWorker = new BackgroundWorker();
            eventWorker.DoWork += eventWorker_DoWork;

            updateMoneyWorker = new BackgroundWorker();
            updateMoneyWorker.DoWork += updateMoneyWorker_DoWork;

            releaseMemoryWorker = new BackgroundWorker();
            releaseMemoryWorker.DoWork += releaseMemoryWorker_DoWork;

            updateLiPinMaWorker = new BackgroundWorker();
            updateLiPinMaWorker.DoWork += updateLiPinMaWorker_DoWork;

            updatePreNamesWorker = new BackgroundWorker();
            updatePreNamesWorker.DoWork += updatePreNamesWorker_DoWork;

            updatePaiHangWorker = new BackgroundWorker();
            updatePaiHangWorker.DoWork += updatePaiHangWorker_DoWork;

            dbWriterWorker = new BackgroundWorker();
            dbWriterWorker.DoWork += dbWriterWorker_DoWork;

            updateLastMailWorker = new BackgroundWorker();
            updateLastMailWorker.DoWork += updateLastMail_DoWork;

            MainDispatcherWorker = new BackgroundWorker();
            MainDispatcherWorker.DoWork += MainDispatcherWorker_DoWork;

            //Whether to display the exception when the dialog box
            UnhandedException.ShowErrMsgBox = false;

            //Start the Global Service Manager
            GlobalServiceManager.initialize();
            GlobalServiceManager.startup();

            //Start the main loop scheduling thread
            if (!MainDispatcherWorker.IsBusy)
            {
                MainDispatcherWorker.RunWorkerAsync();
            }

            //ProgramExtName = string.Format("{0}@{1}", Global.GetSafeAttributeStr(xml, "Database", "dname"), GameDBManager.ZoneID);
            DBWriter.UpdateGameConfig(_DBManger, "gamedb_version", GetVersionDateTime());

            System.Console.WriteLine("The system is finished");
        }

        /// <summary>
        /// Verify area code
        /// </summary>
        public static void ValidateZoneID()
        {
            System.Console.WriteLine("To configure the database table from the growth value, please enter the area code to verify:");

            String readLine = "";

            if (cmdLineARGS != null && cmdLineARGS.Length > 0)
            {
                readLine = cmdLineARGS[0];
            }
            else
            {
                readLine = System.Console.ReadLine();
            }
            

            while (true)
            {
                try
                {
                    int inputZone = int.Parse(readLine);
                    if (inputZone == GameDBManager.ZoneID)
                    {
                        System.Console.WriteLine("The area code is valid!!");
                        break;
                    }
                    else
                    {
                        System.Console.WriteLine("Enter area code is illegal!!");
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

                System.Console.WriteLine("Please re-enter the area code to verify:");
                readLine = System.Console.ReadLine();
            }
        }

        //Close the server
        public void ExitServer()
        {
            if (NeedExitServer)
            {
                return;
            }

            _TCPManager.Stop(); //Stop TCP listening, otherwise the mono can not exit normally

            //Close the Global Service Manager
            GlobalServiceManager.showdown();
            GlobalServiceManager.destroy();

            Window_Closing();

            System.Console.WriteLine("Trying to shut down the server, see the server closed after the completion of the prompts to exit the system");

            if (0 == GetServerPIDFromFile())
            {
                String cmd = System.Console.ReadLine();

                while (true)
                {
                    if (MainDispatcherWorker.IsBusy)
                    {
                        System.Console.WriteLine("Trying to shut down the server");
                        cmd = System.Console.ReadLine();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        #region Thread part

        /// <summary>
        /// Background log threads
        /// </summary>
        BackgroundWorker eventWorker;

        /// <summary>
        /// Background update user reset thread
        /// </summary>
        BackgroundWorker updateMoneyWorker;

        /// <summary>
        /// Background release the memory of the thread
        /// </summary>
        BackgroundWorker releaseMemoryWorker;

        /// <summary>
        /// The thread that loads the gift code in the background
        /// </summary>
        BackgroundWorker updateLiPinMaWorker;

        /// <summary>
        /// The background loads the preallocated name of the thread
        /// </summary>
        BackgroundWorker updatePreNamesWorker;

        /// <summary>
        /// The sort of thread in the background
        /// </summary>
        BackgroundWorker updatePaiHangWorker;

        /// <summary>
        /// Write the DB log thread
        /// </summary>
        BackgroundWorker dbWriterWorker;

        /// <summary>
        /// Scan new mail threads
        /// </summary>
        BackgroundWorker updateLastMailWorker;

        /// <summary>
        /// The main scheduling thread, the thread has been in a circular state, continue to deal with a variety of logical judgments, the equivalent of the original main interface thread
        /// </summary>
        BackgroundWorker MainDispatcherWorker;

        /// <summary>
        /// The last time the memory was released
        /// </summary>
        long LastReleaseMemoryTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// Whether it is an additional gift code
        /// </summary>
        private bool toAppendLiPinMa = false;

        /// <summary>
        /// Execute the background thread object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteBackgroundWorkers(object sender, EventArgs e)
        {
            //Is it necessary for the background workers to keep up?
            if (!eventWorker.IsBusy) { eventWorker.RunWorkerAsync(); }
            if (!updateMoneyWorker.IsBusy) { updateMoneyWorker.RunWorkerAsync(); }
            if (!updatePaiHangWorker.IsBusy) { updatePaiHangWorker.RunWorkerAsync(); }
            if (!dbWriterWorker.IsBusy) { dbWriterWorker.RunWorkerAsync(); }
            if (!updateLastMailWorker.IsBusy) { updateLastMailWorker.RunWorkerAsync(); }

            //Last release of memory execution time (1 minute call once)
            long nowTicks = DateTime.Now.Ticks / 10000;
            if (nowTicks - LastReleaseMemoryTicks >= (1 * 60 * 1000))
            {
                if (!releaseMemoryWorker.IsBusy) { releaseMemoryWorker.RunWorkerAsync(); }
            }
        }

        //Background processing write log work events
        private void eventWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Write the event to the log
                while (GameDBManager.SystemServerSQLEvents.WriteEvent())
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "eventWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //Background processing refresh the user recharge the event
        private void updateMoneyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Update the user to recharge the ingot data
                UserMoneyMgr.UpdateUsersMoney(_DBManger);

                //Scanning the recharge flow to generate binary logs
                UserMoneyMgr.ScanInputLogToDBLog(_DBManger);

                //Scan the GM command flow to the client
                ChatMsgManager.ScanGMMsgToGameServer(_DBManger);
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "updateMoneyWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //The background handles the release of memory events
        private void releaseMemoryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int ticksSlot = (30 * 60 * 1000);
                //int ticksSlot = (60 * 1000);

                //Release user information (no more than 30 minutes)
                _DBManger.dbUserMgr.ReleaseIdleDBUserInfos(ticksSlot);

                //Release role information (no more than 30 minutes)
                _DBManger.DBRoleMgr.ReleaseIdleDBRoleInfos(ticksSlot);

                //Clear the used name
                PreNamesManager.ClearUsedPreNames();
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "releaseMemoryWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //The background loads the gift code from the file
        private void updateLiPinMaWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LiPinMaManager.LoadLiPinMaFromFile(_DBManger, toAppendLiPinMa);
            }
            catch (Exception ex)
            {
                ////System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "updateLiPinMaWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //The background loads events from the file that preallocated names
        private void updatePreNamesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PreNamesManager.LoadFromFiles(_DBManger);
            }
            catch (Exception ex)
            {
                ////System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "updatePreNamesWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //Background sorting events
        private void updatePaiHangWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Deal with charts
                PaiHangManager.ProcessPaiHang(_DBManger, false);

                //Number of online users
                OnlineUserNumMgr.WriteTotalOnlineNumToDB(_DBManger);

                //Writes the current number of online people to the notification statistics server
                OnlineUserNumMgr.NotifyTotalOnlineNumToServer(_DBManger);

                //Recalculate the number of gangs
                BangHuiNumLevelMgr.RecalcBangHuiNumLevel(_DBManger);

                //Deal with dissolution gang
                BangHuiDestroyMgr.ProcessDestroyBangHui(_DBManger);

                //Every morning morning clear Yangzhou city tax
                GameDBManager.BangHuiLingDiMgr.ProcessClearYangZhouTotalTax(_DBManger);
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "updatePaiHangWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// DB write log thread background work function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbWriterWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticks = DateTime.Now.Ticks / 10000;
                if (ticks - LastWriteDBLogTicks < (30 * 1000))
                {
                    return;
                }

                //The last time to write the database log
                LastWriteDBLogTicks = ticks;

                //Write the log in the cache to the file
                //bool ret = GameDBManager.DBEventsWriter.WriteToHardDisk();

                //if (!ret)
                //{
                //    LogManager.WriteLog(LogTypes.Error, string.Format("An error occurred while writing the DB log in the cache to the file:{0}", GameDBManager.DBEventsWriter.LastError));
                //}
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "dbWriterWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //Scanning new messages in the background
        private void updateLastMail_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Scan new messages
                UserMailManager.ScanLastMails(_DBManger);
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // Formatting exception error message
                DataHelper.WriteFormatExceptionLog(ex, "updateLastMail_DoWork", false);
                    //throw ex;
                //});
            }
        }

        #endregion Thread part

        /// <summary>
        /// To determine whether you can exit
        /// </summary>
        /// <returns></returns>
        public Boolean CanExit()
        {
            if (_TCPManager.MySocketListener.ConnectedSocketsCount > 0)
            {
                System.Console.WriteLine(string.Format("There is a game server or a sub-line server connected to {0} and can not be disconnected!", Console.Title));
                return false;
            }

            return true;
        }

        private void Window_Closing()
        {
            //Whether it is closed immediately
            if (MustCloseNow)
            {
                return;
            }

            //Has entered the closed mode
            if (EnterClosingMode)
            {
                return;
            }

            //Whether it has entered the shutdown mode
            EnterClosingMode = true;

            LastWriteDBLogTicks = 0; //Forced to write cache

            //Set the exit flag
            NeedExitServer = true;
        }

        #endregion Game database server specific function part

        #region Get compile date

        /// <summary>
        /// Get the compile date of the program
        /// </summary>
        /// <returns></returns>
        public static string GetVersionDateTime()
        {
            AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int revsion = Assembly.GetExecutingAssembly().GetName().Version.Revision;//Get revision number
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;//Get the build number
            DateTime dtbase = new DateTime(2000, 1, 1, 0, 0, 0);//Microsoft compiles the benchmark time
            TimeSpan tsbase = new TimeSpan(dtbase.Ticks);
            TimeSpan tsv = new TimeSpan(tsbase.Days + build, 0, 0, revsion * 2);//Compile time, note revision number to * 2
            DateTime dtv = new DateTime(tsv.Ticks);//Converted to compile time
            return dtv.ToString("yyyy-MM-dd HH") + string.Format(" {0}", AssemblyFileVersion.FilePrivatePart);
        }

        #endregion Get compile date
    }
}
