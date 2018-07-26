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
    public class IcDeviceEncrypt : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            try
            {
                if (param.FunctionAddress == PortEnums.DealFunctions.Ic && param.Command.ICommand == PortEnums.ICommands.EntryptIcDevice)
                {
                    if (param.IcParam != null)
                    {
                        string msg = $"IC 设备加密{(param.IcParam.Result ? "成功" : "失败")}";
                        ViewCallFunction.ViewEncryptMessage(msg);
                        ViewCallFunction.ViewEncryptOver();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewEncryptOver();
        }
    }
}
