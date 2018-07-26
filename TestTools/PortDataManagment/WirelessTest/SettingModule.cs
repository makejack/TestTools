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
    public class SettingModule : ISerialPortDataReceived
    {
        public bool SettingOver = false;

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.FunctionAddress == PortEnums.DealFunctions.ModularAndVoice)
            {
                //if (param.Command.MCommand == PortEnums.MCommands.SetModule || param.Command.MCommand == PortEnums.MCommands.TestCommunication)
                //{
                    SettingOver = param.ModuleParam.Result;
                //}
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewWirelessOver();
            ViewCallFunction.ViewWirelessMessage("无线设置超时，结束操作。");
        }

    }
}
