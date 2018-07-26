using System;
using System.Collections.Generic;
using System.Text;
using Model;
using Dapper;
using System.Data.SQLite;
using System.Linq;

namespace Dal
{
    public class Dal_Limit
    {
        public static List<NumberLimit> GetInfos()
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " select * from NumberLimit ";
                return conn.Query<NumberLimit>(query, null).ToList();
            }
        }

        public static int Add(NumberLimit info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Insert Into NumberLimit(
LimitNumber
) values(
@LimitNumber
) ; select last_insert_rowid() ;";
                return conn.Query<int>(query, info).SingleOrDefault();
            }
        }

        public static int Update(NumberLimit info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Update NumberLimit set 
LimitNumber = @LimitNumber
where Id = @Id ";
                return conn.Execute(query, info);
            }
        }

        public static int Del(int id)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " Delete From NumberLimit where Id = @Id ";
                return conn.Execute(query, new { Id = id });

            }
        }
    }
}
