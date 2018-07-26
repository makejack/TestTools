using System;
using System.Globalization;
using System.Collections.Generic;
using Model;
using Dal;
using System.Text;
using System.Security.Cryptography;
using SharpConfig;

namespace Bll.Management
{
    public class UserManager
    {
        public static List<UserInfo> UserInfos { get; set; }

        public static void CreateNumberFile(int index, string folder)
        {
            UserInfo info = UserInfos[index];
            folder += $"//{info.UserName}.cfg";
            using (Rijndael rijndael = Rijndael.Create())
            {
                string strKey = Encoding.UTF8.GetString(rijndael.Key);
                string strIv = Encoding.UTF8.GetString(rijndael.IV);
                string aesClientNumber = Utility.AesStr(info.UserNumber.ToString(), strKey, strIv);

                Configuration config = new Configuration();
                config["General"]["ClientNumber"].StringValue = aesClientNumber;
                config["General"]["Key"].StringValue = Utility.Base64(strKey);
                config["General"]["Iv"].StringValue = Utility.Base64(strIv);
                config.SaveToFile(folder);

                Utility.OpenWindowDirectory(folder);
            }
        }

        public static int GetCount(string strContent)
        {
            return Dal_UserInfo.GetCount(strContent);
        }

        public static List<UserInfo> GetInfos(string strContent, int page, int count)
        {
            StringBuilder strWhere = new StringBuilder();
            object param = null;
            if (!string.IsNullOrEmpty(strContent))
            {
                strWhere.Append(" and UserName like @UserName ");
                param = new { UserName = $"%{strContent}%" };
            }
            strWhere.Append($" order by Id desc limit {count} offset {page * count} ");

            UserInfos = Dal_UserInfo.GetInfos(strWhere.ToString(), param);
            return UserInfos;
        }

        public static List<UserInfo> GetInfos()
        {
            List<UserInfo> infos = Dal_UserInfo.GetInfos(string.Empty, null);
            return infos;
        }

        public static UserInfo Add(string json)
        {
            UserInfo info = Utility.JsonDeserializeBySingleData<UserInfo>(json);
            info.RecordTime = DateTime.Now;
            info.Id = Dal_UserInfo.Add(info);
            if (UserInfos == null)
            {
                UserInfos = new List<UserInfo>();
            }
            UserInfos.Insert(0, info);
            return info;
        }

        public static UserInfo Update(string json, int index)
        {
            DateTime time = UserInfos[index].RecordTime;
            UserInfo updateInfo = Utility.JsonDeserializeBySingleData<UserInfo>(json);
            updateInfo.RecordTime = time;
            Dal_UserInfo.Update(updateInfo);
            UserInfos[index] = updateInfo;
            return updateInfo;
        }

        public static void Del(int index)
        {
            UserInfo info = UserInfos[index];
            Dal_UserInfo.Del(info.Id);
            UserInfos.RemoveAt(index);
            LimitManager.Add(info.UserNumber);
        }
    }
}