using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools.Devices;
using Bll;

namespace TestTools
{
    /// <summary>
    /// 串口监视 - 监视串口数量的变化
    /// </summary>
    public class PortMonitor
    {
        public static void StartMonitor()
        {
            SearchSerialPort.SerialPortNamesChange += SerialPortCountChange;
            SearchSerialPort.Start(100);
        }

        private static void SerialPortCountChange(List<string> serialPortNames)
        {
            ViewCallFunction.ViewSerialPortCountChanged(serialPortNames);

            if (SerialPortManager.Device1.SerialPortDevice.IsOpen)
            {
                CurrentPortIsDisconnected(SerialPortManager.Device1.SerialPortDevice, serialPortNames);
            }

            if (SerialPortManager.Device2.SerialPortDevice.IsOpen)
            {
                CurrentPortIsDisconnected(SerialPortManager.Device2.SerialPortDevice, serialPortNames);
            }

            if (ConnectionSerialDevice.AutoConnectioinDevice)
            {
                if (!SerialPortManager.Device1.SerialPortDevice.IsOpen || !SerialPortManager.Device2.SerialPortDevice.IsOpen)
                {
                    ConnectionSerialDevice.Start();
                }
            }
        }

        private static void CurrentPortIsDisconnected(SerialPortEx serial, List<string> serialPortNames)
        {
            try
            {
                foreach (var item in serialPortNames)
                {
                    if (serial.PortName == item)
                    {
                        return;
                    }
                }
                SerialPortManager.CloseSerialPort(serial);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
            finally
            {
                ViewCallFunction.ViewSerialPortChanged(serial);
            }
        }

    }
}
