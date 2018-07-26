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
        public static byte[] Issue(CardInfo info)
        {
            int typeByte = 2;
            StringBuilder sb = new StringBuilder();
            sb.Append($"{info.CardTime:yyMMdd}");
            sb.Append($"{info.CardPartition:X4}");
            return PortAgreement.WriteACard(info.CardNumber, typeByte, 0, sb.ToString());
        }
    }
}
