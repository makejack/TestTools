using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;
using Bll.Management;
using Bll;

namespace TestTools.PortDataManagment
{
    public class CardRefresh : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.DistanceCardParam == null) return;
            if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.ReadAllCard)
            {
                OverTimeManager.Stop();
                if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.End)
                {
                    ViewCallFunction.ViewReadCardOver();
                    return;
                }
                try
                {
                    bool exists = CardManager.CardInfos.Exists(e => e.CardNumber == param.DistanceCardParam.CardNumber);
                    if (exists) return;
                    CardInfo info = CardManager.GetCardInfo(param.DistanceCardParam.CardNumber);
                    if (info == null)
                    {
                        info = new CardInfo();
                        info.CardTime = DateTime.Now;
                    }
                    info.CardNumber = param.DistanceCardParam.CardNumber;
                    info.CardType = (int)param.DistanceCardParam.CardTypeParameter.CardType;
                    info.CardLock = param.DistanceCardParam.CardTypeParameter.CardLock;
                    info.CardDistance = param.DistanceCardParam.CardTypeParameter.Distance;
                    info.Electricity = param.DistanceCardParam.CardTypeParameter.Electricity;
                    if (info.CardType <= 3)
                    {
                        if (param.DistanceCardParam.FunctionByteParam != null)
                        {
                            if (info.Id > 0 && info.CardType < 3)
                            {
                                info.CardType = (int)param.DistanceCardParam.FunctionByteParam.RegistrationType;
                                if (info.CardType == 1)//人卡或组合卡
                                {
                                    info.ParkingRestrictions = param.DistanceCardParam.FunctionByteParam.ParkingRestrictions;
                                }
                            }
                            info.CardReportLoss = param.DistanceCardParam.FunctionByteParam.Loss;
                            info.ViceCardCount = param.DistanceCardParam.FunctionByteParam.ViceCardCount;
                            info.Synchronous = param.DistanceCardParam.FunctionByteParam.Synchronous;
                            info.InOutState = param.DistanceCardParam.FunctionByteParam.InOutState;
                            info.CardCount = param.DistanceCardParam.Count;
                        }
                    }
                    CardManager.CardInfos.Add(info);
                    ViewCallFunction.ViewDisplayReadCardInfo(info);
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                }
                finally
                {
                    OverTimeManager.Start();
                }
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewReadCardOver();
        }
    }
}
