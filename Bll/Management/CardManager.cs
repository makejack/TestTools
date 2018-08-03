using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Dal;

namespace Bll.Management
{
    public class CardManager
    {
        public static List<CardInfo> CardInfos = new List<CardInfo>();

        public static CardInfo BatchParam = null;

        public static List<CardInfo> LossLists = null;

        public static int GetCount(string searchContent)
        {
            return Dal_CardInfo.GetCardCount(searchContent);
        }

        public static List<CardInfo> GetCardInfos(int state)
        {
            string where = $" and CardReportLoss = {state} and CardType >= 0 and CardType <= 2 ";
            return Dal_CardInfo.GetCardInfos(where, null);
        }

        public static List<CardInfo> GetCardInfos(string numbers)
        {
            string where = $" and CardNumber in ({numbers})";
            return Dal_CardInfo.GetCardInfos(where, null);
        }

        public static List<CardInfo> GetCardInfos(string searchContent, int page, int count)
        {
            StringBuilder strWhere = new StringBuilder(" and CardType != -1 ");
            object param = null;
            if (!string.IsNullOrEmpty(searchContent))
            {
                strWhere.Append(" and CardNumber like @content ");
                param = new { content = $"%{searchContent}%" };
            }
            strWhere.Append($" order by Cid desc limit {count} offset {page * count} ");
            CardInfos = Dal_CardInfo.GetCardInfos(strWhere.ToString(), param);
            return CardInfos;
        }

        public static List<CardInfo> GetViceCards(int index)
        {
            StringBuilder strWhere = new StringBuilder(" and CardType = 3 ");
            List<CardInfo> viceInfos = CardInfos[index].ViceCardInfos;
            if (viceInfos != null && viceInfos.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                int count = 1;
                foreach (CardInfo item in viceInfos)
                {
                    sb.Append($"'{item.CardNumber}'");
                    if (count < viceInfos.Count)
                    {
                        sb.Append(",");
                    }
                    count += 1;
                }
                strWhere.Append($" and CardNumber not in ({sb}) ");
            }

            List<CardInfo> viceNumbers = new List<CardInfo>();
            foreach (CardInfo item in CardInfos)
            {
                if (item.Id == 0 && item.CardType == 3)
                {
                    viceNumbers.Add(item);
                }
            }
            List<CardInfo> list = Dal_CardInfo.GetCardInfos(strWhere.ToString(), null);
            if (list != null && list.Count > 0)
            {
                viceNumbers.AddRange(list);
            }
            return viceNumbers;
        }

        public static List<CardInfo> GetViceCards(string[] strNumbers)
        {
            StringBuilder strWhere = new StringBuilder(" and CardType = 3 ");
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (string item in strNumbers)
            {
                sb.Append($"'{item}'");
                if (count < strNumbers.Length)
                {
                    sb.Append(",");
                }
                count += 1;
                strWhere.Append($" and CardNumber in ({sb}) ");
            }
            return Dal_CardInfo.GetCardInfos(strWhere.ToString(), null);
        }

        public static CardInfo GetLicensePlateInfo(string strNumber)
        {
            string strWhere = $" and CardType == -1 and CardNumber = @Number ";
            object objs = new { Number = strNumber };
            List<CardInfo> infos = Dal_CardInfo.GetCardInfos(strWhere, objs);
            if (infos.Count > 0)
            {
                return infos[0];
            }
            return null;
        }

        public static CardInfo GetCardInfo(int index)
        {
            CardInfo info = CardInfos[index];
            if (info.Id > 0 && info.ViceCardInfos == null)
            {
                string strWhere = " and Cid in (select Vid from BundledInfo where HostCardNumber = @Number ) ";
                object param = new { Number = info.CardNumber };
                List<CardInfo> viceCards = Dal_CardInfo.GetCardInfos(strWhere, param);
                if (viceCards.Count > 0)
                {
                    info.ViceCardInfos = viceCards;
                }
            }
            return info;
        }

        public static CardInfo GetCardInfo(string cardNumber)
        {
            string strWhere = " and CardNumber = @CardNumber ";
            object param = new { CardNumber = cardNumber };
            List<CardInfo> list = Dal_CardInfo.GetCardInfos(strWhere, param);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public static void Update(CardInfo info)
        {
            CardInfo[] infos = { info };
            Update(infos);
        }

        public static void Update(CardInfo[] infos)
        {
            Dal_CardInfo.Update(infos);
        }

        public static int Insert(CardInfo info)
        {
            return Dal_CardInfo.Insert(info);
        }

        public static int Insert(CardInfo[] infos)
        {
            return Dal_CardInfo.Insert(infos);
        }

        public static bool Delete()
        {
            int count = Dal_CardInfo.Delete();
            return count > 0;
        }

        public static bool Delete(int index)
        {
            CardInfo info = CardInfos[index];
            int count = Dal_CardInfo.Delete(info.CardNumber);
            if (count > 0)
            {
                CardInfos.RemoveAt(index);
            }
            return count > 0;
        }

        public static void DelViceCardInfos(int hostIndex)
        {
            CardInfo info = CardInfos[hostIndex];
            if (info.ViceCardInfos != null && info.ViceCardInfos.Count > 0)
            {
                foreach (var item in info.ViceCardInfos)
                {
                    BundledInfoManager.Delete(info.CardNumber, item.CardNumber);
                }
                info.ViceCardInfos = null;
            }
        }
    }
}
