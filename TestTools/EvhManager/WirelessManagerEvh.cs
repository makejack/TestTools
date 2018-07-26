using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TestTools;
using TestTools.PortDataManagment;
using Bll;
using Model;
using System.Reflection;

namespace TestTools
{
    public class WirelessManagerEvh
    {
        private static bool m_StopWirelessSearch = false;

        public static void InitEvent()
        {
            Main.GetMain.GlobalObject.AddFunction("HostPostWirelessSetting").Execute += Host_PostWirelessSetting;
            Main.GetMain.GlobalObject.AddFunction("HostPostWirelessSearch").Execute += Host_PostWirelessSearch;
            Main.GetMain.GlobalObject.AddFunction("HostPostStopWirelessSearch").Execute += Host_PostStopWirelessSearch;
            Main.GetMain.GlobalObject.AddFunction("HostPostWirelessTest").Execute += Host_PostWirelessTest;
            Main.GetMain.GlobalObject.AddFunction("HostPostWirelessQuery").Execute += Host_PostWirelessQuery;
            Main.GetMain.GlobalObject.AddFunction("HostPostReadIcCard").Execute += Host_PostReadIcCard;
            Main.GetMain.GlobalObject.AddFunction("HostPostStopReadIcCard").Execute += Host_PostStopReadIcCard;
        }

