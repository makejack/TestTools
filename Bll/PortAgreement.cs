using System;
using System.Collections.Generic;
using System.Text;
using Bll;

namespace Bll
{
    public class PortAgreement
    {
        /// <summary>
        /// 发行器加密协议
        /// </summary>
        /// <param name="clientNumber">客户编号 默认9887</param>
        /// <param name="pwd">密码 6位数字</param>
        /// <returns></returns>
        public static byte[] DistanceDeviceEncryption(string clientNumber, string pwd)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("A00001000000{0}{1}", clientNumber, pwd));
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// 定距卡加密协议
        /// </summary>
        /// <param name="pwd">密码 6位数字</param>
        /// <returns></returns>
        public static byte[] DistanceCardEncryption(string pwd)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("0D0000000000010000{0}", pwd));
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// 获取读取所有卡协议
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] ReadAllCard(int count)
        {
            if (count < 0 || count > 64)
            {
                throw new IndexOutOfRangeException();
            }
            byte[] by = Encoding.ASCII.GetBytes($"0A80000000000100{count:X2}");
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// 获取读取所有卡协议
        /// </summary>
        /// <returns></returns>
        public static byte[] ReadAllCard()
        {
            return ReadAllCard(3);
        }

        /// <summary>
        /// 获取读取某张卡协议
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <returns></returns>
        public static byte[] ReadACard(string cardnumber)
        {
            return ReadACard(cardnumber, 3);
        }

        /// <summary>
        /// 获取读取某张卡协议
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <param name="len">读取的数据长度</param>
        /// <returns></returns>
        public static byte[] ReadACard(string cardnumber, int len)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("1A00{0}000100{1:X2}", cardnumber, len));
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// 写入某张卡的数据
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <param name="start">写入的位置</param>
        /// <param name="data">写入的数据</param>
        /// <returns></returns>
        public static byte[] WriteACard(string cardnumber, int start, string data)
        {
            return WriteACard(cardnumber, 0, start, data);
        }

        /// <summary>
        /// 写入某张卡的数据
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <param name="type">类型 0 - 写入数据 1 - 写入类型 2 - 写入数据和类型</param>
        /// <returns></returns>
        public static byte[] WriteACard(string cardnumber, int type)
        {
            return WriteACard(cardnumber, type, 0, "00");
        }

        /// <summary>
        /// 写入某张卡的数据
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <param name="type">类型 0 - 写入数据 1 - 写入类型 2 - 写入数据和类型</param>
        /// <param name="start">写入的位置</param>
        /// <param name="data">写入的数据</param>
        /// <returns></returns>
        public static byte[] WriteACard(string cardnumber, int type, int start, string data)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("1B00{0}{1:X2}01{2:X2}{3:X2}{4}", cardnumber, type, start, data.Length / 2, data));
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// 搜索定距卡
        /// </summary>
        /// <param name="cardnumber">卡号</param>
        /// <returns></returns>
        public static byte[] SearchACard(string cardnumber)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("1500{0}00000000", cardnumber));
            return CombinatorialProtocol(2, 3, 65, 0, 0, by);
        }

        /// <summary>
        /// IC卡设备加密
        /// </summary>
        /// <param name="pwd">密码 （0-F） 8位</param>
        /// <returns></returns>
        public static byte[] IcDeviceEncryption(string pwd)
        {
            byte[] by = Encoding.ASCII.GetBytes("FFFF" + pwd);
            return CombinatorialProtocol(2, 3, 66, 0, 221, by);
        }

        /// <summary>
        /// IC卡密码
        /// </summary>
        /// <param name="pwd">密码 （0-F) 8位</param>
        /// <returns></returns>
        public static byte[] IcCardEncryption(string pwd)
        {
            byte[] by = Encoding.ASCII.GetBytes("FFFF" + pwd);
            return CombinatorialProtocol(2, 3, 66, 0, 204, by);
        }

        /// <summary>
        /// 读取IC卡
        /// </summary>
        /// <returns></returns>
        public static byte[] ReadIc()
        {
            return CombinatorialProtocol(2, 3, 66, 0, 9);
        }

        /// <summary>
        /// 写入IC卡内容
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static byte[] WriteIc(string content)
        {
            return CombinatorialProtocol(2, 3, 66, 0, 2, content);
        }

        /// <summary>
        /// 打开模块
        /// </summary>
        /// <returns></returns>
        public static byte[] OpenModular()
        {
            return CombinatorialProtocol(2, 3, 67, 0, 209);
        }

        /// <summary>
        /// 关闭模块
        /// </summary>
        /// <returns></returns>
        public static byte[] CloseModular()
        {
            return CombinatorialProtocol(2, 3, 67, 0, 210);
        }

        /// <summary>
        /// 设置模块发送ID
        /// </summary>
        /// <returns></returns>
        public static byte[] SetModuleTid(int id)
        {
            string strTid = $"AT+TID=01{id.ToString().PadLeft(8, '0')}";
            return SetModular(strTid);
        }

        /// <summary>
        /// 设置模块接收ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static byte[] SetModuleRid(int id)
        {
            string strRid = $"AT+RID=01{id.ToString().PadLeft(8, '0')}";
            return SetModular(strRid);
        }

        /// <summary>
        /// 设置模块频率
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public static byte[] SetModuleFrequency(int frequency)
        {
            frequency = 127 - (frequency * 2 - 1);
            string strFrequency = $"AT+FREQ={frequency:X2}";
            return SetModular(strFrequency);
        }

        /// <summary>
        /// 设置模块回传功能
        /// </summary>
        /// <param name="state">1 打开 0 关闭</param>
        /// <returns></returns>
        public static byte[] SetModuleComesBack(int state)
        {
            string strBack = $"AT+BACK={state}";
            return SetModular(strBack);
        }

        /// <summary>
        /// 设置模块
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] SetModular(string content)
        {
            return CombinatorialProtocol(2, 3, 67, 0, 208, content);
        }

        /// <summary>
        /// 设置模块编号
        /// </summary>
        /// <param name="strnumber"></param>
        /// <returns></returns>
        public static byte[] SetModularNumber(string strnumber)
        {
            byte[] by = Encoding.ASCII.GetBytes(string.Format("1234{0}", strnumber.PadLeft(8, '0')));
            return CombinatorialProtocol(2, 3, 50, 1, 1, by);
        }

        /// <summary>
        /// 开门并播报月租剩余天数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] OpenTheDoorAndVoice(OpenTheDoorParam param)
        {
            List<byte> bylist = new List<byte>();
            bylist.AddRange(LincensePlateToByte(param.LicensePlateNumber));
            bylist.Add((byte)(48 + param.LicensePlateColor));
            bylist.AddRange(Encoding.Default.GetBytes(string.Format("{0:yyMMddHHmmss}{1:X2}", param.Time, param.Day)));
            return CombinatorialProtocol(2, 3, 67, param.DeviceAddress, 16, bylist.ToArray());
        }

        /// <summary>
        /// 无线端打开播放语音
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] OpenTheDoorAndVoice(OpenTheDoorParam2 param)
        {
            List<byte> bylist = new List<byte>();
            bylist.AddRange(Encoding.Default.GetBytes(param.IcCardNumber.PadRight(8, '0')));
            bylist.AddRange(LincensePlateToByte(param.LicensePlateNumber));
            bylist.Add((byte)(48 + param.LicensePlateColor));
            bylist.AddRange(Encoding.Default.GetBytes(string.Format("{0:yyMMddHHmmss}", param.Time)));
            return CombinatorialProtocol(2, 3, 67, param.DeviceAddress, 17, bylist.ToArray());
        }

        /// <summary>
        /// 开门不播报语言
        /// </summary>
        /// <param name="deviceaddress"></param>
        /// <returns></returns>
        public static byte[] OpenTheDoorNotVoice(int deviceaddress)
        {
            return CombinatorialProtocol(2, 3, 67, deviceaddress, 19, "123456");
        }

        /// <summary>
        /// 收费语音
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static byte[] ChargesVoice(VoiceParam param)
        {
            List<byte> bylist = new List<byte>();
            bylist.AddRange(LincensePlateToByte(param.LicensePlateNumber));
            bylist.Add((byte)param.LicensePlateColor);
            bylist.AddRange(Encoding.Default.GetBytes(string.Format("{0:X6}{1:X4}", param.Minute, (int)param.Money)));
            return CombinatorialProtocol(2, 3, 67, param.DeviceAddress, 18, bylist.ToArray());
        }

        /// <summary>
        /// 播报语音
        /// </summary>
        /// <returns></returns>
        public static byte[] Voice(VoiceParam2 param)
        {
            List<byte> byList = new List<byte>();
            byList.AddRange(Encoding.ASCII.GetBytes(param.CardNumber));
            byList.AddRange(Utility.IntToAscii((int)param.VoiceNumber));
            byList.AddRange(Encoding.Default.GetBytes(param.VoiceData.PadRight(6, '0')));
            return CombinatorialProtocol(2, 3, 67, param.DeviceAddress, 20, byList.ToArray());
        }

        private static byte[] LincensePlateToByte(string licenseplate)
        {
            byte[] by = new byte[9];
            Encoding.Default.GetBytes(licenseplate, 0, licenseplate.Length, by, 0);
            if (licenseplate.Length == 7)
            {
                string charlicenseplate = licenseplate[6].ToString();
                if (CRegex.IsChinese(charlicenseplate))
                {
                    by[7] = (byte)(PlateProvinces.Provinces)Enum.Parse(typeof(PlateProvinces.Provinces), charlicenseplate);
                }
                by[8] = 126;
            }
            //by[9] = 48;
            return by;
        }

        /// <summary>
        /// 设置客户编号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] SetPersonnelHostNumber(int number)
        {
            byte[] by = Encoding.Default.GetBytes("123456" + number.ToString().PadLeft(4, '0'));
            return CombinatorialProtocol(2, 3, 51, 1, 160, by);
        }

        /// <summary>
        /// 设置主机时间
        /// </summary>
        /// <returns></returns>
        public static byte[] SetPersonnelHostTime()
        {
            byte[] by = Encoding.Default.GetBytes($"ABABAB{DateTime.Now:yyMMddHHmmss}");
            return CombinatorialProtocol(2, 3, 51, 1, 17, by);
        }

        /// <summary>
        /// 设置人员通道主机密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static byte[] SetPersonnelHostPassword(string oldpwd, string pwd)
        {
            byte[] by = Encoding.Default.GetBytes($"ABCDEF{oldpwd}{pwd}");
            return CombinatorialProtocol(2, 3, 51, 1, 1, by);
        }

        /// <summary>
        /// 搜索主机
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] SearchHost(int number)
        {
            return CombinatorialProtocol(2, 3, 49, number, 0);
        }

        /// <summary>
        /// 获取读头读卡数据（无数据时不回复）
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] HostReadCardDataNotResult(int number)
        {
            return CombinatorialProtocol(2, 3, 48, number, 0);
        }

        /// <summary>
        /// 获取读头读卡数据 （无数据时有回复)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] HostReadCardData(int number)
        {
            return CombinatorialProtocol(2, 3, 49, number, 0);
        }

        /// <summary>
        /// 获取人员通道主机读卡数据
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] CorridorReadCardData(int address)
        {
            return CombinatorialProtocol(9, 13, -1, address, 0);
        }

        /// <summary>
        /// 测试人员通道通信
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte[] TestPort(int address)
        {
            return CombinatorialProtocol(2, 3, -1, address, -1, "000");
        }

        private static byte[] CombinatorialProtocol(int head, int end, int functionAddress, int deviceAddress, int command)
        {
            return CombinatorialProtocol(head, end, functionAddress, deviceAddress, command, string.Empty);
        }

        private static byte[] CombinatorialProtocol(int head, int end, int functionAddress, int deviceAddress, int command, string data)
        {
            byte[] by = null;
            if (!string.IsNullOrEmpty(data))
            {
                by = Encoding.ASCII.GetBytes(data);
            }
            return CombinatorialProtocol(head, end, functionAddress, deviceAddress, command, by);
        }

        private static byte[] CombinatorialProtocol(int head, int end, int functionAddress, int deviceAddress, int command, byte[] data)
        {
            List<byte> byList = new List<byte>
            {
                (byte)head
            };
            if (functionAddress > -1)
            {
                byList.Add((byte)functionAddress);
            }
            byList.AddRange(Utility.IntToAscii(deviceAddress));
            if (command > -1)
            {
                byList.AddRange(Utility.IntToAscii(command));
            }
            if (data != null)
            {
                byList.AddRange(data);
            }
            int xor = Utility.Xor(byList);
            byList.AddRange(Utility.IntToAscii(xor));
            byList.Add((byte)end);
            return byList.ToArray();
        }
    }

    public class OpenTheDoorParam
    {
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber;
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public LicensePlateColors LicensePlateColor;
        /// <summary>
        /// 停车时间
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// 剩余天数
        /// </summary>
        public int Day;
        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddress;
    }

    public class OpenTheDoorParam2
    {
        /// <summary>
        /// IC卡编号
        /// </summary>
        public string IcCardNumber;
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber;
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public LicensePlateColors LicensePlateColor;
        /// <summary>
        /// 停车时间
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddress;
    }

    public class VoiceParam
    {
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber;
        /// <summary>
        /// 车牌颜色 
        /// </summary>
        public LicensePlateColors LicensePlateColor;
        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddress;
        /// <summary>
        /// 分钟
        /// </summary>
        public int Minute;
        /// <summary>
        /// 金额
        /// </summary>
        public double Money;
    }

    public enum LicensePlateColors
    {
        Blue = 0,
        Yellow = 1,
        White = 2,
        Black = 3,
        Green = 4
    }

    public class VoiceParam2
    {
        /// <summary>
        /// 设备地址
        /// </summary>
        public int DeviceAddress;
        /// <summary>
        /// 卡片编号 
        /// </summary>
        public string CardNumber;
        /// <summary>
        /// 语音编号 
        /// </summary>
        public VoiceNumbers VoiceNumber;
        /// <summary>
        /// 语音数据
        /// </summary>
        public string VoiceData;
    }

    public enum VoiceNumbers
    {
        /// <summary>
        /// 欢迎光临 此卡有效期XXXX年XX月XX日
        /// </summary>
        EnterTime = 34,
        /// <summary>
        /// 一路顺风 此卡有效期XXXX年XX月XX日
        /// </summary>
        ExitTime = 17,
        /// <summary>
        /// 欢迎光临 此卡剩余XX天
        /// </summary>
        EnterDay = 25,
        /// <summary>
        /// 一 路顺风 此卡剩余XX天
        /// </summary>
        ExitDay = 26,
        /// <summary>
        /// 欢迎光临
        /// </summary>
        Enter = 100,
        /// <summary>
        /// 一路顺风
        /// </summary>
        Exit = 101,
        /// <summary>
        /// 已经入场
        /// </summary>
        AlreadyEnter = 32,
        /// <summary>
        /// 已经出场
        /// </summary>
        AlreadyExit = 33,
        /// <summary>
        /// 此卡过期
        /// </summary>
        Expire = 31,
        /// <summary>
        /// 此卡无效
        /// </summary>
        Invalid = 29
    }

}
