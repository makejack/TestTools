using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SerialPortDataModel
{
    public class PortEnums
    {
        /// <summary>
        /// 协议头功能地址
        /// </summary>
        public enum DealFunctions
        {
            /// <summary>
            /// 定距卡操作
            /// </summary>
            Distance = 65,
            /// <summary>
            /// IC卡操作
            /// </summary>
            Ic = 66,
            /// <summary>
            /// 无线模块或打开操作或语音操作
            /// </summary>
            ModularAndVoice = 67,
            /// <summary>
            /// 主机设备操作 (无数据时不返回)
            /// </summary>
            HostNoResult = 48,
            /// <summary>
            /// 主机设备操作 (无数据时有返回)
            /// 修改主机序列号返回也是使用这个枚举（发送的多功能地址和返回的地址不一致）
            /// </summary>
            HostResult = 49,
            /// <summary>
            /// 更新主机时间
            /// </summary>
            ProsennelHost = 51,
        }

        /// <summary>
        /// 定距操作命令
        /// </summary>
        public enum DistanceCommands
        {
            /// <summary>
            /// 扫描卡片
            /// </summary>
            Scanning = 0,
            /// <summary>
            /// 读Id
            /// </summary>
            ReadId = 1,
            /// <summary>
            /// 关闭所有卡
            /// </summary>
            CloseAllCard = 2,
            /// <summary>
            /// 点亮LED
            /// </summary>
            LightAllLED = 5,
            /// <summary>
            /// 读所有卡
            /// </summary>
            ReadAllCard = 10,
            /// <summary>
            /// 写所有卡
            /// </summary>
            WriteAllCard = 11,
            /// <summary>
            /// 读所有卡的类型（距离 和 锁)
            /// </summary>
            ReadAllCardType = 12,
            /// <summary>
            /// 修改所有卡密码
            /// </summary>
            ModifyAllCardPwd = 13,
            /// <summary>
            /// 关闭某张卡
            /// </summary>
            CloseACard = 18,
            /// <summary>
            /// 点亮某张卡
            /// </summary>
            LightALED = 21,
            /// <summary>
            /// 读取某张卡
            /// </summary>
            ReadACard = 26,
            /// <summary>
            /// 写入某张卡
            /// </summary>
            WriteACard = 27,
            /// <summary>
            /// 读取某张卡类型（距离 和锁)
            /// </summary>
            ReadACardType = 28,
            /// <summary>
            /// 初始化主机
            /// </summary>
            InitializeDevice = 160,
            /// <summary>
            /// 打开主机
            /// </summary>
            OpenHost = 161,
            /// <summary>
            /// 关闭主机
            /// </summary>
            CloseHost = 162,
            /// <summary>
            /// 打开IC
            /// </summary>
            OpenIc = 163,
            /// <summary>
            /// 关闭IC
            /// </summary>
            CloseIc = 164,
            /// <summary>
            /// 打开ID
            /// </summary>
            OpenId = 165,
            /// <summary>
            /// 关闭ID
            /// </summary>
            CloseId = 166,
        }

        /// <summary>
        /// 定距卡命令
        /// </summary>
        public enum DCommands
        {
            Default = 0,
        }

        /// <summary>
        /// IC 卡命令
        /// </summary>
        public enum ICommands
        {
            /// <summary>
            /// 读取IC卡
            /// </summary>
            Read = 9,
            /// <summary>
            /// IC卡加密
            /// </summary>
            EntryptIcCard = 204,
            /// <summary>
            /// IC设备加密
            /// </summary>
            EntryptIcDevice = 221,
        }

        /// <summary>
        /// 主机 命令
        /// </summary>
        public enum HCommands
        {
            /// <summary>
            /// 搜索主机
            /// </summary>
            Search = 1,

            /// <summary>
            /// 设置通道主机密码
            /// </summary>
            Password = 1,

            /// <summary>
            /// 修改主机时间
            /// </summary>
            Time = 17,

            /// <summary>
            /// 读卡数据
            /// </summary>
            ReadData = 64,

            /// <summary>
            /// 修改主机序列号
            /// </summary>
            NumberModify = 160,
        }

        /// <summary>
        /// 人员通道命令
        /// </summary>
        public enum PCommands
        {
            Default = 1
        }

        /// <summary>
        /// 模块命令
        /// </summary>
        public enum MCommands
        { 
            /// <summary>
            /// 设置模块
            /// </summary>
            SetModule = 208,
            /// <summary>
            /// 测试通信
            /// </summary>
            TestCommunication = 9,
        }

        /// <summary>
        /// 出入口
        /// </summary>
        public enum Entrances
        {
            /// <summary>
            /// 入口
            /// </summary>
            Enter = 48,
            /// <summary>
            /// 出口
            /// </summary>
            Exit = 49
        }

        /// <summary>
        /// 定距卡类型
        /// </summary>
        public enum CardTypes
        {
            /// <summary>
            /// 车牌号码
            /// </summary>
            Card0 = -1,
            ///<summary>
            ///   单卡或合一卡
            ///</summary>
            Card1 = 0,
            /// <summary>
            /// 人卡
            /// </summary>
            Card2,
            ///<sumary>
            /// 车牌识别卡
            ///</sumary>
            Card3,
            /// <summary>
            /// 车卡
            /// </summary>
            Card4,
            /// <summary>
            /// 密码错误
            /// </summary>
            PwdError,
            /// <summary>
            /// 挂失卡
            /// </summary>
            Loss
        }

        /// <summary>
        /// 定距卡操作辅助命令
        /// </summary>
        public enum AuxiliaryCommands
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success = 0,
            /// <summary>
            /// 格式出错
            /// </summary>
            FormatError = 1,
            /// <summary>
            /// 读写异常
            /// </summary>
            Abnormal = 7,
            /// <summary>
            /// 没有卡片或者是操作完成
            /// </summary>
            End = 8,
        }
    }
}
