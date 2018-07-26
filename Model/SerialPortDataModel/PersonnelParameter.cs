using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    /// <summary>
    /// 人员通道设备数据
    /// </summary>
    public class PersonnelParameter
    {
        /// <summary>
        /// 通道编号
        /// </summary>
        public int CorridorNumber { get; set; }

        /// <summary>
        /// 主机编号
        /// </summary>
        public int HostNumber { get; set; }

        /// <summary>
        /// 卡片编号
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 卡片时间
        /// </summary>
        public DateTime CardTime { get; set; }

        /// <summary>
        /// 系统时间
        /// </summary>
        public DateTime SystemTime { get; set; }

        /// <summary>
        /// 打门状态
        /// </summary>
        public int OpenTheDoorState { get; set; }
    }
}
