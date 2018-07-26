using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;

namespace Bll
{
    public class DistanceIssue
    {
        public static byte[] Issue(CardInfo info)
        {
            int typeByte = SetTypeByte(info);
            int functionByte = SetFunctionByte(info);

            StringBuilder sb = new StringBuilder($"{functionByte:X2}{info.CardCount:X4}");
            switch ((PortEnums.CardTypes)info.CardType)
            {
                case PortEnums.CardTypes.Card1:
                    sb.Append($"{info.CardTime:yyMMdd}");
                    sb.Append($"{info.CardPartition:X4}");
                    break;
                case PortEnums.CardTypes.Card2:
                    if (info.ViceCardInfos != null)
                    {
                        foreach (CardInfo item in info.ViceCardInfos)
                        {
                            sb.Append($"{item.CardTime:yyMMdd}");
                            sb.Append($"{item.CardPartition:X4}");
                            sb.Append($"{item.CardCount:X4}");
                            sb.Append($"{item.CardNumber}");
                        }
                    }
                    else
                    {
                        sb.Append("FFFFFFFFFFFFFFFFFFFFFFFFFF");
                    }
                    break;
                case PortEnums.CardTypes.Card3:
                    if (info.ViceCardInfos != null)
                    {
                        foreach (CardInfo item in info.ViceCardInfos)
                        {
                            sb.Append($"{item.CardTime:yyMmdd}");
                            sb.Append($"{item.CardPartition:X4}");
                            sb.Append($"{PlateNumberToHex(item.CardNumber)}");
                        }
                    }
                    else
                    {
                        sb.Append("FFFFFFFFFFFFFFFFFFFFFFFFFF");
                    }
                    break;
                case PortEnums.CardTypes.Card4:
                    sb = new StringBuilder($"{functionByte:X2}");
                    break;
            }
            return PortAgreement.WriteACard(info.CardNumber, typeByte, 0, $"{sb.ToString()}");
        }

        private static string PlateNumberToHex(string platenumber)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            platenumber = platenumber.PadRight(8, '~');
            if (platenumber[0] == 'W' && platenumber[1] == 'J')
            {
                if (platenumber.Length != 7)
                {
                    sb.Append($"{36:X2}");
                    index = 2;
                }
            }
            for (int i = index; i < platenumber.Length; i++)
            {
                string strChar = platenumber[i].ToString();
                if (CRegex.IsChinese(strChar))
                {
                    int province = (int)Enum.Parse(typeof(PlateProvinces.Provinces), strChar);
                    sb.Append($"{province:X2}");
                }
                else
                {
                    sb.Append($"{Encoding.ASCII.GetBytes(strChar)[0]:X2}");
                }
            }
            return sb.ToString();
        }

        public static int SetCount(int count)
        {
            count += 1;
            if (count > 65500)
            {
                count = 10;
            }
            return count;
        }

        private static int SetFunctionByte(CardInfo info)
        {
            int functionByte = 0;
            if (info.CardReportLoss == 1)
            {
                info.CardReportLoss = 0;//解除挂
                info.Synchronous = info.Synchronous == 0 ? 1 : 0;
            }
            functionByte = Utility.SetIntegeSomeBit(functionByte, 7, info.CardReportLoss == 1);
            functionByte = Utility.SetIntegeSomeBit(functionByte, 6, info.Synchronous == 1);
            bool fifthPlace = false;
            bool firstOne = false;
            switch ((PortEnums.CardTypes)info.CardType)
            {
                case PortEnums.CardTypes.Card1://单卡 或 合一卡
                    fifthPlace = true;
                    break;
                //case PortEnums.CardTypes.Card2://人卡 或 组合卡

                //break;
                case PortEnums.CardTypes.Card3://车牌识别卡
                    firstOne = true;
                    break;
            }
            functionByte = Utility.SetIntegeSomeBit(functionByte, 5, fifthPlace);
            functionByte = Utility.SetIntegeSomeBit(functionByte, 1, firstOne);
            functionByte = Utility.SetIntegeSomeBit(functionByte, 4, info.ParkingRestrictions == 1);
            if (info.ViceCardInfos != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    int value = Utility.GetIntegerSomeBit(info.ViceCardInfos.Count, i);
                    functionByte = Utility.SetIntegeSomeBit(functionByte, 3 - i, value == 1);
                }
            }
            functionByte = Utility.SetIntegeSomeBit(functionByte, 0, info.InOutState == 1);
            return functionByte;
        }

        private static int SetTypeByte(CardInfo info)
        {
            int typeByte = 2; //0 写入数据  1 写入类型 2 定入数据和类型
            if (info.CardLock == 1)
            {
                info.CardLock = 0;//解锁
            }
            typeByte = Utility.SetIntegeSomeBit(typeByte, 7, info.CardLock == 1);
            if (info.CardDistance == 0)
            {
                typeByte = Utility.SetIntegeSomeBit(typeByte, 6, false);
            }
            else
            {
                typeByte = Utility.SetIntegeSomeBit(typeByte, 6, true);

                for (int i = 0; i < 2; i++)
                {
                    int distance = Utility.GetIntegerSomeBit(info.CardDistance - 1, i);
                    typeByte = Utility.SetIntegeSomeBit(typeByte, 4 + i, distance != 0);
                }
            }
            return typeByte;
        }
    }
}
