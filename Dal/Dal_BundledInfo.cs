using System;
using System.Collections.Generic;
using System.Text;
using Model;
using System.Linq;
using System.Data.SQLite;
using System.Data;
using Dapper;

namespace Dal
{
    public class Dal_BundledInfo
    {
        public static void Insert(params BundledInfo[] infos)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " Insert Into BundledInfo(Cid,HostCardNumber,Vid,ViceCardNumber) values(@Cid,@HostCardNumber,@Vid,@ViceCardNumber) ";
                conn.Execute(query, infos);
            }
        }

        public static int Delete(int id)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " Delete From BundledInfo where Bid = Id ";
                return conn.Execute(query, new { Id = id });
            }
        }

        public static int Delete(string hostnumber, string vicenumber)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = "Delete From BundledInfo where HostCardNumber = @HostNumber and ViceCardNumber = @ViceNumber";
                return conn.Execute(query, new { HostNumber = hostnumber, ViceNumber = vicenumber });
            }
        }

    }
}
