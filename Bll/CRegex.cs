using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace Bll
{
    public class CRegex
    {
        public static bool IsDecimal(object obj)
        {
            return obj != null && IsDecimal(obj.ToString());
        }

        public static bool IsDecimal(string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        public static bool IsHex(object obj)
        {
            return obj != null && IsHex(obj.ToString());
        }

        public static bool IsHex(string str)
        {
            return Regex.IsMatch(str, @"^\d[A-F][a-f]+$");
        }

        public static bool IsBinary(object obj)
        {
            return obj != null && IsBinary(obj.ToString());
        }

        public static bool IsBinary(string str)
        {
            return Regex.IsMatch(str, @"^[0-1]+$");
        }

        /// <summary>
        /// 验证时间
        /// </summary>
        /// <param name="str">yyMMddHHmmss</param>
        /// <returns></returns>
        public static bool IsTime(string str)
        {
            return Regex.IsMatch(str, @"^\d{2}(0[1-9]|1[0-2])(0[1-9]|1\d|2\d|3[0-1])(0[0-9]|1\d|2[0-3])([0-5]\d)([0-5]\d)$");
        }

        public static bool IsPlate(string str)
        {
            return Regex.IsMatch(str, @"^[\u4e00-\u9fa50-9a-zA-Z]{7,8}$");
        }

        public static bool IsChinese(string value)
        {
            return Regex.IsMatch(value, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 验证导出文件名称
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDeviceInfoFileName(string value)
        {
            return Regex.IsMatch(value, @"FORM(([A-F]+[0-9])|([0-9]+[A-F])|[A-F]{2}|\d{2})");
        }

        public static bool IsEnterExitRecordFileName(string filename)
        {
            return Regex.IsMatch(filename, @"^[0-9]{2}[月][0-9A-Fa-f]{4}$");
        }

        public static bool IsIpAddress(string ipaddress)
        {
            return Regex.IsMatch(ipaddress, "^(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|[1-9])\\.(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\.(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\.(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)$");
        }
    }
}