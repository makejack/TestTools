using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.VisualBasic.Devices;

namespace Bll
{
    public class SearchSerialPort
    {
        private static Timer m_Timer;
        private static Computer m_Computer = new Computer();
        private static int m_PortCount = 0;
        private static object m_Lock = new object();

        public static List<string> SerialPortNames;

        public delegate void SerialPortNamesCall(List<string> serialportnames);
        public static event SerialPortNamesCall SerialPortNamesChange;
        private static void OnSerialPortNamesChange(List<string> serialportnames)
        {
            SerialPortNamesChange?.Invoke(serialportnames);
        }
        
        public static void Start()
        {
            Start(200);
        }

        public static void Start(int interval)
        {
            if (m_Timer == null)
            {
                m_Timer = new Timer(interval)
                {
                    AutoReset = true
                };
                m_Timer.Elapsed += Timer_Elapsed;
            }
            m_Timer.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (m_Lock)
            {
                if(m_Computer.Ports.SerialPortNames.Count != m_PortCount)
                {
                    SerialPortNames = new List<string>(m_Computer.Ports.SerialPortNames);
                    m_PortCount = m_Computer.Ports.SerialPortNames.Count;
                    OnSerialPortNamesChange(SerialPortNames);
                }
            }
        }

        public static void Stop()
        {
            if (m_Timer.Enabled)
            {
                m_Timer.Stop();
            }
        }
        
    }
}
