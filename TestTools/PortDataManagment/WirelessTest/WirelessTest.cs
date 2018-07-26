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
    public class WirelessTest : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.FunctionAddress == PortEnums.DealFunctions.ModularAndVoice)
            {
                if (param.Command.MCommand == PortEnums.MCommands.TestCommunication)
                {
                    if(param.ModuleParam.Result)
                    {
                        ViewCallFunction.ViewWirelessMessage("测试数据发送成功。");
                    }
                    else
                    {
                        ViewCallFunction.ViewWirelessMessage("测试数据发送失败，请检测无线ID（编号）或频率是否正确。");
                    }
                }
            }
        }

        public void OverTime()
        {
             
        }
    }
}
