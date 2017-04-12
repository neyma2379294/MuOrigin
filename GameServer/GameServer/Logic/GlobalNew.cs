using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Interface;
using GameServer.Server;
using System.Windows;
using Server.Tools;
using Server.Data;
using ProtoBuf;

namespace GameServer.Logic
{
    class GlobalNew
    {
        #region 功能开启

        /// <summary>
        /// 配置的功能是否开启
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsGongNengOpened(GameClient client, GongNengIDs id)
        {
            SystemXmlItem xmlItem = null;
            if (GameManager.SystemSystemOpen.SystemXmlItemDict.TryGetValue((int)id, out xmlItem))
            {
                int trigger = xmlItem.GetIntValue("TriggerCondition");
                if (trigger == 7)
                {
                    if (client.ClientData.MainTaskID < xmlItem.GetIntValue("TimeParameters"))
                    {
                        return false;
                    }
                    return true;
                }
                else if (trigger == 1)
                {
                    int[] paramArray = xmlItem.GetIntArrayValue("TimeParameters");
                    if (paramArray.Length == 2)
                    {
                        if (Global.GetUnionLevel(paramArray[0], paramArray[1]) > Global.GetUnionLevel(client))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            return true;
        }

        //刷新当前的功能开启状态，处理有关逻辑
        public static void RefreshGongNeng(GameClient client)
        {
            CaiJiLogic.InitRoleDailyCaiJiData(client, false, false);
            Global.InitRoleDailyTaskData(client, false);
        }

        #endregion 功能开启


        #region 任务相关

        public class NpcCircleTaskData
        {
            public int taskclass = 0;
            public int oldTaskID = 0;
            public List<int> NpcAttachedTaskID = new List<int>();
            public int DoRandomTaskID(GameClient client)
            {
                if (0 == NpcAttachedTaskID.Count)
                    return -1;

                if (taskclass == (int)TaskClasses.DailyTask)
                {
                    return Global.GetDailyCircleTaskIDBaseChangeLifeLev(client);
                }
                else if (taskclass == (int)TaskClasses.TaofaTask)
                {
                    return Global.GetTaofaTaskIDBaseChangeLifeLev(client);
                }
                else
                {
                    int randIndex = Global.GetRandomNumber(0, NpcAttachedTaskID.Count);
                    return NpcAttachedTaskID[randIndex];
                }
            }
        }

        public static bool GetNpcTaskData(GameClient client, int extensionID, NPCData npcData)
        {
            //再查询属于指定NPC的是否有可以接的任务(要判断前置任务和后置任务等条件，以及级别等条件)

            //查询NPC上挂载的任务
            List<int> tasksList = null;
            if (!GameManager.NPCTasksMgr.SourceNPCTasksDict.TryGetValue(extensionID, out tasksList))
                return false;   //npc上没任务
            if (0 == tasksList.Count)
                return false;   //npc上没任务

            Dictionary<int, NpcCircleTaskData> all_circleTask = null;

            //遍历npc上挂载的所有任务，将跑环任务暂存起来稍后处理，非跑环任务直接处理
            for (int i = 0; i < tasksList.Count; i++)
            {
                int taskID = tasksList[i];
                SystemXmlItem systemTask = null;
                if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
                {
                    continue;   //配置错误，没这个任务
                }

                int taskClass = systemTask.GetIntValue("TaskClass");

                if (taskClass >= (int)TaskClasses.CircleTaskStart && taskClass <= (int)TaskClasses.CircleTaskEnd) //如果是跑环任务
                {
                    // 是否能接这种跑环任务
                    if (!Global.CanTaskPaoHuanTask(client, taskClass))
                        continue;
                    //判断是否是能接的新任务
                    if (!Global.CanTakeNewTask(client, taskID, systemTask))
                        continue;
                    
                    //本次请求内是否处理过这种跑环任务
                    NpcCircleTaskData circletask = null;
                    if (null == all_circleTask || !all_circleTask.TryGetValue(taskClass, out circletask))
                    {
                        circletask = new NpcCircleTaskData();
                        circletask.taskclass = taskClass;

                        //之前随机的任务ID
                        circletask.oldTaskID = PaoHuanTasksMgr.FindPaoHuanHistTaskID(client.ClientData.RoleID, taskClass);
                        if (circletask.oldTaskID >= 0)
                        {
                            //验证还是否能继续接
                            //判断是否是能接的新任务
                            if (!Global.CanTakeNewTask(client, circletask.oldTaskID))
                            {
                                circletask.oldTaskID = -1;
                            }
                        }

                        if (null == all_circleTask)
                            all_circleTask = new Dictionary<int, NpcCircleTaskData>();

                        all_circleTask[taskClass] = circletask;
                    }
                    //添加到列表
                    if (null != circletask)
                        circletask.NpcAttachedTaskID.Add(taskID);
                }
                else //非跑环任务，比如主线任务
                {
                    //判断是否是能接的新任务
                    if (!Global.CanTakeNewTask(client, taskID, systemTask))
                        continue;

                    //记录这个任务
                    if (null == npcData.NewTaskIDs)
                    {
                        npcData.NewTaskIDs = new List<int>();
                    }

                    npcData.NewTaskIDs.Add(taskID);

                    if ((int)TaskClasses.SpecialTask == taskClass) //如果是循环任务，要计算已经做过的次数
                    {
                        OldTaskData oldTaskData = Global.FindOldTaskByTaskID(client, tasksList[i]);
                        int doneCount = (null == oldTaskData) ? 0 : oldTaskData.DoCount;

                        if (null == npcData.NewTaskIDsDoneCount)
                        {
                            npcData.NewTaskIDsDoneCount = new List<int>();
                        }
                        npcData.NewTaskIDsDoneCount.Add(doneCount);
                    }
                    else
                    {
                        if (null == npcData.NewTaskIDsDoneCount)
                        {
                            npcData.NewTaskIDsDoneCount = new List<int>();
                        }
                        npcData.NewTaskIDsDoneCount.Add(0);
                    }
                }
            }

            //处理刚才暂存的跑环任务

            if (null == all_circleTask)
                return true;

            foreach (var circletask in all_circleTask)
            {
                bool needRandom = false;
                if (-1 != circletask.Value.oldTaskID)
                {   //之前随机过任务ID

                    if (0 == circletask.Value.NpcAttachedTaskID.Count)
                        continue;   //npc上没这种跑环任务

                    //验证之前随机到的任务是否是存在的
                    if (-1 != circletask.Value.NpcAttachedTaskID.IndexOf(circletask.Value.oldTaskID))
                    {
                        //记录这个任务
                        if (null == npcData.NewTaskIDs)
                        {
                            npcData.NewTaskIDs = new List<int>();
                        }
                        npcData.NewTaskIDs.Add(circletask.Value.oldTaskID);
                        if (null == npcData.NewTaskIDsDoneCount)
                        {
                            npcData.NewTaskIDsDoneCount = new List<int>();
                        }
                        npcData.NewTaskIDsDoneCount.Add(0);
                    }
                    else
                    {
                        needRandom = true;
                    }
                }
                else
                {
                    needRandom = true;
                }

                if (needRandom)
                {
                    int randTaskId = circletask.Value.DoRandomTaskID(client);
                    if (-1 != randTaskId)
                    {
                        //记录这个任务
                        if (null == npcData.NewTaskIDs)
                        {
                            npcData.NewTaskIDs = new List<int>();
                        }
                        npcData.NewTaskIDs.Add(randTaskId);
                        if (null == npcData.NewTaskIDsDoneCount)
                        {
                            npcData.NewTaskIDsDoneCount = new List<int>();
                        }
                        npcData.NewTaskIDsDoneCount.Add(0);
                        PaoHuanTasksMgr.SetPaoHuanHistTaskID(client.ClientData.RoleID, circletask.Value.taskclass, randTaskId);
                    }
                }
            }

            return true;
        }

        public static bool GetNpcFunctionData(GameClient client, int extensionID, NPCData npcData, SystemXmlItem systemNPC)
        {
            if (null == systemNPC)
                return false;

            //查询是否有系统功能
            string operaIDsByString = systemNPC.GetStringValue("Operations");
            operaIDsByString.Trim();
            if (operaIDsByString != "")
            {
                int[] operaIDsByInt = Global.StringArray2IntArray(operaIDsByString.Split(','));
                if (null == npcData.OperationIDs)
                {
                    npcData.OperationIDs = new List<int>();
                }

                for (int i = 0; i < operaIDsByInt.Length; i++)
                {
                    //过滤功能
                    if (Global.FilterNPCOperationByID(client, operaIDsByInt[i], extensionID))
                    {
                        continue;
                    }

                    npcData.OperationIDs.Add(operaIDsByInt[i]);
                }
            }

            //查询是否有NPC功能脚本
            string scriptIDsByString = systemNPC.GetStringValue("Scripts");
            if (null != scriptIDsByString)
            {
                scriptIDsByString = scriptIDsByString.Trim();
            }

            if (!string.IsNullOrEmpty(scriptIDsByString))
            {
                int[] scriptIDsByInt = Global.StringArray2IntArray(scriptIDsByString.Split(','));
                if (null == npcData.ScriptIDs)
                {
                    npcData.ScriptIDs = new List<int>();
                }

                for (int i = 0; i < scriptIDsByInt.Length; i++)
                {
                    int errorCode = 0;

                    //过滤功能脚本
                    if (Global.FilterNPCScriptByID(client, scriptIDsByInt[i], out errorCode))
                    {
                        continue;
                    }

                    npcData.ScriptIDs.Add(scriptIDsByInt[i]);
                }
            }

            return true;
        }

        #endregion 任务相关
    }   //class
}   //namespace
