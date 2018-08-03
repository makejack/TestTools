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
    public class CardIssuePersonnel : ISerialPortDataReceived
    {
        public CardInfo IssueInfo { get; set; }

        public void DataReceived(byte[] by)
        {
            PortDataParameter param = DataParsing.Parsing(by);
            if (param.DistanceCardParam == null) return;
            if (param.DistanceCardParam.Command == PortEnums.DistanceCommands.WriteACard)
            {
                OverTimeManager.Stop();
                if (IssueInfo == null) return;
                try
                {
                    if (param.DistanceCardParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                    {
                        if (IssueInfo.Id > 0)
                        {
                            CardManager.Update(IssueInfo);
                        }
                        else
                        {
                            IssueInfo.Id = CardManager.Insert(IssueInfo);
                        }
                    }
                    else
                    {
                        IssueInfo = null;
                    }
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    ViewCallFunction.ViewAlert(ex.Message);
                }
                finally
                {
                    ViewCallFunction.ViewIssuePersonnelOver(IssueInfo);
                    IssueInfo = null;
                }
            }
        }

        public void OverTime()
        {
            IssueInfo = null;
            ViewCallFunction.ViewCloseLoading();
        }
    }
}
