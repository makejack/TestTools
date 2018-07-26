using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.SerialPortDataModel;

namespace Bll
{
    /// <summary>
    /// 定距卡挂失
    /// </summary>
    public class DistanceLoss
    {
        public static byte[] Lose(List<CardInfo> cards)
        {
            StringBuilder sb = new StringBuilder();
            int dataType = 16777215;
            int cardType = 1;
            int index = 23;
            foreach (CardInfo item in cards)
            {
                for (int i = 0; i < 2; i++)
                {
                    int typeBinary = Utility.GetIntegerSomeBit(cardType, i);
                    dataType = Utility.SetIntegeSomeBit(dataType, index - i, typeBinary == 1);
                }
                index -= 2;

                if (item.CardReportLoss == 0)
                {
                    item.CardReportLoss = 0;
                    item.Synchronous = item.Synchronous == 0 ? 1 : 0;
                }
                int functionByte = 255;
                Utility.SetIntegeSomeBit(functionByte, 7, true);
                Utility.SetIntegeSomeBit(functionByte, 6, item.Synchronous == 1);

                sb.Append(item.CardNumber);
                sb.Append($"{functionByte:X2}");
                sb.Append($"{item.CardTime.AddMonths(1):yyMM}");
            }
            return PortAgreement.WriteACard("797979", 0, $"{cards.Count:X2}{dataType:X6}{sb.ToString()}");
        }

    }
}
