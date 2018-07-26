using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTools.PortDataManagment
{
    public class ReceivedManager
    {
        public static ISerialPortDataReceived ReceivedFunction { get; set; }

        public static void SetReceivedFunction<T>()
        {
            if (ReceivedFunction == null ||
               (ReceivedFunction != null && ReceivedFunction.GetType().Name != typeof(T).Name))
            {
                ReceivedFunction = (ISerialPortDataReceived)Activator.CreateInstance<T>();
            }
            OverTimeManager.Start();
        }

        public static T GetReceivedFun<T>()
        {
            if (ReceivedFunction == null) throw new Exception("ReceivedFunction 当前为空值");            
            return (T)ReceivedFunction;
        }
    }
}
