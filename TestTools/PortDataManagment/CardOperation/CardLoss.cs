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
    public class CardLoss : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.DistanceCardParam == null) return;
            if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.WriteACard)
            {
                OverTimeManager.Stop();
                if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                {
                    foreach (CardInfo item in CardManager.LossLists)
                    {
                        item.CardReportLoss = 1;
                    }
                    CardManager.Update(CardManager.LossLists.ToArray());
                    CardManager.LossLists = null;
                }
                else
                {

                }

            }
        }

        public void OverTime()
        {

        }
    }
}
