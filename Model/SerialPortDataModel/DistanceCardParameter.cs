using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    public class DistanceCardParameter
    {
        /// <summary>
        /// 命令
        /// </summary>
        public PortEnums.DistanceCommands Command { get; set; }

        /// <summary>
        /// 辅助命令
        /// </summary>
        public PortEnums.AuxiliaryCommands AuxiliaryCommand { get; set; }

        /// <summary>
        /// 卡片编号
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// 卡片类型值
        /// </summary>
        /// <returns></returns>
        public int CardTypeValue { get; set; }

        /// <summary>
        /// 卡片类型字节
        /// </summary>
        public CardTypeParameter CardTypeParameter { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public int Area { get; set; }

        /// <summary>
        /// 起始位置
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int Len { get; set; }

        /// <summary>
        /// 功能字节值
        /// </summary>
        public int FunctionByteValue { get; set; }

        /// <summary>
        /// 功能字节参数
        /// </summary>
        public FunctionByteParameter FunctionByteParam { get; set; }

        /// <summary>
        /// 计数
        /// </summary>
        public int Count { get; set; }
    }

    public class FunctionByteParameter
    {
        /// <summary>
        /// 挂失
        /// </summary>
        public int Loss { get; set; }

        /// <summary>
        /// 同步位
        /// </summary>
        public int Synchronous { get; set; }

        /// <summary>
        /// 注册后的卡片类型
        /// </summary>
        public PortEnums.CardTypes RegistrationType { get; set; }

        /// <summary>
        /// 车位限制
        /// </summary>
        public int ParkingRestrictions { get; set; }

        /// <summary>
        /// 副卡的数量
        /// </summary>
        public int ViceCardCount { get; set; }

        /// <summary>
        /// 进出的状态
        /// </summary>
        public int InOutState { get; set; }
    }

    public class CardTypeParameter
    {

        /// <summary>
        /// 锁卡 0：正常 1：锁
        /// </summary>
        /// <returns></returns>
        public int CardLock { get; set; }
        /// <summary>
        /// 卡片距离调节选择 0：读头调节（卡片距离无效）1：卡片调节（卡片距离有效)
        /// </summary>
        /// <returns></returns>
        public int DistanceSate { get; set; }
        /// <summary>
        /// 卡片距离 0：1.2米 1：2.5米 2:3.8米 3：5米
        /// </summary>
        /// <returns></returns>
        public int Distance { get; set; }
        /// <summary>
        /// 电池电量  0：正常 1：电量低
        /// </summary>
        /// <returns></returns>
        public int Electricity { get; set; }
        /// <summary>
        /// 卡片类型（1或5：人车合一 2：人卡 3：车卡 ），挂失卡或密码错误时为扩展功能除电池电量外（其它参数无效）
        /// </summary>
        /// <returns></returns>
        public PortEnums.CardTypes CardType { get; set; }
    }
}
