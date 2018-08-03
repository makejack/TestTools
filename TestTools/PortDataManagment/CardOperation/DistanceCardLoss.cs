using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bll;
using Bll.Management;
using Model;
using Model.SerialPortDataModel;

namespace TestTools.PortDataManagment
{
    #region << 版 本 注 释 >>
    /*----------------------------------------------------------------
	* 类 名 称 ：DistanceCardLoss
	* 类 描 述 ：定距卡挂失操作
	* 作    者 ：Administrator
	* 创建时间 ：2018/8/3 8:45:14
	* 更新人员 ：
	* 更新时间 ：2018/8/3 8:45:14
	* 版 本 号 ：v1.0.0.0
	*******************************************************************
	* Copyright @ Administrator 2018. All rights reserved.
	*******************************************************************
	*----------------------------------------------------------------*/
    #endregion
    public class DistanceCardLoss : ISerialPortDataReceived
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
                        by = DistanceLoss.DistanceLose(CardManager.LossLists);
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
                            item.CardReportLoss = 1;
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
            ViewCallFunction.ViewMessage("定距卡挂失超时，请重新操作。", "2");
        }
    }
}
