using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    public class PortDataParameter
    {
        public PortDataParameter()
        {
            Command = new DealCommand();
        }

        /// <summary>
        /// 协议头
        /// </summary>
        public int Header { get; set; }

        /// <summary>
        /// 协议尾
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// 多功能地址
        /// </summary>
        public PortEnums.DealFunctions FunctionAddress { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddress { get; set; }

        /// <summary>
        /// 命令
        /// </summary>
        public DealCommand Command { get; set; }

        /// <summary>
        /// 定距卡数据
        /// </summary>
        public DistanceCardParameter DistanceCardParam { get; set; }

        /// <summary>
        /// IC卡数据
        /// </summary>
        /// <returns></returns>
        public IcParameter IcParam { get; set; }

        /// <summary>
        /// 定距发行器数据
        /// </summary>
        public DistanceDeviceParameter DistanceDeviceParam { get; set; }

        /// <summary>
        /// 主机数据
        /// </summary>
        public HostParameter HostParam { get; set; }

        /// <summary>
        /// 人员通道设备数据
        /// </summary>
        public PersonnelParameter PersonnelParam { get; set; }

        /// <summary>
        /// 模块数据
        /// </summary>
        public ModuleParameter ModuleParam { get; set; }
    }
}
