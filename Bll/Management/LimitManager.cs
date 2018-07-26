using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Dal;

namespace Bll.Management
{
    public class LimitManager
    {

        public static List<NumberLimit> NumberInfos { get; set; }

        public static List<NumberLimit> GetInfos()
        {
            NumberInfos = Dal_Limit.GetInfos();
            return NumberInfos;
        }

        public static bool Add(int number)
        {
            if (NumberInfos == null)
            {
                NumberInfos = new List<NumberLimit>();
            }
            int count = NumberInfos.Where(e => e.LimitNumber == number).Count();
            bool ret = count > 0;
            if (!ret)
            {
                NumberLimit info = new NumberLimit()
                {
                    LimitNumber = number,
                };
                info.Id = Dal_Limit.Add(info);
                NumberInfos.Add(info);
            }
            return ret;
        }

        public static int Update(NumberLimit info)
        {
            return Dal_Limit.Update(info);
        }

        public static void Del(int index)
        {
            NumberLimit info = NumberInfos[index];
            Dal_Limit.Del(info.Id);
            NumberInfos.RemoveAt(index);
        }
    }
}
