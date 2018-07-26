using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Model;
using Model.SerialPortDataModel;

namespace TestTools.PortDataManagment
{
    public class WirelessQuery : ISerialPortDataReceived
    {
        public bool SettingOver = false;

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.FunctionAddress == PortEnums.DealFunctions.ModularAndVoice)
            {
                if (param.Command.MCommand == PortEnums.MCommands.SetModule)
                {
                    SettingOver = param.ModuleParam.Result;
                }
                if (by.Length > 12)
                {
                    string str = Encoding.ASCII.GetString(by);
                    ViewCallFunction.ViewWirelessMessage(str);
                }
            }
        }

        public void OverTime()
        {

        }
    }
}
