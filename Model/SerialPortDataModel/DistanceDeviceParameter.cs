using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    /// <summary>
    /// 定距发行器设备数据
    /// </summary>
    public class DistanceDeviceParameter
    {
        /// <summary>
        /// 命令
        /// </summary>
        public PortEnums.DistanceCommands Command { get; set; }

        /// <summary>
        /// 辅助命令
        /// </summary>
        public PortEnums.AuxiliaryCommands AuxiliaryCommand { get; set; }
    }
}
