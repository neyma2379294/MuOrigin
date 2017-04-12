using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Server.CmdProcessor;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Logic.Wing;

namespace GameDBServer.Logic
{

    /// <summary>
    /// 功能模块管理器接口
    /// </summary>
    public interface IManager
    {
        bool initialize();
        bool startup();
        bool showdown();
        bool destroy();
    }

    /// <summary>
    /// 全局服务管理器
    /// 负责统一开启和关闭各个功能模块服务
    /// </summary>
    public class GlobalServiceManager
    {
        public static void initialize()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().initialize();

            //竞技场管理器
            JingJiChangManager.getInstance().initialize();

            // 万魔塔管理
            WanMoTaManager.getInstance().initialize();

            // 翅膀排行管理
            WingPaiHangManager.getInstance().initialize();

            //指令管理器
            CmdRegisterTriggerManager.getInstance().initialize();
        }

        public static void startup()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().startup();

            //竞技场管理器
            JingJiChangManager.getInstance().startup();

            // 万魔塔管理
            WanMoTaManager.getInstance().startup();

            // 翅膀排行管理
            WingPaiHangManager.getInstance().startup();

            //指令管理器
            CmdRegisterTriggerManager.getInstance().startup();
        }

        public static void showdown()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().showdown();

            //竞技场管理器
            JingJiChangManager.getInstance().showdown();

            // 万魔塔管理
            WanMoTaManager.getInstance().showdown();

            // 翅膀排行管理
            WingPaiHangManager.getInstance().showdown();

            //指令管理器
            CmdRegisterTriggerManager.getInstance().showdown();
        }

        public static void destroy()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().destroy();

            // 竞技场管理器
            JingJiChangManager.getInstance().destroy();

            // 万魔塔管理
            WanMoTaManager.getInstance().destroy();

            // 翅膀排行管理
            WingPaiHangManager.getInstance().destroy();

            //指令管理器
            CmdRegisterTriggerManager.getInstance().destroy();
        }
    }
}
