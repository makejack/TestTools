using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;
using Bll;
using Bll.Management;
using System.Threading;

namespace TestTools.PortDataManagment
{
    public class IcCardEncrypt : ISerialPortDataReceived
    {
        public string Pwd { get; set; }

        public bool StopEncrypt = false;

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            try
            {
                if (param.FunctionAddress == PortEnums.DealFunctions.Ic)
                {
                    if (param.Command.ICommand == PortEnums.ICommands.EntryptIcDevice)
                    {
                        if (param.IcParam.Result)
                        {
                            EncryptNewPwd();
                        }
                        else
                        {
                            ViewCallFunction.ViewEncryptMessage("加密失败，请重新操作。");
                            ViewCallFunction.ViewEncryptOver();
                        }
                    }
                    else if (param.Command.ICommand == PortEnums.ICommands.EntryptIcCard)
                    {
                        if (param.IcParam != null)
                        {
                            string msg = $"IC 卡加密{(param.IcParam.Result ? "成功" : "失败")}";
                            ViewCallFunction.ViewEncryptMessage(msg);

                            Task.Factory.StartNew(() =>
                            {
                                for (int i = 0; i < 150; i++)
                                {
                                    Thread.Sleep(10);
                                    if (StopEncrypt)
                                    {
                                        StopEncrypt = false;
                                        OverEncrypt();
                                        return;
                                    }
                                }
                                EncryptNewPwd();
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private void EncryptNewPwd()
        {
            byte[] bys = PortAgreement.IcCardEncryption(Pwd);
            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
            OverTimeManager.Start();
        }

        private void OverEncrypt()
        {
            try
            {
                byte[] by = PortAgreement.IcDeviceEncryption(Pwd);
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                ReceivedManager.SetReceivedFunction<IcDeviceEncrypt>();
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        public void OverTime()
        {
            
        }
    }
}