        private static void Host_PostWirelessSetting(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int wirelessNumber = e.Arguments[0].IntValue;
                int frequency = e.Arguments[1].IntValue;

                Task.Factory.StartNew(() =>
                {
                    SettingModule settingModule = SettingReceived();
                    try
                    {
                        ViewCallFunction.ViewWirelessMessage("打开模块设置功能。");
                        byte[] by = PortAgreement.OpenModular();
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(20);

                        #region 设置模块发送ID

                        for (int i = 0; i < 3; i++)
                        {
                            settingModule.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("设置模块发送ID（编号）。");
                            by = PortAgreement.SetModuleTid(wirelessNumber);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (settingModule.SettingOver)
                            {
                                break;
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            settingModule.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("设置模块接收ID（编号）。");
                            by = PortAgreement.SetModuleRid(wirelessNumber);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (settingModule.SettingOver)
                            {
                                break;
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            settingModule.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("设置模块频率。");
                            by = PortAgreement.SetModuleFrequency(frequency);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (settingModule.SettingOver)
                            {
                                break;
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            settingModule.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("设置模块回传功能。");
                            by = PortAgreement.SetModuleComesBack(1);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (settingModule.SettingOver)
                            {
                                break;
                            }
                        }

                        ViewCallFunction.ViewWirelessMessage("关闭模块设置功能。");
                        by = PortAgreement.CloseModular();
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(20);

                        for (int i = 0; i < 3; i++)
                        {
                            settingModule.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("发送数据测试。");
                            by = PortAgreement.SetModular("ABCDEF");
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (settingModule.SettingOver)
                            {
                                break;
                            }
                        }

                        #endregion 设置模块发送ID
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.ErrorInfo(ex.Message, ex);
                        ViewCallFunction.ViewAlert(ex.Message);
                    }
                    finally
                    {
                        OverTimeManager.Stop();
                        ViewCallFunction.ViewWirelessOver();
                    }
                });
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            
        }

        private static void Host_PostWirelessSearch(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    SettingModule settingModule = SettingReceived();
                    OverTimeManager.Stop();

                    byte[] by;
                    try
                    {
                        for (int i = 1; i <= 64; i++)
                        {
                            if (m_StopWirelessSearch) break;

                            ViewCallFunction.ViewWirelessMessage($"搜索目标频率{i}/64。");

                            //ViewCallFunction.ViewWirelessMessage("打开模块设置功能。");
                            by = PortAgreement.OpenModular();
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(20);

                            for (int j = 0; j < 3; j++)
                            {
                                if (m_StopWirelessSearch) break;
                                settingModule.SettingOver = false;
                                ViewCallFunction.ViewWirelessMessage("设置模块频率。");
                                by = PortAgreement.SetModuleFrequency(i);
                                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                                Thread.Sleep(250);
                                if (settingModule.SettingOver)
                                {
                                    break;
                                }
                            }

                            for (int j = 0; j < 3; j++)
                            {
                                if (m_StopWirelessSearch) break;
                                settingModule.SettingOver = false;
                                //ViewCallFunction.ViewWirelessMessage("设置模块回传功能。");
                                by = PortAgreement.SetModuleComesBack(1);
                                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                                Thread.Sleep(250);
                                if (settingModule.SettingOver)
                                {
                                    break;
                                }
                            }

                            //ViewCallFunction.ViewWirelessMessage("关闭模块设置功能。");
                            by = PortAgreement.CloseModular();
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(20);

                            for (int j = 0; j < 3; j++)
                            {
                                if (m_StopWirelessSearch) break;
                                settingModule.SettingOver = false;
                                ViewCallFunction.ViewWirelessMessage("发送数据测试。");
                                by = PortAgreement.SetModular("ABCDEF");
                                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                                Thread.Sleep(250);
                                if (settingModule.SettingOver)
                                {
                                    ViewCallFunction.ViewWirelessMessage($"无线频率 {i} 发现设备。");
                                    break;
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
                        m_StopWirelessSearch = false;
                        ViewCallFunction.ViewWirelessOver();
                    }
                });
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            
        }

        private static void Host_PostStopWirelessSearch(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            m_StopWirelessSearch = true;
            
        }

        private static void Host_PostWirelessTest(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                ReceivedManager.SetReceivedFunction<WirelessTest>();
                byte[] by = PortAgreement.OpenTheDoorAndVoice(new OpenTheDoorParam2()
                {
                    DeviceAddress = 0,
                    IcCardNumber = "093D2446",
                    LicensePlateNumber = "福A000000",
                    LicensePlateColor = LicensePlateColors.Blue,
                    Time = DateTime.Now,
                });
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            finally
            {
                OverTimeManager.Stop();
            }
            
        }

        private static void Host_PostWirelessQuery(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    ReceivedManager.SetReceivedFunction<WirelessQuery>();
                    WirelessQuery queryReceived = ReceivedManager.GetReceivedFun<WirelessQuery>();
                    try
                    {
                        ViewCallFunction.ViewWirelessMessage("打开模块设置功能。");
                        byte[] by = PortAgreement.OpenModular();
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(20);

                        for (int i = 0; i < 3; i++)
                        {
                            queryReceived.SettingOver = false;
                            ViewCallFunction.ViewWirelessMessage("关闭模块回传功能。");
                            by = PortAgreement.SetModuleComesBack(0);
                            SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                            Thread.Sleep(250);
                            if (queryReceived.SettingOver)
                            {
                                break;
                            }
                        }

                        ViewCallFunction.ViewWirelessMessage("关闭模块设置功能。");
                        by = PortAgreement.CloseModular();
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(20);

                        ViewCallFunction.ViewWirelessMessage("查询无线ID（编号）。");
                        string strQueryFrequency = "AT+FREQ?";
                        by = PortAgreement.SetModular(strQueryFrequency);
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(250);

                        ViewCallFunction.ViewWirelessMessage("查询无线的频率。");
                        string strQueryRid = "AT+TID?";
                        by = PortAgreement.SetModular(strQueryRid);
                        SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
                        Thread.Sleep(250);
                    }
                    catch (Exception ex)
                    {
                        Log4Helper.ErrorInfo(ex.Message, ex);
                        ViewCallFunction.ViewAlert(ex.Message);
                    }
                    finally
                    {
                        OverTimeManager.Stop();
                        ViewCallFunction.ViewWirelessOver();
                    }
                });
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            
        }

        private static void Host_PostReadIcCard(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                byte[] by = PortAgreement.ReadIc();
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);

                ReceivedManager.SetReceivedFunction<ReadIcCard>();
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
            finally
            {
                OverTimeManager.Stop();
            }
            
        }

        private static void Host_PostStopReadIcCard(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                byte[] by = PortAgreement.CloseModular();
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, by);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }


         private static SettingModule SettingReceived()
        {
            ReceivedManager.SetReceivedFunction<SettingModule>();
            return ReceivedManager.GetReceivedFun<SettingModule>();
        }
    }
}
