using System;
using System.Collections.Generic;
using System.Text;
using Model;
using System.Data.SQLite;
using System.Linq;
using Dapper;

namespace Dal
{
    public class Dal_ConfirmInfo
    {
        public static List<DeviceInfo> GetInfos(string strPage)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = $" Select * from DeviceInfo where 1=1 {strPage} ";
                return conn.Query<DeviceInfo>(query, null).ToList();
            }
        }

        public static int Add(DeviceInfo info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Insert into DeviceInfo(
HostNumber,
IOMouth,
BrakeNumber,
OpenModel,
Partition,
SAPBF,
Detection,
CardReadDistance,
ReadCardDelay,
CameraDetection,
WirelessNumber,
FrequencyOffset,
Language,
FuzzyQuery
) values(
@HostNumber,
@IOMouth,
@BrakeNumber,
@OpenModel,
@Partition,
@SAPBF,
@Detection,
@CardReadDistance,
@ReadCardDelay,
@CameraDetection,
@WirelessNumber,
@FrequencyOffset,
@Language,
@FuzzyQuery
) ;select last_insert_rowid() ;  ";
                return conn.Query<int>(query, info).SingleOrDefault();
            }
        }

        public static int Update(DeviceInfo info)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = @" Update DeviceInfo set 
HostNumber = @HostNumber,
IOMouth = @IOMouth,
BrakeNumber = @BrakeNumber,
OpenModel = @OpenModel,
Partition = @Partition,
SAPBF = @SAPBF,
Detection = @Detection,
CardReadDistance = @CardReadDistance,
ReadCardDelay = @ReadCardDelay,
CameraDetection = @CameraDetection,
WirelessNumber = @WirelessNumber,
FrequencyOffset = @FrequencyOffset,
Language = @Language,
FuzzyQuery = @FuzzyQuery 
where Did = @Did ";
                return conn.Execute(query, info);
            }
        }

        public static int Del(int id)
        {
            using (SQLiteConnection conn = (SQLiteConnection)DapperHelper.GetInstance())
            {
                string query = " Delete from DeviceInfo where Did = @Id ";
                return conn.Execute(query, new { Id = id });
            }
        }
        
    }
}
