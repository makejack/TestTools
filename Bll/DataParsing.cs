using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;

namespace Bll
{
    public class DataParsing
    {
        public static PortDataParameter Parsing(byte[] by)
        {
            PortDataParameter param = new PortDataParameter()
            {
                Header = by[0],
                End = by[by.Length - 1],
            };
            if (param.Header == 2)
            {
                param.FunctionAddress = (PortEnums.DealFunctions)by[1];
                param.DeviceAddress = Utility.HexToInt(by[2], by[3]);
                int command = Utility.HexToInt(by[4], by[5]);
                switch (param.FunctionAddress)
                {
                    case PortEnums.DealFunctions.Distance:
                        PortEnums.DistanceCommands dCommand = (PortEnums.DistanceCommands)Utility.HexToInt(by[6], by[7]);
                        param.Command.DCommand = (PortEnums.DCommands)command;
                        switch (dCommand)
                        {
                            case PortEnums.DistanceCommands.WriteAllCard:
                            case PortEnums.DistanceCommands.WriteACard:
                            case PortEnums.DistanceCommands.ReadAllCard:
                            case PortEnums.DistanceCommands.ReadACard:
                            case PortEnums.DistanceCommands.ModifyAllCardPwd:
                                DistanceCardParameter distanceParam = new DistanceCardParameter
                                {
                                    Command = dCommand,
                                    AuxiliaryCommand = (PortEnums.AuxiliaryCommands)Utility.HexToInt(by[8], by[9])
                                };
                                if (distanceParam.AuxiliaryCommand == PortEnums.AuxiliaryCommands.Success)
                                {
                                    distanceParam.CardNumber = Encoding.ASCII.GetString(by, 10, 6);
                                    distanceParam.CardTypeValue = Utility.HexToInt(by[16], by[17]);
                                    distanceParam.CardTypeParameter = ParsingDistanceCardType(distanceParam.CardTypeValue);
                                    distanceParam.Area = Utility.HexToInt(by[18], by[19]);
                                    distanceParam.Start = Utility.HexToInt(by[20], by[21]);
                                    distanceParam.Len = Utility.HexToInt(by[22], by[23]);
                                    if (((int)distanceParam.CardTypeParameter.CardType) < 2)
                                    {
                                        if (dCommand == PortEnums.DistanceCommands.ReadACard || dCommand == PortEnums.DistanceCommands.ReadAllCard)
                                        {
                                            if (distanceParam.Len >= 1)
                                            {
                                                distanceParam.FunctionByteValue = Utility.HexToInt(by[24], by[25]);
                                                distanceParam.FunctionByteParam = ParsingDistanceFunctionByte(distanceParam.FunctionByteValue);
                                            }
                                            if (distanceParam.Len >= 3)
                                            {
                                                distanceParam.Count = Utility.HexToInt(by[26], by[27], by[28], by[29]);
                                            }
                                        }
                                    }
                                }
                                param.DistanceCardParam = distanceParam;
                                break;
                            case PortEnums.DistanceCommands.InitializeDevice:
                                DistanceDeviceParameter deviceParam = new DistanceDeviceParameter()
                                {
                                    Command = dCommand,
                                    AuxiliaryCommand = (PortEnums.AuxiliaryCommands)Utility.HexToInt(by[8], by[9]),
                                };
                                param.DistanceDeviceParam = deviceParam;
                                break;
                        }

                        break;
                    case PortEnums.DealFunctions.Ic:
                        param.Command.ICommand = (PortEnums.ICommands)command;
                        IcParameter icParam = new IcParameter();
                        switch (param.Command.ICommand)
                        {
                            case PortEnums.ICommands.Read:
                                string icNumber = Encoding.ASCII.GetString(by, 6, 8);
                                icParam.IcNumber = icNumber;
                                string licensePlate = Encoding.Default.GetString(by, 14, 10);
                                icParam.LicensePlate = licensePlate;
                                string strTime = Encoding.Default.GetString(by, 24, 12);
                                DateTime time = DateTime.Now;
                                if (CRegex.IsTime(strTime))
                                {
                                    time = DateTime.ParseExact(strTime, "yyMMddHHmmss", System.Globalization.CultureInfo.InstalledUICulture);
                                }
                                icParam.Time = time;
                                break;
                            case PortEnums.ICommands.EntryptIcCard:
                            case PortEnums.ICommands.EntryptIcDevice:
                                icParam.Result = Utility.HexToInt(by[6], by[7]) == 0;
                                break;
                        }
                        param.IcParam = icParam;
                        break;
                    case PortEnums.DealFunctions.ModularAndVoice:
                        param.Command.MCommand = (PortEnums.MCommands)command;
                        param.ModuleParam = new ModuleParameter();
                        switch (param.Command.MCommand)
                        {
                            case PortEnums.MCommands.SetModule:
                                param.ModuleParam.Result = by[6] == 89;
                                break;
                            case PortEnums.MCommands.TestCommunication:
                                param.ModuleParam.Result = by[6] == 83;
                                break;
                        }
                        break;
                    case PortEnums.DealFunctions.HostResult:
                        param.Command.HCommand = (PortEnums.HCommands)command;
                        param.HostParam = new HostParameter();
                        switch (param.Command.HCommand)
                        {
                            case PortEnums.HCommands.Search:
                                break;
                           
                            case PortEnums.HCommands.ReadData:
                                break;
                        }
                        break;
                    case PortEnums.DealFunctions.HostNoResult:
                    case PortEnums.DealFunctions.ProsennelHost:
                        param.Command.HCommand = (PortEnums.HCommands)command;
                        param.HostParam = new HostParameter();
                        switch (param.Command.HCommand)
                        {
                            case PortEnums.HCommands.Password:
                            case PortEnums.HCommands.Time:
                                param.HostParam.Result = by[12] == 49;
                                break;
                            case PortEnums.HCommands.NumberModify:
                                param.HostParam.Result = by[6] == 49;
                                break;
                        }
                        break;
                }
            }
            else if (param.Header == 10)
            {
                param.DeviceAddress = Utility.HexToInt(by[1], by[2]);
                param.Command.PCommand = (PortEnums.PCommands)Utility.HexToInt(by[3], by[4]);
                switch (param.Command.PCommand)
                {
                    case PortEnums.PCommands.Default:
                        string strTime = Encoding.Default.GetString(by, 15, 6);
                        DateTime cardTime = DateTime.MinValue;
                        if (CRegex.IsTime(strTime))
                        {
                            cardTime = DateTime.ParseExact(strTime, "yyMMdd", System.Globalization.CultureInfo.InstalledUICulture);
                        }
                        strTime = Encoding.Default.GetString(by, 21, 6);
                        DateTime systemTime = DateTime.MinValue;
                        if (CRegex.IsTime(strTime))
                        {
                            systemTime = DateTime.ParseExact(strTime, "yyMMdd", System.Globalization.CultureInfo.InstalledUICulture);
                        }
                        param.PersonnelParam = new PersonnelParameter()
                        {
                            CorridorNumber = Utility.HexToInt(by[5], by[6]),
                            HostNumber = Utility.HexToInt(by[7], by[8]),
                            CardNumber = Encoding.ASCII.GetString(by, 9, 6),
                            CardTime = cardTime,
                            SystemTime = systemTime,
                            OpenTheDoorState = Utility.HexToInt(by[27], by[28])
                        };
                        break;
                }
            }
            return param;
        }

