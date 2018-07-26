using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Bll.Management;
using Model;
using Model.SerialPortDataModel;

namespace TestTools.PortDataManagment
{
    public class ViceCardRemoveLock : ISerialPortDataReceived
    {

        public CardInfo RemoveLockInfo { get; set; }

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.DistanceCardParam == null) return;
            if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.WriteACard)
            {
                OverTimeManager.Stop();
                if (RemoveLockInfo == null) return;
                try
                {
                    if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                    {
                        CardManager.Update(RemoveLockInfo);
                    }
                    else
                    {
                        RemoveLockInfo.CardLock = 1;
                    }
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                }
                finally
                {
                    RemoveLockInfo = null;
                }
            }
        }

        public void OverTime()
        {
            RemoveLockInfo.CardLock = 1;//解锁失败恢复参数
            RemoveLockInfo = null;
        }
    }
}
