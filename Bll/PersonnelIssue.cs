using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Bll
{
    public class PersonnelIssue
    {
        public static byte[] Issue(CardInfo info, int datatype, string customdata)
        {
            int typeByte = 2;
            StringBuilder sb = new StringBuilder();
            if (datatype == 1)
            {
                sb.Append($"{info.CardTime:yyMMdd}");
                sb.Append($"{info.CardPartition:X4}");
            }
            else
            {
                sb.Append(customdata);
            }
            return PortAgreement.WriteACard(info.CardNumber, typeByte, 0, sb.ToString());
        }
    }
}
