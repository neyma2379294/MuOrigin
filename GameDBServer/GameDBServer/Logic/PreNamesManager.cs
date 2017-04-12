using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Server.Protocol;
using Server.Tools;
//using System.Windows.Forms;
using GameDBServer.DB;
using Server.Data;
using ProtoBuf;
using GameDBServer.Logic;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 预先分配的名字项
    /// </summary>
    public class PreNameItem
    {
        /// <summary>
        /// 可以使用的名字
        /// </summary>
        public string Name = "";

        /// <summary>
        /// 名字的性别
        /// </summary>
        public int Sex = 0;

        /// <summary>
        /// 名字是否已经被使用
        /// </summary>
        public int Used = 0;
    };

    /// <summary>
    /// 预先的名字分配初始化
    /// </summary>
    public class PreNamesManager
    {
        /// <summary>
        /// 线程间的保护对象
        /// </summary>
        private static object _Mutex = new object();

        /// <summary>
        /// 随机数发生器
        /// </summary>
        private static Random rand = new Random();

        /// <summary>
        /// 名字项字典
        /// </summary>
        private static Dictionary<string, PreNameItem> _PreNamesDict = new Dictionary<string, PreNameItem>(200000);

        /// <summary>
        /// 男性名字的列表
        /// </summary>
        private static List<PreNameItem> _MalePreNamesList = new List<PreNameItem>(100000);

        /// <summary>
        /// 女性名字的列表
        /// </summary>
        private static List<PreNameItem> _FemalePreNamesList = new List<PreNameItem>(100000);

        /// <summary>
        /// 已经使用过的名字的队列
        /// </summary>
        private static Queue<PreNameItem> _UsedPreNamesQueue = new Queue<PreNameItem>(5000);

        /// <summary>
        /// 添加一个项到列表中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="used"></param>
        public static void AddPreNameItem(string name, int sex, int used)
        {
            PreNameItem preNameItem = new PreNameItem()
            {
                Name = name,
                Sex = sex,
                Used = used,
            };

            lock (_Mutex)
            {
                _PreNamesDict[name] = preNameItem;
                if (0 == sex) //男性
                {
                    _MalePreNamesList.Add(preNameItem);
                }
                else //女性
                {
                    _FemalePreNamesList.Add(preNameItem);
                }
            }
        }

        /// <summary>
        /// 根据性别获取一个随机的名字
        /// </summary>
        /// <returns></returns>
        public static string GetRandomName(int Sex)
        {
            string preName = "";
            List<PreNameItem> preNamesList = null;
            lock (_Mutex)
            {
                if (0 == Sex) //男性
                {
                    preNamesList = _MalePreNamesList;
                }
                else //女性
                {
                    preNamesList = _FemalePreNamesList;
                }

                if (preNamesList.Count > 0)
                {
                    int count = 10;
                    while (count-- >= 0) //只有十次机会
                    {
                        int randIndex = rand.Next(0, preNamesList.Count);
                        if (preNamesList[randIndex].Used <= 0)
                        {
                            preName = preNamesList[randIndex].Name;
                            break;
                        }
                    }
                }
            }

            return preName;
        }

        /// <summary>
        /// 设置名字已经被使用过了
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool SetUsedPreName(string name)
        {
            lock (_Mutex)
            {
                PreNameItem preNameItem = null;
                if (_PreNamesDict.TryGetValue(name, out preNameItem))
                {
                    if (preNameItem.Used <= 0)
                    {
                        preNameItem.Used = 1;
                        _UsedPreNamesQueue.Enqueue(preNameItem);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 清除使用过的名字(每次只清除50个, 防止占用太长的CPU)
        /// </summary>
        public static void ClearUsedPreNames()
        {
            lock (_Mutex)
            {
                int count = 50;
                PreNameItem preNameItem = null;
                while (_UsedPreNamesQueue.Count > 0 && count-- >= 0)
                {
                    preNameItem = _UsedPreNamesQueue.Dequeue();
                    if (null != preNameItem)
                    {
                        _PreNamesDict.Remove(preNameItem.Name);
                        if (0 == preNameItem.Sex) //男性
                        {
                            _MalePreNamesList.Remove(preNameItem);
                        }
                        else //女性
                        {
                            _FemalePreNamesList.Remove(preNameItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将文本文件的内容加载为列表
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static List<string> LoadListFromFileByName(string fileName)
        {
            List<string> strList = new List<string>();

            try
            {
                //判断新的姓文件是否存在
                if (!File.Exists(fileName))
                {
                    return strList;
                }

                StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("gb2312"));
                if (null == sr)
                {
                    return strList;
                }

                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    strList.Add(line);
                }

                sr.Close();
                sr = null;

                return strList;
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "", false);
                    //throw ex;
                //});
            }

            return strList;
        }

        /// <summary>
        /// 从文件加载名字
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void LoadFromFiles(DBManager dbMgr)
        {
            try
            {
                //判断新的姓文件是否存在
                if (!File.Exists("./名字库/姓.txt"))
                {
                    return;
                }

                //判断新的男性文件是否存在
                if (!File.Exists("./名字库/男.txt"))
                {
                    return;
                }

                //判断新的女性文件是否存在
                if (!File.Exists("./名字库/女.txt"))
                {
                    return;
                }

                //将文本文件的内容加载为列表
                List<string> xingList = LoadListFromFileByName("./名字库/姓.txt");
                List<string> nanList = LoadListFromFileByName("./名字库/男.txt");
                List<string> nvList = LoadListFromFileByName("./名字库/女.txt");

                string preName = "";
                for (int xingIndex = 0; xingIndex < xingList.Count; xingIndex++)
                {
                    for (int nanIndex = 0; nanIndex < nanList.Count; nanIndex++)
                    {
                        preName = xingList[xingIndex] + nanList[nanIndex];

                        //插入一个新的预先分配的名字
                        if (DBWriter.InsertNewPreName(dbMgr, preName, 0) >= 0) //成功
                        {
                            //添加一个项到列表中
                            AddPreNameItem(preName, 0, 0);
                        }
                    }

                    for (int nvIndex = 0; nvIndex < nvList.Count; nvIndex++)
                    {
                        preName = xingList[xingIndex] + nvList[nvIndex];

                        //插入一个新的预先分配的名字
                        if (DBWriter.InsertNewPreName(dbMgr, preName, 1) >= 0) //成功
                        {
                            //添加一个项到列表中
                            AddPreNameItem(preName, 1, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "", false);
                    //throw ex;
                //});
            }
        }

        /// <summary>
        /// 从数据库中加载预先分配的名字
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="fileName"></param>
        public static void LoadPremNamesFromDB(DBManager dbMgr)
        {
            //查询加载预先分配的名字
            DBQuery.QueryPreNames(dbMgr, _PreNamesDict, _MalePreNamesList, _FemalePreNamesList);
        }
    }
}
