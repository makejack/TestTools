using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Dal;

namespace Bll
{
    public class DataBaseManager
    {
        public static void LoadFile()
        {
            string databasePath = Environment.CurrentDirectory + "\\Data.db";

            string connectionStr = ConfigurationManager.ConnectionStrings["SqliteConnectionStr"].ConnectionString;
            DapperHelper.ConnectionStr = string.Format(connectionStr, databasePath);
            if (!File.Exists(databasePath))
            {
                string createDatabaseStr = SqliteDataBaseCreateStr();
                DapperHelper.CreateSqliteDataBase(databasePath);
                DapperHelper.ExecuteSqlStatement(createDatabaseStr);
            }
        }

        private static string SqliteDataBaseCreateStr()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" Create Table UserInfo ( ID integer primary key AUTOINCREMENT ,UserName text , UserNumber int ,Description text, RecordTime datetime );");

            sb.Append(@" Create Table NumberLimit ( ID integer primary key autoincrement, LimitNumber int ) ;");
            sb.Append(@" Insert Into NumberLimit (LimitNumber) values(9887); ");

            sb.Append(
                @" Create Table CardInfo(Cid integer primary key autoincrement,CardNumber NvarChar(10) ,CardType Int ,CardTime DateTime, CardDistance Int ,CardLock Int, CardReportLoss Int,Synchronous Int, CardPartition Int, ParkingRestrictions Int ,InOutState Int, Electricity Int, CardCount Int ,ViceCardCount Int); ");

            sb.Append(@" Create Table BundledInfo(Bid integer primary key autoincrement,Cid integer, HostCardNumber NvarChar(10),Vid integer,ViceCardNumber NvarChar(10) );");

            sb.Append(
                @" Create Table DeviceInfo(Did integer primary key autoincrement,HostNumber int ,IOMouth int, BrakeNumber int ,OpenModel int,Partition int,SAPBF int,Detection int,CardReadDistance int,ReadCardDelay int,CameraDetection int,WirelessNumber int,FrequencyOffset int ,Language int , FuzzyQuery int ); ");

            sb.Append(@" Create Table ModuleNumber (Mid integer Primary key AUTOINCREMENT, Number Int) ; ");
            return sb.ToString();
        }

        public static void UpdateDataBaseStr()
        {
            //string query = " select sql from Sqlite_Master where name = 'BundledInfo' ";
            //StringBuilder sb = new StringBuilder();
            //string strRet = DapperHelper.QuerySql(query);
            //if (!strRet.Contains("ViceTime"))
            //{
            //    sb.Append(" Alter table BundledInfo add Column ViceTime DateTime default(now) ; ");
            //}
            //if (!strRet.Contains("VicePartition"))
            //{
            //    sb.Append(" Alter table BundledInfo add Column VicePartition int default(0) ; ");
            //}
            //DapperHelper.ExecuteSqlStatement(sb.ToString());
        }
    }
}
