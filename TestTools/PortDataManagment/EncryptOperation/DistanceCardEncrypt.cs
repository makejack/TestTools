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
    public class DistanceCardEncrypt : ISerialPortDataReceived
    {
        public string NewPwd { get; set; }

        public string ClientNumber { get; set; }

        private bool EncryptNewDevice = false;

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            OverTimeManager.Stop();
            try
            {
                if (param.Command.DCommand == PortEnums.DCommands.Default)
                {
                    if (param.DistanceDeviceParam != null && param.DistanceDeviceParam.Command == PortEnums.DistanceCommands.InitializeDevice)
                    {
                        if (param.DistanceDeviceParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                        {
                            if (!EncryptNewDevice)
                            {
                                byte[] bys = PortAgreement.DistanceCardEncryption(NewPwd);
                                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
                                EncryptNewDevice = true;
                                OverTimeManager.Start();
                                return;
                            }
                            else
                            {
                                ViewCallFunction.ViewEncryptMessage("定距卡加密结束。");
                                ViewCallFunction.ViewEncryptOver();
                            }
                        }
                        else
                        {
                            ViewCallFunction.ViewEncryptMessage("定距卡加密失败，请重新操作。");
                            ViewCallFunction.ViewEncryptOver();
                        }
                        EncryptNewDevice = false;
                    }
                    else if (param.DistanceCardParam != null && param.DistanceCardParam.Command == PortEnums.DistanceCommands.ModifyAllCardPwd)
                    {
                        if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                        {
                            DistanceCardParameter distanceParam = param.DistanceCardParam;
                            string msg = $"定距卡：{distanceParam.CardNumber} 密码（口令)加密{(distanceParam.CardTypeParameter.CardType != PortEnums.CardTypes.PwdError ? "成功" : "失败")}";
                            ViewCallFunction.ViewEncryptMessage(msg);
                        }
                        else
                        {
                            DeviceNewPwd();
                        }
                        OverTimeManager.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewEncryptOver();
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private void DeviceNewPwd()
        {
            byte[] bys = PortAgreement.DistanceDeviceEncryption(ClientNumber, NewPwd);
            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
        }

        public void OverTime()
        {
            DeviceNewPwd();
        }
    }
}
