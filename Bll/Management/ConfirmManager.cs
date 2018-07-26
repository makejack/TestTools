using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Model;
using Dal;

namespace Bll.Management
{
    public class ConfirmManager
    {
        public static List<DeviceInfo> ConfirmInfos { get; set; }

        public static int GetCount()
        {
            return Dal_ConfirmInfo.GetInfos(string.Empty).Count();
        }

        public static List<DeviceInfo> GetInfos(int page, int count)
        {
            string strPage = $" order by Did desc limit {count} offset {page * count}";
            ConfirmInfos = Dal_ConfirmInfo.GetInfos(strPage);
            return ConfirmInfos;
        }

        public static int GetMaxId()
        {
            List<DeviceInfo> list = Dal_ConfirmInfo.GetInfos(string.Empty);
            int max = 0;
            if (list.Count > 0)
            {
                max = list.Max(e => e.Did);
            }
            return max;
        }

        public static DeviceInfo Add(string json)
        {
            DeviceInfo info = Utility.JsonDeserializeBySingleData<DeviceInfo>(json);
            info.Did = Dal_ConfirmInfo.Add(info);
            if (ConfirmInfos == null)
            {
                ConfirmInfos = new List<DeviceInfo>();
            }
            ConfirmInfos.Insert(0, info);
            return info;
        }

        public static DeviceInfo Update(string json)
        {
            DeviceInfo updateinfo = Utility.JsonDeserializeBySingleData<DeviceInfo>(json);
            Dal_ConfirmInfo.Update(updateinfo);
            return updateinfo;
        }

        public static int Del(int id)
        {
            return Dal_ConfirmInfo.Del(id);
        }
    }
}
