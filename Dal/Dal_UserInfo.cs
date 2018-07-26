using System;
using System.Collections;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Data.SQLite;
using Model;

namespace Dal
{
    public class Dal_UserInfo
    {
        public static int GetCount(string strContent)
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(strContent))
            {
                strWhere = " and  UserName = @UserName ";
            }
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = $" select * from UserInfo where 1=1 {strWhere} ";
                return conn.Query<int>(query, new
                {
                    UserName = $"%{strContent}%",
                }).Count();
            }
        }

        public static List<UserInfo> GetInfos(string strWhere, object param)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = $" Select * from UserInfo where 1=1 {strWhere} ";
                return conn.Query<UserInfo>(query, param).ToList();
            }
        }

        public static int Add(UserInfo info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Insert into UserInfo(
UserName,
UserNumber,
Description,
RecordTime
) values(
@UserName,
@UserNumber,
@Description,
@RecordTime
) ; select last_insert_rowid() ;";
                return conn.Query<int>(query, info).SingleOrDefault();
            }
        }

        public static int Update(UserInfo info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Update UserInfo set 
UserName = @UserName,
UserNumber = @UserNumber,
Description = @Description
where Id = @Id ";
                return conn.Execute(query, info);
            }
        }

        public static int Del(int id)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " Delete From UserInfo where Id = @Id ";
                return conn.Execute(query, new { Id = id });
            }
        }
    }
}