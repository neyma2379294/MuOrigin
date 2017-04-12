using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Data;
using Server.Tools;
using Server.TCP;

namespace GameServer.Server
{
    public class TCPSession
    {
        //套接字
        private TMSKSocket _currentSocket = null;

        //指令队列
        private Queue<TCPCmdWrapper> _cmdWrapperQueue = new Queue<TCPCmdWrapper>();

        //对象锁
        private Object _lock = new Object();

        public TCPSession(TMSKSocket socket) 
        {
            this._currentSocket = socket;

            //this._lock = new Object();
            //_cmdWrapperQueue = new Queue<TCPCmdWrapper>();

            //this._SendCmdLock = new Object();
            //_SendCmdWrapperQueue = new Queue<SendCmdWrapper>();
        }

        public void release()
        {
            this._currentSocket = null;

            lock (_cmdWrapperQueue)
            {
                if (null != _cmdWrapperQueue)
                {
                    _cmdWrapperQueue.Clear();
                }
                //_cmdWrapperQueue = null;
            }

            //lock (_SendCmdWrapperQueue)
            //{
            //    if (null != _SendCmdWrapperQueue)
            //    {
            //        _SendCmdWrapperQueue.Clear();
            //    }
            //    _SendCmdWrapperQueue = null;
            //}
        }

        public TMSKSocket CurrentSocket
        {
            get { return _currentSocket; }
        }

        public Object Lock
        {
            get { return _lock; }
        }

        public static int MaxPosCmdNumPer5Seconds = GameManager.GameConfigMgr.GetGameConfigItemInt("maxposcmdnum", 8);
        public static int MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 500);
        public static int MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);


        private int cmdNum = 0;
        private long beginTime = TimeUtil.NOW();

        //public long ParsePositionTicks(TCPCmdWrapper wrapper)
        //{
        //    SpritePositionData cmdData = null;

        //    try
        //    {
        //        cmdData = DataHelper.BytesToObject<SpritePositionData>(wrapper.Data, 0, wrapper.Count);
        //    }
        //    catch (Exception) //解析错误
        //    {
        //        return 0;
        //    }

        //    //解析用户名称和用户密码
        //    //string[] fields = cmdData.Split(':');
        //    //if (fields.Length != 5)
        //    if (null == cmdData)
        //    {
        //        return 0;
        //    }

        //    long currentPosTicks = cmdData.currentPosTicks;
        //    return currentPosTicks;
        //}

        public void addTCPCmdWrapper(TCPCmdWrapper wrapper, out int posCmdNum)
        {
            posCmdNum = 0;

            lock (_cmdWrapperQueue)
            {
                _cmdWrapperQueue.Enqueue(wrapper);
            }

            if ((int)TCPGameServerCmds.CMD_SPR_CHECK/*TCPGameServerCmds.CMD_SPR_POSITION*/ == wrapper.NID)
            {
//                 long ticks = ParsePositionTicks(wrapper);
//                 if (ticks <= 0)
//                 {
                    cmdNum++;

                    if ((TimeUtil.NOW() - beginTime) >= TimeUtil.SECOND * 10)
                    {
                        if (cmdNum >= MaxPosCmdNumPer5Seconds)
                        {
                            posCmdNum = cmdNum;
                            cmdNum = 0;
                            beginTime = TimeUtil.NOW();
                        }
                        else
                        {
                            cmdNum = 0;
                            beginTime = TimeUtil.NOW();
                        }
                    }
//                }
            }
        }

        public void CheckCmdNum(int cmdID, out int posCmdNum)
        {
            posCmdNum = 0;
            if ((int)TCPGameServerCmds.CMD_SPR_CHECK/*TCPGameServerCmds.CMD_SPR_POSITION*/ == cmdID)
            {
                cmdNum++;

                if ((TimeUtil.NOW() - beginTime) >= TimeUtil.SECOND * 10)
                {
                    if (cmdNum >= MaxPosCmdNumPer5Seconds)
                    {
                        posCmdNum = cmdNum;
                        cmdNum = 0;
                        beginTime = TimeUtil.NOW();
                    }
                    else
                    {
                        cmdNum = 0;
                        beginTime = TimeUtil.NOW();
                    }
                }
            }
        }

        public TCPCmdWrapper getNextTCPCmdWrapper()
        {
            lock (_cmdWrapperQueue)
            {
                if (_cmdWrapperQueue.Count > 0)
                {
                    return _cmdWrapperQueue.Dequeue();
                }
            }

            return null;
        }

        #region 发送指令缓存队列

        ////指令队列
        //private Queue<SendCmdWrapper> _SendCmdWrapperQueue = null;

        ////对象锁
        //private Object _SendCmdLock = null;

        //public Object SendCmdLock
        //{
        //    get { return _SendCmdLock; }
        //}

        //public void addSendCmdWrapper(SendCmdWrapper wrapper)
        //{
        //    lock (_SendCmdWrapperQueue)
        //    {
        //        _SendCmdWrapperQueue.Enqueue(wrapper);
        //    }           
        //}

        //public SendCmdWrapper getNextSendCmdWrapper()
        //{
        //    lock (_SendCmdWrapperQueue)
        //    {
        //        return _SendCmdWrapperQueue.Dequeue();
        //    }           
        //}

        #endregion 发送指令缓存队列
    }
}
