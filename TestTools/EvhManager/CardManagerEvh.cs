using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Bll.Management;
using Model;
using Model.SerialPortDataModel;
using TestTools.Devices;
using TestTools.PortDataManagment;
using System.Threading;
using System.Reflection;

namespace TestTools
{
    public class CardManagerEvh
    {

        public static void InitEvent()
        {
            Main.GetMain.GlobalObject.AddFunction("HostPostDelRowData").Execute += Host_PostDelRowData;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelAllData").Execute += Host_PostDelAllData;
            Main.GetMain.GlobalObject.AddFunction("HostPostCardInfos").Execute += Host_PostCardInfos;
            Main.GetMain.GlobalObject.AddFunction("HostRefreshOperation").Execute += Host_RefreshOperation;
            Main.GetMain.GlobalObject.AddFunction("HostPostCardInfo").Execute += Host_PostCardInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostViceCardInfo").Execute += Host_PostViceCardInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelViceCardInfo").Execute += Host_PostDelViceCardInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelViceCardInfos").Execute += Host_PostDelViceCardInfos;
            Main.GetMain.GlobalObject.AddFunction("HostPostViceCardCount").Execute += Host_PostViceCardCount;
            Main.GetMain.GlobalObject.AddFunction("HostPostViceCardList").Execute += Host_PostViceCardList;
            Main.GetMain.GlobalObject.AddFunction("HostPostHostType").Execute += Host_PostHostType;
            Main.GetMain.GlobalObject.AddFunction("HostPostUpdateViceCardInfo").Execute += Host_PostUpdateViceCardInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostIssue").Execute += Host_PostIssue;
            Main.GetMain.GlobalObject.AddFunction("HostPortBatchParameter").Execute += Host_PortBatchParameter;
            Main.GetMain.GlobalObject.AddFunction("HostPostUpdateBatchParam").Execute += Host_PostUpdateBatchParam;
            Main.GetMain.GlobalObject.AddFunction("HostPostBatch").Execute += Host_PostBatch;
            Main.GetMain.GlobalObject.AddFunction("HostPostPersonnelIssue").Execute += Host_PostPersonnelIssue;
            Main.GetMain.GlobalObject.AddFunction("HostPostPersonnelBatch").Execute += Host_PostPersonnelBatch;
        }

