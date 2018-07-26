using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TestTools.Devices;
using TestTools.PortDataManagment;

namespace TestTools
{
    public class OverTimeManager
    {
        private static Timer m_Timer;

        private static int m_Interval = 5000;

        public static int Interval
        {
            get { return m_Interval; }
            set
            {
                if (m_Interval != value)
                {
                    m_Interval = value;
                    if (m_Timer != null)
                    {
                        m_Timer.Interval = m_Interval;
                    }
                }
            }
        }

        /// <summary>
        /// 开始超时等待
        /// </summary>
        public static void Start()
        {
            if (m_Timer == null)
            {
                m_Timer = new Timer(Interval)
                {
                    AutoReset = false
                };
                m_Timer.Elapsed += OverTimer;
            }
            m_Timer.Start();
        }

        private static void OverTimer(object sender, ElapsedEventArgs e)
        {
            if (ReceivedManager.ReceivedFunction != null)
            {
                ReceivedManager.ReceivedFunction.OverTime();
            }
        }

        public static void Stop()
        {
            m_Timer.Stop();
        }
    }
}