        private static CardTypeParameter ParsingDistanceCardType(int value)
        {
            CardTypeParameter param = new CardTypeParameter();
            int cardtype = Utility.GetBitToInt(value, 0, 3);
            if (cardtype == 0 || cardtype == 4) //扩展功能
            {
                if (cardtype != 4)
                {
                    cardtype = Utility.GetBitToInt(value, 4, 4);
                    if (cardtype == 7)
                    {
                        param.CardType = PortEnums.CardTypes.Loss; //挂失
                    }
                    else if (cardtype == 14)
                    {
                        param.CardType = PortEnums.CardTypes.PwdError;//密码错误
                    }
                }
                else
                {
                    param.CardType = PortEnums.CardTypes.Loss;
                }
            }
            else
            {
                switch (cardtype)
                {
                    case 1:
                    case 5:
                        param.CardType = PortEnums.CardTypes.Card1;
                        break;
                    case 2:
                    case 6:
                        param.CardType = PortEnums.CardTypes.Card2;
                        break;
                    case 3:
                    case 7:
                        param.CardType = PortEnums.CardTypes.Card4;
                        break;
                }
                param.CardLock = Utility.GetIntegerSomeBit(value, 7);
                param.DistanceSate = Utility.GetIntegerSomeBit(value, 6);
                if (param.DistanceSate == 1)
                {
                    param.Distance = Utility.GetBitToInt(value, 4, 2) + 1;
                }
                param.Electricity = Utility.GetIntegerSomeBit(value, 3);
            }
            return param;
        }

        private static FunctionByteParameter ParsingDistanceFunctionByte(int value)
        {
            FunctionByteParameter param = new FunctionByteParameter()
            {
                Loss = Utility.GetIntegerSomeBit(value, 7),
                Synchronous = Utility.GetIntegerSomeBit(value, 6),
                ParkingRestrictions = Utility.GetIntegerSomeBit(value, 4),
                ViceCardCount = Utility.GetBitToInt(value, 2, 2),
                InOutState = Utility.GetIntegerSomeBit(value, 0),
            };
            int type = Utility.GetIntegerSomeBit(value, 5);
            if (type == 0)
            {
                type = Utility.GetIntegerSomeBit(value, 1);
                param.RegistrationType = (type == 1 ? PortEnums.CardTypes.Card3 : PortEnums.CardTypes.Card2);
            }
            else
            {
                param.RegistrationType = PortEnums.CardTypes.Card1;
            }
            return param;
        }

    }
}
