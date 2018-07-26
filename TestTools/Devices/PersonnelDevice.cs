using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bll;
using TestTools.PortDataManagment;

namespace TestTools.Devices
{
    /// <summary>
    /// 人员通道设备
    /// </summary>
    public class PersonnelDevice : IDevice
    {
        public SerialPortEx SerialPortDevice { get; set; }

        private DataValidation m_Validation;

        public PersonnelDevice(SerialPortEx serial, string name)
        {
            SerialPortDevice = serial;
            SerialPortDevice.BaudRate = WinApi.B9600;

            SetDataReceived();
            if (!string.IsNullOrEmpty(name))
            {
                SerialPortDevice.Name = name;
            }

            m_Validation = new DataValidation();
            m_Validation.DealHeadEnds.Add(new DealHeadEnd(10, 13));
        }

        public void SetDataReceived()
        {
            SerialPortDevice.DataReceived += DataReceived;
        }

        private void DataReceived(int port)
        {
            Thread.Sleep(10);
            int len = SerialPortDevice.GetIqueue;
            if (len <= 0) return;
            byte[] by = SerialPortDevice.Read(len);
            try
            {
                by = m_Validation.Combination(by);
                if (by != null && ReceivedManager.ReceivedFunction != null)
                {
                    ReceivedManager.ReceivedFunction.DataReceived(by);
                }
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
        }
    }
}
