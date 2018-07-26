using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Dal;

namespace Bll.Management
{
    public class BundledInfoManager
    {
        public static void Insert(BundledInfo[] infos)
        {
            Dal_BundledInfo.Insert(infos);
        }

        public static int Delete(int id)
        {
            return Dal_BundledInfo.Delete(id);
        }

        public static int Delete(string hostnumber, string vicenumber)
        {
            return Dal_BundledInfo.Delete(hostnumber, vicenumber);
        }

    }
}
