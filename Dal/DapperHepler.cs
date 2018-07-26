using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using Dapper;
using Model;

namespace Dal
{
    public class DapperHelper
    {
        public static string ConnectionStr { get; set; }

        public static IDbConnection GetInstance()
        {
            return GetInstance(ConnectionStr);
        }

        public static IDbConnection GetInstance(string nameOrConnectionStr)
        {
            return new SQLiteConnection(nameOrConnectionStr);
        }

        public static void CreateSqliteDataBase(string databasePath)
        {
            SQLiteConnection.CreateFile(databasePath);
        }

        public static void ExecuteSqlStatement(string cmdtext)
        {
            using (SQLiteConnection conn = (SQLiteConnection)GetInstance())
            {
                conn.Execute(cmdtext, null);
            }
        }

        public static string QuerySql(string cmdtext)
        {
            using (SQLiteConnection conn = (SQLiteConnection)GetInstance())
            {
                return conn.Query<string>(cmdtext, null).SingleOrDefault();
            }
        }
    }
}

