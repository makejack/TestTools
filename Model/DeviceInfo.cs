using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class DeviceInfo
    {
        public int Did { get; set; }

        /// <summary>
        /// 主机编号
        /// </summary>
        /// <returns></returns>
        public int HostNumber { get; set; }

        /// <summary>
        /// 进出口
        /// </summary>
        public int IOMouth { get; set; }

        /// <summary>
        /// 道闸编号
        /// </summary>
        /// <returns></returns>
        public int BrakeNumber { get; set; }

        /// <summary>
        /// 开闸模式
        /// </summary>
        /// <returns></returns>
        public int OpenModel { get; set; }

        /// <summary>
        /// 车场分区
        /// </summary>
        /// <returns></returns>
        public int Partition { get; set; }

        /// <summary>
        /// 防潜回
        /// </summary>
        public int SAPBF { get; set; }

        /// <summary>
        /// 离开检测
        /// </summary>
        /// <returns></returns>
        public int Detection { get; set; }

        /// <summary>
        /// 读卡距离
        /// </summary>
        /// <returns></returns>
        public int CardReadDistance { get; set; }

        /// <summary>
        /// 读卡延迟
        /// </summary>
        /// <returns></returns>
        public int ReadCardDelay { get; set; }

        /// <summary>
        /// 车牌识别
        /// </summary>
        /// <returns></returns>
        public int CameraDetection { get; set; }

        /// <summary>
        /// 无线编号
        /// </summary>
        /// <returns></returns>
        public int WirelessNumber { get; set; }

        /// <summary>
        /// 频率偏移
        /// </summary>
        /// <returns></returns>
        public int FrequencyOffset { get; set; }

        /// <summary>
        /// 语言种类
        /// </summary>
        /// <returns></returns>
        public int Language { get; set; }

        /// <summary>
        /// 模糊车牌查询
        /// </summary>
        /// <returns></returns>
        public int FuzzyQuery { get; set; }
    }
}
