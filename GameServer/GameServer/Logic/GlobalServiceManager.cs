using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LiXianGuaJi;
using GameServer.Server.CmdProcesser;
using GameServer.Server;
using GameServer.Logic.Copy;
using GameServer.Logic.BossAI;

namespace GameServer.Logic
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
    /// 全局功能模块服务管理器
    /// 负责统一初始化，开启，关闭，销毁所有的功能模块管理器
    /// </summary>
    public class GlobalServiceManager
    {
        public static void initialize()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().initialize();

            //竞技场管理器
            JingJiChangManager.getInstance().initialize();

            //离线摆摊
            LiXianBaiTanManager.getInstance().initialize();

            //离线挂机
            LiXianGuaJiManager.getInstance().initialize();

            //副本活动组队管理器
            CopyTeamManager.getInstance().initialize();

            //指令注册管理器
            CmdRegisterTriggerManager.getInstance().initialize();

            //发送指令管理
            SendCmdManager.getInstance().initialize();

            //Boss AI管理器
            BossAIManager.getInstance().initialize();

            //洗炼管理器
            WashPropsManager.initialize();

            //MU交易所
            SaleManager.getInstance().initialize();

            //炼制系统
            LianZhiManager.GetInstance().initialize();

            // 成就升级
            ChengJiuManager.GetInstance().initialize();

            //恶魔来袭
            EMoLaiXiCopySceneManager.LoadEMoLaiXiCopySceneInfo();
        }
        
        public static void startup()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().startup();

            //竞技场管理器
            JingJiChangManager.getInstance().startup();

            //离线摆摊
            LiXianBaiTanManager.getInstance().startup();

            //离线挂机
            LiXianGuaJiManager.getInstance().startup();

            //副本活动组队管理器
            CopyTeamManager.getInstance().startup();

            //指令注册管理器
            CmdRegisterTriggerManager.getInstance().startup();

            //发送指令管理
            SendCmdManager.getInstance().startup();

            //Boss AI管理器
            BossAIManager.getInstance().startup();

            //MU交易所
            SaleManager.getInstance().startup();

            //炼制系统
            LianZhiManager.GetInstance().startup();

            // 成就升级
            ChengJiuManager.GetInstance().startup();
        }
        
        public static void showdown()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().showdown();

            //竞技场管理器
            JingJiChangManager.getInstance().showdown();

            //离线摆摊
            LiXianBaiTanManager.getInstance().showdown();

            //离线挂机
            LiXianGuaJiManager.getInstance().showdown();

            //副本活动组队管理器
            CopyTeamManager.getInstance().showdown();

            //指令注册管理器
            CmdRegisterTriggerManager.getInstance().showdown();

            //发送指令管理
            SendCmdManager.getInstance().showdown();

            //Boss AI管理器
            BossAIManager.getInstance().showdown();

            //MU交易所
            SaleManager.getInstance().showdown();

            //炼制系统
            LianZhiManager.GetInstance().showdown();

            // 成就升级
            ChengJiuManager.GetInstance().showdown();
        }
        
        public static void destroy()
        {
            //战盟事件管理器
            ZhanMengShiJianManager.getInstance().destroy();

            //竞技场管理器
            JingJiChangManager.getInstance().destroy();

            //离线摆摊
            LiXianBaiTanManager.getInstance().destroy();

            //离线挂机
            LiXianGuaJiManager.getInstance().destroy();

            //副本活动组队管理器
            CopyTeamManager.getInstance().destroy();

            //指令注册管理器
            CmdRegisterTriggerManager.getInstance().destroy();

            //发送指令管理
            SendCmdManager.getInstance().destroy();

            //Boss AI管理器
            BossAIManager.getInstance().destroy();

            //MU交易所
            SaleManager.getInstance().destroy();

            //炼制系统
            LianZhiManager.GetInstance().destroy();

            // 成就升级
            ChengJiuManager.GetInstance().destroy();
        }

    }
}
