using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bll;
using System.Threading;

namespace TestTools.PortDataManagment
{
    public class TestPort : ISerialPortDataReceived
    {
        public void DataReceived(byte[] by)
        {
            try
            {
                if (by[0] == 9 && by[by.Length - 1] == 12)
                {
                    OverTimeManager.Stop();

                    ViewCallFunction.ViewDownloadOver(by.Length == 24);
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
            finally
            {
                Thread.Sleep(100);
                SerialPortManager.Device2.SerialPortDevice.SetBaudRate(WinApi.B9600);
            }
        }

        public void OverTime()
        {
            ViewCallFunction.ViewCloseLoading();
            
            SerialPortManager.Device2.SerialPortDevice.SetBaudRate(WinApi.B9600);
        }
    }
}
