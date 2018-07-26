using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTools.PortDataManagment
{
    public interface ISerialPortDataReceived
    {
        void DataReceived(byte[] by);

        void OverTime();
    }
}
