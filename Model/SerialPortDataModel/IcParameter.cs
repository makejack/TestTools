using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    public class IcParameter
    {
        /// <summary>
        /// Ic卡编号
        /// </summary>
        /// <returns></returns>
        public string IcNumber { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        /// <returns></returns>
        public string LicensePlate { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        /// <returns></returns>
        public DateTime Time { get; set; }

        /// <summary>
        /// 写入或加密结果
        /// </summary>
        /// <returns>true 成功 false 失败</returns>
        public bool Result { get; set; }
    }
}
