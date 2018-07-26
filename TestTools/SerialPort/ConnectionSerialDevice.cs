using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading;
using TestTools.Devices;
using Bll;
using Model.SerialPortDataModel;

namespace TestTools
{
    /// <summary>
    /// 自动连接串口设备
    /// </summary>
    public class ConnectionSerialDevice
    {
        private static System.Timers.Timer m_Timer;

        private static SerialPortEx m_SerialPort;
        private static bool m_StopTimer;
        private static bool m_StartTimer;

        private static DataValidation m_Validation;
        private static List<byte> m_Bytes;
        private static bool m_Over = false;

        /// <summary>
        /// 串口自动连接设备
        /// </summary>
        public static bool AutoConnectioinDevice { get; set; }

        public static void Start()
        {
            if (m_Timer == null)
            {
                m_Timer = new System.Timers.Timer(10)
                {
                    AutoReset = false
                };
                m_Timer.Elapsed += OnTimerEvent;
            }
            if (m_Validation == null)
            {
                m_Validation = new DataValidation();
                m_Validation.DealHeadEnds.Add(new DealHeadEnd(10, 13));
            }
            if (m_Bytes == null)
            {
                m_Bytes = new List<byte>();
            }
            if (!m_StartTimer)
            {
                m_Timer.Enabled = true;
            }
            m_StopTimer = false;
        }

        private static void OnTimerEvent(object sender, ElapsedEventArgs e)
        {
            m_StartTimer = true;
            byte[] by = null;
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (m_StopTimer) return;
                    foreach (string portName in SearchSerialPort.SerialPortNames)
                    {
                        if (m_StopTimer) return;

                        m_Over = false;
                        if (m_SerialPort == null)
                        {
                            m_SerialPort = SerialPortManager.CreateSerialPort();
                            m_SerialPort.DataReceived += SerialPortDataReceived;
                        }

                        m_SerialPort.PortName = portName;
                        try
                        {
                            m_SerialPort.BaudRate = WinApi.B19200;
                            m_SerialPort.Open();

                            if (!SerialPortManager.Device1.SerialPortDevice.IsOpen)
                            {
                                if (m_StopTimer) return;
                                try
                                {
                                    by = PortAgreement.DistanceDeviceEncryption(DefaultParams.DEVICEDEFAULTCLIENTNUMBER, DefaultParams.DEVICEDEFAULTPASSWORD);
                                    m_SerialPort.Write(by);

                                    bool ret = WaitResult(SerialPortManager.Device1);
                                    if (ret) continue;
                                }
                                catch (Exception ex)
                                {
                                    Log4Helper.ErrorInfo(ex.Message, ex);
#if DEBUG
                                    Console.WriteLine(ex.Message);
#endif
                                }
                            }

                            m_SerialPort.SetBaudRate(WinApi.B9600);
                            Thread.Sleep(50);

                            if (!SerialPortManager.Device2.SerialPortDevice.IsOpen)
                            {
                                try
                                {
                                    for (int number = 0; number < 50; number++)
                                    {
                                        if (m_StopTimer) return;
                                        by = PortAgreement.CorridorReadCardData(number);
                                        m_SerialPort.Write(by);
                                        Thread.Sleep(50);
                                        if (m_Over)
                                        {
                                            ReplaceSerialPort(SerialPortManager.Device2);
                                            break;
                                        }
                                    }

                                    if (m_Over) continue;
                                    bool ret = WaitResult(SerialPortManager.Device2);
                                    if (ret) continue;
                                }
                                catch (Exception ex)
                                {
                                    Log4Helper.ErrorInfo(ex.Message, ex);
#if DEBUG
                                    Console.WriteLine(ex.Message);
#endif
                                }
                            }

                            m_SerialPort.Close();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
            }
            finally
            {
                by = null;
                m_Validation.Dispose();
                if (m_SerialPort != null && m_SerialPort.IsOpen)
                {
                    m_SerialPort.Close();
                }
                m_SerialPort = null;
                if (!m_StopTimer && (!SerialPortManager.Device1.SerialPortDevice.IsOpen && !SerialPortManager.Device2.SerialPortDevice.IsOpen))
                {
                    ViewCallFunction.ViewConnectionFailedMessage();
                }
                m_StartTimer = false;
            }
        }

        private static void SerialPortDataReceived(int port)
        {
            Thread.Sleep(10);
            byte[] by;
            try
            {
                int len = m_SerialPort.GetIqueue;
                if (len <= 0)
                {
                    by = m_Validation.Combination(m_Bytes.ToArray());
                    if (by != null)
                    {
                        m_Over = true;
                        m_Bytes.Clear();
                    }
                    return;
                }
                by = m_SerialPort.Read(len);
                m_Bytes.AddRange(by);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
        }

        private static bool WaitResult(IDevice device)
        {
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(10);
                if (m_Over)
                {
                    ReplaceSerialPort(device);
                    return true;
                }
            }
            return false;
        }

        private static void ReplaceSerialPort(IDevice device)
        {
            try
            {
                m_SerialPort.DataReceived -= SerialPortDataReceived;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
            SerialPortManager.CompleteSerialPortConnection(m_SerialPort, device);
            m_SerialPort = null;
            m_Validation.Dispose();
        }

        public static void Stop()
        {
            if (m_StartTimer)
            {
                m_StopTimer = true;
            }
        }
    }
}
