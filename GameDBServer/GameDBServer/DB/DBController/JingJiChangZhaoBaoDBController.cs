using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;

namespace GameDBServer.DB.DBController
{
    public class JingJiChangZhaoBaoDBController : DBController<JingJiChallengeInfoData>
    {
        private static JingJiChangZhaoBaoDBController instance = new JingJiChangZhaoBaoDBController();

        private JingJiChangZhaoBaoDBController() : base() { }

        public static JingJiChangZhaoBaoDBController getInstnace()
        {
            return instance;
        }

        public List<JingJiChallengeInfoData> getChallengeInfoListByRoleId(int roleId)
        {
            string sql = string.Format("select * from t_jingjichang_zhanbao where roleId={0} order by pkid desc limit 50;", roleId);

            return this.queryForList(sql);
        }

        public bool insertZhanBao(JingJiChallengeInfoData zhanbaoData)
        {
            string sql = string.Format("insert into t_jingjichang_zhanbao (roleId, zhanbaoType, challengeName,value,createTime) values ({0},{1},'{2}',{3},'{4}');",zhanbaoData.roleId,zhanbaoData.zhanbaoType,zhanbaoData.challengeName,zhanbaoData.value,zhanbaoData.createTime);

            return this.insert(sql) > 0;
        }

        public bool deleteByRoleId(int roleId, string createTime)
        {
            string sql = string.Format("delete from t_jingjichang_zhanbao where roleId = {0} and createTime < '{1}';", roleId, createTime);

            return this.delete(sql) > 0;
        }
    }
}
