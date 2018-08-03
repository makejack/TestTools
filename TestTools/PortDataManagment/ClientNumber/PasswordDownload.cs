using Bll;
using Model.SerialPortDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTools.PortDataManagment
{
    public class PasswordDownload : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            if (param.Command.HCommand == PortEnums.HCommands.Password)
            {
                if (param.HostParam != null)
                {
                    ViewCallFunction.ViewDownloadOver(param.HostParam.Result);
                }
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewCloseLoading();
        }
    }
}
