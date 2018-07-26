using System;
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
    public class ReadIcCard : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.FunctionAddress == PortEnums.DealFunctions.Ic)
            {
                if (param.Command.ICommand == PortEnums.ICommands.Read)
                {
                    IcParameter icInfo = param.IcParam;
                    string msg = $"IC卡号：{icInfo.IcNumber} 车牌号码：{icInfo.LicensePlate} 时间：{icInfo.Time} ";
                    ViewCallFunction.ViewWirelessMessage(msg);
                }
            }
        }

        public void OverTime()
        {

        }
    }
}
