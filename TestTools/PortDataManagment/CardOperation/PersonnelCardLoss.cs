using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Bll.Management;
using Model;
using Model.SerialPortDataModel;

namespace TestTools.PortDataManagment
{
    public class PersonnelCardLoss : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            try
            {
                PortDataParameter param = DataParsing.Parsing(by);
                if (param.DistanceCardParam == null) return;
                OverTimeManager.Stop();
                if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.ReadACard)
                {
                    if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                    {
                        int state = CardManager.LossLists[0].CardReportLoss;
                        by = DistanceLoss.PersonnelLoseOrRecovery(CardManager.LossLists, state == 1 ? 1 : 2);
                        bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        if (!ret)
                        {
                            ViewCallFunction.ViewCloseLoading();
                        }
                        else
                        {
                            OverTimeManager.Start();
                        }
                    }
                    else
                    {
                        ViewCallFunction.ViewCloseLoading();
                        ViewCallFunction.ViewMessage("未能获取到挂失卡，请重新操作。", "2");
                    }
                }
                else if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.WriteACard)
                {
                    if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                    {
                        foreach (CardInfo item in CardManager.LossLists)
                        {
                            item.CardReportLoss = item.CardReportLoss == 1 ? 0 : 1;
                        }
                        CardManager.Update(CardManager.LossLists.ToArray());
                        CardManager.LossLists = null;
                        ViewCallFunction.ViewLoseOver();
                    }
                    else
                    {
                        ViewCallFunction.ViewCloseLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewCloseLoading();
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewCloseLoading();
            ViewCallFunction.ViewMessage("操作超时，请重新操作。", "2");
        }
    }
}
