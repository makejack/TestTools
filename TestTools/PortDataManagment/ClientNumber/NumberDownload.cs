﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools.PortDataManagment;
using Bll;
using Model;
using Model.SerialPortDataModel;

namespace TestTools.PortDataManagment
{
    public class NumberDownload : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            if (param.Command.HCommand == PortEnums.HCommands.NumberModify)
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
