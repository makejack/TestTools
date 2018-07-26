using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;

namespace TestTools.Devices
{
    public interface IDevice
    {

        SerialPortEx SerialPortDevice { get; set; }
        
        void SetDataReceived();
    }
}
