using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class CardInfo
    {
        public int Id { get; set; }

        public string CardNumber { get; set; }
        /// <summary>
        /// 定距卡类型 -1 车牌号码 0 单卡 1 组合卡 2 车牌识别卡 3 副卡
        /// </summary>
        /// <returns></returns>
        public int CardType { get; set; }
        /// <summary>
        /// 有效期限
        /// </summary>
        public DateTime CardTime { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public int CardDistance { get; set; }
        /// <summary>
        /// 锁
        /// </summary>
        public int CardLock { get; set; }
        /// <summary>
        /// 挂失
        /// </summary>
        public int CardReportLoss { get; set; }
        /// <summary>
        /// 停车限制
        /// </summary>
        public int ParkingRestrictions { get; set; }
        /// <summary>
        /// 车场分区
        /// </summary>
        public int CardPartition { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public int Electricity { get; set; }
        /// <summary>
        /// 同步位
        /// </summary>
        public int Synchronous { get; set; }

        public int InOutState { get; set; }

        public int CardCount { get; set; }

        public int ViceCardCount { get; set; }
        /// <summary>
        /// 副卡信息
        /// </summary>
        public List<CardInfo> ViceCardInfos { get; set; }

    }
    
}

