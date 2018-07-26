using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;
using Bll;
using Bll.Management;

namespace TestTools.PortDataManagment
{
    public class DistanceDeviceEncrypt : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            if (param.DistanceDeviceParam.Command == PortEnums.DistanceCommands.InitializeDevice)
            {
                string msg = null;
                if (param.DistanceDeviceParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                {
                    msg = "定距发行器加密成功";
                }
                else
                {
                    msg = "定距发行器加密失败";
                }
                ViewCallFunction.ViewEncryptMessage(msg);
                ViewCallFunction.ViewEncryptOver();
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewEncryptOver();
        }
    }
}