        private static void Host_PostDelAllData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                bool ret = CardManager.Delete();
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelRowData(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                var index = e.Arguments[0].IntValue;
                bool ret = CardManager.Delete(index);
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostViceCardCount(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                int count = 0;
                if (CardManager.CardInfos[index].ViceCardInfos != null)
                {
                    count = CardManager.CardInfos[index].ViceCardInfos.Count;
                }
                e.SetReturnValue(count);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostViceCardInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                int viceIndex = e.Arguments[1].IntValue;
                CardInfo viceInfo = CardManager.CardInfos[index].ViceCardInfos[viceIndex];
                string json = Utility.JsonSerializerBySingleData(viceInfo);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostCardInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                CardInfo info = CardManager.GetCardInfo(index);
                string json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_RefreshOperation(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            //刷新操作
            CardManager.CardInfos.Clear();
            byte[] bys;
            try
            {
                bys = PortAgreement.ReadAllCard();
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<CardRefresh>();
                }
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            finally
            {
                bys = null;
            }
        }

        private static void Host_PostCardInfos(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            //获取数据
            string strWhere = e.Arguments[0].StringValue;
            int page = e.Arguments[1].IntValue;

            try
            {
                List<CardInfo> infos = CardManager.GetCardInfos(strWhere, page - 1, DefaultParams.PAGE_ROW_COUNT);
                string json = Utility.JsonSerializerByArrayData(infos.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelViceCardInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int hostIndex = e.Arguments[0].IntValue;
                int viceIndex = e.Arguments[1].IntValue;

                CardInfo hostInfo = CardManager.CardInfos[hostIndex];
                CardInfo viceInfo = hostInfo.ViceCardInfos[viceIndex];

                BundledInfoManager.Delete(hostInfo.CardNumber, viceInfo.CardNumber);
                hostInfo.ViceCardInfos.RemoveAt(viceIndex);
                viceInfo = null;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelViceCardInfos(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                CardManager.DelViceCardInfos(index);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostViceCardList(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                List<CardInfo> viceCards = CardManager.GetViceCards(index);
                string json = Utility.JsonSerializerByArrayData(viceCards.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostHostType(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                CardInfo info = CardManager.CardInfos[index];
                e.SetReturnValue(info.CardType);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUpdateViceCardInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                int viceIndex = e.Arguments[1].IntValue;
                string strNumber = e.Arguments[2].StringValue;
                int type = e.Arguments[3].IntValue;
                Chromium.Remote.CfrTime cTime = e.Arguments[4].DateValue;
                DateTime time = new DateTime(cTime.Year, cTime.Month, cTime.DayOfMonth);
                int partition = e.Arguments[5].IntValue;
                CardInfo info = CardManager.CardInfos[index];

                if (viceIndex > -1)
                {
                    CardInfo viceInfo = info.ViceCardInfos[viceIndex];
                    viceInfo.CardNumber = strNumber;
                    viceInfo.CardTime = time;
                    viceInfo.CardPartition = partition;
                    viceInfo.CardCount = DistanceIssue.SetCount(viceInfo.CardCount);

                    CardManager.Update(viceInfo);
                }
                else
                {
                    if (info.ViceCardInfos == null) info.ViceCardInfos = new List<CardInfo>();
                    List<BundledInfo> bundledInfos = new List<BundledInfo>();
                    string[] strNumbers = strNumber.Split(',');
                    if (type == (int)PortEnums.CardTypes.Card2)//车卡
                    {
                        List<CardInfo> viceInfos = CardManager.CardInfos.Where(c => c.Id == 0 && c.CardType == (int)PortEnums.CardTypes.Card4).ToList();
                        if (viceInfos.Count > 0)
                        {
                            foreach (CardInfo item in viceInfos)
                            {
                                foreach (string number in strNumbers)
                                {
                                    if (item.CardNumber != number) continue;
                                    item.CardTime = time;
                                    item.CardPartition = partition;
                                    item.CardCount = DistanceIssue.SetCount(item.CardCount);
                                    item.Id = CardManager.Insert(item);
                                    info.ViceCardInfos.Add(item);

                                    bundledInfos.Add(new BundledInfo()
                                    {
                                        //Cid = info.Id,
                                        HostCardNumber = info.CardNumber,
                                        Vid = item.Id,
                                        ViceCardNumber = item.CardNumber
                                    });
                                    break;
                                }
                            }
                        }

                        viceInfos = CardManager.GetViceCards(strNumbers);
                        if (viceInfos.Count > 0)
                        {
                            foreach (CardInfo item in viceInfos)
                            {
                                item.CardTime = time;
                                item.CardPartition = partition;
                                item.CardCount = DistanceIssue.SetCount(item.CardCount);

                                bundledInfos.Add(new BundledInfo()
                                {
                                    //Cid = info.Id,
                                    HostCardNumber = info.CardNumber,
                                    Vid = item.Id,
                                    ViceCardNumber = item.CardNumber
                                });
                            }
                            info.ViceCardInfos.AddRange(viceInfos);
                            CardManager.Update(viceInfos.ToArray());
                        }
                    }
                    else if (type == (int)PortEnums.CardTypes.Card3)//车牌号码
                    {
                        CardInfo licensePlateInfo = CardManager.GetLicensePlateInfo(strNumber);
                        if (licensePlateInfo != null)
                        {
                            licensePlateInfo.CardTime = time;
                            licensePlateInfo.CardPartition = partition;
                            CardManager.Update(licensePlateInfo);
                        }
                        else
                        {
                            licensePlateInfo = new CardInfo()
                            {
                                CardNumber = strNumber,
                                CardTime = time,
                                CardPartition = partition,
                                CardType = -1,
                            };
                            licensePlateInfo.Id = CardManager.Insert(licensePlateInfo);
                        }
                        info.ViceCardInfos.Add(licensePlateInfo);
                        bundledInfos.Add(new BundledInfo()
                        {
                            //Cid = info.Id,
                            HostCardNumber = info.CardNumber,
                            Vid = licensePlateInfo.Id,
                            ViceCardNumber = licensePlateInfo.CardNumber,
                        });
                    }
                    if (bundledInfos.Count > 0)
                    {
                        BundledInfoManager.Insert(bundledInfos.ToArray());
                    }
                }
                string json = Utility.JsonSerializerByArrayData(info.ViceCardInfos.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostIssue(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                Chromium.Remote.CfrTime cTime = e.Arguments[1].DateValue;
                DateTime time = new DateTime(cTime.Year, cTime.Month, cTime.DayOfMonth);
                int distance = e.Arguments[2].IntValue;
                int limit = e.Arguments[3].IntValue;
                int type = e.Arguments[4].IntValue;
                int partition = e.Arguments[5].IntValue;

                CardInfo info = CardManager.CardInfos[index];
                info.CardType = type;
                info.CardDistance = distance;
                info.CardTime = time;
                info.ParkingRestrictions = limit;
                info.CardPartition = partition;
                info.CardCount = DistanceIssue.SetCount(info.CardCount);

                byte[] by;
                if (info.CardType == (int)PortEnums.CardTypes.Card2 && (info.ViceCardInfos != null && info.ViceCardInfos.Count > 0))
                {
                    info.CardTime = info.ViceCardInfos.Max(w => w.CardTime);

                    ViceCardRemoveLock lockReceived = null;
                    foreach (CardInfo item in info.ViceCardInfos)
                    {
                        if (item.CardLock == 1)
                        {
                            by = DistanceIssue.Issue(item);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);

                            ReceivedManager.SetReceivedFunction<ViceCardRemoveLock>();
                            if (lockReceived == null)
                            {
                                lockReceived = ReceivedManager.GetReceivedFun<ViceCardRemoveLock>();
                            }
                            lockReceived.RemoveLockInfo = item;

                            for (int i = 0; i < 250; i++)
                            {
                                Thread.Sleep(10);
                                if (lockReceived.RemoveLockInfo == null)
                                {
                                    if (item.CardLock == 0)
                                    {
                                        ViewCallFunction.ViewRemoveLock(info.ViceCardInfos);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                by = DistanceIssue.Issue(info);
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<CardIssue>();
                    ReceivedManager.GetReceivedFun<CardIssue>().IssueInfo = info;
                }
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PortBatchParameter(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                if (CardManager.BatchParam == null)
                {
                    CardManager.BatchParam = new CardInfo()
                    {
                        CardTime = DateTime.Now
                    };
                }
                string json = Utility.JsonSerializerBySingleData(CardManager.BatchParam);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUpdateBatchParam(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                Chromium.Remote.CfrTime cTime = e.Arguments[0].DateValue;
                DateTime time = new DateTime(cTime.Year, cTime.Month, cTime.DayOfMonth);
                int distance = e.Arguments[1].IntValue;
                int partition = e.Arguments[2].IntValue;

                CardManager.BatchParam.CardTime = time;
                CardManager.BatchParam.CardDistance = distance;
                CardManager.BatchParam.CardPartition = partition;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
        }

        private static void Host_PostBatch(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            UpdateBatchParam(e);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int index = -1;
                    CardBatch batchReceived = null;
                    foreach (CardInfo item in CardManager.CardInfos)
                    {
                        if (item.Id == 0 && item.CardType < 3)
                        {
                            index += 1;

                            item.CardTime = CardManager.BatchParam.CardTime;
                            item.CardDistance = CardManager.BatchParam.CardDistance;
                            item.ParkingRestrictions = CardManager.BatchParam.ParkingRestrictions;
                            item.CardPartition = CardManager.BatchParam.CardPartition;

                            byte[] by = DistanceIssue.Issue(item);
                            bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            if (ret)
                            {
                                ReceivedManager.SetReceivedFunction<CardBatch>();
                                if (batchReceived == null)
                                {
                                    batchReceived = ReceivedManager.GetReceivedFun<CardBatch>();
                                }
                                batchReceived.IssueInfo = item;

                                for (int i = 0; i < 250; i++)
                                {
                                    Thread.Sleep(10);
                                    if (batchReceived.IssueInfo == null)
                                    {
                                        if (item.Id > 0)
                                        {
                                            ViewCallFunction.ViewDisplayBatchContent(item, index);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    ViewCallFunction.ViewAlert(ex.Message);
                }
                finally
                {
                    int count = CardManager.CardInfos.Where(w => w.Id == 0 && w.CardType < 3).Count();
                    ViewCallFunction.ViewBatchOver(count);
                }
            });
        }

        private static void Host_PostPersonnelIssue(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                Chromium.Remote.CfrTime cTime = e.Arguments[1].DateValue;
                DateTime time = new DateTime(cTime.Year, cTime.Month, cTime.DayOfMonth);
                int partition = e.Arguments[2].IntValue;

                CardInfo info = CardManager.CardInfos[index];
                info.CardTime = time;
                info.CardPartition = partition;

                byte[] by = PersonnelIssue.Issue(info);
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<CardIssuePersonnel>();
                    ReceivedManager.GetReceivedFun<CardIssuePersonnel>().IssueInfo = info;
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostPersonnelBatch(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            UpdateBatchParam(e);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    int index = -1;
                    CardBatchPersonnel batchPersonnel = null;
                    foreach (CardInfo item in CardManager.CardInfos)
                    {
                        if (item.Id > 0 || item.CardType > 2) continue;
                        index += 1;

                        item.CardTime = CardManager.BatchParam.CardTime;
                        item.CardPartition = CardManager.BatchParam.CardPartition;

                        byte[] by = PersonnelIssue.Issue(item);
                        bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        if (ret)
                        {
                            ReceivedManager.SetReceivedFunction<CardBatchPersonnel>();
                            if (batchPersonnel == null)
                            {
                                batchPersonnel = ReceivedManager.GetReceivedFun<CardBatchPersonnel>();
                            }
                            batchPersonnel.IssueInfo = item;
                            for (int i = 0; i < 250; i++)
                            {
                                Thread.Sleep(10);
                                if (batchPersonnel.IssueInfo == null)
                                {
                                    if (item.Id > 0)
                                    {
                                        ViewCallFunction.ViewDisplayBatchContent(item, index);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    ViewCallFunction.ViewAlert(ex.Message);
                }
                finally
                {
                    int count = CardManager.CardInfos.Where(w => w.Id == 0).Count();
                    ViewCallFunction.ViewBatchOver(count);
                }
            });
        }

        private static void UpdateBatchParam(Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            Chromium.Remote.CfrTime cTime = e.Arguments[0].DateValue;
            DateTime time = new DateTime(cTime.Year, cTime.Month, cTime.DayOfMonth);
            int distance = e.Arguments[1].IntValue;
            int partition = e.Arguments[2].IntValue;

            CardManager.BatchParam.CardTime = time;
            CardManager.BatchParam.CardDistance = distance;
            CardManager.BatchParam.CardPartition = partition;
        }
    }
}
