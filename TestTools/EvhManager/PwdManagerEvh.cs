using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools.PortDataManagment;
using Bll;
using Bll.Management;
using Model;
using System.Reflection;

namespace TestTools
{
    public class PwdManagerEvh
    {
        public static void InitEvent()
        {
            Main.GetMain.GlobalObject.AddFunction("HostPostDistanceDeviceEncrypt").Execute += Host_PostDistanceDeviceEncrypt;
            Main.GetMain.GlobalObject.AddFunction("HostPostDistanceCardEncrypt").Execute += Host_PostDistanceCardEncrypt;
            Main.GetMain.GlobalObject.AddFunction("HostPostIcDeviceEncrypt").Execute += Host_PostIcDeviceEncrypt;
            Main.GetMain.GlobalObject.AddFunction("HostPostStartIcCardEncrypt").Execute += Host_PostStartIcCardEncrypt;
            Main.GetMain.GlobalObject.AddFunction("HostPostStopIcCardEncrypt").Execute += Host_PostStopIcCardEncrypt;
        }

        private static void Host_PostDistanceDeviceEncrypt(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string pwd = e.Arguments[0].StringValue;
                string clientnumber = e.Arguments[1].StringValue;

                byte[] bys = PortAgreement.DistanceDeviceEncryption(clientnumber, pwd);
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
                ReceivedManager.SetReceivedFunction<DistanceDeviceEncrypt>();
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDistanceCardEncrypt(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string oldpwd = e.Arguments[0].StringValue;
                string pwd = e.Arguments[1].StringValue;
                string clientnumber = e.Arguments[2].StringValue;

                byte[] bys = PortAgreement.DistanceDeviceEncryption(clientnumber, oldpwd);
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<DistanceCardEncrypt>();
                    DistanceCardEncrypt received = ReceivedManager.GetReceivedFun<DistanceCardEncrypt>();
                    received.ClientNumber = clientnumber;
                    received.NewPwd = pwd;
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostIcDeviceEncrypt(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string pwd = e.Arguments[0].StringValue;

                byte[] bys = PortAgreement.IcDeviceEncryption(pwd);
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);

                ReceivedManager.SetReceivedFunction<IcDeviceEncrypt>();
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostStartIcCardEncrypt(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string oldpwd = e.Arguments[0].StringValue;
                string pwd = e.Arguments[1].StringValue;

                byte[] bys = PortAgreement.IcDeviceEncryption(oldpwd);
                SerialPortManager.WriteSerialPortData(SerialPortManager.Device1, bys);

                ReceivedManager.SetReceivedFunction<IcCardEncrypt>();
                IcCardEncrypt received = ReceivedManager.GetReceivedFun<IcCardEncrypt>();
                received.Pwd = pwd;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostStopIcCardEncrypt(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                ISerialPortDataReceived dataReceived = ReceivedManager.ReceivedFunction;
                if (dataReceived is IcCardEncrypt)
                {
                    ((IcCardEncrypt)dataReceived).StopEncrypt = true;
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }
    }
}
