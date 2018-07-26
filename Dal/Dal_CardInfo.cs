using System;
using System.Collections.Generic;
using System.Text;
using Model;
using System.Linq;
using System.Data.SQLite;
using Dapper;

namespace Dal
{
    public class Dal_CardInfo
    {
        public static int GetCardCount(string content)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string searchWhere = " and CardType > -1 ";
                if (!string.IsNullOrEmpty(content))
                {
                    searchWhere = " and CardNumber like @content ";
                }
                string query = $" select * from CardInfo where 1=1 {searchWhere}  ";
                return conn.Query(query, new
                {
                    content = "%" + content + "%"
                }).Count();
            }
        }

        public static List<CardInfo> GetCardInfos(string strWhere, object param)
        {
            using (SQLiteConnection con = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = $@"select Cid as Id,
CardNumber,
CardType,
CardTime,
CardDistance,
CardLock,
CardReportLoss,
ParkingRestrictions,
CardPartition,
Electricity,
Synchronous,
InOutState,
CardCount,
ViceCardCount
from CardInfo
where 1 = 1 {strWhere} ";
                return con.Query<CardInfo>(query, param).ToList();
            }
        }

        public static int Update(params CardInfo[] infos)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" update CardInfo set 
CardNumber = @CardNumber,
CardType = @CardType,
CardTime = @CardTime,
CardDistance = @CardDistance,
CardLock = @CardLock,
CardReportLoss = @CardReportLoss,
ParkingRestrictions = @ParkingRestrictions,
CardPartition = @CardPartition,
CardCount = @CardCount
where Cid = @Id ";
                return conn.Execute(query, infos);
            }
        }

        public static int Insert(CardInfo info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Insert Into CardInfo(
CardNumber,
CardType,
CardTime,
CardDistance,
CardLock,
CardReportLoss,
ParkingRestrictions,
CardPartition,
CardCount
) values(
@CardNumber,
@CardType,
@CardTime,
@CardDistance,
@CardLock,
@CardReportLoss,
@ParkingRestrictions,
@CardPartition,
@CardCount
) ; select last_insert_rowid() ; ";
                return conn.Query<int>(query, info).SingleOrDefault();
            }
        }

        public static int Insert(CardInfo[] infos)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Insert Into CardInfo(
CardNumber,
CardType,
CardTime,
CardDistance,
CardLock,
CardReportLoss,
ParkingRestrictions,
CardPartition,
CardCount
) values(
@CardNumber,
@CardType,
@CardTime,
@CardDistance,
@CardLock,
@CardReportLoss,
@ParkingRestrictions,
@CardPartition,
@CardCount
) ";
                return conn.Execute(query, infos);
            }
        }

        public static int Delete(string cardnumber)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " delete from CardInfo where CardNumber = @CardNumber ; delete from BundledInfo where HostCardNumber = @CardNumber ";
                return conn.Execute(query, new { CardNumber = cardnumber });
            }
        }

        public static int Delete()
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " delete from CardInfo where 1=1 ;  delete from BundledInfo where 1=1 ";
                return conn.Execute(query, null);
            }
        }
    }
}
