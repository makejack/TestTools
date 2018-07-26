using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TestTools.Devices;
using Bll;

namespace TestTools
{
    /// <summary>
    /// 串口管理器
    /// </summary>
    public class SerialPortManager
    {
        public static DistanceDevice Device1;

        public static PersonnelDevice Device2;

        public static void InitSerialDevice()
        {
            Device1 = new DistanceDevice(CreateSerialPort(), "Device1");

            Device2 = new PersonnelDevice(CreateSerialPort(), "Device2");
        }

        public static SerialPortEx CreateSerialPort()
        {
            SerialPortEx serialPort = new SerialPortEx()
            {
                StopBit = WinApi.STOP_1,
                DataBit = WinApi.BIT_8,
                BaudRate = WinApi.B19200,
                Parity = WinApi.p_NONE,
            };
            return serialPort;
        }

        public static void CloseSerialPort(SerialPortEx port)
        {
            port.Close();
        }

        public static void OpenSerialPort(SerialPortEx port, string portName)
        {
            port.PortName = portName;
            port.Open();
        }

        public static bool WriteSerialPortData(IDevice device, byte[] by)
        {
            try
            {
                if (device.SerialPortDevice.IsOpen)
                {
                    device.SerialPortDevice.Write(by);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// 完成串口对设备的连接
        /// </summary>
        /// <param name="connectionDevice">连接的设备</param>
        /// <param name="useDevice">使用的设备</param>
        public static void CompleteSerialPortConnection(SerialPortEx serial, IDevice device)
        {
            serial.Name = device.SerialPortDevice.Name;
            serial.Index = device.SerialPortDevice.Index;
            device.SerialPortDevice = serial;
            device.SetDataReceived();

            ViewCallFunction.ViewSerialPortChanged(device.SerialPortDevice);
        }
    }
}
