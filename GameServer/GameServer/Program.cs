#define UseTimer
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Threading;
//using System.Windows.Threading;
using Server.Protocol;
using System.Net;
using System.Net.Sockets;
using Server.TCP;
using Server.Tools;
using GameServer.Logic;
//using System.Windows.Resources;
using System.ComponentModel;
using GameServer.Server;
using System.Text.RegularExpressions; // 引用正则的命名空间
using System.Windows;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.CompilerServices;
using Server.Data;
using GameServer.Core.Executor;
using GameServer.Logic.MUWings;
using System.Runtime;
using GameServer.Logic.Copy;
using GameServer.Logic.RefreshIconState;
using GameServer.Logic.BossAI;
using GameServer.Logic.ExtensionProps;
using System.Diagnostics;

namespace GameServer
{
    //主程序
    public class Program
    {
        /// <summary>
        /// 文件版本信息
        /// </summary>
        public static FileVersionInfo AssemblyFileVersion;

#if Windows
        #region 控制台关闭控制 windows
        public delegate bool ControlCtrlDelegate(int CtrlType);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);

        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("工具被强制关闭(ctrl + c)"); //Ctrl+C关闭   
                    break;
                case 2:
                    //Console.WriteLine("工具被强制关闭(界面关闭按钮)");//按控制台关闭按钮关闭   
                    break;
            }

            //关闭事件被捕获之后，不需要进行关闭处理
            return true;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// 禁用关闭按钮
        /// </summary>
        static void HideCloseBtn()
        {
            Console.Title = "Server_" + Global.GetRandomNumber(0, 100000);
            IntPtr windowHandle = FindWindow(null, Console.Title);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        #endregion 控制台关闭控制
#endif
        /// <summary>
        /// 全局控制台实例
        /// </summary>
        public static Program ServerConsole = new Program();

        /// <summary>
        /// 命令回调
        /// </summary>
        /// <param name="cmd"></param>
        public delegate void CmdCallback(String cmd);

        /// <summary>
        /// 命令词典
        /// </summary>
        private static Dictionary<String, CmdCallback> CmdDict = new Dictionary<string, CmdCallback>();

        public static Boolean NeedExitServer = false;

        #region 全局的未捕获异常的处理

        /// <summary>
        /// 截获线程中未处理的异常代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception exception = e.ExceptionObject as Exception;

                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(exception, "CurrentDomain_UnhandledException", UnhandedException.ShowErrMsgBox, true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 挂接线程异常处理钩子
        /// </summary>
        static void ExceptionHook()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        #endregion 全局的未捕获的异常的处理

        #region 启动、关闭程序时文件操作部分
        /// <summary>
        /// 删除某个指定的文件
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
        /// 将进程ID写入到某个文件
        /// </summary>
        public static void WritePIDToFile(String strFile)
        {
            String strFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + strFile;

            Process processes = Process.GetCurrentProcess();
            int nPID = processes.Id;
            File.WriteAllText(strFileName, "" + nPID);
        }

        /// <summary>
        /// 从文件中获取进程ID
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

        #endregion 启动、关闭程序时文件操作部分

        #region 控制台程序主体部分
        static void Main(string[] args)
        {
            // 启动时删除相应的文件
            DeleteFile("Start.txt");
            DeleteFile("Stop.txt");
            DeleteFile("GameServerStop.txt");

            if (!GCSettings.IsServerGC && Environment.ProcessorCount > 2) 
            {
                System.Console.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
                Console.WriteLine("GC模式不正确,禁止启动,尝试自动设置为正确模式");

                string configFile = Process.GetCurrentProcess().MainModule.FileName + ".config";
                XElement xml = XElement.Load(configFile);
                XElement xml1 = xml.Element("runtime");
                if (null == xml1)
                {
                    xml.SetElementValue("runtime", "");
                    xml1 = xml.Element("runtime");
                }
                xml1.SetElementValue("gcServer", "");
                xml1.Element("gcServer").SetAttributeValue("enabled", "true");
                xml.Save(configFile);

                Console.WriteLine("自动设置为服务器模式,重新启动即可");
                Console.Read();
                return;
            }
#if BetaConfig
            string path = Process.GetCurrentProcess().MainModule.FileName;
            int idx = path.LastIndexOf('\\');
            path = path.Substring(0, idx);
            Directory.SetCurrentDirectory(path);
#endif
#if Windows
            #region 控制台关闭控制 windows

            HideCloseBtn();

            SetConsoleCtrlHandler(newDelegate, true);

            if (Console.WindowWidth < 88)
            {
                Console.BufferWidth = 88;
                Console.WindowWidth = 88;
            }

            #endregion
#endif
            ///挂接线程异常处理钩子
            ExceptionHook();

            TimeUtil.Init();
            InitCommonCmd();

            //启动服务器
            OnStartServer();

            ShowCmdHelpInfo();

            // 启动成功时将进程ID写入文件
            WritePIDToFile("Start.txt");

            String cmd = System.Console.ReadLine();

            while (!NeedExitServer)
            {
                //ctrl + c 会得到null
                if (null == cmd || 0 == cmd.CompareTo("exit"))
                {
                    System.Console.WriteLine("确认退出吗(输入 y 将立即退出)？");
                    cmd = System.Console.ReadLine();
                    if (0 == cmd.CompareTo("y"))
                    {
                        break;
                    }
                }

                //不是退出指令，则进行额外解析
                ParseInputCmd(cmd);

                cmd = System.Console.ReadLine();
            }

            //退出服务器
            OnExitServer();
        }

        /// <summary>
        /// 解析输入命令
        /// </summary>
        /// <param name="cmd"></param>
        private static void ParseInputCmd(String cmd)
        {
            CmdCallback cb = null;
            int index = cmd.IndexOf('/');
            string cmd0 = cmd;
            if (index > 0)
            {
                cmd0 = cmd.Substring(0, index - 1).TrimEnd();
            }
            if (CmdDict.TryGetValue(cmd0, out cb) && null != cb)
            {
                cb(cmd);
            }
            else
            {
                System.Console.WriteLine("未知命令,输入 help 查看具体命令信息");
            }
        }
        /// <summary>
        /// 启动服务器
        /// </summary>
        private static void OnStartServer()
        {
            ServerConsole.InitServer();

            Console.Title = string.Format("游戏服务器{0}线@{1}@{2}", GameManager.ServerLineID, GetVersionDateTime(), ProgramExtName);
        }
        /// <summary>
        /// 进程退出
        /// </summary>
        private static void OnExitServer()
        {
            ServerConsole.ExitServer();
        }

        public static void Exit()
        {
            NeedExitServer = true;
            //主线程处于接收输入状态，如何唤醒呢？
        }

        #endregion 控制台程序主体部分

        #region 命令功能

        /// <summary>
        /// 初始化公共命令，出去exit 之外的其他命令
        /// </summary>
        private static void InitCommonCmd()
        {
            CmdDict.Add("help", ShowCmdHelpInfo);
            CmdDict.Add("gc", GarbageCollect);
            CmdDict.Add("show dbconnect", ShowDBConnectInfo);
            CmdDict.Add("show baseinfo", ShowServerBaseInfo);
            CmdDict.Add("show tcpinfo", ShowServerTCPInfo);
            CmdDict.Add("show copymapinfo", ShowCopyMapInfo);
            CmdDict.Add("show gcinfo", ShowGCInfo);
            CmdDict.Add("show roleinfo", ShowRoleInfo);
            //CmdDict.Add("loadiplist", LoadIPList);


            //三种压测模式
            CmdDict.Add("testmode 5", SetTestMode);
            CmdDict.Add("testmode 1", SetTestMode);
            CmdDict.Add("testmode 0", SetTestMode);
            //运行修补程序
            CmdDict.Add("patch", RunPatchFromConsole);
            CmdDict.Add("show objinfo", ShowObjectInfo);
        }

        //public static void LoadIPList(String cmd = null)
        //{
        //    try
        //    {
        //        string path = "../IP白名单.txt";
        //        string[] ipList = null;
        //        if (File.Exists(path))
        //        {
        //            ipList = File.ReadAllLines(path, Encoding.GetEncoding("gb2312"));
        //        }
        //        bool enabeld = true;
        //        if (string.IsNullOrEmpty(cmd) || cmd == "0")
        //        {
        //            enabeld = false;
        //        }
                
        //        List<string> resultList = Global._TCPManager.MySocketListener.InitIPWhiteList(ipList, enabeld);

        //        if (resultList.Count > 0)
        //        {
        //            Console.WriteLine("IP白名单列表内容如下:");
        //            foreach (var ip in resultList)
        //            {
        //                Console.WriteLine(ip);
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("IP白名单为空,不限制IP登录");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("读取IP白名单异常,所以不限制IP登录.异常信息:\n" + ex.ToString());
        //    }
        //}

        public static void LoadIPList(string strCmd)
        {
            try
            {
                if (String.IsNullOrEmpty(strCmd))
                {
                    strCmd = GameManager.GameConfigMgr.GetGameConfigItemStr("whiteiplist", "");
                }

                LogManager.WriteLog(LogTypes.Error, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", strCmd));
                bool enabeld = true;
                string[] ipList = strCmd.Split(',');
                List<string> resultList = Global._TCPManager.MySocketListener.InitIPWhiteList(ipList, enabeld);

                if (resultList.Count > 0)
                {
                    Console.WriteLine("IP白名单列表内容如下:");
                    foreach (var ip in resultList)
                    {
                        Console.WriteLine(ip);
                    }
                }
                else
                {
                    Console.WriteLine("IP白名单为空,不限制IP登录");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("读取IP白名单异常,所以不限制IP登录.异常信息:\n" + ex.ToString());
            }
        }

        static int[] GCCollectionCounts = new int[3];
        static int[] GCCollectionCounts1 = new int[3];
        static int[] GCCollectionCounts5 = new int[3];
        static int[] GCCollectionCountsNow = new int[3];
        static int[] MaxGCCollectionCounts1s = new int[3];
        static int[] MaxGCCollectionCounts5s = new int[3];
        static long[] MaxGCCollectionCounts1sTicks = new long[3];
        static long[] MaxGCCollectionCounts5sTicks = new long[3];

        /// <summary>
        /// 计算GC计数和频率信息
        /// </summary>
        public static void CalcGCInfo()
        {
            long ticks = DateTime.Now.Ticks / 10000;
            for (int i = 0; i < 3; i++)
            {
                GCCollectionCounts1[i] = GC.CollectionCount(i);
                if (GCCollectionCounts[i] != 0)
                {
                    int count = GCCollectionCounts1[i] - GCCollectionCounts[i];
                    if (ticks >= MaxGCCollectionCounts1sTicks[i] + 1000)
                    {
                        if (count > MaxGCCollectionCounts1s[i])
                        {
                            MaxGCCollectionCounts1s[i] = count;
                        }
                        MaxGCCollectionCounts1sTicks[i] = ticks;
                    }
                    if (ticks >= MaxGCCollectionCounts5sTicks[i] + 5000)
                    {
                        if (GCCollectionCounts5[i] != 0)
                        {
                            int count5s = GCCollectionCounts1[i] - GCCollectionCounts5[i];
                            if (count5s > MaxGCCollectionCounts5s[i])
                            {
                                MaxGCCollectionCounts5s[i] = count5s;
                            }
                        }
                        MaxGCCollectionCounts5sTicks[i] = ticks;
                        GCCollectionCounts5[i] = GCCollectionCounts1[i];
                    }
                    GCCollectionCountsNow[i] = count;
                }

                GCCollectionCounts[i] = GCCollectionCounts1[i];
            }
        }

        /// <summary>
        /// 获取GC信息
        /// </summary>
        private static void ShowGCInfo(String cmd = null)
        {
            try
            {
                string output = string.Format("GC计数类别    {0,-10} {1,-10} {2,-10}", "0 gen", "1 gen", "2 gen");
                Console.WriteLine(output);
                output = string.Format("总计GC计数    {0,-10} {1,-10} {2,-10}", GCCollectionCounts[0], GCCollectionCounts[1], GCCollectionCounts[2]);
                Console.WriteLine(output);
                output = string.Format("每秒GC计数    {0,-10} {1,-10} {2,-10}", GCCollectionCountsNow[0], GCCollectionCountsNow[1], GCCollectionCountsNow[2]);
                Console.WriteLine(output);
                output = string.Format("1秒GC最大     {0,-10} {1,-10} {2,-10}", MaxGCCollectionCounts1s[0], MaxGCCollectionCounts1s[1], MaxGCCollectionCounts1s[2]);
                Console.WriteLine(output);
                output = string.Format("5秒GC最大     {0,-10} {1,-10} {2,-10}", MaxGCCollectionCounts5s[0], MaxGCCollectionCounts5s[1], MaxGCCollectionCounts5s[2]);
                Console.WriteLine(output);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShowGCInfo()", false);
            }
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private static void ShowCmdHelpInfo(String cmd = null)
        {
            System.Console.WriteLine(string.Format("游戏服务器{0}:", GameManager.ServerLineID));
            System.Console.WriteLine("输入 help， 显示帮助信息");
            System.Console.WriteLine("输入 exit， 然后输入y退出？");
            System.Console.WriteLine("输入 gc， 执行垃圾回收");
            System.Console.WriteLine("输入 show dbconnect， 查看数据库链接信息");
            System.Console.WriteLine("输入 show baseinfo， 查看基础运行信息");
            System.Console.WriteLine("输入 show tcpinfo， 查看通讯相关信息");
            System.Console.WriteLine("输入 show copymapinfo， 查看副本相关信息");
            System.Console.WriteLine("输入 show gcinfo， 查看GC相关信息");
            //System.Console.WriteLine("输入 loadiplist， 加载IP白名单,仅允许白名单IP登录");
        }

        /// <summary>
        /// 垃圾回收
        /// </summary>
        private static void GarbageCollect(String cmd = null)
        {
            try
            {
                //释放内存
                GC.Collect();
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "GarbageCollect()", false);
            }
        }

        /// <summary>
        /// 读取密码,以星号显示
        /// </summary>
        /// <returns></returns>
        private static string ReadPasswd()
        {
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo k;
            while (true)
            {
                k = Console.ReadKey();

                if (k.Key == ConsoleKey.Enter)
                {
                    return sb.ToString();
                }
                if (Console.CursorLeft > 0)
                {
                    Console.CursorLeft--;
                    Console.Write("*");
                    sb.Append(k.KeyChar);
                }
            }
        }

        /// <summary>
        /// 切换压测模式
        /// </summary>
        /// <param name="cmd"></param>
        private static void SetTestMode(String cmd = null)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            if ("tmsk201405" == ReadPasswd())
            {
                if (cmd.IndexOf("testmode 5") == 0)
                {
                    GameManager.TestGamePerformanceMode = true;
                    GameManager.TestGamePerformanceAllPK = true;
                    Console.WriteLine("开启压测模式,全体PK");
                }
                else if (cmd.IndexOf("testmode 1") == 0)
                {
                    GameManager.TestGamePerformanceMode = true;
                    GameManager.TestGamePerformanceAllPK = false;
                    Console.WriteLine("开启压测模式,和平模式");
                }
                else
                {
                    GameManager.TestGamePerformanceMode = false;
                    GameManager.TestGamePerformanceAllPK = false;
                    Console.WriteLine("关闭压测模式");
                }
            }
        }

        delegate string PatchDelegate(string[] args);

        public static void RunPatchFromConsole(string cmd)
        {
            try
            {
                if (string.IsNullOrEmpty(cmd))
                {
                    return;
                }

                string arg = null;
                if ("tmsk201405" != ReadPasswd())
                {
                    return;
                }
                Console.WriteLine("输入补丁信息:");

                arg = Console.ReadLine();
                RunPatch(arg);
            }
            catch (System.Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
            }
        }
        public static void RunPatch(string arg, bool console = true)
        {
            try
            {
                if (string.IsNullOrEmpty(arg))
                {
                    return;
                }

                if (!string.IsNullOrEmpty(arg))
                {
                    char[] spliteChars = new char[] { ' '};
                    string[] args = arg.Split(spliteChars, StringSplitOptions.RemoveEmptyEntries);
                    if (null != args && args.Length >= 3 && !string.IsNullOrEmpty(args[0]) && !string.IsNullOrEmpty(args[1]) && !string.IsNullOrEmpty(args[2]))
                    {
                        string assemblyName = AppDomain.CurrentDomain.BaseDirectory + args[0];
                        if (File.Exists(assemblyName))
                        {
                            //加载程序集
                            Assembly t = Assembly.LoadFrom(assemblyName);
                            if (null != t)
                            {
                                //加载类型
                                Type a = t.GetType(args[1]);
                                if (null != a)
                                {
                                    MethodInfo mi1 = a.GetMethod(args[2]);
                                    if (null != mi1)
                                    {
                                        //静态方法的调用
                                        object[] param = new object[1]{args};
                                        string s2 = (string)mi1.Invoke(null, param);
                                        LogManager.WriteLog(LogTypes.SQL, "执行修补程序" + arg + ",结果:" + s2);
                                        if (console && null != s2 && s2.Length < 4096)
                                        {
                                            Console.WriteLine(s2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "执行修补程序异常");
            }
        }

        /// <summary>
        /// 玩家对象相关信息显示
        /// </summary>
        /// <param name="cmd"></param>
        public static void ShowObjectInfo(string cmd)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("在线玩家数{0}\n", GameManager.ClientMgr.GetClientCountFromDict());
                sb.AppendFormat("各地图人数\n{0}", GameManager.ClientMgr.GetAllMapRoleNumStr());
                sb.AppendFormat("地图对象引用的角色对象数\n{0}", GameManager.MapGridMgr.GetAllMapClientCountForConsole());
                sb.AppendLine("命令执行结束\n");
                Console.WriteLine(sb.ToString());
            }
            catch (System.Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "执行ShowGameClientInfo异常");
            }
        }

        /// <summary>
        /// 数据库链接信息
        /// </summary>
        private static void ShowDBConnectInfo(String cmd = null)
        {
            try
            {
                foreach (var item in ServerConsole.DBServerConnectDict)
                {
                    System.Console.WriteLine(item.Value);
                }

                foreach (var item in ServerConsole.LogDBServerConnectDict)
                {
                    System.Console.WriteLine(item.Value);
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShowDBConnectInfo()", false);
            }
        }

        /// <summary>
        /// 显示公共信息
        /// </summary>
        private static void ShowServerBaseInfo(String cmd = null)
        {
            // 通知界面修改连接数
            string info = string.Format("在线数量 {0}/{1}", GameManager.ClientMgr.GetClientCount(),
                Global._TCPManager.MySocketListener.ConnectedSocketsCount);

            System.Console.WriteLine(info);

            int workerThreads = 0;
            int completionPortThreads = 0;
            System.Threading.ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            //DB数据库指令对象信息
            info = string.Format("线程池信息 workerThreads={0}, completionPortThreads={1}", workerThreads, completionPortThreads);

            System.Console.WriteLine(info);

            //DB数据库指令对象信息
            info = string.Format("TCP事件读写缓存数量 readPool={0}/{2}, writePool={1}/{2}", Global._TCPManager.MySocketListener.ReadPoolCount, Global._TCPManager.MySocketListener.WritePoolCount, Global._TCPManager.MySocketListener.numConnections * 3);

            System.Console.WriteLine(info);

            //DB数据库指令对象信息
            info = string.Format("数据库指令数量 {0}", GameManager.DBCmdMgr.GetDBCmdCount());

            System.Console.WriteLine(info);

            //与DBServer的连接池信息
            info = string.Format("与DbServer的连接数量{0}/{1}", Global._TCPManager.tcpClientPool.GetPoolCount(), Global._TCPManager.tcpClientPool.InitCount);

            System.Console.WriteLine(info);

            //显示进出的Packet包的缓冲个数信息
            info = string.Format("TcpOutPacketPool个数:{0}, 实例: {1}, TcpInPacketPool个数:{2}, 实例: {3}, TCPCmdWrapper个数: {4}, SendCmdWrapper: {5}", Global._TCPManager.TcpOutPacketPool.Count, TCPOutPacket.GetInstanceCount(), Global._TCPManager.TcpInPacketPool.Count, TCPInPacket.GetInstanceCount(), TCPCmdWrapper.GetTotalCount(), SendCmdWrapper.GetInstanceCount());

            System.Console.WriteLine(info);

            //显示内存缓存信息
            info = Global._MemoryManager.GetCacheInfoStr();

            System.Console.WriteLine(info);

            //缓冲区满错误信息
            info = Global._FullBufferManager.GetFullBufferInfoStr();

            System.Console.WriteLine(info);

            //缓存命令数据包信息
            info = Global._TCPManager.GetAllCacheCmdPacketInfo();
            System.Console.WriteLine(info);
        }

        /// <summary>
        /// 显示Tcp相关信息
        /// </summary>
        private static void ShowServerTCPInfo(String cmd = null)
        {
            bool clear = cmd.Contains("/c");
            bool detail = cmd.Contains("/d");

            string info = string.Format("总接收字节: {0:0.00} MB", Global._TCPManager.MySocketListener.TotalBytesReadSize / (1024.0 * 1024.0));
            System.Console.WriteLine(info);
            info = string.Format("总发送字节: {0:0.00} MB", Global._TCPManager.MySocketListener.TotalBytesWriteSize / (1024.0 * 1024.0));
            System.Console.WriteLine(info);

            info = string.Format("总处理指令个数 {0}", TCPCmdHandler.TotalHandledCmdsNum);
            System.Console.WriteLine(info);
            info = string.Format("当前正在处理指令的线程数 {0}", TCPCmdHandler.GetHandlingCmdCount());
            System.Console.WriteLine(info);
            info = string.Format("单个指令消耗的最大时间 {0}", TCPCmdHandler.MaxUsedTicksByCmdID);
            System.Console.WriteLine(info);
            info = string.Format("消耗的最大时间指令ID {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID);
            System.Console.WriteLine(info);
            info = string.Format("发送调用总次数 {0}", Global._TCPManager.MySocketListener.GTotalSendCount);
            System.Console.WriteLine(info);
            info = string.Format("发送的最大包的大小 {0}", Global._SendBufferManager.MaxOutPacketSize);
            System.Console.WriteLine(info);
            info = string.Format("发送的最大包的指令ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID);
            System.Console.WriteLine(info);

            //////////////////////////////////////////
            info = string.Format("指令处理平均耗时（毫秒）{0}", ProcessSessionTask.processCmdNum != 0 ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum) : 0);
            System.Console.WriteLine(info);
            info = string.Format("指令处理耗时详情");
            System.Console.WriteLine(info);

            int count = 0;
            lock (ProcessSessionTask.cmdMoniter)
            {
                foreach (GameServer.Logic.PorcessCmdMoniter m in ProcessSessionTask.cmdMoniter.Values)
                {
                    Console.ForegroundColor = (ConsoleColor)(count % 5 + ConsoleColor.Green); //逐行设置字体颜色
                    if (detail)
                    {
                        if (count++ == 0)
                        {
                            info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}{4, 7}{5, 5}", "消息", "已处理次数", "平均处理时长", "总计消耗时长", "最大处理时长", "处理结果");
                            System.Console.WriteLine(info);
                        }
                        info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}{4, 13:0.##}{5, 3}/{6}/{7}", (TCPGameServerCmds)m.cmd, m.processNum, TimeUtil.TimeMS(m.avgProcessTime()), TimeUtil.TimeMS(m.processTotalTime), TimeUtil.TimeMS(m.processMaxTime), m.Num_Faild, m.Num_OK, m.Num_WithData);
                        System.Console.WriteLine(info);
                    }
                    else
                    {
                        if (count++ == 0)
                        {
                            info = string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", "消息", "已处理次数", "平均处理时长", "总计消耗时长");
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
                Console.ForegroundColor = ConsoleColor.White; //恢复字体颜色
            }
        }

        /// <summary>
        /// 显示副本相关信息
        /// </summary>
        private static void ShowCopyMapInfo(String cmd = null)
        {
            //显示内存缓存信息
            string info = GameManager.CopyMapMgr.GetCopyMapStrInfo();
            System.Console.WriteLine(info);

        }

        /// <summary>
        /// 显示所以角色的地图和位置(性能分析)
        /// </summary>
        /// <param name="cmd"></param>
        private static void ShowRoleInfo(String cmd = null)
        {
            List<GameClient> clientList = GameManager.ClientMgr.GetAllClients();
            StringBuilder sb = new StringBuilder();
            foreach (var client in clientList)
            {
                sb.AppendFormat("{0, -12} : {4, -11} : {5, -11} : {6, -11} : {1}({2},{3})\n", client.ClientData.RoleName, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY,
                    client.CodeRevision, client.MainExeVer, client.ResVer);
            }
            if (sb.Length == 0)
            {
                System.Console.WriteLine("没有玩家在线");
            }
            else
            {
                System.Console.WriteLine(sb.ToString());
            }
        }

        #endregion 命令功能

        #region 外部调用接口

        /// <summary>
        /// 与dbserver的链接信息词典
        /// </summary>
        public Dictionary<int, String> DBServerConnectDict = new Dictionary<int, string>();

        /// <summary>
        /// 增加数据服务器链接信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="info"></param>
        public void AddDBConnectInfo(int index, String info)
        {
            lock (DBServerConnectDict)
            {
                if (DBServerConnectDict.ContainsKey(index))
                {
                    DBServerConnectDict[index] = info;
                }
                else
                {
                    DBServerConnectDict.Add(index, info);
                }
            }
        }

        /// <summary>
        /// 与dbserver的链接信息词典
        /// </summary>
        public Dictionary<int, String> LogDBServerConnectDict = new Dictionary<int, string>();

        /// <summary>
        /// 增加数据服务器链接信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="info"></param>
        public void AddLogDBConnectInfo(int index, String info)
        {
            lock (LogDBServerConnectDict)
            {
                if (LogDBServerConnectDict.ContainsKey(index))
                {
                    LogDBServerConnectDict[index] = info;
                }
                else
                {
                    LogDBServerConnectDict.Add(index, info);
                }
            }
        }

        #endregion

        #region 游戏服务器具体功能部分

        /// <summary>
        /// 程序额外的名称
        /// </summary>
        private static string ProgramExtName = "";

        /// <summary>
        /// 初始化应用程序名称
        /// </summary>
        /// <returns></returns>
        private static void InitProgramExtName()
        {
            ProgramExtName = AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 初始化游戏资源信息
        /// 原来的 Window_Loaded(object sender, RoutedEventArgs e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InitServer()
        {
            InitProgramExtName();

            int workerThreads = 0;
            int completionPortThreads = 0;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMinThreads(Math.Max(workerThreads, 16), Math.Max(completionPortThreads, 64));
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMaxThreads(Math.Min(workerThreads, 16), Math.Min(completionPortThreads, 360));

            //long UnixStartTicks = DataHelper.ConvertToTicks("1970-01-01 08:00");
            //long ticks = DateTime.Now.Ticks / 10000;
            //long secs = (ticks - UnixStartTicks);

            if (!File.Exists(@"Policy.xml"))
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", @"Policy.xml"));
            }

            TCPPolicy.LoadPolicyServerFile(@"Policy.xml");

            Global.LoadLangDict();
            SearchTable.Init(14);

            System.Console.WriteLine("正在初始化游戏资源目录");

            //初始化游戏资源目录
            XElement xml = InitGameResPath();

            System.Console.WriteLine("正在初始化数据库连接");
            // 初始化数据库连接
            InitTCPManager(xml, true);

            System.Console.WriteLine("正在初始化GameRes压缩资源");

            /// 初始化GameRes压缩资源
            InitGameRes();

            

            System.Console.WriteLine("正在初始化游戏管理对象");

            try
            {
                // 初始化游戏管理对象
                InitGameManager(xml);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "InitGameManager", true);
            }

            // 初始化Lua环境
            LuaExeManager.getInstance().InitLuaEnv();

            System.Console.WriteLine("正在初始化游戏的所有地图和地图中的怪物");

            /// 初始化游戏的所有地图和地图中的怪物
            InitGameMapsAndMonsters();

            System.Console.WriteLine("正在设置后台工作者线程");

            //设置后台工作者线程
            eventWorker = new BackgroundWorker();
            eventWorker.DoWork += eventWorker_DoWork;

            dbCommandWorker = new BackgroundWorker();
            dbCommandWorker.DoWork += dbCommandWorker_DoWork;

            logDBCommandWorker = new BackgroundWorker();
            logDBCommandWorker.DoWork += logDBCommandWorker_DoWork;

            clientsWorker = new BackgroundWorker();
            clientsWorker.DoWork += clientsWorker_DoWork;

            buffersWorker = new BackgroundWorker();
            buffersWorker.DoWork += buffersWorker_DoWork;

            spriteDBWorker = new BackgroundWorker();
            spriteDBWorker.DoWork += spriteDBWorker_DoWork;

            othersWorker = new BackgroundWorker();
            othersWorker.DoWork += othersWorker_DoWork;

            FightingWorker = new BackgroundWorker();
            FightingWorker.DoWork += FightingWorker_DoWork;

            chatMsgWorker = new BackgroundWorker();
            chatMsgWorker.DoWork += chatMsgWorker_DoWork;

            fuBenWorker = new BackgroundWorker();
            fuBenWorker.DoWork += fuBenWorker_DoWork;

            dbWriterWorker = new BackgroundWorker();
            dbWriterWorker.DoWork += dbWriterWorker_DoWork;

            SocketSendCacheDataWorker = new BackgroundWorker();
            SocketSendCacheDataWorker.DoWork += SocketSendCacheDataWorker_DoWork;

            ShengXiaoGuessWorker = new BackgroundWorker();
            ShengXiaoGuessWorker.DoWork += ShengXiaoGuessWorker_DoWork;

            MainDispatcherWorker = new BackgroundWorker();
            MainDispatcherWorker.DoWork += MainDispatcherWorker_DoWork;

            //RoleExtensionWorker = new BackgroundWorker();
            //RoleExtensionWorker.DoWork += RoleExtensionWorker_DoWork;            

            /*CmdPacketProcessWorkers = new BackgroundWorker[MaxCmdPacketProcessWorkerNum];
            for (int nThread = 0; nThread < MaxCmdPacketProcessWorkerNum; nThread++)
            {
                CmdPacketProcessWorkers[nThread] = new BackgroundWorker();
                CmdPacketProcessWorkers[nThread].DoWork += CmdPacketProcessWorker_DoWork;
            }*/

//             MonsterProcessWorkers = new BackgroundWorker[MaxMonsterProcessWorkersNum];
//             for (int nThread = 0; nThread < MaxMonsterProcessWorkersNum; nThread++)
//             {
//                 MonsterProcessWorkers[nThread] = new BackgroundWorker();
//                 MonsterProcessWorkers[nThread].DoWork += MonsterProcessWorker_DoWork;
//             }
#if UseTimer
            ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask((s, e) =>
                {
                    GameManager.GridMagicHelperMgrEx.ExecuteAllItemsEx();
                }), 1000, 200);

            //基于场景调度驱动
            for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
            {
                int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;

                if (mapCode == (int)FRESHPLAYERSCENEINFO.FRESHPLAYERMAPCODEID)
                {
                    for (int subMapCode = 0; subMapCode < 25; subMapCode++)
                    {
                        ScheduleExecutor2.Instance.scheduleExecute(new MonsterTask(mapCode, subMapCode), 0, 80);
                    }
                }
                else
                {
                    ScheduleExecutor2.Instance.scheduleExecute(new MonsterTask(mapCode), 0, 80);
                }

            }
#else
            monsterExecutor = new ScheduleExecutor(MaxMonsterProcessWorkersNum);

            monsterExecutor.start();

            //基于场景调度驱动
            for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
            {
                int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;

                if (mapCode == (int)FRESHPLAYERSCENEINFO.FRESHPLAYERMAPCODEID)
                {
                    for (int subMapCode = 0; subMapCode < 25; subMapCode++)
                    {
                        monsterExecutor.scheduleExecute(new MonsterTask(mapCode, subMapCode), 0, 80);
                    }
                }
                else
                {
                    monsterExecutor.scheduleExecute(new MonsterTask(mapCode), 0, 80);
                }

            }
#endif

            /*BackgroundClientsWorkers = new BackgroundWorker[MaxBackgroundClientsWorkersNum];
            for (int nThread = 0; nThread < MaxBackgroundClientsWorkersNum; nThread++)
            {
                BackgroundClientsWorkers[nThread] = new BackgroundWorker();
                BackgroundClientsWorkers[nThread].DoWork += clientsWorker_DoWork;
            }*/

            Gird9UpdateWorkers = new BackgroundWorker[MaxGird9UpdateWorkersNum];
            for (int nThread = 0; nThread < MaxGird9UpdateWorkersNum; nThread++)
            {
                Gird9UpdateWorkers[nThread] = new BackgroundWorker();
                Gird9UpdateWorkers[nThread].DoWork += Gird9UpdateWorker_DoWork;
            }

            /// <summary>
            /// 角色故事版驱动线程
            /// </summary>
            RoleStroyboardDispatcherWorker = new BackgroundWorker();
            RoleStroyboardDispatcherWorker.DoWork += RoleStroyboardDispatcherWorker_DoWork;

            //是否显示异常时的对话框
            UnhandedException.ShowErrMsgBox = false;

            // 设置开始接受新的用户
            // LoadIPList("1");
            Global._TCPManager.MySocketListener.DontAccept = false;

            //启动主循环调度线程
            if (!MainDispatcherWorker.IsBusy)
            {
                MainDispatcherWorker.RunWorkerAsync();
            }
            
            //启动角色辅助伤害调度线程
            //if (!RoleExtensionWorker.IsBusy)
            //{
            //    RoleExtensionWorker.RunWorkerAsync();
            //}

            //启动数据包处理线程
            /*for (int nThread = 0; nThread < MaxCmdPacketProcessWorkerNum; nThread++)
            {
                if (!CmdPacketProcessWorkers[nThread].IsBusy)
                {
                    CmdPacketProcessWorkers[nThread].RunWorkerAsync();
                }
            }*/

            //启动怪物处理线程
//             for (int nThread = 0; nThread < MaxMonsterProcessWorkersNum; nThread++)
//             {
//                 if (!MonsterProcessWorkers[nThread].IsBusy)
//                 {
//                     MonsterProcessWorkers[nThread].RunWorkerAsync(nThread);
//                 }
//             }

            //启动9宫格更新处理线程
            for (int nThread = 0; nThread < MaxGird9UpdateWorkersNum; nThread++)
            {
                if (!Gird9UpdateWorkers[nThread].IsBusy)
                {
                    Gird9UpdateWorkers[nThread].RunWorkerAsync(nThread);
                }
            }            

            //启动角色故事版调度线程
            if (!RoleStroyboardDispatcherWorker.IsBusy)
            {
                RoleStroyboardDispatcherWorker.RunWorkerAsync();
            }

            //初始化线程驱动定时器
            StartThreadPoolDriverTimer();

            //ProgramExtName = GameManager.GameConfigMgr.GetGameConfigItemStr("ext_program_name", "unknown");

            //更改DB游戏参数
            Global.UpdateDBGameConfigg("gameserver_version", GetVersionDateTime());

            //重新恢复显示PK之王
            GameManager.ArenaBattleMgr.ReShowPKKing();

            // 载入世界等级
            WorldLevelManager.getInstance().ResetWorldLevel();

            System.Console.WriteLine("正在初始化通信监听");
            // 初始化通信监听，睡眠3秒后才启动
            Thread.Sleep(3000);
            InitTCPManager(xml, false);

            System.Console.WriteLine(string.Format("服务器GC运行在:{0}, {1}", GCSettings.IsServerGC ? "服务器模式" : "工作站模式", GCSettings.LatencyMode));
            System.Console.WriteLine("服务器已经正常启动");
        }

        //关闭服务器
        public void ExitServer()
        {
            if (NeedExitServer)
            {
                return;
            }

            //全局服务管理器关闭
            GlobalServiceManager.showdown();

            //全局服务管理器销毁
            GlobalServiceManager.destroy();

            Global._TCPManager.Stop(); //停止TCP的侦听，否则mono无法正常退出

            Window_Closing();

            System.Console.WriteLine("正在尝试关闭服务器,看到服务器关闭完毕提示后回车退出系统");

            if (0 == GetServerPIDFromFile())
            {
                String cmd = System.Console.ReadLine();

                while (true)
                {
                    if (MainDispatcherWorker.IsBusy)
                    {
                        System.Console.WriteLine("正在尝试关闭服务器");
                        cmd = System.Console.ReadLine();
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                StopThreadPoolDriverTimer();
            }
        }

        #region 初始化部分

        /// <summary>
        /// 初始化游戏资源目录
        /// </summary>
        private XElement InitGameResPath()
        {
            XElement xml = null;

            try
            {
                xml = XElement.Load(@"AppConfig.xml");
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", @"AppConfig.xml"));
            }

            //游戏资源的存放位置
            Global.AbsoluteGameResPath = Global.GetSafeAttributeStr(xml, "Resource", "Path");
            
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            if (Global.AbsoluteGameResPath.IndexOf("$SERVER$") >= 0)
            {
                Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("$SERVER$", appPath);
            }

            if (!string.IsNullOrEmpty(Global.AbsoluteGameResPath))
            {
                Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.Replace("\\", "/");
                Global.AbsoluteGameResPath = Global.AbsoluteGameResPath.TrimEnd('/');
            }

            return xml;
        }

        /// <summary>
        /// 初始化GameRes压缩资源
        /// </summary>
        private void InitGameRes()
        {
            //加载配置参数
            try
            {
                Global.AddXElement(Global.GAME_CONFIG_SETTINGS_NAME, Global.GetGameResXml(Global.GAME_CONFIG_SETTINGS_FILE));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败 错误信息:{1}", Global.GAME_CONFIG_SETTINGS_FILE, ex.Message));
            }

            //游戏的升级经验值配置文件
            try
            {
                Global.AddXElement(Global.GAME_CONFIG_LEVELUP_NAME, Global.GetGameResXml(Global.GAME_CONFIG_LEVELUP_FILE));
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", Global.GAME_CONFIG_LEVELUP_FILE));
            }

            //游戏的装备和物品配置文件
            try
            {
                Global.AddXElement(Global.GAME_CONFIG_GOODS_NAME, Global.GetGameResXml(Global.GAME_CONFIG_GOODS_FILE));
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", Global.GAME_CONFIG_GOODS_FILE));
            }

            /// 单元格移动需要的Tick
            Data.WalkUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "SpeedConfig"), "WalkUnitCost");
            Data.RunUnitCost = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "SpeedConfig"), "RunUnitCost");

            //获取移动或者动画的速度列表
            string[] ticks = Global.GetSafeAttribute(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "SpeedConfig"), "Tick").Value.Split(',');
            Data.SpeedTickList = new int[ticks.Length];
            for (int i = 0; i < ticks.Length; i++)
            {
                Data.SpeedTickList[i] = Convert.ToInt32(ticks[i]);
            }

            //获取距离相关配置
            XElement distConfig = Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "DistanceConfig");

            /// 走一步的距离
            Data.WalkStepWidth = (int)Global.GetSafeAttributeLong(distConfig, "WalkStepWidth");

            /// 跑一步的距离
            Data.RunStepWidth = (int)Global.GetSafeAttributeLong(distConfig, "RunStepWidth");

            /// 发起物理攻击的距离
            Data.MaxAttackDistance = (int)Global.GetSafeAttributeLong(distConfig, "MaxAttackDistance");

            /// 物理攻击时的最短距离
            Data.MinAttackDistance = (int)Global.GetSafeAttributeLong(distConfig, "MinAttackDistance");

            /// 发起魔法攻击的距离
            Data.MaxMagicDistance = (int)Global.GetSafeAttributeLong(distConfig, "MaxMagicDistance");

            /// 最小攻击间隔
            Data.MaxAttackSlotTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "SpeedConfig"), "MaxAttackSlotTick");

            //获取精灵相关配置
            XElement SpriteConfig = Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "SpriteConfig");

            /// 生命条的宽度
            Data.LifeTotalWidth = (int)Global.GetSafeAttributeLong(SpriteConfig, "LifeTotalWidth");

            /// 精灵占据的格子宽度(个数)
            Data.HoldWidth = (int)Global.GetSafeAttributeLong(SpriteConfig, "HoldWidth");

            /// 精灵占据的格子高度(个数)
            Data.HoldHeight = (int)Global.GetSafeAttributeLong(SpriteConfig, "HoldHeight");

            /// <summary>
            /// 可以抢夺别人的物品包的最大时间间隔
            /// </summary>
            Data.GoodsPackOvertimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "GoodsPack"), "MaxOvertimeTick");

            /// <summary>
            /// 包裹消失的时间隔
            /// </summary>
            Data.PackDestroyTimeTick = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "GoodsPack"), "PackDestroyTimeTick");

            /// <summary>
            /// 最大的任务追踪个数
            /// </summary>
            Data.TaskMaxFocusCount = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "Task"), "MaxFocusNum");

            //原地复活的道具ID
            Data.AliveGoodsID = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "Alive"), "GoodsID");

            //原地复活需要的最大级别
            Data.AliveMaxLevel = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "Alive"), "MaxLevel");

            //是否自动拾取物品进入背包
            Data.AutoGetThing = (int)Global.GetSafeAttributeLong(Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "Bag"), "AutoGetThing");

            int maxLevel = 0;

            //加载升级经验值表到内存
            IEnumerable<XElement> levelList = Global.XmlInfo[Global.GAME_CONFIG_LEVELUP_NAME].Elements("Experience");
            if (null != levelList)
            {
                int listCount = levelList.Count();
                maxLevel = listCount;
                Data.LevelUpExperienceList = new long[listCount];
                for (int i = 0; i < listCount; i++)
                {
                    Data.LevelUpExperienceList[i] = Convert.ToInt64(levelList.Single(X => X.Attribute("Level").Value == i.ToString()).Attribute("Value").Value);
                }
            }

            //加载打坐收益表
            LoadRoleSitExpList(maxLevel);

            //加载角色属性表
            LoadRoleBasePropItems(maxLevel);

            // 加载转职 [9/29/2013 LiaoWei]
            LoadRoleZhuanZhiInfo();

            // 加载转生 [9/29/2013 LiaoWei]
            //LoadRoleZhuanShengInfo();
            GameManager.ChangeLifeMgr.LoadRoleZhuanShengInfo();

            // 加载职业属性加点表 [9/29/2013 LiaoWei]
            LoadRoleOccupationAddPointInfo();

            // 加载转生家电信息 [3/6/2014 LiaoWei]
            LoadRoleChangeLifeAddPointInfo();

            // 加载武器佩戴限制信息 【7/11/2014 ChenXiaojun】
            WeaponAdornManager.LoadWeaponAdornInfo();
        
            // 血色城堡信息 [11/04/2013 LiaoWei] 
            LoadBloodCastleDataInfo();

            //初始化所有地图的摆摊位置列表
            InitMapStallPosList();

            //初始化所有地图的名称字典
            InitMapNameDictionary();
            //初始化首充配置表
            Global.InitFirstChargeConfigData();
        }

        /// <summary>
        /// 加载打坐收益表
        /// </summary>
        /// <param name="maxLevel"></param>
        private void LoadRoleSitExpList(int maxLevel)
        {
            Data.RoleSitExpList = new RoleSitExpItem[maxLevel];
            XElement xmlFile = null;

            try
            {
                xmlFile = Global.GetGameResXml(string.Format("Config/RoleSiteExp.xml"));
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/RoleSiteExp.xml")));
            }

            IEnumerable<XElement> xmlItems = xmlFile.Elements();
            foreach (var xmlItem in xmlItems)
            {
                int level = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                if (level >= Data.RoleSitExpList.Length)
                {
                    break;
                }

                Data.RoleSitExpList[level] = new RoleSitExpItem()
                {
                    Level = level,
                    Experience = (int)Global.GetSafeAttributeLong(xmlItem, "Experience"),
                    InterPower = (int)Global.GetSafeAttributeLong(xmlItem, "InterPower"),
                    SkilledDegrees = (int)Global.GetSafeAttributeLong(xmlItem, "SkilledDegrees"),
                    PKPoint = (int)Global.GetSafeAttributeLong(xmlItem, "PkPoints"),
                };
            }
        }

        /// <summary>
        /// 加载角色属性表
        /// </summary>
        /// <param name="maxLevel"></param>
        private void LoadRoleBasePropItems(int maxLevel)
        {
            //加载角色基础属性值列表
            for (int i = 0; i < 3; i++)
            {
                RoleBasePropItem[] roleBasePropItems = new RoleBasePropItem[maxLevel];
                XElement xmlFile = null;

                try
                {
                    xmlFile = Global.GetGameResXml(string.Format("Config/Roles/{0}.xml", i));
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/{0}.xml", i)));
                }
                int j = 0;
                IEnumerable<XElement> propLevels = xmlFile.Elements("Levels").Elements();
                foreach (var xmlItem in propLevels)
                {
                    roleBasePropItems[j] = new RoleBasePropItem()
                    {
                        LifeV = Global.GetSafeAttributeDouble(xmlItem, "LifeV"),
                        MagicV = Global.GetSafeAttributeDouble(xmlItem, "MagicV"),
                        MinDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV"),
                        MaxDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV"),
                        MinMDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV"),
                        MaxMDefenseV = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV"),
                        MinAttackV = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV"),
                        MaxAttackV = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV"),
                        MinMAttackV = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV"),
                        MaxMAttackV = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV"),
                        RecoverLifeV = Global.GetSafeAttributeDouble(xmlItem, "RecoverLifeV"),
                        RecoverMagicV = Global.GetSafeAttributeDouble(xmlItem, "RecoverMagicV"),
                        Dodge = Global.GetSafeAttributeDouble(xmlItem, "Dodge"),
                        HitV = Global.GetSafeAttributeDouble(xmlItem, "HitV"),
                        PhySkillIncreasePercent = Global.GetSafeAttributeDouble(xmlItem, "PhySkillIncreasePercent"),
                        MagicSkillIncreasePercent = Global.GetSafeAttributeDouble(xmlItem, "MagicSkillIncreasePercent"),
                        AttackSpeed = Global.GetSafeAttributeDouble(xmlItem, "AttackSpeed"),
                    };

                    j++;
                    if (j >= roleBasePropItems.Length)
                    {
                        break;
                    }
                }

                Data.RoleBasePropList.Add(roleBasePropItems);
            }
        }

        
        /// <summary>
        /// 转职表  [9/28/2013 LiaoWei]
        /// </summary>
        private void LoadRoleZhuanZhiInfo()
        {
            //加载角色转职信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanZhi.xml"));
                if (null == xmlFile) 
                    return ;

                IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ZhuanZhis").Elements();
                foreach (var xmlItem in ChgOccpXEle)
                {
                    if (null != xmlItem)
                    {
                        ChangeOccupInfo tmpChgOccuInfo = new ChangeOccupInfo();

                        int nOccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");

                        tmpChgOccuInfo.OccupationID     = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
                        tmpChgOccuInfo.NeedLevel        = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
                        tmpChgOccuInfo.NeedMoney        = (int)Global.GetSafeAttributeLong(xmlItem, "NeedJinBi");
                        tmpChgOccuInfo.AwardPropPoint   = (int)Global.GetSafeAttributeLong(xmlItem, "AwardShuXing");

                        string sGoodsID = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
                        if (string.IsNullOrEmpty(sGoodsID))
                            LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空"));
                        else
                        {
                            string[] fields = sGoodsID.Split('|');
                            if (fields.Length <= 0)
                                LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空"));
                            else
                                tmpChgOccuInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID, "转职文件"); //将物品字符串列表解析成物品数据列表
                        }

                        string sGoodsID1 = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
                        if (string.IsNullOrEmpty(sGoodsID1))
                            LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空"));
                        else
                        {
                            string[] fields1 = sGoodsID1.Split('|');
                            if (fields1.Length <= 0)
                                LogManager.WriteLog(LogTypes.Warning, string.Format("转职文件NeedGoods为空"));
                            else
                                tmpChgOccuInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID1, "转职文件"); //将物品字符串列表解析成物品数据列表
                        }

                        Data.ChangeOccupInfoList.Add(nOccupationID, tmpChgOccuInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml")));
            }

        }

        /// <summary>
        /// 转生表  [9/28/2013 LiaoWei]
        /// </summary>
        /*private void LoadRoleZhuanShengInfo()
        {
            //加载角色转生信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanSheng.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ZhuanShengs").Elements();
                foreach (var xmlItem in ChgOccpXEle)
                {
                    if (null != xmlItem)
                    {
                        ChangeLifeInfo tmpChgLifeInfo = new ChangeLifeInfo();

                        int nLifeID = (int)Global.GetSafeAttributeLong(xmlItem, "ChangeLifeID");

                        tmpChgLifeInfo.ChangeLifeID = (int)Global.GetSafeAttributeLong(xmlItem, "ChangeLifeID");
                        tmpChgLifeInfo.NeedLevel = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
                        tmpChgLifeInfo.NeedMoney = (int)Global.GetSafeAttributeLong(xmlItem, "NeedJinBi");
                        tmpChgLifeInfo.NeedMoJing = (int)Global.GetSafeAttributeLong(xmlItem, "NeedMoJing");
                        tmpChgLifeInfo.AwardPropPoint = (int)Global.GetSafeAttributeLong(xmlItem, "AwardShuXing");
                        tmpChgLifeInfo.ExpProportion = Global.GetSafeAttributeDouble(xmlItem, "ExpProportion");

                        string sGoodsID = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
                        if (string.IsNullOrEmpty(sGoodsID))
                            LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空"));
                        else
                        {
                            string[] fields = sGoodsID.Split('|');
                            if (fields.Length <= 0)
                                LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空"));
                            else
                                tmpChgLifeInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID, "转生文件"); //将物品字符串列表解析成物品数据列表
                        }

                        string sGoodsID1 = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
                        if (string.IsNullOrEmpty(sGoodsID1))
                            LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空"));
                        else
                        {
                            string[] fields1 = sGoodsID1.Split('|');
                            if (fields1.Length <= 0)
                                LogManager.WriteLog(LogTypes.Warning, string.Format("转生文件NeedGoods为空"));
                            else
                                tmpChgLifeInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(sGoodsID1, "转生文件"); //将物品字符串列表解析成物品数据列表
                        }

                        Data.ChangeLifeInfoList.Add(nLifeID, tmpChgLifeInfo);
                        if (nLifeID > Data.MaxChangeLifeCount)
                        {
                            Data.MaxChangeLifeCount = nLifeID;
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanSheng.xml")));
            }

        }*/

        /// <summary>
        /// 职业加点信息表  [9/28/2013 LiaoWei]
        /// </summary>
        private void LoadRoleOccupationAddPointInfo()
        {
            //加载角色转职信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Roles/OccupationAddPoint.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ShuXings").Elements();
                foreach (var xmlItem in ChgOccpXEle)
                {
                    if (null != xmlItem)
                    {
                        OccupationAddPointInfo tmpInfo = new OccupationAddPointInfo();

                        int nOccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");

                        tmpInfo.OccupationID = (int)Global.GetSafeAttributeLong(xmlItem, "OccupationID");
                        tmpInfo.AddPoint = (int)Global.GetSafeAttributeLong(xmlItem, "JiaDian");

                        Data.OccupationAddPointInfoList.Add(nOccupationID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanZhi.xml")));
            }

        }

        /// <summary>
        // 转生加点信息 [3/6/2014 LiaoWei]
        /// </summary>
        private void LoadRoleChangeLifeAddPointInfo()
        {
            //加载角色转职信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Roles/ZhuanShengAddPoint.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> ChgOccpXEle = xmlFile.Elements("ShuXings").Elements();
                foreach (var xmlItem in ChgOccpXEle)
                {
                    if (null != xmlItem)
                    {
                        ChangeLifeAddPointInfo tmpInfo = new ChangeLifeAddPointInfo();

                        int nChangeLev = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuanShengLevel");

                        tmpInfo.ChangeLevel = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuanShengLevel");
                        tmpInfo.AddPoint = (int)Global.GetSafeAttributeLong(xmlItem, "JiaDian");

                        // 增加力量、敏捷、智慧、体力上限
                        tmpInfo.nStrLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Strength");
                        tmpInfo.nDexLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Dexterity");
                        tmpInfo.nIntLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Intelligence");
                        tmpInfo.nConLimit = (int)Global.GetSafeAttributeLong(xmlItem, "Constitution");

                        Data.ChangeLifeAddPointInfoList.Add(nChangeLev, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanShengAddPoint.xml")));
            }

        }

        /// <summary>
        /// 血色城堡信息 [11/04/2013 LiaoWei] 
        /// </summary>
        private void LoadBloodCastleDataInfo()
        {
            //加载血色城堡信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/BloodCastleInfo.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> BloodCastleXEle = xmlFile.Elements("BloodCastleInfos").Elements();
                foreach (var xmlItem in BloodCastleXEle)
                {
                    if (null != xmlItem)
                    {
                        BloodCastleDataInfo tmpInfo = new BloodCastleDataInfo();

                        int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");

                        tmpInfo.MapCode = nMapCodeID;
                        tmpInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
                        tmpInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");
                        tmpInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxEnter");
                        tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
                        tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");

                        string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
                        string[] fields = goodsIDs.Split(',');
                        int[] nArray = Global.StringArray2IntArray(fields);

                        tmpInfo.NeedGoodsID = nArray[0];
                        tmpInfo.NeedGoodsNum= nArray[1];
                        tmpInfo.MaxPlayerNum= (int)Global.GetSafeAttributeLong(xmlItem, "MaxPlayer");
                        tmpInfo.NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Level");
                        tmpInfo.NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Num");
                        tmpInfo.NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster2ID");
                        tmpInfo.NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster2Num");
                        tmpInfo.NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedCreateMonster2Num");
                        tmpInfo.NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xmlItem, "NeedCreateMonster2Pos");
                        tmpInfo.NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xmlItem, "NeedCreateMonster2Radius");
                        tmpInfo.NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xmlItem, "PursuitRadius");
                        tmpInfo.GateID = (int)Global.GetSafeAttributeLong(xmlItem, "GateID");
                        tmpInfo.GatePos = Global.GetSafeAttributeStr(xmlItem, "GatePos");
                        tmpInfo.CrystalID = (int)Global.GetSafeAttributeLong(xmlItem, "CrystalID");
                        tmpInfo.CrystalPos = Global.GetSafeAttributeStr(xmlItem, "CrystalPos");
                        tmpInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xmlItem, "TimeModulus");
                        tmpInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xmlItem, "ExpModulus");
                        tmpInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xmlItem, "MoneyModulus");

                        string goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem1");
                        string[] sfields = goodsID.Split('|');
                        tmpInfo.AwardItem1 = sfields;

                        goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem2");
                        sfields = goodsID.Split('|');
                        tmpInfo.AwardItem2 = sfields;

                        List<string> timePointsList = new List<string>();
                        string[] sField = null;
                        string timePoints = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
                        if (null != timePoints && timePoints != "")
                        {
                            sField = timePoints.Split(',');
                            for (int i = 0; i < sField.Length; i++)
                                timePointsList.Add(sField[i].Trim());
                        }

                        tmpInfo.BeginTime = timePointsList;
                        tmpInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xmlItem, "PrepareTime");
                        tmpInfo.DurationTime= (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
                        tmpInfo.LeaveTime   = (int)Global.GetSafeAttributeLong(xmlItem, "LeaveTime");
                        tmpInfo.DiaoXiangID = (int)Global.GetSafeAttributeLong(xmlItem, "DiaoXiangID");
                        tmpInfo.DiaoXiangPos = Global.GetSafeAttributeStr(xmlItem, "DiaoXiangPos");

                        Data.BloodCastleDataInfoList.Add(nMapCodeID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BloodCastleInfo.xml")));
            }

        }


        /// <summary>
        /// 副本评分信息 [11/15/2013 LiaoWei]
        /// </summary>
        private void LoadCopyScoreDataInfo()
        {
            //加载副本评分信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/FuBenPingFen.xml"));
                if (null == xmlFile)
                    return;

                int[] nArray = GameManager.systemParamsList.GetParamValueIntArrayByName("CopyScoreDataMapInfo");

                List<CopyScoreDataInfo> CopyScoreList = new List<CopyScoreDataInfo>();

                IEnumerable<XElement> CopyScoreXEle = xmlFile.Elements("CopyScoreInfos").Elements();
                foreach (var xmlItem in CopyScoreXEle)
                {
                    if (null != xmlItem)
                    {
                        CopyScoreDataInfo tmpInfo = new CopyScoreDataInfo();

                        int nCopyMapID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");

                        tmpInfo.CopyMapID   = nCopyMapID;
                        tmpInfo.ScoreName   = Global.GetSafeAttributeStr(xmlItem, "PingFenName");
                        tmpInfo.MinScore    = (int)Global.GetSafeAttributeLong(xmlItem, "MinFen");
                        tmpInfo.MaxScore    = (int)Global.GetSafeAttributeLong(xmlItem, "MaxFen");
                        tmpInfo.ExpModulus  = Global.GetSafeAttributeDouble(xmlItem, "ExpXiShu");
                        tmpInfo.MoneyModulus = Global.GetSafeAttributeDouble(xmlItem, "JinBiXiShu");
                        tmpInfo.FallPacketID= (int)Global.GetSafeAttributeLong(xmlItem, "GoodsList");

                        CopyScoreList.Add(tmpInfo);
                    }
                }

                for (int i = 0; i < nArray.Length; ++i)
                {
                    int nID = nArray[i];
                    List<CopyScoreDataInfo> CopyScoreListTmp = new List<CopyScoreDataInfo>();
                    
                    for (int nIndex = 0; nIndex < CopyScoreList.Count; ++nIndex)
                    {
                        if (CopyScoreList[nIndex].CopyMapID == nID)
                            CopyScoreListTmp.Add(CopyScoreList[nIndex]);
                    }
                    Data.CopyScoreDataInfoList.Add(nID, CopyScoreListTmp);
                    //CopyScoreListTmp.Clear();
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FuBenPingFen.xml")));
            }

        }

        /// <summary>
        /// 新手场景类 [12/1/2013 LiaoWei]
        /// </summary>
        private void LoadFreshPlayerCopySceneInfo()
        {
            //加载副本评分信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/FreshPlayerCopySceneInfo.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> CopyScoreXEle = xmlFile.Elements("FreshPlayerCopySceneInfos").Elements();
                foreach (var xmlItem in CopyScoreXEle)
                {
                    if (null != xmlItem)
                    {   
                        FreshPlayerCopySceneInfo FreshPlayerCopySceneInfo = new FreshPlayerCopySceneInfo();
                        
                        FreshPlayerCopySceneInfo.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
                        FreshPlayerCopySceneInfo.NeedKillMonster1Level = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Level");
                        FreshPlayerCopySceneInfo.NeedKillMonster1Num = (int)Global.GetSafeAttributeLong(xmlItem, "NeedKillMonster1Num");
                        FreshPlayerCopySceneInfo.NeedKillMonster2ID = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiID");
                        FreshPlayerCopySceneInfo.NeedKillMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "KillWuShiNum");
                        FreshPlayerCopySceneInfo.NeedCreateMonster2Num = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiNum");
                        FreshPlayerCopySceneInfo.NeedCreateMonster2Pos = Global.GetSafeAttributeStr(xmlItem, "WuShiPos");
                        FreshPlayerCopySceneInfo.NeedCreateMonster2Radius = (int)Global.GetSafeAttributeLong(xmlItem, "WuShiRadius");
                        FreshPlayerCopySceneInfo.NeedCreateMonster2PursuitRadius = (int)Global.GetSafeAttributeLong(xmlItem, "PursuitRadius");
                        FreshPlayerCopySceneInfo.GateID = (int)Global.GetSafeAttributeLong(xmlItem, "GateID");
                        FreshPlayerCopySceneInfo.GatePos = Global.GetSafeAttributeStr(xmlItem, "GatePos");
                        FreshPlayerCopySceneInfo.CrystalID = (int)Global.GetSafeAttributeLong(xmlItem, "CrystalID");
                        FreshPlayerCopySceneInfo.CrystalPos = Global.GetSafeAttributeStr(xmlItem, "CrystalPos");
                        FreshPlayerCopySceneInfo.DiaoXiangID = (int)Global.GetSafeAttributeLong(xmlItem, "DiaoXiangID");
                        FreshPlayerCopySceneInfo.DiaoXiangPos = Global.GetSafeAttributeStr(xmlItem, "DiaoXiangPos");

                        Data.FreshPlayerSceneInfo = FreshPlayerCopySceneInfo;
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/FreshPlayerCopySceneInfo.xml")));
            }

        }

        /// <summary>
        /// 任务星级信息 [12/3/2013 LiaoWei]
        /// </summary>
        private void LoadTaskStarDataInfo()
        {
            //加载任务星级信息表
            try
            {
                string fileName = "Config/TaskStarInfos.xml";
                //xmlFile = Global.GetGameResXml(string.Format("Config/TaskStarInfo.xml"));
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xml)
                    return;

                IEnumerable<XElement> TaskStarInfoXEle = xml.Elements("TaskStarInfos").Elements();
                foreach (var xmlItem in TaskStarInfoXEle)
                {
                    if (null != xmlItem)
                    {
                        TaskStarDataInfo TaskStarInfo = new TaskStarDataInfo();

                        TaskStarInfo.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                        TaskStarInfo.ExpModulus = Global.GetSafeAttributeDouble(xmlItem, "EXPXiShu");
                        TaskStarInfo.BindYuanBaoModulus = Global.GetSafeAttributeDouble(xmlItem, "BindZhuanShiXiShu");
                        TaskStarInfo.StarSoulModulus = Global.GetSafeAttributeDouble(xmlItem, "XingHunXiShu");
                        TaskStarInfo.Probability = (int)(Global.GetSafeAttributeDouble(xmlItem, "GaiLv") * 10000);

                        Data.TaskStarInfo.Add(TaskStarInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/TaskStarInfos.xml")));
            }

        }

        /// <summary>
        /// 日常跑环任务奖励信息 [12/3/2013 LiaoWei]
        /// </summary>
        private void LoadDailyCircleTaskAwardInfo()
        {
            //加载副本评分信息表
            try
            {
                string fileName = "Config/DailyCircleTaskAward.xml";
                XElement xmlFile = null;
                //xmlFile = Global.GetGameResXml(string.Format("Config/DailyCircleTaskAward.xml.xml"));
                xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> xmlItems = xmlFile.Elements();
                foreach (var xmlItem in xmlItems)
                {
                    if (xmlItem != null)
                    {
                        DailyCircleTaskAwardInfo DailyCircleTaskAward = new DailyCircleTaskAwardInfo();

                        DailyCircleTaskAward.ID                 = (int)Global.GetSafeAttributeLong(xmlItem, "Id");
                        DailyCircleTaskAward.MinChangeLifeLev   = (int)Global.GetSafeAttributeLong(xmlItem, "MinzhuanshengLevel");
                        DailyCircleTaskAward.MaxChangeLifeLev   = (int)Global.GetSafeAttributeLong(xmlItem, "MaxzhuanshengLevel");
                        DailyCircleTaskAward.MinLev             = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
                        DailyCircleTaskAward.MaxLev             = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
                        DailyCircleTaskAward.Experience         = (int)Global.GetSafeAttributeLong(xmlItem, "EXP");
                        DailyCircleTaskAward.XingHun = (int)Global.GetSafeAttributeLong(xmlItem, "XingHun");
                        
                        string strGoodInfo = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
                        string[] fields = strGoodInfo.Split(',');
                        int[] nArray = Global.StringArray2IntArray(fields);
                        DailyCircleTaskAward.GoodsID = nArray[0];
                        DailyCircleTaskAward.GoodsNum = nArray[1];
                        DailyCircleTaskAward.Binding = nArray.Length >= 3 ? nArray[2] : 1;
                        Data.DailyCircleTaskAward.Add(DailyCircleTaskAward);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/DailyCircleTaskAward.xml")));
            }

        }

        /// <summary>
        /// 讨伐任务额外奖励信息（全部完成）
        /// </summary>
        private void LoadTaofaTaskAwardInfo()
        {
            //加载副本评分信息表
            try
            {
                //目前只有绑钻奖励
                int ExBangZuan = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskAward");
                Data.TaofaTaskExAward.BangZuan = ExBangZuan;
                Global.MaxTaofaTaskNumForMU = (int)GameManager.systemParamsList.GetParamValueIntByName("PriceTaskNum");
            }
            catch (Exception)
            {
                throw new Exception(string.Format("load PriceTaskAward : {0} fail", string.Format("systemParamsList.PriceTaskAward")));
            }

        }

        /// <summary>
        /// 战斗力信息表[12/13/2013 LiaoWei]
        /// </summary>
        private void LoadCombatForceInfoInfo()
        {
            //加载副本评分信息表
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Roles/CombatForceInfo.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> xmlItems = xmlFile.Elements();
                foreach (var xmlItem in xmlItems)
                {
                    if (xmlItem != null)
                    {
                        CombatForceInfo CombatForceData = new CombatForceInfo();
                        int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");

                        CombatForceData.ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
                        CombatForceData.MaxHPModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeV");
                        CombatForceData.MaxMPModulus = Global.GetSafeAttributeDouble(xmlItem, "MagicV");
                        CombatForceData.MinPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinDefenseV");
                        CombatForceData.MaxPhysicsDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxDefenseV");
                        CombatForceData.MinMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMDefenseV");
                        CombatForceData.MaxMagicDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMDefenseV");
                        CombatForceData.MinPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinAttackV");
                        CombatForceData.MaxPhysicsAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxAttackV");
                        CombatForceData.MinMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MinMAttackV");
                        CombatForceData.MaxMagicAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "MaxMAttackV");
                        CombatForceData.HitValueModulus = Global.GetSafeAttributeDouble(xmlItem, "HitV");
                        CombatForceData.DodgeModulus = Global.GetSafeAttributeDouble(xmlItem, "Dodge");
                        CombatForceData.AddAttackInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttackInjure");
                        CombatForceData.DecreaseInjureModulus = Global.GetSafeAttributeDouble(xmlItem, "DecreaseInjureValue");
                        CombatForceData.LifeStealModulus = Global.GetSafeAttributeDouble(xmlItem, "LifeSteal");
                        CombatForceData.AddAttackModulus = Global.GetSafeAttributeDouble(xmlItem, "AddAttack");
                        CombatForceData.AddDefenseModulus = Global.GetSafeAttributeDouble(xmlItem, "AddDefense");

                        Data.CombatForceDataInfo.Add(nID, CombatForceData);
                    }
                }

            }
            catch (Exception e)
            {
                e.ToString();
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/CombatForceInfo.xml")));
            }

        }

        /// <summary>
        /// 恶魔广场场景信息 [12/24/2013 LiaoWei]
        /// </summary>
        private void LoadDaimonSquareDataInfo()
        {
            // 加载恶魔广场场景信息
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/Demon.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> BloodCastleXEle = xmlFile.Elements("DaimonSquareInfos").Elements();
                foreach (var xmlItem in BloodCastleXEle)
                {
                    if (null != xmlItem)
                    {
                        DaimonSquareDataInfo tmpInfo = new DaimonSquareDataInfo();

                        int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");

                        tmpInfo.MapCode = nMapCodeID;

                        tmpInfo.MinChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
                        
                        tmpInfo.MaxChangeLifeNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");

                        tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");

                        tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
                        
                        tmpInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxEnter");

                        string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
                        string[] fields = goodsIDs.Split(',');
                        int[] nArray = Global.StringArray2IntArray(fields);
                        tmpInfo.NeedGoodsID = nArray[0];
                        tmpInfo.NeedGoodsNum = nArray[1];

                        tmpInfo.MaxPlayerNum = (int)Global.GetSafeAttributeLong(xmlItem, "MaxPlayer");

                        string sMonsterID = Global.GetSafeAttributeStr(xmlItem, "MonsterID");
                        tmpInfo.MonsterID = sMonsterID.Split('|');
                        int nMonsterIDLength = tmpInfo.MonsterID.Length;

                        string sMonsterNum = Global.GetSafeAttributeStr(xmlItem, "MonsterNumber");
                        tmpInfo.MonsterNum = sMonsterNum.Split('|');
                        int nMonsterNumLength = tmpInfo.MonsterNum.Length;

                        string sMonsterPos = Global.GetSafeAttributeStr(xmlItem, "MonsterPos");
                        string[] sArraysPos = sMonsterPos.Split(',');
                        tmpInfo.posX = Global.SafeConvertToInt32(sArraysPos[0]);
                        tmpInfo.posZ = Global.SafeConvertToInt32(sArraysPos[1]);
                        tmpInfo.Radius = Global.SafeConvertToInt32(sArraysPos[2]);

                        tmpInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xmlItem, "MonsterSum");

                        string sMonsterCond = Global.GetSafeAttributeStr(xmlItem, "SuccessConditions");
                        tmpInfo.CreateNextWaveMonsterCondition = sMonsterCond.Split('|');
                        int nMonsterCondLength = tmpInfo.CreateNextWaveMonsterCondition.Length;

                        if (nMonsterIDLength != nMonsterNumLength || nMonsterIDLength != nMonsterCondLength) { ; } // 报错

                        tmpInfo.TimeModulus = (int)Global.GetSafeAttributeLong(xmlItem, "TimeModulus");
                        tmpInfo.ExpModulus = (int)Global.GetSafeAttributeLong(xmlItem, "ExpModulus");
                        tmpInfo.MoneyModulus = (int)Global.GetSafeAttributeLong(xmlItem, "MoneyModulus");

                        string goodsID = Global.GetSafeAttributeStr(xmlItem, "AwardItem1");
                        string[] sfields = goodsID.Split('|');
                        tmpInfo.AwardItem = sfields;

                        List<string> timePointsList = new List<string>();
                        string[] sField = null;
                        string timePoints = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
                        if (null != timePoints && timePoints != "")
                        {
                            sField = timePoints.Split(',');
                            for (int i = 0; i < sField.Length; i++)
                                timePointsList.Add(sField[i].Trim());
                        }

                        tmpInfo.BeginTime = timePointsList;
                        tmpInfo.PrepareTime = (int)Global.GetSafeAttributeLong(xmlItem, "PrepareTime");
                        tmpInfo.DurationTime = (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
                        tmpInfo.LeaveTime = (int)Global.GetSafeAttributeLong(xmlItem, "LeaveTime");

                        Data.DaimonSquareDataInfoList.Add(nMapCodeID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Demon.xml")));
            }

        }

        /// <summary>
        /// 缓存住SystemParams.xml表中用的频繁的数据 提高服务器性能 [1/25/2014 LiaoWei]
        /// </summary>
        private void LoadSystemParamsDataForCache()
        {
            try
            {
                int[] nValue = null;
                double[] dValue = null;

                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiJiaCheng");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.WingForgeLevelAddShangHaiJiaCheng = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.WingForgeLevelAddDefenseRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingZhuiJiaLevelAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.WingZhuiJiaLevelAddDefenseRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingForgeLevelAddShangHaiXiShou");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.WingForgeLevelAddShangHaiXiShou = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddAttackRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ForgeLevelAddAttackRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddAttackRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ZhuiJiaLevelAddAttackRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ForgeLevelAddDefenseRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuiJiaLevelAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ZhuiJiaLevelAddDefenseRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ForgeLevelAddMaxLifeVRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ForgeLevelAddMaxLifeVRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddAttackRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ZhuoYueAddAttackRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.ZhuoYueAddDefenseRates = dValue;
                }

                string str = GameManager.systemParamsList.GetParamValueByName("ShiJieChuanSong");
                if (!string.IsNullOrEmpty(str))
                {
                    string[] fields1 = str.Split('|');
                    for (int i = 0; i < fields1.Length; i++)
                    {
                        string[] fields2 = fields1[i].Split(',');
                        int mapCode = Global.SafeConvertToInt32(fields2[0]);
                        int needMoney = Global.SafeConvertToInt32(fields2[1]);
                        Data.MapTransNeedMoneyDict.Add(mapCode, needMoney);
                    }
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddAttackRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.EquipChangeLifeAddAttackRates = dValue;
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EquipZhuanShengAddDefenseRates");

                if (dValue != null && dValue.Length > 0)
                {
                    Data.EquipChangeLifeAddDefenseRates = dValue;
                }

                int nValue1 = 0;
                nValue1 = (int)GameManager.systemParamsList.GetParamValueDoubleByName("ZuanshiVIPExp");

                if (nValue1 != 0)
                {
                    Data.DiamondToVipExpValue = nValue1;
                }

                nValue = null;
                nValue = GameManager.systemParamsList.GetParamValueIntArrayByName("BossStaticDataIDForChengJiu");

                if (nValue != null && nValue.Length > 0)
                {
                    Data.KillBossCountForChengJiu = nValue;
                }

                str = null;
                str = GameManager.systemParamsList.GetParamValueByName("ForgeProtectStoneGoodsIDS");
                if (!string.IsNullOrEmpty(str))
                {
                    int nID = 0;
                    int nNum = 0;

                    string[] fields1 = str.Split('|');

                    Data.ForgeProtectStoneGoodsID = new int[fields1.Length];
                    Data.ForgeProtectStoneGoodsNum = new int[fields1.Length];
                    
                    for (int i = 0; i < fields1.Length; i++)
                    {
                        string[] fields2 = fields1[i].Split(',');
                        nID = Global.SafeConvertToInt32(fields2[0]);
                        nNum = Global.SafeConvertToInt32(fields2[1]);

                        Data.ForgeProtectStoneGoodsID[i] = nID;
                        Data.ForgeProtectStoneGoodsNum[i] = nNum;
                    }
                }

                str = GameManager.systemParamsList.GetParamValueByName("MoBaiNumber");
                if (!string.IsNullOrEmpty(str))
                {
                    Data.PKkingadrationData.AdrationMaxLimit = Global.SafeConvertToInt32(str);
                }

                str = GameManager.systemParamsList.GetParamValueByName("JiBiMoBai");
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strFelds = null;
                    strFelds = str.Split(',');

                    Data.PKkingadrationData.GoldAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
                    Data.PKkingadrationData.GoldAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
                    Data.PKkingadrationData.GoldAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
                }

                str = GameManager.systemParamsList.GetParamValueByName("ZuanShiMoBai");
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strFelds = null;
                    strFelds = str.Split(',');

                    Data.PKkingadrationData.DiamondAdrationSpend = Global.SafeConvertToInt32(strFelds[0]);
                    Data.PKkingadrationData.DiamondAdrationExpModulus = Global.SafeConvertToInt32(strFelds[1]);
                    Data.PKkingadrationData.DiamondAdrationShengWangModulus = Global.SafeConvertToInt32(strFelds[2]);
                }

                str = GameManager.systemParamsList.GetParamValueByName("CangKuAward");

                if (!string.IsNullOrEmpty(str))
                {
                    string[] strFelds = null;
                    strFelds = str.Split('|');

                    Data.InsertAwardtPortableBagTaskID = Global.SafeConvertToInt32(strFelds[0]);
                    Data.InsertAwardtPortableBagGoodsInfo = strFelds[1];
                }

                dValue = null;
                dValue = GameManager.systemParamsList.GetParamValueDoubleArrayByName("HongMingDebuff");
                if (dValue != null)
                {
                    Data.RedNameDebuffInfo = dValue;
                }

                str = null;
                str = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsIDs");

                if (str != null && str.Length > 0)
                {
                    Data.ForgeNeedGoodsID = Global.String2StringArray(str);

                    /*string[] strID = null;
                    for (int i = 0; i < strID.Length; ++i)
                    {
                        string[] strFile = strID[i].Split(',');

                        string strData = strFile[i];

                        Data.ForgeNeedGoodsID[i] = strData;
                    }*/
                }

                str = null;
                str = GameManager.systemParamsList.GetParamValueByName("ForgeNeedGoodsNum");

                if (str != null && str.Length > 0)
                {
                    Data.ForgeNeedGoodsNum = Global.String2StringArray(str);
                }

                if (Data.ForgeNeedGoodsID.Length != Data.ForgeNeedGoodsNum.Length)
                {
                    throw new Exception(string.Format("load file : {0} error", string.Format("LoadSystemParamsDataForCache")));
                }

                nValue1 = 0;
                nValue1 = (int)GameManager.systemParamsList.GetParamValueDoubleByName("PaiHangChongBai");

                if (nValue1 != 0)
                {
                    Data.PaihangbangAdration = nValue1;
                }

                nValue = null;
                nValue = GameManager.systemParamsList.GetParamValueIntArrayByName("BloodCastleCopyMapCodeID");

                if (nValue != null && nValue.Length > 0)
                {
                    Data.BloodCastleCopySceneList = nValue;
                }

                nValue = null;
                nValue = GameManager.systemParamsList.GetParamValueIntArrayByName("DemonCopyMapCodeIds");

                if (nValue != null && nValue.Length > 0)
                {
                    Data.DaimonSquareCopySceneList = nValue;
                }

                nValue = null;
                nValue = GameManager.systemParamsList.GetParamValueIntArrayByName("storycopymapid");

                if (nValue != null && nValue.Length > 0)
                {
                    Data.StoryCopyMapID = nValue;
                }

                nValue1 = 0;
                nValue1 = (int)GameManager.systemParamsList.GetParamValueDoubleByName("QiFuTime");

                if (nValue1 != 0)
                {
                    Data.FreeImpetrateIntervalTime = nValue1 * 60;
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load file : {0} fail", string.Format("LoadSystemParamsDataForCache")));
            }

        }

        /// <summary>
        /// 累计登陆奖励信息 [2/11/2014 LiaoWei]
        /// </summary>
        private void LoadTotalLoginDataInfo()
        {
            // 加载累计登陆奖励信息
            try
            {
                string fileName = "Config/Gifts/NewHuoDongLoginNumGift.xml";
                XElement xmlFile = null;
                xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> TotalLoginXEle = xmlFile.Elements("HuoDongLoginNumGift").Elements();
                foreach (var xmlItem in TotalLoginXEle)
                {
                    if (null != xmlItem)
                    {
                        TotalLoginDataInfo tmpInfo = new TotalLoginDataInfo();

                        int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");

                        tmpInfo.TotalLoginDays = (int)Global.GetSafeAttributeLong(xmlItem, "TimeOl");

                        string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
                        string[] fields = goodsIDs.Split('|');
                        if (fields.Length != 0)
                        {
                            tmpInfo.NormalAward = new List<GoodsData>();

                            for (int i = 0; i < fields.Length; ++i)
                            {
                                string goods = fields[i];
                                
                                string[] goodsProp = goods.Split(',');

                                if (goodsProp.Length != 7)
                                    continue;

                                GoodsData goodsdata = new GoodsData();

                                goodsdata.GoodsID       = Global.SafeConvertToInt32(goodsProp[0]);
                                goodsdata.GCount        = Global.SafeConvertToInt32(goodsProp[1]);
                                goodsdata.Binding       = Global.SafeConvertToInt32(goodsProp[2]);
                                goodsdata.Forge_level   = Global.SafeConvertToInt32(goodsProp[3]);
                                goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
                                goodsdata.Lucky         = Global.SafeConvertToInt32(goodsProp[5]);
                                goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);

                                tmpInfo.NormalAward.Add(goodsdata);
                            }
                        }

                        goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
                        fields = goodsIDs.Split('|');
                        if (fields.Length != 0)
                        {
                            tmpInfo.Award0 = new List<GoodsData>();
                            tmpInfo.Award1 = new List<GoodsData>();
                            tmpInfo.Award2 = new List<GoodsData>();

                            for (int i = 0; i < fields.Length; ++i)
                            {
                                string goods = fields[i];

                                string[] goodsProp = goods.Split(',');

                                if (goodsProp.Length != 7)
                                    continue;

                                GoodsData goodsdata = new GoodsData();

                                goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
                                goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
                                goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
                                goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
                                goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
                                goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
                                goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);

                                int nOcu = -1;
                                nOcu = Global.GetGoodsToOccupation(goodsdata.GoodsID);

                                if (nOcu == 0)
                                    tmpInfo.Award0.Add(goodsdata);
                                else if (nOcu == 1)
                                    tmpInfo.Award1.Add(goodsdata);
                                else if (nOcu == 2)
                                    tmpInfo.Award2.Add(goodsdata);
                            }
                        }

                        Data.TotalLoginDataInfoList.Add(nID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/NewHuoDongLoginNumGift.xml")));
            }

        }

        /// <summary>
        /// VIP奖励信息 [2/19/2014 LiaoWei]
        /// </summary>
        private void LoadVIPDataInfo()
        {
            // 加载VIP奖励信息
            try
            {
                string fileName = "Config/Gifts/VipDailyAwards.xml";
                XElement xmlFile = null;
                xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> VIPXEle = xmlFile.Elements();
                foreach (var xmlItem in VIPXEle)
                {
                    if (null != xmlItem)
                    {
                        VIPDataInfo tmpInfo = new VIPDataInfo();

                        int nAwardID = (int)Global.GetSafeAttributeLong(xmlItem, "AwardID");

                        tmpInfo.AwardID = nAwardID;

                        tmpInfo.ZuanShi = (int)Global.GetSafeAttributeDouble(xmlItem, "ZuanShi");

                        tmpInfo.BindZuanShi = (int)Global.GetSafeAttributeDouble(xmlItem, "BindZuanShi");

                        tmpInfo.JinBi = (int)Global.GetSafeAttributeDouble(xmlItem, "JinBi");

                        tmpInfo.BindJinBi = (int)Global.GetSafeAttributeDouble(xmlItem, "BindJinBi");

                        tmpInfo.VIPlev = (int)Global.GetSafeAttributeDouble(xmlItem, "VIPlev");

                        tmpInfo.XiHongMing = (int)Global.GetSafeAttributeDouble(xmlItem, "XiHongMing");

                        tmpInfo.XiuLi = (int)Global.GetSafeAttributeDouble(xmlItem, "XiuLi");

                        tmpInfo.DailyMaxUseTimes = (int)Global.GetSafeAttributeDouble(xmlItem, "DailyMaxUseTimes");

                        string strBuff = null;
                        strBuff = Global.GetSafeAttributeStr(xmlItem, "BufferGoods");

                        if (strBuff != null)
                        {
                            string[] strField = strBuff.Split(',');

                            if (strField.Count() > 0)
                            {
                                tmpInfo.BufferGoods = new int[strField.Count()];

                                int nValue = 0;
                                for (int i = 0; i < strField.Count(); ++i)
                                {
                                    nValue = Global.SafeConvertToInt32(strField[i]);

                                    tmpInfo.BufferGoods[i] = nValue;
                                }
                            }
                        }

                        string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
                        
                        if (!string.IsNullOrEmpty(goodsIDs))
                        {
                            string[] fields = goodsIDs.Split('|');
                            if (fields.Length != 0)
                            {
                                tmpInfo.AwardGoods = new List<GoodsData>();

                                for (int i = 0; i < fields.Length; ++i)
                                {
                                    string goods = fields[i];

                                    string[] goodsProp = goods.Split(',');

                                    GoodsData goodsdata = new GoodsData();

                                    if (goodsProp.Length == 7)
                                    {
                                        goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
                                        goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
                                        goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
                                        goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
                                        goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
                                        goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
                                        goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
                                    }
                                    else
                                        goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);


                                    tmpInfo.AwardGoods.Add(goodsdata);
                                }
                            }
                        }

                        Data.VIPDataInfoList.Add(nAwardID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/Gifts/VipDailyAwards.xml")));
            }

        }

        /// <summary>
        /// VIP奖励信息 [2/19/2014 LiaoWei]
        /// </summary>
        private void LoadVIPLevAwardAndExpInfo()
        {
            // 加载VIP奖励信息
            try
            {
                XElement xmlFile = null;

                xmlFile = Global.GetGameResXml(string.Format("Config/MuVip.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> VIPXEle = xmlFile.Elements();
                foreach (var xmlItem in VIPXEle)
                {
                    if (null != xmlItem)
                    {
                        VIPLevAwardAndExpInfo tmpInfo = new VIPLevAwardAndExpInfo();

                        int nVipLev = (int)Global.GetSafeAttributeLong(xmlItem, "VIPLevel");
                        
                        tmpInfo.VipLev = nVipLev;

                        string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "LiBaoAward");
                        string[] fields = goodsIDs.Split('|');
                        if (fields.Length != 0)
                        {
                            tmpInfo.AwardList = new List<GoodsData>();

                            for (int i = 0; i < fields.Length; ++i)
                            {
                                string goods = fields[i];

                                string[] goodsProp = goods.Split(',');

                                GoodsData goodsdata = new GoodsData();

                                if (goodsProp.Length == 7)
                                {
                                    goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);
                                    goodsdata.GCount = Global.SafeConvertToInt32(goodsProp[1]);
                                    goodsdata.Binding = Global.SafeConvertToInt32(goodsProp[2]);
                                    goodsdata.Forge_level = Global.SafeConvertToInt32(goodsProp[3]);
                                    goodsdata.AppendPropLev = Global.SafeConvertToInt32(goodsProp[4]);
                                    goodsdata.Lucky = Global.SafeConvertToInt32(goodsProp[5]);
                                    goodsdata.ExcellenceInfo = Global.SafeConvertToInt32(goodsProp[6]);
                                }
                                else
                                    goodsdata.GoodsID = Global.SafeConvertToInt32(goodsProp[0]);

                                tmpInfo.AwardList.Add(goodsdata);
                            }
                        }

                        tmpInfo.NeedExp = (int)Global.GetSafeAttributeLong(xmlItem, "NeedExp");

                        Data.VIPLevAwardAndExpInfoList.Add(nVipLev, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/MuVip.xml")));
            }

        }

        /// <summary>
        // 冥想静态数据 [3/5/2014 LiaoWei]
        /// </summary>
        private void LoadMeditateInfo()
        {
            // 加载VIP奖励信息
            try
            {
                XElement xmlFile = null;

                xmlFile = Global.GetGameResXml(string.Format("Config/MingXiang.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> MeditateXEle = xmlFile.Elements();
                foreach (var xmlItem in MeditateXEle)
                {
                    if (null != xmlItem)
                    {
                        MeditateData tmpInfo = new MeditateData();

                        int ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");

                        tmpInfo.MeditateID = ID;

                        tmpInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng");
                        tmpInfo.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MaxZhuanSheng");
                        tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
                        tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
                        tmpInfo.Experience = (int)Global.GetSafeAttributeLong(xmlItem, "Experience");
                        tmpInfo.StarSoul = (int)Global.GetSafeAttributeLong(xmlItem, "Xinghun");

                        Data.MeditateInfoList.Add(ID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/VIPExp.xml")));
            }

        }

        /// <summary>
        /// 经验副本  [3/18/2014 LiaoWei]
        /// </summary>
        private void LoadExperienceCopyMapDataInfo()
        {
            // 加载经验场景信息
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/JinYanFuBen.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> ExperienceXEle = xmlFile.Elements("JinYanFuBen").Elements();
                foreach (var xmlItem in ExperienceXEle)
                {
                    if (null != xmlItem)
                    {
                        ExperienceCopyMapDataInfo tmpInfo = new ExperienceCopyMapDataInfo();

                        int nMapCodeID = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
                        tmpInfo.CopyMapID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");

                        tmpInfo.MapCodeID = nMapCodeID;

                        tmpInfo.MonsterIDList = new Dictionary<int, List<int>>();
                        
                        string sMonsterID = Global.GetSafeAttributeStr(xmlItem, "MonsterID");
                        string[] sID = sMonsterID.Split('|');

                        for (int i = 0; i < sID.Length; ++i)
                        {
                            string[] sFildID = sID[i].Split(',');
                            List<int> tmpIDList = new List<int>();

                            for (int n = 0; n < sFildID.Length; ++n)
                            {
                                int nid = Global.SafeConvertToInt32(sFildID[n]);
                                tmpIDList.Add(nid);
                            }
                            tmpInfo.MonsterIDList.Add(i, tmpIDList);
                        }

                        tmpInfo.MonsterNumList = new Dictionary<int, List<int>>();

                        string sMonsterNum = Global.GetSafeAttributeStr(xmlItem, "MonsterNumber");
                        string[] sNum = sMonsterNum.Split('|');

                        for (int i = 0; i < sNum.Length; ++i)
                        {
                            string[] sFildNum = sNum[i].Split(',');
                            List<int> tmpNumList = new List<int>();

                            for (int n = 0; n < sFildNum.Length; ++n)
                            {
                                int nnum = Global.SafeConvertToInt32(sFildNum[n]);
                                tmpNumList.Add(nnum);                                
                            }
                            tmpInfo.MonsterNumList.Add(i, tmpNumList);
                        }
                        
                        string sMonsterPos = Global.GetSafeAttributeStr(xmlItem, "MonsterPos");
                        string[] sArraysPos = sMonsterPos.Split(',');
                        tmpInfo.posX = Global.SafeConvertToInt32(sArraysPos[0]);
                        tmpInfo.posZ = Global.SafeConvertToInt32(sArraysPos[1]);
                        tmpInfo.Radius = Global.SafeConvertToInt32(sArraysPos[2]);

                        tmpInfo.MonsterSum = (int)Global.GetSafeAttributeLong(xmlItem, "MonsterSum");

                        string sMonsterCond = Global.GetSafeAttributeStr(xmlItem, "SuccessConditions");
                        string[] sCon = sMonsterCond.Split('|');

                        tmpInfo.CreateNextWaveMonsterCondition = new int[sCon.Length];

                        for (int i = 0; i< sCon.Length; ++i)
                        {
                            tmpInfo.CreateNextWaveMonsterCondition[i] = Global.SafeConvertToInt32(sCon[i]);
                        }

                        int nMonsterCondLength = tmpInfo.CreateNextWaveMonsterCondition.Length;

                        //if (nMonsterIDLength != nMonsterNumLength || nMonsterIDLength != nMonsterCondLength) { ; } // 报错

                        Data.ExperienceCopyMapDataInfoList.Add(nMapCodeID, tmpInfo);
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/JinYanFuBen.xml")));
            }

        }

        /// <summary>
        /// Boss之家静态数据 [4/7/2014 LiaoWei]
        /// </summary>
        private void LoadBossHomeInfo()
        {
            // 加载Boss之家静态数据
            try
            {
                XElement xmlFile = null;

                xmlFile = Global.GetGameResXml(string.Format("Config/BossZhiJia.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> BossHomeXEle = xmlFile.Elements("BossZhiJias").Elements();
                foreach (var xmlItem in BossHomeXEle)
                {
                    if (null != xmlItem)
                    {
                        BossHomeData tmpInfo = new BossHomeData();

                        tmpInfo.MapID = (int)Global.GetSafeAttributeDouble(xmlItem, "MapCode");

                        tmpInfo.VIPLevLimit = (int)Global.GetSafeAttributeLong(xmlItem, "KaiQiVipLevel");
                        tmpInfo.MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
                        tmpInfo.MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");
                        tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
                        tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
                        tmpInfo.EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "EnterNeedZuanShi");
                        tmpInfo.OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "MapTimeNeedZuanShi");

                        Data.BosshomeData = tmpInfo;
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/BossZhiJia.xml")));
            }

        }

        /// <summary>
        /// 黄金神庙静态数据 [4/7/2014 LiaoWei]
        /// </summary>
        private void LoadGoldTempleInfo()
        {
            // 加载黄金神庙静态数据
            try
            {
                XElement xmlFile = null;

                xmlFile = Global.GetGameResXml(string.Format("Config/HuangJinShengDian.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> BossHomeXEle = xmlFile.Elements("HuangJinShengDians").Elements();
                foreach (var xmlItem in BossHomeXEle)
                {
                    if (null != xmlItem)
                    {
                        GoldTempleData tmpInfo = new GoldTempleData();

                        tmpInfo.MapID = (int)Global.GetSafeAttributeDouble(xmlItem, "MapCode");

                        tmpInfo.VIPLevLimit = (int)Global.GetSafeAttributeLong(xmlItem, "KaiQiVipLevel");
                        tmpInfo.MinChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MinChangeLife");
                        tmpInfo.MaxChangeLifeLimit = (int)Global.GetSafeAttributeLong(xmlItem, "MaxChangeLife");
                        tmpInfo.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
                        tmpInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
                        tmpInfo.EnterNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "EnterNeedZuanShi");
                        tmpInfo.OneMinuteNeedDiamond = (int)Global.GetSafeAttributeLong(xmlItem, "MapTimeNeedZuanShi");

                        Data.GoldtempleData = tmpInfo;
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/HuangJinShengDian.xml")));
            }

        }

        /// <summary>
        /// 图鉴系统静态数据 [5/3/2014 LiaoWei]
        /// </summary>
        private void LoadPictureJudgeInfo()
        {
            // 加载图鉴系统静态数据
            try
            {
                string fileName = "Config/TuJianItems.xml";

                XElement xmlFile = null;
                xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> TuJianXEle = xmlFile.Elements();
                foreach (var xmlItem in TuJianXEle)
                {
                    if (null != xmlItem)
                    {
                        PictureJudgeData tmpInfo = new PictureJudgeData();

                        int ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
                        tmpInfo.PictureJudgeID = ID;

                        int nType = (int)Global.GetSafeAttributeDouble(xmlItem, "Type");
                        tmpInfo.PictureJudgeType = nType;

                        string strInfos = null;
                        strInfos = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");

                        string[] sArry = null;
                        sArry = strInfos.Split(',');

                        tmpInfo.NeedGoodsID = Global.SafeConvertToInt32(sArry[0]);
                        tmpInfo.NeedGoodsNum = Global.SafeConvertToInt32(sArry[1]);

                        strInfos = null;
                        strInfos = Global.GetSafeAttributeStr(xmlItem, "ShuXing");

                        sArry = null;
                        sArry = strInfos.Split('|');

                        string[] sArryInfo = null;

                        if (sArry != null)
                        {
                            for (int n = 0; n < sArry.Length; ++n)
                            {
                                sArryInfo = null;

                                sArryInfo = sArry[n].Split(',');

                                string strPorpName = null;
                                strPorpName = sArryInfo[0];

                                string strPorpValue = null;
                                strPorpValue = sArryInfo[1];

                                string[] strArrayPorpValue = null;
                                strArrayPorpValue = strPorpValue.Split('-');

                                if (strPorpName == "Defense")
                                {
                                    tmpInfo.PropertyID1 = (int)ExtPropIndexes.MinDefense;
                                    tmpInfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID2 = (int)ExtPropIndexes.MaxDefense;
                                    tmpInfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Mdefense")
                                {
                                    tmpInfo.PropertyID3 = (int)ExtPropIndexes.MinMDefense;
                                    tmpInfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID4 = (int)ExtPropIndexes.MaxMDefense;
                                    tmpInfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Attack")
                                {
                                    tmpInfo.PropertyID5 = (int)ExtPropIndexes.MinAttack;
                                    tmpInfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                    
                                    tmpInfo.PropertyID6 = (int)ExtPropIndexes.MaxAttack;
                                    tmpInfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Mattack")
                                {
                                    tmpInfo.PropertyID7 = (int)ExtPropIndexes.MinMAttack;
                                    tmpInfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID8 = (int)ExtPropIndexes.MaxMAttack;
                                    tmpInfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "HitV")
                                {
                                    tmpInfo.PropertyID9 = (int)ExtPropIndexes.HitV;
                                    tmpInfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }
                                else if (strPorpName == "Dodge")
                                {
                                    tmpInfo.PropertyID10 = (int)ExtPropIndexes.Dodge;
                                    tmpInfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }
                                else if (strPorpName == "MaxLifeV")
                                {
                                    tmpInfo.PropertyID11 = (int)ExtPropIndexes.MaxLifeV;
                                    tmpInfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }

                            }
                        }

                        Data.PicturejudgeData[ID] = tmpInfo;
                    }
                }

            }
            catch (Exception)
            {
                throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/TuJianItems.xml")));
            }

        }

        /// <summary>
        /// 图鉴系统图鉴类型静态数据 [7/12/2014 LiaoWei]
        /// </summary>
        private void LoadPictureJudgeTypeInfo()
        {
            // 加载图鉴系统类型静态数据
            try
            {
                string fileName = "Config/TuJianType.xml";

                XElement xmlFile = null;
                xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> TuJianXEle = xmlFile.Elements("TuJian").Elements();
                //IEnumerable<XElement> TuJianXEle = xmlFile.Elements();
                foreach (var xmlItem in TuJianXEle)
                {
                    if (null != xmlItem)
                    {
                        PictureJudgeTypeData tmpInfo = new PictureJudgeTypeData();

                        int ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
                        tmpInfo.PictureJudgeTypeID = ID;

                        string sLevelInfo = null;
                        sLevelInfo = Global.GetSafeAttributeStr(xmlItem, "KaiQiLevel");

                        string[] sArrayLevelInfo = null;
                        sArrayLevelInfo = sLevelInfo.Split(',');
                        tmpInfo.OpenChangeLifeLevel = Global.SafeConvertToInt32(sArrayLevelInfo[0]);
                        tmpInfo.OpenLevel= Global.SafeConvertToInt32(sArrayLevelInfo[1]);

                        int nCount = (int)Global.GetSafeAttributeDouble(xmlItem, "TuJianNum");
                        tmpInfo.TotalNum = nCount;
                        
                        string strInfos = null;
                        strInfos = Global.GetSafeAttributeStr(xmlItem, "ShuXiangJiaCheng");

                        string[] sArry = null;
                        sArry = strInfos.Split('|');

                        string[] sArryInfo = null;

                        if (sArry != null)
                        {
                            for (int n = 0; n < sArry.Length; ++n)
                            {
                                sArryInfo = null;

                                sArryInfo = sArry[n].Split(',');

                                string strPorpName = null;
                                strPorpName = sArryInfo[0];

                                string strPorpValue = null;
                                strPorpValue = sArryInfo[1];

                                string[] strArrayPorpValue = null;
                                strArrayPorpValue = strPorpValue.Split('-');

                                if (strPorpName == "Defense")
                                {
                                    tmpInfo.PropertyID1 = (int)ExtPropIndexes.MinDefense;
                                    tmpInfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID2 = (int)ExtPropIndexes.MaxDefense;
                                    tmpInfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Mdefense")
                                {
                                    tmpInfo.PropertyID3 = (int)ExtPropIndexes.MinMDefense;
                                    tmpInfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID4 = (int)ExtPropIndexes.MaxMDefense;
                                    tmpInfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Attack")
                                {
                                    tmpInfo.PropertyID5 = (int)ExtPropIndexes.MinAttack;
                                    tmpInfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID6 = (int)ExtPropIndexes.MaxAttack;
                                    tmpInfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "Mattack")
                                {
                                    tmpInfo.PropertyID7 = (int)ExtPropIndexes.MinMAttack;
                                    tmpInfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(strArrayPorpValue[0]);

                                    tmpInfo.PropertyID8 = (int)ExtPropIndexes.MaxMAttack;
                                    tmpInfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
                                }
                                else if (strPorpName == "HitV")
                                {
                                    tmpInfo.PropertyID9 = (int)ExtPropIndexes.HitV;
                                    tmpInfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }
                                else if (strPorpName == "Dodge")
                                {
                                    tmpInfo.PropertyID10 = (int)ExtPropIndexes.Dodge;
                                    tmpInfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }
                                else if (strPorpName == "MaxLifeV")
                                {
                                    tmpInfo.PropertyID11 = (int)ExtPropIndexes.MaxLifeV;
                                    tmpInfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
                                }

                            }
                        }

                        Data.PicturejudgeTypeData[ID] = tmpInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/TuJianType.xml")));
            }

        }

        /// <summary>
        /// 装备进阶数据 [4/30/2014 LiaoWei]
        /// </summary>
        private void LoadEquipUpgradeInfo()
        {
            // 加载装备进阶数据
            try
            {
                XElement xmlFile = null;

                xmlFile = Global.GetGameResXml(string.Format("Config/MuEquipUp.xml"));
                if (null == xmlFile)
                    return;

                IEnumerable<XElement> equipXEle = xmlFile.Elements("Equip");
                foreach (var xmlItem in equipXEle)
                {
                    IEnumerable<XElement> items = xmlItem.Elements("Item");

                    Dictionary<int, MuEquipUpgradeData> tmpData = new Dictionary<int, MuEquipUpgradeData>();

                    int nID = (int)Global.GetSafeAttributeLong(xmlItem, "Categoriy");

                    foreach (var item in items)
                    {
                        if (null != item)
                        {
                            MuEquipUpgradeData tmpInfo = new MuEquipUpgradeData();

                            int nSuitID = (int)Global.GetSafeAttributeLong(item, "SuitID");

                            tmpInfo.CategoriyID = nID;
                            tmpInfo.SuitID = nSuitID;
                            tmpInfo.NeedMoJing = (int)Global.GetSafeAttributeLong(item, "NeedMoJing");

                            tmpData[nSuitID] = tmpInfo;
                        }
                    }
                    Data.EquipUpgradeData.Add(nID, tmpData);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/MuEquipUp.xml")));
            }

        }

        /// <summary>
        /// 金币副本数据 [6/11/2014 LiaoWei]
        /// </summary>
        private void LoadGoldCopySceneInfo()
        {
            try
            {
                XElement xmlFile = null;
                xmlFile = Global.GetGameResXml(string.Format("Config/JinBiFuBen.xml"));
                if (null == xmlFile)
                    return;

                GoldCopySceneData tmpGoldCopySceneInfo = new GoldCopySceneData();

                XElement args = xmlFile.Element("PatrolPath");
                if (null != args)
                {
                    string sPatorlPathID = Global.GetSafeAttributeStr(args, "Path");

                    if (string.IsNullOrEmpty(sPatorlPathID))
                        LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空"));
                    else
                    {
                        string[] fields = sPatorlPathID.Split('|');
                        if (fields.Length <= 0)
                            LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪路径为空"));
                        else
                        {
                            tmpGoldCopySceneInfo.m_MonsterPatorlPathList = new List<int[]>();

                            for (int i = 0; i < fields.Length; i++)
                            {
                                string[] sa = fields[i].Split(',');
                                if (sa.Length != 2)
                                {
                                    LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败,坐标有误", "Config/JinBiFuBen.xml"));
                                    continue;
                                }
                                int[] pos = Global.StringArray2IntArray(sa);
                                tmpGoldCopySceneInfo.m_MonsterPatorlPathList.Add(pos);
                            }
                            Data.Goldcopyscenedata.m_MonsterPatorlPathList = tmpGoldCopySceneInfo.m_MonsterPatorlPathList;
                        }
                    }

                }

                IEnumerable<XElement> MonstersXEle = xmlFile.Elements("Monsters").Elements();
                foreach (var xmlItem in MonstersXEle)
                {
                    if (null != xmlItem)
                    {
                        GoldCopySceneMonster tmpGoldCopySceneMon = new GoldCopySceneMonster();

                        tmpGoldCopySceneMon.m_MonsterID = new List<int>();
                        int nWave = (int)Global.GetSafeAttributeLong(xmlItem, "WaveID");

                        tmpGoldCopySceneMon.m_Wave = nWave;
                        tmpGoldCopySceneMon.m_Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
                        tmpGoldCopySceneMon.m_Delay1 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay1");
                        tmpGoldCopySceneMon.m_Delay2 = (int)Global.GetSafeAttributeLong(xmlItem, "Delay2");

                        string sMonstersID = Global.GetSafeAttributeStr(xmlItem, "MonsterList");
                        if (string.IsNullOrEmpty(sMonstersID))
                            LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空"));
                        else
                        {
                            string[] fields = sMonstersID.Split('|');
                            if (fields.Length <= 0)
                                LogManager.WriteLog(LogTypes.Warning, string.Format("金币副本怪ID为空"));
                            else
                            {
                                for (int i = 0; i < fields.Length; i++)
                                {
                                    int Monsters = Global.SafeConvertToInt32(fields[i]);
                                    tmpGoldCopySceneMon.m_MonsterID.Add(Monsters);
                                }
                            }
                        }
                        tmpGoldCopySceneInfo.GoldCopySceneMonsterData.Add(nWave, tmpGoldCopySceneMon);
                    }
                }
                Data.Goldcopyscenedata.GoldCopySceneMonsterData = tmpGoldCopySceneInfo.GoldCopySceneMonsterData;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/JinBiFuBen.xml")));
            }

        }

        /// <summary>
        /// 初始化所有地图的摆摊位置列表
        /// </summary>
        private void InitMapStallPosList()
        {
            Data.MapStallList.Clear();

            if (null == Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME]) return;
            IEnumerable<XElement> mapList = null;

            try
            {
                XElement xmlRoot = Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "MapStalls");
                if (null != xmlRoot)
                {
                    mapList = xmlRoot.Elements("Stall");
                }
            }
            catch (Exception)
            {
            }

            if (null == mapList) return;

            foreach (var xmlItem in mapList)
            {
                int mapCode = (int)Global.GetSafeAttributeLong(xmlItem, "Code");
                Point toPos = new Point((int)Global.GetSafeAttributeLong(xmlItem, "X"), (int)Global.GetSafeAttributeLong(xmlItem, "Y"));
                int radius = (int)Global.GetSafeAttributeLong(xmlItem, "Radius");
                MapStallItem mapStallItem = new MapStallItem()
                {
                    MapID = mapCode,
                    ToPos = toPos,
                    Radius = radius,
                };

                if (null != mapStallItem)
                {
                    Data.MapStallList.Add(mapStallItem);
                }
            }
        }

        /// <summary>
        /// 初始化所有地图的名称字典
        /// </summary>
        private void InitMapNameDictionary()
        {
            Data.MapNamesDict.Clear();

            if (null == Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME]) return;
            IEnumerable<XElement> mapList = null;

            try
            {
                XElement xmlRoot = Global.GetSafeXElement(Global.XmlInfo[Global.GAME_CONFIG_SETTINGS_NAME], "Maps");
                if (null != xmlRoot)
                {
                    mapList = xmlRoot.Elements("Map");
                }
            }
            catch (Exception)
            {
            }

            if (null == mapList) return;

            foreach (var xmlItem in mapList)
            {
                int mapCode = (int)Global.GetSafeAttributeLong(xmlItem, "Code");
                string mapName = Global.GetSafeAttributeStr(xmlItem, "Name");
                Data.MapNamesDict[mapCode] = mapName;
            }
        }

        /// <summary>
        /// 初始化游戏的所有地图和地图中的怪物
        /// </summary>
        private void InitGameMapsAndMonsters()
        {
            XElement xml = null;

            try
            {
                xml = XElement.Load(@"MapConfig.xml");
            }
            catch (Exception)
            {
                throw new Exception(string.Format("启动时加载xml文件: {0} 失败", @"MapConfig.xml"));
            }

            IEnumerable<XElement> mapItems = xml.Elements();

            //在这里初始化SpriteContainer和MonsterContainer的_MapObjectDict,省的在程序运行时动态初始化

            GameManager.ClientMgr.initialize(mapItems);
            GameManager.MonsterMgr.initialize(mapItems);

            foreach (var mapItem in mapItems)
            {
                //获取地图的图片配置编号
                int mapPicCode = Global.GetMapPicCodeByCode((int)Global.GetSafeAttributeLong(mapItem, "Code"));
                string name = string.Format("MapConfig/{0}/obs.xml", mapPicCode);
                XElement xmlMask = null;

                try
                {
                    xmlMask = Global.GetGameResXml(name);
                }
                catch (Exception ex)
                {
#if BetaConfig
                    continue;
#else
                    throw new Exception(string.Format("启动时加载xml文件: {0} 失败 {1}", name, ex.ToString()));
#endif
                }

                int mapWidth = Convert.ToInt32(Global.GetSafeAttribute(xmlMask, "MapWidth").Value.ToString());
                int mapHeight = Convert.ToInt32(Global.GetSafeAttribute(xmlMask, "MapHeight").Value.ToString());

                //初始化地图管理对象
                GameMap gameMap = GameManager.MapMgr.InitAddMap((int)Global.GetSafeAttributeLong(mapItem, "Code"), mapPicCode,
                    mapWidth, mapHeight, (int)Global.GetSafeAttributeLong(mapItem, "BirthPosX"), (int)Global.GetSafeAttributeLong(mapItem, "BirthPosY"),
                    (int)Global.GetSafeAttributeLong(mapItem, "BirthRadius"));

                //初始化地图格子管理对象
                GameManager.MapGridMgr.InitAddMapGrid((int)Global.GetSafeAttributeLong(mapItem, "Code"),
                    mapWidth, mapHeight, GameManager.MapGridWidth, GameManager.MapGridHeight, gameMap);

                //初始化安全区列表特效
                //gameMap.InitSafeRegionListDeco();

                /// <summary>
                /// 地图爆怪区域管理类
                /// </summary>
#if BetaConfig
                int m = gameMap.MapCode;
                if (m == 1 || m == 3000 || m == 4000 || m == 10000 || m == 8000 || m == 5100 || m == 4100 || m == 6004 || m == 7004 || m == 11 || m == 22 || m == 43 || m == 31 || m == 91 || m == 73 || m == 9000)
#endif
                {
                    GameManager.MonsterZoneMgr.AddMapMonsters((int)Global.GetSafeAttributeLong(mapItem, "Code"), gameMap);
                }

                //加载地图NPC到相应格子
                NPCGeneralManager.LoadMapNPCRoles((int)Global.GetSafeAttributeLong(mapItem, "Code"), gameMap);
            }

            //根据领地的帮会分布初始化插入的帮旗
            JunQiManager.InitLingDiJunQi();

            //生成动态刷怪需要的怪物种子
            //GameManager.MonsterZoneMgr.InitDynamicMonsterSeed();

            System.Console.WriteLine(StringUtil.substitute("所有地图的怪物总的个数为:{0}", GameManager.MonsterMgr.GetTotalMonstersCount()));
        }

        /// <summary>
        /// 缓存信息初始化
        /// </summary>
        private void InitCache(XElement xml)
        {
            //发送缓冲区发送错误管理对象
            Global._FullBufferManager = new FullBufferManager();
            //发送缓冲列表
            Global._SendBufferManager = new SendBufferManager();
            //最小取50毫秒
            SendBuffer.SendDataIntervalTicks = Global.GMax(20, Global.GMin(500, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataIntervalTicks")));
            //这个值必须小于10240,大于等于1500
            SendBuffer.MaxSingleSocketSendBufferSize = Global.GMax(18000, Global.GMin(256000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "MaxSingleSocketSendBufferSize")));
            //发送超时判读 单位毫秒
            SendBuffer.SendDataTimeOutTicks = Global.GMax(3000, Global.GMin(20000, (int)Global.GetSafeAttributeLong(xml, "SendDataParam", "SendDataTimeOutTicks")));
            //
            SendBuffer.MaxBufferSizeForLargePackge = SendBuffer.MaxSingleSocketSendBufferSize * 2 / 3;

            //内存管理器
            Global._MemoryManager = new MemoryManager();
            //size,num|size,num|size,num
            string cacheMemoryBlocks = Global.GetSafeAttributeStr(xml, "CacheMemoryParam", "CacheMemoryBlocks");
            if (string.IsNullOrWhiteSpace(cacheMemoryBlocks))
            {
                Global._MemoryManager.AddBatchBlock(100, 1500);
                Global._MemoryManager.AddBatchBlock(600, 400);
                Global._MemoryManager.AddBatchBlock(600, 50);
                Global._MemoryManager.AddBatchBlock(600, 100);
            }
            else
            {
                string[] items = cacheMemoryBlocks.Split('|');
                foreach (var item in items)
                {
                    string[] pair = item.Split(',');
                    int blockSize = int.Parse(pair[0]);
                    int blockNum = int.Parse(pair[1]);
                    blockNum = Global.GMax(blockNum, 80); //缓存数不少于80
                    if (blockSize > 0 && blockNum > 0)
                    {
                        Global._MemoryManager.AddBatchBlock(blockNum, blockSize);
                        GameManager.MemoryPoolConfigDict[blockSize] = blockNum;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化通讯管理对象
        /// </summary>
        private void InitTCPManager(XElement xml, bool bConnectDB)
        {
            if (bConnectDB)
            {
                // 默认的新手村地图编号
                GameManager.DefaultMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "Code");

                // 默认的主城地图编号
                GameManager.MainMapCode = (int)Global.GetSafeAttributeLong(xml, "Map", "MainCode");

                // Server线路ID
                GameManager.ServerLineID = (int)Global.GetSafeAttributeLong(xml, "Server", "LineID");
                //this.Title = string.Format("游戏服务器{0}", GameManager.ServerLineID);

                // 自动给予的物品的ID列表
                GameManager.AutoGiveGoodsIDList = null; //转移到参数表中读取
                /*string autoGiveGoods = Global.GetSafeAttributeStr(xml, "Goods", "GoodsIDs");
                if (!string.IsNullOrEmpty(autoGiveGoods))
                {
                    GameManager.AutoGiveGoodsIDList = new List<int>();
                    string[] autoGiveGoodsFields = autoGiveGoods.Split(',');
                    for (int i = 0; i < autoGiveGoodsFields.Length; i++)
                    {
                        GameManager.AutoGiveGoodsIDList.Add(Global.SafeConvertToInt32(autoGiveGoodsFields[i]));
                    }
                }*/

                // 程序日志级别
                LogManager.LogTypeToWrite = (LogTypes)(int)Global.GetSafeAttributeLong(xml, "Server", "LogType");

                // 事件日志级别
                GameManager.SystemServerEvents.EventLevel = (EventLevels)(int)Global.GetSafeAttributeLong(xml, "Server", "EventLevel");
                GameManager.SystemRoleLoginEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleLogoutEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleTaskEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleDeathEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBuyWithTongQianEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBuyWithYinLiangEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBuyWithYinPiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBuyWithYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleSaleEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleExchangeEvents1.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleExchangeEvents2.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleExchangeEvents3.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleUpgradeEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleGoodsEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleFallGoodsEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleYinLiangEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleHorseEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBangGongEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleJingMaiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleRefreshQiZhenGeEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleWaBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleMapEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleFuBenAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleWuXingAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRolePaoHuanOkEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleYaBiaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleLianZhanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleHuoDongMonsterEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleDigTreasureWithYaoShiEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleAutoSubYuanBaoEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleAutoSubGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleAutoSubEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleBuyWithTianDiJingYuanEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleFetchVipAwardEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;

                GameManager.SystemRoleFetchMailMoneyEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;

                GameManager.SystemRoleBuyWithGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;
                GameManager.SystemRoleGoldEvents.EventLevel = GameManager.SystemServerEvents.EventLevel;

                int dbLog = Global.GMax(0, (int)Global.GetSafeAttributeLong(xml, "DBLog", "DBLogEnable"));

                //将缓存中的日志写入文件中
                //GameManager.DBEventsWriter.Enable = (dbLog > 0);
                //GameManager.DBEventsWriter.EventDiskWriter.EventRootPath = Global.GetSafeAttributeStr(xml, "DBLog", "Path");
                //GameManager.DBEventsWriter.MaxCacheCount = 10000 * 10;

                //初始化缓存信息
                InitCache(xml);

                try
                {
                    Global.Flag_NameServer = true;
                    NameServerNamager.Init(xml);
                }
                catch (System.Exception ex)
                {
                    Global.Flag_NameServer = false;
                    Console.WriteLine(ex.ToString());
                    //throw ex;
                }

                TCPManager.getInstance().initialize((int)Global.GetSafeAttributeLong(xml, "Socket", "capacity"));

                //初始化通讯管理器
                // Global._TCPManager = new TCPManager((int)Global.GetSafeAttributeLong(xml, "Socket", "capacity"));
                Global._TCPManager = TCPManager.getInstance();

                Global._TCPManager.tcpClientPool.RootWindow = this;
                Global._TCPManager.tcpClientPool.Init(
                    (int)Global.GetSafeAttributeLong(xml, "DBServer", "pool"),
                    Global.GetSafeAttributeStr(xml, "DBServer", "ip"),
                    (int)Global.GetSafeAttributeLong(xml, "DBServer", "port"),
                    "DBServer");

                Global._TCPManager.tcpLogClientPool.RootWindow = this;
                Global._TCPManager.tcpLogClientPool.Init(
                    (int)Global.GetSafeAttributeLong(xml, "LogDBServer", "pool"),
                    Global.GetSafeAttributeStr(xml, "LogDBServer", "ip"),
                    (int)Global.GetSafeAttributeLong(xml, "LogDBServer", "port"),
                    "LogDBServer");

                //初始化GM管理命令项
                GameManager.systemGMCommands.InitGMCommands(null);
            }
            else
            {
                TCPCmdHandler.KeySHA1 = Global.GetSafeAttributeStr(xml, "Token", "sha1");
                TCPCmdHandler.KeyData = Global.GetSafeAttributeStr(xml, "Token", "data");
                TCPCmdHandler.WebKey = Global.GetSafeAttributeStr(xml, "Token", "webkey");

                Global._TCPManager.tcpRandKey.Init(
                (int)Global.GetSafeAttributeLong(xml, "Token", "count"),
                (int)Global.GetSafeAttributeLong(xml, "Token", "randseed"));

                //启动通讯管理对象
                Global._TCPManager.RootWindow = this;
                Global._TCPManager.Start(Global.GetSafeAttributeStr(xml, "Socket", "ip"),
                    (int)Global.GetSafeAttributeLong(xml, "Socket", "port"));
            }
        }

        /// <summary>
        /// 动态装入配置文件
        /// </summary>
        public static void DyLoadConfig()
        {
        }

        /// <summary>
        /// 初始化游戏管理对象
        /// </summary>
        private void InitGameManager(XElement xml)
        {
            //程序主窗口
            GameManager.AppMainWnd = this;

            //初始化任务配置
            GameManager.SystemTasksMgr.LoadFromXMlFile("Config/SystemTasks.xml", "Tasks", "ID", 1);

            //加载NPC和任务映射配置
            GameManager.NPCTasksMgr.LoadNPCTasks(GameManager.SystemTasksMgr);

            //初始化NPC配置
            GameManager.SystemNPCsMgr.LoadFromXMlFile("Config/npcs.xml", "NPCs", "ID");

            // 初始化系统操作列表管理
            GameManager.SystemOperasMgr.LoadFromXMlFile("Config/SystemOperations.xml", "Operations", "ID");

            // 物品列表管理
            GameManager.SystemGoods.LoadFromXMlFile("Config/Goods.xml", "Goods", "ID");

            // NPC交易列表
            GameManager.NPCSaleListMgr.LoadSaleList();

            // 物品名字索引管理
            GameManager.SystemGoodsNamgMgr.LoadGoodsItemsDict(GameManager.SystemGoods);

            //初始化物品的公式列表
            GameManager.SystemMagicActionMgr.ParseGoodsActions(GameManager.SystemGoods);

            // 技能列表管理
            GameManager.SystemMagicsMgr.LoadFromXMlFile("Config/Magics.xml", "Magics", "ID");

            // 技能列表快速索引管理
            GameManager.SystemMagicQuickMgr.LoadMagicItemsDict(GameManager.SystemMagicsMgr);

            //初始化技能的公式列表
            GameManager.SystemMagicActionMgr.ParseMagicActions(GameManager.SystemMagicsMgr);

            //初始化技能的公式列表2
            GameManager.SystemMagicActionMgr2.ParseMagicActions2(GameManager.SystemMagicsMgr);

            //初始化技能扫描类型
            GameManager.SystemMagicScanTypeMgr.ParseScanTypeActions2(GameManager.SystemMagicsMgr);

            //初始化节能多段伤害配置项
            MagicsManyTimeDmageCachingMgr.ParseManyTimeDmageItems(GameManager.SystemMagicsMgr);
            
            //爆怪的物品列表xml管理
            GameManager.SystemMonsterGoodsList.LoadFromXMlFile("Config/MonsterGoodsList.xml", "", "ID", 1);

            //限制时间爆怪的物品列表xml管理
            GameManager.SystemLimitTimeMonsterGoodsList.LoadFromXMlFile("Config/HuoDongMonsterGoodsList.xml", "", "ID", 1);

            //爆怪的物品品质ID列表管理
            GameManager.SystemGoodsQuality.LoadFromXMlFile("Config/GoodsQuality.xml", "", "ID", 1);

            //爆怪的物品级别ID列表管理
            GameManager.SystemGoodsLevel.LoadFromXMlFile("Config/GoodsLevel.xml", "", "ID", 1);

            //爆怪的物品天生ID列表管理
            GameManager.SystemGoodsBornIndex.LoadFromXMlFile("Config/GoodsBorn.xml", "", "ID", 1);

            //爆怪的物品追加ID列表管理
            GameManager.SystemGoodsZhuiJia.LoadFromXMlFile("Config/GoodsZhuiJia.xml", "", "ID", 1);
        
            //爆怪的物品卓越ID列表管理
            GameManager.SystemGoodsExcellenceProperty.LoadFromXMlFile("Config/ExcellencePropertyRandom.xml", "ExcellenceProperty", "ID", 1);      

            //炎黄战场的调度xml管理
            GameManager.SystemBattle.LoadFromXMlFile("Config/Battle.xml", "", "ID");

            //阵营战排名奖励表
            GameManager.SystemBattlePaiMingAwards.LoadFromXMlFile("Config/BattlePaiMingAward.xml", "", "ID");

            //竞技场决斗赛的调度xml管理
            GameManager.SystemArenaBattle.LoadFromXMlFile("Config/ArenaBattle.xml", "", "ID");

            //NPC功能脚本列表管理
            GameManager.systemNPCScripts.LoadFromXMlFile("Config/NPCScripts.xml", "Scripts", "ID");

            //初始化NPC功能脚本的公式列表
            GameManager.SystemMagicActionMgr.ParseNPCScriptActions(GameManager.systemNPCScripts);

            //宠物列表管理
            GameManager.systemPets.LoadFromXMlFile("Config/Pet.xml", "Pets", "ID");

            //根据坐骑强化表加载, 并进行缓存
            HorseCachingManager.LoadHorseEnchanceItems();

            //物品合成类型列表管理
            GameManager.systemGoodsMergeTypes.LoadFromXMlFile("Config/GoodsMergeType.xml", "Types", "ID", 1);

            //物品合成类型项管理
            GameManager.systemGoodsMergeItems.LoadFromXMlFile("Config/GoodsMergeItems.xml", "Items", "ID", 1);

            //系统参数配置列表
            GameManager.systemParamsList.LoadParamsList();

            //闭关收益表
            //GameManager.systemBiGuanMgr.LoadFromXMlFile("Config/BiGuan.xml", "", "ID");   // 注释掉 [1/24/2014 LiaoWei]

            //商城物品列表
            GameManager.systemMallMgr.LoadFromXMlFile("Config/Mall.xml", "Mall", "ID", 1);

            //进入游戏时公告信息
            GongGaoDataManager.LoadGongGaoData();

            //加载经脉项到缓存中
            JingMaiCacheManager.LoadJingMaiItems();

            //加载技能项缓存管理
            MagicsCacheManager.LoadMagicItems();

            //加载BOSS图标刷新管理
            TimerBossManager.getInstance();

            //冲穴经验收益表
            GameManager.systemJingMaiExpMgr.LoadFromXMlFile("Config/JingMaiExp.xml", "", "ID");

            //物品包配置管理
            GameManager.systemGoodsBaoGuoMgr.LoadFromXMlFile("Config/GoodsPack.xml", "", "ID");

            //挖宝设置表
            GameManager.systemWaBaoMgr.LoadFromXMlFile("Config/Dig.xml", "", "ID");

            //周连续登录送礼配置表
            GameManager.systemWeekLoginGiftMgr.LoadFromXMlFile("Config/Gifts/LoginNumGift.xml", "", "ID", 1);

            //当月在线时长送礼配置表
            GameManager.systemMOnlineTimeGiftMgr.LoadFromXMlFile("Config/Gifts/OnlieTimeGift.xml", "", "ID", 1);

            //新手见面送礼配置表
            GameManager.systemNewRoleGiftMgr.LoadFromXMlFile("Config/Gifts/NewRoleGift.xml", "", "ID", 1);

            //升级有礼配置表
            GameManager.systemUpLevelGiftMgr.LoadFromXMlFile("Config/Gifts/UpLevelGift.xml", "", "ID", 1);

            //副本配置表
            GameManager.systemFuBenMgr.LoadFromXMlFile("Config/FuBen.xml", "", "ID");

            //押镖配置表
            GameManager.systemYaBiaoMgr.LoadFromXMlFile("Config/Yabiao.xml", "", "ID");

            //特殊的时间表
            GameManager.systemSpecialTimeMgr.LoadFromXMlFile("Config/SpecialTimes.xml", "", "ID", 1);

            //英雄逐擂配置表
            GameManager.systemHeroConfigMgr.LoadFromXMlFile("Config/Hero.xml", "", "ID");

            //帮旗升级配置表
            GameManager.systemBangHuiFlagUpLevelMgr.LoadFromXMlFile("Config/FlagUpLevel.xml", "Flag", "ID");

            //帮旗属性配置表
            GameManager.systemJunQiMgr.LoadFromXMlFile("Config/JunQi.xml", "", "ID");

            //旗座位置配置表
            GameManager.systemQiZuoMgr.LoadFromXMlFile("Config/QiZuo.xml", "", "ID");

            //领地所属地图旗帜配置表
            GameManager.systemLingQiMapQiZhiMgr.LoadFromXMlFile("Config/LingDiQiZhi.xml", "", "ID");

            //奇珍阁物品配置表
            GameManager.systemQiZhenGeGoodsMgr.LoadFromXMlFile("Config/QiZhenGeGoods.xml", "Mall", "ID");

            //皇城复活点配置表
            GameManager.systemHuangChengFuHuoMgr.LoadFromXMlFile("Config/HuangCheng.xml", "", "ID");

            //隋唐战场定时经验表
            GameManager.systemBattleExpMgr.LoadFromXMlFile("Config/BattleExp.xml", "", "ID");

            //皇城，血战地府，领地战定时给予的收益表
            GameManager.systemBangZhanAwardsMgr.LoadFromXMlFile("Config/BangZhanAward.xml", "", "ID");

            //隋唐战场出生点表
            GameManager.systemBattleRebirthMgr.LoadFromXMlFile("Config/Rebirth.xml", "", "ID");

            //隋唐战场奖励表
            GameManager.systemBattleAwardMgr.LoadFromXMlFile("Config/BattleAward.xml", "", "ID");

            //装备天生洗练表
            GameManager.systemEquipBornMgr.LoadFromXMlFile("Config/EquipBorn.xml", "", "ID");

            //装备天生属性名称表
            GameManager.systemBornNameMgr.LoadFromXMlFile("Config/BornName.xml", "", "ID");

            //Vip每日奖励缓存表
            GameManager.systemVipDailyAwardsMgr.LoadFromXMlFile("Config/Gifts/VipDailyAwards.xml", "", "AwardID", 1);

            //Vip活动引导提示缓存表
            GameManager.systemActivityTipMgr.LoadFromXMlFile("Config/Activity/ActivityTip.xml", "", "ID");

            //杨公宝库幸运值奖励缓存表【奖励项不要超过64,因为数据库最多记录64条奖励项】
            GameManager.systemLuckyAwardMgr.LoadFromXMlFile("Config/LuckyAward.xml", "", "ID");

            //砸金蛋幸运值奖励缓存表【奖励项不要超过64,因为数据库最多记录64条奖励项】
            GameManager.systemLuckyAward2Mgr.LoadFromXMlFile("Config/LuckyAward2.xml", "", "ID");

            //杨公宝库幸运值规则表
            GameManager.systemLuckyMgr.LoadFromXMlFile("Config/Lucky.xml", "", "Number");

            //成就管理
            GameManager.systemChengJiu.LoadFromXMlFile("Config/ChengJiu.xml", "ChengJiu", "ChengJiuID");

            //成就Buffer管理
            GameManager.systemChengJiuBuffer.LoadFromXMlFile("Config/ChengJiuBuff.xml", "", "ID");

            //武器通灵配置管理
            GameManager.systemWeaponTongLing.LoadFromXMlFile("Config/TongLing.xml", "", "ID");

            //乾坤袋配置管理
            //GameManager.systemQianKunMgr.LoadFromXMlFile("Config/NewDig.xml", "Type", "TypeID");
            QianKunManager.LoadImpetrateItemsInfo();

            // 祈福分级配置管理[8/28/2014 LiaoWei]
            GameManager.systemImpetrateByLevelMgr.LoadFromXMlFile("Config/DigType.xml", "", "ID");

            //幸运抽奖配置管理
            GameManager.systemXingYunChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig1.xml", "", "ID");

            //月度大转盘抽奖配置管理
            GameManager.systemYueDuZhuanPanChouJiangMgr.LoadFromXMlFile("Config/RiChangGifts/NewDig2.xml", "GiftList", "ID");

            // 每日在线奖励管理
            GameManager.systemEveryDayOnLineAwardMgr.LoadFromXMlFile("Config/Gifts/MUNewRoleGift.xml", "", "ID", 1);

            // 每日登陆奖励管理
            GameManager.systemSeriesLoginAwardMgr.LoadFromXMlFile("Config/Gifts/MULoginNumGift.xml", "", "ID", 1);

            //怪物管理
            GameManager.systemMonsterMgr.LoadFromXMlFile("Config/Monsters.xml", "Monsters", "ID");

            // 经脉等级管理
            GameManager.SystemJingMaiLevel.LoadFromXMlFile("Config/JingMai.xml", "", "ID");

            // 武学等级管理
            GameManager.SystemWuXueLevel.LoadFromXMlFile("Config/WuXue.xml", "", "ID");

            // 过场动画文件管理
            GameManager.SystemTaskPlots.LoadFromXMlFile("Config/TaskPlot.xml", "", "ID", 1);

            // 抢购管理
            GameManager.SystemQiangGou.LoadFromXMlFile("Config/QiangGou.xml", "", "ID", 1);

            // 合服抢购管理
            GameManager.SystemHeFuQiangGou.LoadFromXMlFile("Config/HeFuGifts/HeFuQiangGou.xml", "", "ID", 0);

            // 节日抢购管理
            GameManager.SystemJieRiQiangGou.LoadFromXMlFile("Config/JieRiGifts/JieRiQiangGou.xml", "", "ID", 0);

            // 钻皇等级管理
            GameManager.SystemZuanHuangLevel.LoadFromXMlFile("Config/ZuanHuang.xml", "", "ID");

            //系统激活项
            GameManager.SystemSystemOpen.LoadFromXMlFile("Config/SystemOpen.xml", "", "ID");

            //系统掉落金钱管理管理
            GameManager.SystemDropMoney.LoadFromXMlFile("Config/DropMoney.xml", "", "ID");

            //系统限时连续登录送大礼活动配置文件
            GameManager.SystemDengLuDali.LoadFromXMlFile("Config/Gifts/HuoDongLoginNumGift.xml", "GoodsList", "ID", 1);

            // 系统限时补偿活动配置文件
            GameManager.SystemBuChang.LoadFromXMlFile("Config/BuChang.xml", "", "ID");

            // 战魂等级管理
            GameManager.SystemZhanHunLevel.LoadFromXMlFile("Config/ZhanHun.xml", "", "ID");

            // 荣誉等级管理
            GameManager.SystemRongYuLevel.LoadFromXMlFile("Config/RongYu.xml", "", "ID");

//             // 军衔等级管理
//             GameManager.SystemShengWangLevel.LoadFromXMlFile("Config/JunXian.xml", "", "ID");

            // 魔晶和祈福兑换管理
            GameManager.SystemExchangeMoJingAndQiFu.LoadFromXMlFile("Config/DuiHuanItems.xml", "Items", "ID", 1);

            //采集物管理
            GameManager.systemCaiJiMonsterMgr.LoadFromXMlFile("Config/CrystalMonster.xml", "", "MonsterID");

            // 加载庆功宴配置
            GameManager.QingGongYanMgr.LoadQingGongYanConfig();

            // 冥想管理
            //GameManager.SystemMeditateInfo.LoadFromXMlFile("Config/MingXiang.xml", "", "Level");

            //加载翅膀升星项配置到缓存中
            WingStarCacheManager.LoadWingStarItems();
            
            //VIP等级配置
            Global.LoadVipLevelAwardList();

            //成就索引相关
            ChengJiuManager.InitFlagIndex();

            //根据加载装备进阶能项，并进行缓存
            EquipUpgradeCacheMgr.LoadEquipUpgradeItems();

            //从文件中加载副本到地图编号的映射
            FuBenManager.LoadFuBenMap();

            //加载缓存物品包项的词典
            GoodsBaoGuoCachingMgr.LoadGoodsBaoGuoDict();

            //加载五行奇阵的配置文件
            WuXingMapMgr.LoadXuXingConfig();

            //加载五行奇阵奖励项
            WuXingMapMgr.LoadWuXingAward();

            //加载文字播放列表
            BroadcastInfoMgr.LoadBroadcastInfoItemList();

            //加载弹窗列表
            PopupWinMgr.LoadPopupWinItemList();

            //初始化商城物品价格
            MallGoodsMgr.InitMallGoodsPriceDict();

            //缓存所有属性加成
            //SingleEquipAddPropMgr.LoadAllSingleEquipProps();

            //加载缓存的属性列表
            ChuanQiQianHua.LoadEquipQianHuaProps();

            // 副本评分信息 [11/15/2013 LiaoWei]
            LoadCopyScoreDataInfo();

            // 新手场景信息 [12/1/2013 LiaoWei]
            LoadFreshPlayerCopySceneInfo();

            // 任务星级信息 [12/3/2013 LiaoWei]
            LoadTaskStarDataInfo();

            // 日常跑环任务奖励信息 [12/3/2013 LiaoWei]
            LoadDailyCircleTaskAwardInfo();

            // 讨伐任务奖励信息
            LoadTaofaTaskAwardInfo();

            // 加载战斗力信息表 [12/17/2013 LiaoWei]
            LoadCombatForceInfoInfo();

            // 恶魔广场场景信息表 [12/24/2013 LiaoWei]
            LoadDaimonSquareDataInfo();

            // 缓存住SystemParams.xml表中用的频繁的数据 提高服务器性能 [1/25/2014 LiaoWei]
            LoadSystemParamsDataForCache();

            // 加载累计登陆奖励信息表 [2/11/2014 LiaoWei]
            LoadTotalLoginDataInfo();

            /// VIP奖励信息 [2/19/2014 LiaoWei]
            LoadVIPDataInfo();

            /// VIP等级奖励和经验信息 [2/19/2014 LiaoWei]
            LoadVIPLevAwardAndExpInfo();

            // 冥想信息 [3/5/2014 LiaoWei]
            LoadMeditateInfo();

            // 每日活跃信息数据 [2/25/2014 LiaoWei]
            GameManager.systemDailyActiveInfo.LoadFromXMlFile("Config/DailyActiveInfor.xml", "DailyActive", "DailyActiveID");

            // 每日活跃奖励数据 [2/25/2014 LiaoWei]
            GameManager.systemDailyActiveAward.LoadFromXMlFile("Config/DailyActiveAward.xml", "DailyActiveAward", "ID");

            // 每日活跃管理 [2/26/2014 LiaoWei]
            DailyActiveManager.InitDailyActiveFlagIndex();

            // 图鉴管理器 [5/3/2014 LiaoWei]
            PictureJudgeManager.InitPictureJudgeFlagIndex();

            // 经验副本信息 [3/18/2014 LiaoWei]
            LoadExperienceCopyMapDataInfo();

            // 天使神殿信息 [3/23/2014 LiaoWei]
            GameManager.systemAngelTempleData.LoadFromXMlFile("Config/AngelTemple.xml", "", "ID");
            GameManager.AngelTempleAward.LoadFromXMlFile("Config/AngelTempleAward.xml", "", "ID");
            GameManager.AngelTempleLuckyAward.LoadFromXMlFile("Config/AngelTempleLuckyAward.xml", "", "ID");

            //任务章节
            GameManager.TaskZhangJie.LoadFromXMlFile("Config/TaskZhangJie.xml", "", "ID", 1);
            ReloadXmlManager.InitTaskZhangJieInfo();

            //交易所
            GameManager.JiaoYiTab.LoadFromXMlFile("Config/JiaoYiTab.xml", "", "TabID");
            GameManager.JiaoYiType.LoadFromXMlFile("Config/JiaoYiType.xml", "", "ID");

            // 战盟建设
            GameManager.SystemZhanMengBuild.LoadFromXMlFile("Config/ZhanMengBuild.xml", "", "ID");

            // 翅膀进阶配置表
            GameManager.SystemWingsUp.LoadFromXMlFile("Config/Wing/WingUp.xml", "", "Level");

            // Boss AI配置表
            GameManager.SystemBossAI.LoadFromXMlFile("Config/AI.xml", "", "ID");

            //初始化BossAI的公式列表
            GameManager.SystemMagicActionMgr.ParseBossAIActions(GameManager.SystemBossAI);

            //使用双键值缓存boss AI项
            BossAICachingMgr.LoadBossAICachingItems(GameManager.SystemBossAI);

            // 拓展属性配置表
            GameManager.SystemExtensionProps.LoadFromXMlFile("Config/TuoZhan.xml", "", "ID");

            //解析公式
            GameManager.SystemMagicActionMgr.ParseExtensionPropsActions(GameManager.SystemExtensionProps);

            //使用键值缓存拓展属性项
            ExtensionPropsMgr.LoadCachingItems(GameManager.SystemExtensionProps);

            // boss之家信息 [4/7/2014 LiaoWei]
            LoadBossHomeInfo();

            // 黄金神庙信息 [4/7/2014 LiaoWei]
            LoadGoldTempleInfo();

            // 图鉴系统 [5/3/2014 LiaoWei]
            LoadPictureJudgeInfo();
            
            // 装备进阶数据 [4/30/2014 LiaoWei]
            LoadEquipUpgradeInfo();

            // 金币副本数据 [6/12/2014 LiaoWei]
            LoadGoldCopySceneInfo();

            // 图鉴类型信息 [7/13/2014 LiaoWei]
            LoadPictureJudgeTypeInfo();

            Global.LoadSpecialMachineConfig();
			
            ElementhrtsManager.LoadRefineType();
            ElementhrtsManager.LoadElementHrtsBase();
            ElementhrtsManager.LoadElementHrtsLevelInfo();
            ElementhrtsManager.LoadSpecialElementHrtsExp();

            ////////////////////////////////////////////////////////////////////////////////////
            
            //从数据库中获取配置参数
            GameManager.GameConfigMgr.LoadGameConfigFromDBServer();

            // 装入白名单
            LoadIPList("");

            //初始化炎黄战场
            GameManager.BattleMgr.Init();

            //初始化角斗场
            GameManager.ArenaBattleMgr.Init();

            //初始化生肖竞猜
            GameManager.ShengXiaoGuessMgr.Init();

            ////////////////////////////////////////////////////////////////////////////////////

            //从DBServer获取永久的公告数据
            GameManager.BulletinMsgMgr.LoadBulletinMsgFromDBServer();

            //从DBServer加载帮旗字典数据
            JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();

            //从DBServer加载领地帮会字典数据
            JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();

            //解析插旗战的日期和时间
            JunQiManager.ParseWeekDaysTimes();

            //程序启动时从DBServer更新皇帝的ID
            HuangChengManager.LoadHuangDiRoleIDFromDBServer(JunQiManager.GetBHIDByLingDiID((int)LingDiIDs.HuangCheng));

            //解析皇城战的日期和时间
            HuangChengManager.ParseWeekDaysTimes();

            //程序启动时从DBServer更新王族的信息D
            WangChengManager.UpdateWangZuBHNameFromDBServer(JunQiManager.GetBHIDByLingDiID((int)LingDiIDs.HuangGong));

            //解析王城战的日期和时间
            WangChengManager.ParseWeekDaysTimes();

            //初始化背包相关参数
            Global.InitBagParams();

            //初始化古墓地图相关
            Global.InitGuMuMapCodes();
            Global.InitVipGumuExpMultiple();

            //初始化冥界地图列表
            Global.InitMingJieMapCodeList();

            //初始化套装品质加成信息
            Global.InitDecreaseInjureInfo();

            //初始化套装强化加成信息
            Global.InitAllForgeLevelInfo();

            //载入物品记录日志标记
            Global.LoadItemLogMark();

            //复活时需要公告的怪物ID
            Global.LoadReliveMonsterGongGaoMark();

            ////////////////////////////////////////////////////////////////////////////////////

            //lua语言解析器对象(线程安全)
            //GameManager.SystemLuaVM.Init((int)Global.GetSafeAttributeLong(xml, "Lua", "InitNum"));

            //初始化活动配置项，失败抛出异常
            HuodongCachingMgr.LoadActivitiesConfig();

            //加载节日活动配置项,失败抛出异常
            HuodongCachingMgr.LoadJieriActivitiesConfig();

            Global.InitMapSceneTypeDict();

            // 初始化血色堡垒 [11/8/2013 LiaoWei]
            //GameManager.BloodCastleMgr.InitBloodCastle();

            // 初始化恶魔广场 [12/29/2013 LiaoWei]
            //DaimonSquareSceneManager.InitDaimonSquare();

            // 初始化天使神殿 [3/25/2014 LiaoWei]
            GameManager.AngelTempleMgr.InitAngelTemple();

            // 初始化血色城堡副本(最高积分等信息) [7/8/2014 LiaoWei]
            GameManager.BloodCastleCopySceneMgr.InitBloodCastleCopyScene();

            // 初始化恶魔广场(最高积分等信息) [7/11/2014 LiaoWei]
            GameManager.DaimonSquareCopySceneMgr.InitDaimonSquareCopyScene();

            // 加载星座信息 [7/31/2014 LiaoWei]
            GameManager.StarConstellationMgr.LoadStarConstellationTypeInfo();
            GameManager.StarConstellationMgr.LoadStarConstellationDetailInfo();

            //加载采集配置
            CaiJiLogic.LoadConfig();
            GameManager.GuildCopyMapMgr.LoadGuildCopyMapOrder();

            //加载合服活动配置项,失败抛出异常
            //HuodongCachingMgr.LoadHeFuActivitiesConfig();

            //全局服务管理器初始化
            GlobalServiceManager.initialize();

            //全局服务管理器启动
            GlobalServiceManager.startup();

        }

        /// <summary>
        /// 初始化怪物管理对象
        /// </summary>
        private void InitMonsterManager()
        {
            //GameManager.MonsterMgr.CycleExecute += ExecuteBackgroundWorkers;
        }

        /// <summary>
        /// 线程池驱动定时器
        /// </summary>
        private static Timer ThreadPoolDriverTimer = null;

        /// <summary>
        /// 日志线程池驱动定时器
        /// </summary>
        private static Timer LogThreadPoolDriverTimer = null;

        /// <summary>
        /// 初始化线程池驱动定时器
        /// </summary>
        protected static void StartThreadPoolDriverTimer()
        {
            ThreadPoolDriverTimer = new Timer(ThreadPoolDriverTimer_Tick, null, 1000, 1000);
            LogThreadPoolDriverTimer = new Timer(LogThreadPoolDriverTimer_Tick, null, 500, 500);
        }

        /// <summary>
        /// 停止定时器
        /// </summary>
        protected static void StopThreadPoolDriverTimer()
        {
            ThreadPoolDriverTimer.Change(Timeout.Infinite, Timeout.Infinite);
            LogThreadPoolDriverTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }        

        /// <summary>
        /// 线程驱动定时器
        /// </summary>
        /// <param name="sender"></param>
        protected static void ThreadPoolDriverTimer_Tick(Object sender)
        {
            try
            {
                //驱动后台线程池
                ServerConsole.ExecuteBackgroundWorkers(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                {
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "ThreadPoolDriverTimer_Tick", false);
                    //throw ex;
                }//);
            }
        }

        /// <summary>
        /// 日志线程驱动定时器
        /// </summary>
        /// <param name="sender"></param>
        public static void LogThreadPoolDriverTimer_Tick(Object sender)
        {
            try
            {
                ServerConsole.ExecuteBackgroundLogWorkers(null, EventArgs.Empty);                
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "LogThreadPoolDriverTimer_Tick", false);
            }
        }

        #endregion 初始化部分

        #region 线程部分

        /// <summary>
        /// 后台线程
        /// </summary>
        BackgroundWorker eventWorker;

        /// <summary>
        /// 数据库命令执行线程
        /// </summary>
        BackgroundWorker dbCommandWorker;

        /// <summary>
        /// 日志数据库命令执行线程
        /// </summary>
        BackgroundWorker logDBCommandWorker;

        /// <summary>
        /// 角色调度线程
        /// </summary>
        BackgroundWorker clientsWorker;

        /// <summary>
        /// 角色Buffers补血补魔线程
        /// </summary>
        BackgroundWorker buffersWorker;

        /// <summary>
        /// 角色DB线程
        /// </summary>
        BackgroundWorker spriteDBWorker;

        /// <summary>
        /// 后台处理线程
        /// </summary>
        BackgroundWorker othersWorker;

        /// <summary>
        /// 角色战斗调度线程
        /// </summary>
        BackgroundWorker FightingWorker;

        /// <summary>
        /// 转发聊天消息调度线程
        /// </summary>
        BackgroundWorker chatMsgWorker;

        /// <summary>
        /// 副本度线程
        /// </summary>
        BackgroundWorker fuBenWorker;

        /// <summary>
        /// 写DB日志线程
        /// </summary>
        BackgroundWorker dbWriterWorker;

        /// <summary>
        /// 套接字缓冲数据发送线程
        /// </summary>
        BackgroundWorker SocketSendCacheDataWorker;

        /// <summary>
        /// 生肖竞猜调度线程
        /// </summary>
        BackgroundWorker ShengXiaoGuessWorker;

        /// <summary>
        /// 主调度线程,这个线程一直处于循环状态，不断的处理各种逻辑判断,相当于原来的主界面线程
        /// </summary>
        BackgroundWorker MainDispatcherWorker;

        /// <summary>
        /// 角色拓展线程
        /// </summary>
        //BackgroundWorker RoleExtensionWorker;

        /// <summary>
        /// 网络数据包处理线程多个
        /// </summary>
        //BackgroundWorker[] CmdPacketProcessWorkers;

        /// <summary>
        /// 最大工作线程
        /// </summary>
        //private int MaxCmdPacketProcessWorkerNum = 16;

//         /// <summary>
//         /// 怪物驱动线程多个
//         /// </summary>
//         BackgroundWorker[] MonsterProcessWorkers;

        /// <summary>
        /// 怪物驱动线程池
        /// </summary>
        private ScheduleExecutor monsterExecutor = null;

        /// <summary>
        /// 最大怪物驱动线程
        /// </summary>
        private int MaxMonsterProcessWorkersNum = 5;

        /// <summary>
        /// 角色后台驱动线程多个
        /// </summary>
        //BackgroundWorker[] BackgroundClientsWorkers;

        /// <summary>
        /// 角色后台驱动线程个数
        /// </summary>
        //private int MaxBackgroundClientsWorkersNum = 5;

        /// <summary>
        /// 九宫格状态更新线程多个
        /// </summary>
        BackgroundWorker[] Gird9UpdateWorkers;

        /// <summary>
        /// 最大九宫格状态更新线程个数
        /// </summary>
        public static int MaxGird9UpdateWorkersNum = 5;

        /// <summary>
        /// 角色故事版驱动线程
        /// </summary>
        BackgroundWorker RoleStroyboardDispatcherWorker;

        /// <summary>
        /// 是否是要立刻关闭
        /// </summary>
        private bool MustCloseNow = false;

        /// <summary>
        /// 是否进入了关闭模式
        /// </summary>
        private bool EnterClosingMode = false;

        /// <summary>
        /// 60秒钟的倒计时器
        /// </summary>
        private int ClosingCounter = 30 * 200;

        /// <summary>
        /// 最近一次写数据库日志的时间
        /// </summary>
        private long LastWriteDBLogTicks = DateTime.Now.Ticks / 10000;     
   
        /// <summary>
        /// 执行日志后台线程对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteBackgroundLogWorkers(object sender, EventArgs e)
        {
            try
            {
                if (!logDBCommandWorker.IsBusy) { logDBCommandWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "logDBCommandWorker", false);
            }
        }

        /// <summary>
        /// 执行后台线程对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteBackgroundWorkers(object sender, EventArgs e)
        {
            //后台工作者事件难道需要不断的激发

            /*
            if (!eventWorker.IsBusy) { eventWorker.RunWorkerAsync(); }
            if (!dbCommandWorker.IsBusy) { dbCommandWorker.RunWorkerAsync(); }
            if (!clientsWorker.IsBusy) { clientsWorker.RunWorkerAsync(); }
            if (!FightingWorker.IsBusy) { FightingWorker.RunWorkerAsync(); }
            if (!chatMsgWorker.IsBusy) { chatMsgWorker.RunWorkerAsync(); }
            if (!fuBenWorker.IsBusy) { fuBenWorker.RunWorkerAsync(); }
            if (!dbWriterWorker.IsBusy) { dbWriterWorker.RunWorkerAsync(); }
            if (!SocketSendCacheDataWorker.IsBusy) { SocketSendCacheDataWorker.RunWorkerAsync(); }
            if (!ShengXiaoGuessWorker.IsBusy) { ShengXiaoGuessWorker.RunWorkerAsync(); }
            if (!TrivalWorker.IsBusy) { TrivalWorker.RunWorkerAsync(); }
            */

            try
            {
                if (!eventWorker.IsBusy) { eventWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "eventWorker", false);
            }

            try
            {
                if (!dbCommandWorker.IsBusy) { dbCommandWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "dbCommandWorker", false);
            }

            try
            {
                if (!clientsWorker.IsBusy) { clientsWorker.RunWorkerAsync(0); }

                //启动角色处理线程
                /*for (int nThread = 0; nThread < MaxBackgroundClientsWorkersNum; nThread++)
                {
                    if (!BackgroundClientsWorkers[nThread].IsBusy)
                    {
                        BackgroundClientsWorkers[nThread].RunWorkerAsync(nThread);
                    }
                }*/
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "clientsWorker", false);
            }
            
            try
            {
                if (!buffersWorker.IsBusy) { buffersWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "buffersWorker", false);
            }
            
            try
            {
                if (!spriteDBWorker.IsBusy) { spriteDBWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "spriteDBWorker", false);
            }
            
            try
            {
                if (!othersWorker.IsBusy) { othersWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "othersWorker", false);
            }

            try
            {
                if (!FightingWorker.IsBusy) { FightingWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "FightingWorker", false);
            }

            try
            {
                if (!chatMsgWorker.IsBusy) { chatMsgWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "chatMsgWorker", false);
            }
            try
            {
                if (!fuBenWorker.IsBusy) { fuBenWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "fuBenWorker", false);
            }

            try
            {
                if (!dbWriterWorker.IsBusy) { dbWriterWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "dbWriterWorker", false);
            }

            try
            {
                if (!SocketSendCacheDataWorker.IsBusy) { SocketSendCacheDataWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "SocketSendCacheDataWorker", false);
            }

            try
            {
                if (!ShengXiaoGuessWorker.IsBusy) { ShengXiaoGuessWorker.RunWorkerAsync(); }
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ShengXiaoGuessWorker", false);
            }

            CalcGCInfo();
        }

        /// <summary>
        /// 原来的 closingTimer_Tick(object sender, EventArgs e)
        /// 显示关闭信息的计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string title = "";

                //关闭角色
                GameClient client = GameManager.ClientMgr.GetRandomClient();
                if (null != client)
                {
                    title = string.Format("游戏服务器{0}, 关闭中, 剩余{1}个角色", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());
                    Global.ForceCloseClient(client);
                }
                else
                {
                    //关闭倒计时
                    ClosingCounter -= 200;

                    //判断DB的命令队列是否已经执行完毕?
                    if (ClosingCounter <= 0)
                    {
                        //不再发送数据
                        Global._SendBufferManager.Exit = true;

                        //是否立刻关闭
                        MustCloseNow = true;

                        //程序主窗口
                        //GameManager.AppMainWnd.Close();
                        //Window_Closing();//没必要调用
                    }
                    else
                    {
                        int counter = GameManager.DBCmdMgr.GetDBCmdCount() + (ClosingCounter / 200);
                        title = string.Format("游戏服务器{0}, 关闭中, 倒计时:{1}", GameManager.ServerLineID, counter);
                    }
                }

                //设置标题
                Console.Title = title;
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                {
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "closingTimer_Tick", false);
                    //throw ex;
                }//);
            }
        }

        private long LastAuxiliaryTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 怪物Ai攻击索引
        /// </summary>
        //private int IndexOfMonsterAiAttack = 0;

        /// <summary>
        /// 计时器函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void auxiliaryTimer_Tick(object sender, EventArgs e)
        {
            string warning = "";

            long ticksA = DateTime.Now.Ticks / 10000;
            try
            {
                long ticks1 = DateTime.Now.Ticks / 10000;

                if (ticks1 - LastAuxiliaryTicks > 1000)
                {
                    warning = String.Format("\r\nauxiliaryTimer_Tick开始执行经过时间:{0}毫秒", ticks1 - LastAuxiliaryTicks);
                    DoLog(warning);
                }

                LastAuxiliaryTicks = ticks1;

                /*
                ticks1 = DateTime.Now.Ticks / 10000;

                //执行后台线程对象
                //ExecuteBackgroundWorkers(null, EventArgs.Empty);

                long ticks2 = DateTime.Now.Ticks / 10000;

                if (ticks2 > ticks1 + 1000)
                {
                    warning = String.Format("ExecuteBackgroundWorkers 消耗:{0}毫秒", ticks2 - ticks1);
                    DoLog(warning);
                }
                */

                ticks1 = DateTime.Now.Ticks / 10000;

                //if (IndexOfMonsterAiAttack >= 320000)
                //{
                //    IndexOfMonsterAiAttack = 0;
                //}

                //IndexOfMonsterAiAttack++;

                // 简单的战斗调度
                //GameManager.MonsterMgr.DoMonsterAttack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, IndexOfMonsterAiAttack);

                //long ticks2 = DateTime.Now.Ticks / 10000;

                //if (ticks2 > ticks1 + 1000)
                //{
                //    warning = String.Format("DoMonsterAttack 消耗:{0}毫秒", ticks2 - ticks1);
                //    DoLog(warning);
                //}

                ticks1 = DateTime.Now.Ticks / 10000;

                //定时刷新怪
                GameManager.MonsterZoneMgr.RunMapMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                long ticks2 = DateTime.Now.Ticks / 10000;

                if (ticks2 > ticks1 + 1000)
                {
                    warning = String.Format("RunMapMonsters 消耗:{0}毫秒", ticks2 - ticks1);
                    DoLog(warning);
                }

                ticks1 = DateTime.Now.Ticks / 10000;

                //怪物死亡调度
                GameManager.MonsterMgr.DoMonsterDeadCall();

                ticks2 = DateTime.Now.Ticks / 10000;

                if (ticks2 > ticks1 + 1000)
                {
                    warning = String.Format("DoMonsterDeadCall 消耗:{0}毫秒", ticks2 - ticks1);
                    DoLog(warning);
                }

                ticks1 = DateTime.Now.Ticks / 10000;

                //补充DBserver连接
                Global._TCPManager.tcpClientPool.Supply();

                //补充LogDBserver连接
                Global._TCPManager.tcpLogClientPool.Supply();

                ticks2 = DateTime.Now.Ticks / 10000;

                if (ticks2 > ticks1 + 1000)
                {
                    warning = String.Format("tcpClientPool.Supply 消耗:{0}毫秒", ticks2 - ticks1);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                {
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "auxiliaryTimer_Tick", false);
                    //throw ex;
                }//);
            }

            long ticksB = DateTime.Now.Ticks / 10000;

            //超过2秒记录
            if (ticksB > ticksA + 1000)
            {
                warning = String.Format("auxiliaryTimer_Tick 消耗:{0}毫秒", ticksB - ticksA);
                DoLog(warning);
            }
        }

        /// <summary>
        /// 记录日志，控制台打印或者记录到文件
        /// </summary>
        /// <param name="warning"></param>
        private void DoLog(String warning)
        {
            LogManager.WriteLog(LogTypes.Error, warning);
        }

        //private long LastMonsterHeartTicks = DateTime.Now.Ticks / 10000;

//         /// <summary>
//         /// 怪物生命心跳计时器函数
//         /// </summary>
//         /// <param name="sender"></param>
//         /// <param name="e"></param>
//         //public void monsterHeartTimer_Tick(object sender, EventArgs e)
//         public void monsterHeartTimer_Tick(int threadID, bool canDoAttack)
//         {
//             try
//             {
//                 long startTicks = DateTime.Now.Ticks / 10000;
// 
//                 //if (ticks1 - LastMonsterHeartTicks > 1000)//一般都超过160毫秒
//                 //{
//                 //    DoLog(String.Format("\r\nmonsterHeartTimer_Tick开始执行经过时间:{0}毫秒", ticks1 - LastMonsterHeartTicks));
//                 //}
// 
//                 //LastMonsterHeartTicks = ticks1;
// 
//                 long ticks1 = DateTime.Now.Ticks / 10000;
// 
//                 for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
//                 {
//                     if (i % MaxMonsterProcessWorkersNum != threadID)
//                     {
//                         continue;
//                     }
// 
//                     int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;
// 
//                     // 怪物生命心跳调度函数
//                     GameManager.MonsterMgr.DoMonsterHeartTimer(mapCode);
//                 }
// 
//                 long ticks2 = DateTime.Now.Ticks / 10000;
// 
//                 if (ticks2 > ticks1 + 1000)
//                 {
//                     DoLog(String.Format("DoMonsterHeartTimer 消耗:{0}毫秒", ticks2 - ticks1));
//                 }
// 
//                 ticks1 = DateTime.Now.Ticks / 10000;
// 
//                 //是否需要执行
//                 if (canDoAttack)
//                 {
//                     for (int i = 0; i < GameManager.MapMgr.DictMaps.Values.Count; i++)
//                     {
//                         if (i % MaxMonsterProcessWorkersNum != threadID)
//                         {
//                             continue;
//                         }
// 
//                         int mapCode = GameManager.MapMgr.DictMaps.Values.ElementAt(i).MapCode;
// 
//                         // 简单的战斗调度
//                         GameManager.MonsterMgr.DoMonsterAttack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 0, mapCode);
//                     }
//                 }
// 
//                 ticks2 = DateTime.Now.Ticks / 10000;
// 
//                 if (ticks2 > ticks1 + 500)
//                 {
//                     DoLog(String.Format("DoMonsterAttack 消耗:{0}毫秒", ticks2 - ticks1));
//                 }
// 
//                 ticks2 = DateTime.Now.Ticks / 10000;
//                 if (ticks2 - startTicks > 500)
//                 {
//                     DoLog(String.Format("monsterHeartTimer_Tick 消耗:{0}毫秒", ticks2 - startTicks));
//                 }
//             }
//             catch (Exception ex)
//             {
//                 //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
//                 {
//                     // 格式化异常错误信息
//                     DataHelper.WriteFormatExceptionLog(ex, "monsterHeartTimer_Tick", false);
//                     //throw ex;
//                 }//);
//             }
//         }

        /// <summary>
        /// 后台主调度线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainDispatcherWorker_DoWork(object sender, EventArgs e)
        {
            long lastTicks = DateTime.Now.Ticks / 10000;
            long startTicks = DateTime.Now.Ticks / 10000;
            long endTicks = DateTime.Now.Ticks / 10000;

            //睡眠时间
            int maxSleepMs = 100;
            int sleepMs = 100;

            int nTimes = 0;

            while (true)
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000;

                    //if (nTimes % 5 == 0)
                    if (startTicks - lastTicks >= 500)
                    {
                        lastTicks = startTicks;

                        //辅助调度--->500毫秒执行一次
                        auxiliaryTimer_Tick(null, null);
                    }

                    //怪物调度---> 原来50毫秒执行一次，现在放宽，100毫秒执行一次
                    //monsterHeartTimer_Tick(null, null);

                    //驱动怪物的移动故事板
                    //StoryBoardEx.runStoryBoards();

                    if (NeedExitServer)
                    {
                        if (nTimes % 2 == 0)
                        {
                            //调度关闭操作--->原来200毫秒执行一次
                            closingTimer_Tick(null, null);

                            //关闭完毕，自己也该退出了
                            if (MustCloseNow)
                            {
                                break;
                            }
                        }
                    }

                    endTicks = DateTime.Now.Ticks / 10000;

                    //最多睡眠100毫秒，最少睡眠1毫秒
                    sleepMs = (int)Math.Max(5, maxSleepMs - (endTicks - startTicks));

                    Thread.Sleep(sleepMs);

                    nTimes++;

                    if (nTimes >= 100000)
                    {
                        nTimes = 0;
                    }
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

            System.Console.WriteLine("主循环线程退出，回车退出系统");
            if (0 != GetServerPIDFromFile())
            {
                // 结束时将进程ID写入文件
                WritePIDToFile("Stop.txt");

                StopThreadPoolDriverTimer();
            }
        }

        /// <summary>
        /// 角色故事版驱动线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoleStroyboardDispatcherWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            long startTicks = DateTime.Now.Ticks / 10000;
            long endTicks = DateTime.Now.Ticks / 10000;

            //睡眠时间
            int maxSleepMs = 100; //这里不比客户端太频繁了，没啥好处
            int sleepMs = 100;

            int nTimes = 0;

            while (true)
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000;

                    long ticks1 = startTicks;

                    //驱动角色故事版
                    StoryBoard4Client.runStoryBoards();

                    long ticks2 = DateTime.Now.Ticks / 10000;

                    if (ticks2 > ticks1 + 1000)
                    {
                        DoLog(String.Format("StoryBoard4Client.runStoryBoards 消耗:{0}毫秒", ticks2 - ticks1));
                    }

                    if (NeedExitServer)
                    {
                        if (nTimes % 2 == 0)
                        {
                            //调度关闭操作--->原来200毫秒执行一次
                            closingTimer_Tick(null, null);

                            //关闭完毕，自己也该退出了
                            if (MustCloseNow)
                            {
                                break;
                            }
                        }
                    }

                    endTicks = DateTime.Now.Ticks / 10000;

                    //最多睡眠20毫秒，最少睡眠1毫秒
                    sleepMs = (int)Math.Max(5, maxSleepMs - (endTicks - startTicks));

                    Thread.Sleep(sleepMs);

                    nTimes++;

                    if (nTimes >= 100000)
                    {
                        nTimes = 0;
                    }

                    //Thread.Sleep((int)Global.GetRandomNumber(200, 300)); ///模拟测试不能穿人的地图的卡顿情况
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "RoleStroyboardDispatcherWorker_DoWork", false);
                }
            }

            System.Console.WriteLine("角色故事版驱动线程退出，回车退出系统");
        }

        //后台处理工作事件
        private void eventWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 将事件写入日志
                while (GameManager.SystemServerEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleLoginEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleLogoutEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleTaskEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleDeathEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithTongQianEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithYinLiangEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithJunGongEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithYinPiaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithYuanBaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleQiZhenGeBuyWithYuanBaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleQiangGouBuyWithYuanBaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleSaleEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleExchangeEvents1.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleExchangeEvents2.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleExchangeEvents3.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleUpgradeEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleGoodsEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleFallGoodsEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleYinLiangEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleHorseEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBangGongEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleJingMaiEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleRefreshQiZhenGeEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleWaBaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleMapEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleFuBenAwardEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleWuXingAwardEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRolePaoHuanOkEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleYaBiaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleLianZhanEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleHuoDongMonsterEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleAutoSubYuanBaoEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleAutoSubGoldEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleAutoSubEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleDigTreasureWithYaoShiEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithTianDiJingYuanEvents.WriteEvent())
                {
                    ;
                }

                while (GameManager.SystemRoleFetchMailMoneyEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleFetchVipAwardEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleGoldEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithGoldEvents.WriteEvent())
                {
                    ;
                }

                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithJingYuanZhiEvents.WriteEvent())
                {
                    ;
                }
                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithLieShaZhiEvents.WriteEvent())
                {
                    ;
                }
                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithZhuangBeiJiFenEvents.WriteEvent())
                {
                    ;
                }
                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithJunGongZhiEvents.WriteEvent())
                {
                    ;
                }
                // 将事件写入日志
                while (GameManager.SystemRoleBuyWithZhanHunEvents.WriteEvent())
                {
                    ;
                }
                // 将事件写入日志
                while (GameManager.SystemRoleUserMoneyEvents.WriteEvent())
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "eventWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        //后台数据库命令处理工作事件
        private void dbCommandWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //向DBserver发送服务器心跳信息, 2011-05-31 修改，精简指令
                //Global.SendServerHeart();

                //处理反注册的用户ID
                //多余的操作，会导致重复登录的漏洞，录入A冲掉B，B马上登录，A服务器的延迟反注册会清掉B登录的标记。导致A也可以登录
                //UnregisterUserIDMgr.ProcessUnRegisterUserIDsQueue();

                //处理超时为关闭的连接，清空用户数据
                DelayForceClosingMgr.ProcessDelaySockets();

                //向数据库提交数据异步的数据修改操作
                GameManager.DBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool);
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "dbCommandWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        // 后台日志数据库命令处理工作事件
        private void logDBCommandWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // 向日志数据库提交数据异步的数据修改操作
                GameManager.logDBCmdMgr.ExecuteDBCmd(Global._TCPManager.tcpLogClientPool, Global._TCPManager.TcpOutPacketPool);
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "logDBCommandWorker_DoWork", false);
            }
        }

        /// <summary>
        /// 角色和包裹后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientsWorker_DoWork(object sender, EventArgs e)
        {
            DoWorkEventArgs de = e as DoWorkEventArgs;

            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //角色的后台工作
                GameManager.ClientMgr.DoSpriteBackgourndWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("clientsWorker_DoWork{0} 消耗:{1}毫秒", (int)de.Argument, ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, string.Format("clientsWorker_DoWork{0}", (int)de.Argument), false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 角色buffers后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buffersWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //角色的buffer后台工作
                GameManager.ClientMgr.DoSpriteBuffersWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("buffersWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(ex, string.Format("buffersWorker_DoWork"), false);
                //throw ex;
                //});
            }
        }

        /// <summary>
        /// 角色DB后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spriteDBWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //角色的buffer后台工作
                GameManager.ClientMgr.DoSpriteDBWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("spriteDBWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(ex, string.Format("spriteDBWorker_DoWork"), false);
                //throw ex;
                //});
            }
        }

        /// <summary>
        /// 杂项后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void othersWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //基于格子的魔法
                GameManager.GridMagicHelperMgr.ExecuteAllItems();

                //处理超时的公告消息
                GameManager.BulletinMsgMgr.ProcessBulletinMsg();

                /// 处理已经掉落的特效超时
                DecorationManager.ProcessAllDecos(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                /// 处理已经掉落的包裹超时
                GameManager.GoodsPackMgr.ProcessAllGoodsPackItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                /// 处理已经镖车的超时
                BiaoCheManager.ProcessAllBiaoCheItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                /// 处理已经假人的超时
                FakeRoleManager.ProcessAllFakeRoleItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                /// 处理是否是打坐经验和灵力翻倍的时间
                SpecailTimeManager.ProcessDoulbeExperience();

                /// 处理开服在线有礼的动作
                HuodongCachingMgr.ProcessKaiFuGiftAwardActions();

                // 延迟动作心跳 [11/23/2013 LiaoWei]
                DelayActionManager.HeartBeatDelayAction();

                // 消息推送 [5/3/2014 LiaoWei]
                //Global.TiggerPushMessage();
                
                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("othersWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(ex, "othersWorker_DoWork", false);
                //throw ex;
                //});
            }
        }

        /// <summary>
        /// 点将台和大乱斗调度和管理后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FightingWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //角斗场调度
                GameManager.ArenaBattleMgr.Process();

                //炎黄战场调度
                GameManager.BattleMgr.Process();

                //点将台调度
                GameManager.DJRoomMgr.ProcessFighting();

                /// 处理帮旗死亡操作
                JunQiManager.ProcessAllJunQiItems(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool);

                /// 处理领地战帮旗安插的结果
                JunQiManager.ProcessLingDiZhanResult();

                //处理皇城战的战斗结果
                HuangChengManager.ProcessHuangChengZhanResult();

                //处理王城战的战斗结果
                WangChengManager.ProcessWangChengZhanResult();

                // 血色堡垒 [11/9/2013 LiaoWei]
                //GameManager.BloodCastleMgr.HeartBeatBloodCastScene();

                // 恶魔广场 [12/25/2013 LiaoWei]
                //DaimonSquareSceneManager.HeartBeatDaimonSquareScene();

                // 天使神殿 [3/25/2014 LiaoWei]
                GameManager.AngelTempleMgr.HeartBeatAngelTempleScene();

                // boss之家 [4/8/2014 LiaoWei]
                GameManager.BosshomeMgr.HeartBeatBossHomeScene();

                // 黄金神庙 [4/8/2014 LiaoWei]
                GameManager.GoldTempleMgr.HeartBeatGoldtempleScene();

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("FightingWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "FightingWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 生肖运程竞猜调度和管理后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShengXiaoGuessWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //生肖运程调度
                GameManager.ShengXiaoGuessMgr.Process();

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("ShengXiaoGuessWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "ShengXiaoGuessWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 聊天消息转发后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatMsgWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //从DBServer更新禁止聊天发言的列表
                BanChatManager.GetBanChatDictFromDBServer();

                //处理聊天消息转发的操作
                GameManager.ClientMgr.HandleTransferChatMsg();

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("chatMsgWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
               //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "chatMsgWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 副本度线程后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fuBenWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                //一些副本要计算角色伤害列表,发送给客户端
                GameManager.CopyMapMgr.CheckCopyTeamDamage(ticksA, false);

                //处理超时的副本，从副本管理中删除
                GameManager.CopyMapMgr.ProcessEndCopyMap();

                //设置开始更新的标志
                GameManager.CopyMapMgr.ProcessEndGuildCopyMapFlag();

                //处理跨周的帮会副本，从副本管理中删除
                GameManager.CopyMapMgr.ProcessEndGuildCopyMap(ticksA);

                // 处理新手场景 -- 心跳 [10/18/2013 LiaoWei]
                FreshPlayerCopySceneManager.HeartBeatFreshPlayerCopyMap();

                // 处理经验副本 -- 心跳 [3/18/2014 LiaoWei]
                ExperienceCopySceneManager.HeartBeatExperienceCopyMap();

                // 处理金币副本 -- 心跳 [6/11/2014 LiaoWei]
                GlodCopySceneManager.HeartBeatGlodCopyScene();

                EMoLaiXiCopySceneManager.HeartBeatEMoLaiXiCopyScene();

                // 处理血色城堡副本 -- 心跳 [7/7/2014 LiaoWei]
                GameManager.BloodCastleCopySceneMgr.HeartBeatBloodCastScene();

                // 处理恶魔广场副本 -- 心跳 [7/11/2014 LiaoWei]
                GameManager.DaimonSquareCopySceneMgr.HeartBeatDaimonSquareScene();

                //驱动广播列表
                BroadcastInfoMgr.ProcessBroadcastInfos();

                //驱动弹窗列表
                PopupWinMgr.ProcessPopupWins();

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("fuBenWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "fuBenWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// DB写日志线程后台工作函数
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

                //最近一次写数据库日志的时间
                LastWriteDBLogTicks = ticks;

                //将缓存中的日志写入文件中
                //bool ret = GameManager.DBEventsWriter.WriteToHardDisk();

                //if (!ret)
                //{
                //    LogManager.WriteLog(LogTypes.Error, string.Format("将缓存中的DB日志写入文件中时发生错误:{0}", GameManager.DBEventsWriter.LastError));
                //}
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "dbWriterWorker_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 客户端套接字发送数据线程后台工作函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketSendCacheDataWorker_DoWork(object sender, EventArgs e)
        {
            try
            {
                long ticksA = DateTime.Now.Ticks / 10000;

                Global._SendBufferManager.TrySendAll();

                long ticksB = DateTime.Now.Ticks / 10000;

                //超过2秒记录
                if (ticksB > ticksA + 1000)
                {
                    string warning = String.Format("SocketSendCacheDataWorker_DoWork 消耗:{0}毫秒", ticksB - ticksA);
                    DoLog(warning);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "SocketFlushBuffer_DoWork", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 网络命令包处理线程后台工作函数,只要系统不退出，它就一直运行--->同时有很多线程在运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdPacketProcessWorker_DoWork(object sender, EventArgs e)
        {
            Queue<CmdPacket> ls = new Queue<CmdPacket>();

            //只要不退出，就一直执行
            while (!NeedExitServer)
            {
                try
                {
                    Global._TCPManager.ProcessCmdPackets(ls);
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "CmdPacketProcessWorker_DoWork", false);
                }

                //简单随眠一毫秒就行
                Thread.Sleep(5);
            }
        }

//         /// <summary>
//         /// 怪物驱动处理线程后台工作函数,只要系统不退出，它就一直运行--->同时有很多线程在运行
//         /// </summary>
//         /// <param name="sender"></param>
//         /// <param name="e"></param>
//         private void MonsterProcessWorker_DoWork(object sender, EventArgs e)
//         {
//             DoWorkEventArgs de = e as DoWorkEventArgs;
//             //System.Diagnostics.Debug.WriteLine("MonsterProcessWorker_DoWork, theadID=" + de.Argument.ToString());
// 
//             long startTicks = DateTime.Now.Ticks / 10000;
//             long endTicks = DateTime.Now.Ticks / 10000;
// 
//             //睡眠时间
//             int maxSleepMs = 80;
//             int sleepMs = 80;
// 
//             int nTimes = 0;
// 
//             while (!NeedExitServer)
//             {
//                 try
//                 {
//                     startTicks = DateTime.Now.Ticks / 10000;
// 
//                     //怪物调度---> 原来50毫秒执行一次，现在放宽，100毫秒执行一次
//                     monsterHeartTimer_Tick((int)de.Argument, nTimes % 5 == 0);
// 
//                     endTicks = DateTime.Now.Ticks / 10000;
// 
//                     //最多睡眠100毫秒，最少睡眠1毫秒
//                     sleepMs = (int)Math.Max(5, maxSleepMs - (endTicks - startTicks));
// 
//                     Thread.Sleep(sleepMs);
// 
//                     nTimes++;
// 
//                     if (nTimes >= 100000)
//                     {
//                         nTimes = 0;
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     DataHelper.WriteFormatExceptionLog(ex, "MonsterProcessWorker_DoWork", false);
//                 }
//             }
// 
//             System.Console.WriteLine("怪物驱动线程退出...");
//         }
        
        /// <summary>
        /// 9宫格更新驱动处理线程后台工作函数,只要系统不退出，它就一直运行--->同时有很多线程在运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gird9UpdateWorker_DoWork(object sender, EventArgs e)
        {
            DoWorkEventArgs de = e as DoWorkEventArgs;
            //System.Diagnostics.Debug.WriteLine("Gird9UpdateWorker_DoWork, theadID=" + de.Argument.ToString());

            long startTicks = DateTime.Now.Ticks / 10000;
            long endTicks = DateTime.Now.Ticks / 10000;

            //睡眠时间
            int maxSleepMs = 100;
            int sleepMs = 100;

            int nTimes = 0;

            while (!NeedExitServer)
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000;

                    long ticks1 = startTicks;

                    if (GameManager.Update9GridUsingNewMode <= 0)
                    {
                        maxSleepMs = 100;

                        //角色的定时舞台对象刷新后台工作
                        GameManager.ClientMgr.DoSpritesMapGridMove((int)de.Argument);
                    }
                    else
                    {
                        //更新移动位置时九宫格的时间
                        maxSleepMs = GameManager.MaxSlotOnPositionUpdate9GridsTicks;

                        //角色的定时舞台对象刷新后台工作
                        GameManager.ClientMgr.DoSpritesMapGridMoveNewMode((int)de.Argument);
                    }

                    long ticks2 = DateTime.Now.Ticks / 10000;
                    if (ticks2 > ticks1 + 1000)
                    {
                        DoLog(String.Format("DoSpritesMapGridMove, 序号:{0} 消耗:{1}毫秒", (int)de.Argument, ticks2 - ticks1));
                    }

                    // 同怪物一样，分地图驱动
                    //ticks1 = DateTime.Now.Ticks / 10000;

                    ////角色的buffer后台工作
                    //GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)de.Argument, MaxGird9UpdateWorkersNum);

                    //ticks2 = DateTime.Now.Ticks / 10000;
                    //if (ticks2 > ticks1 + 1000)
                    //{
                    //    DoLog(String.Format("DoSpriteExtensionWork, 序号:{0} 消耗:{1}毫秒", (int)de.Argument, ticks2 - ticks1));
                    //}

                    endTicks = DateTime.Now.Ticks / 10000;

                    //最多睡眠100毫秒，最少睡眠1毫秒
                    sleepMs = (int)Math.Max(5, maxSleepMs - (endTicks - startTicks));

                    Thread.Sleep(sleepMs);

                    nTimes++;

                    if (nTimes >= 100000)
                    {
                        nTimes = 0;
                    }
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "Gird9UpdateWorker_DoWork", false);
                }
            }

            System.Console.WriteLine(string.Format("9宫格更新驱动线程{0}退出...", (int)de.Argument));
        }

        /// <summary>
        /// 角色拓展线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoleExtensionWorker_DoWork(object sender, EventArgs e)
        {
            long startTicks = DateTime.Now.Ticks / 10000;
            long endTicks = DateTime.Now.Ticks / 10000;

            //睡眠时间
            int maxSleepMs = 100;
            int sleepMs = 100;

            int nTimes = 0;

            while (!NeedExitServer)
            {
                try
                {
                    startTicks = DateTime.Now.Ticks / 10000;

                    long ticks1 = startTicks;

                    //角色的buffer后台工作
                    GameManager.ClientMgr.DoSpriteExtensionWork(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 0, 0);                  

                    long ticks2 = DateTime.Now.Ticks / 10000;
                    if (ticks2 > ticks1 + 1000)
                    {
                        DoLog(String.Format("RoleExtensionWorker_DoWork, 消耗:{0}毫秒", ticks2 - ticks1));
                    }

                    endTicks = DateTime.Now.Ticks / 10000;

                    //最多睡眠100毫秒，最少睡眠1毫秒
                    sleepMs = (int)Math.Max(5, maxSleepMs - (endTicks - startTicks));

                    Thread.Sleep(sleepMs);

                    nTimes++;

                    if (nTimes >= 100000)
                    {
                        nTimes = 0;
                    }
                }
                catch (Exception ex)
                {
                    DataHelper.WriteFormatExceptionLog(ex, "RoleExtensionWorker_DoWork", false);
                }
            }

            System.Console.WriteLine("角色拓展线程退出");
        }

        #endregion 线程部分

        /// <summary>
        /// 退出程序
        /// 原来的Window_Closing(object sender, CancelEventArgs e)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing()
        {
            //是否立刻关闭
            if (MustCloseNow)
            {
                return;
            }

            //已经进入了关闭模式
            if (EnterClosingMode)
            {
                return;
            }

            //是否进入了关闭模式
            EnterClosingMode = true;

            //设置不再接受新的请求，就是接受到后，立刻关闭
            //是否不再接受新的用户
            Global._TCPManager.MySocketListener.DontAccept = true;

            LastWriteDBLogTicks = 0; //强迫写缓存

            //设置退出标志
            NeedExitServer = true;
        }

        #endregion 游戏服务器具体功能部分

        #region 获取编译日期

        /// <summary>
        /// 获取程序的编译日期
        /// </summary>
        /// <returns></returns>
        public static string GetVersionDateTime()
        {
            AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            int revsion = Assembly.GetExecutingAssembly().GetName().Version.Revision;//获取修订号
            int build = Assembly.GetExecutingAssembly().GetName().Version.Build;//获取内部版本号
            DateTime dtbase = new DateTime(2000, 1, 1, 0, 0, 0);//微软编译基准时间
            TimeSpan tsbase = new TimeSpan(dtbase.Ticks);
            TimeSpan tsv = new TimeSpan(tsbase.Days + build, 0, 0, revsion * 2);//编译时间，注意修订号要*2
            DateTime dtv = new DateTime(tsv.Ticks);//转换成编译时间
            return dtv.ToString("yyyy-MM-dd HH") + string.Format(" {0}", AssemblyFileVersion.FilePrivatePart);
        }

        #endregion 获取编译日期
    }
}
