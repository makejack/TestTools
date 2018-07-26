using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;

namespace Bll
{
    public class Utility
    {
        #region 限制用户输入

        /// <summary>
        /// 输入数字
        /// </summary>
        /// <param name="keychar"></param>
        /// <returns>Ture 数字 False 不是数字</returns>
        public static bool InputNumber(char keychar)
        {
            return !((keychar >= 48 && keychar <= 57) || (keychar == (char)Keys.Back));
        }

        /// <summary>
        /// 输入十六进制数
        /// </summary>
        /// <param name="keychar"></param>
        /// <returns>True 十六进制数 False 不是十六进制数</returns>
        public static bool InputHex(char keychar)
        {
            return !(((keychar >= 48 && keychar <= 57) || (keychar >= 65 && keychar <= 70) || (keychar >= 97 && keychar <= 102)) || (keychar == (char)Keys.Back));
        }

        /// <summary>
        /// 输入英文
        /// </summary>
        /// <param name="keychar"></param>
        /// <returns>True 英文 False 不是英文</returns>
        public static bool InputEnglish(char keychar)
        {
            return !(((keychar >= 65 && keychar <= 90) || (keychar >= 97 && keychar <= 122)) || (keychar == (char)Keys.Back));
        }

        #endregion 限制用户输入

        #region 进制转换


        public static string IntToHex(int dec)
        {
            if (dec < 0)
                throw new ArgumentOutOfRangeException(@"value值超于数值范围。");
            return string.Format("{0:X2}", dec);
        }

        public static string BinaryToHex(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            if (!CRegex.IsBinary(str))
            {
                throw new ArgumentException();
            }
            return string.Format("{0:X2}", Convert.ToInt32(str, 2));
        }

        public static string IntToBinary(int dec)
        {
            if (dec < 0)
                throw new ArgumentOutOfRangeException(@"value值超于数值范围。");
            return Convert.ToString(dec, 2).PadLeft(8, '0');
        }

        public static string HexToBinary(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            if (!CRegex.IsHex(str))
            {
                throw new ArgumentException();
            }
            int value = Convert.ToInt32(str, 16);
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }

        public static int StrToInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            if (!CRegex.IsDecimal(str))
            {
                throw new ArgumentException();
            }
            return Convert.ToInt32(str);
        }

        public static int ObjToInt(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            return StrToInt(obj.ToString());
        }

        public static string ObjToStr(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        public static int HexToInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            if (!CRegex.IsHex(str))
            {
                throw new ArgumentException();
            }
            return Convert.ToInt32(str, 16);
        }

        public static int BinaryToInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException();
            }
            if (!CRegex.IsBinary(str))
            {
                throw new ArgumentException();
            }
            return Convert.ToInt32(str, 2);
        }

        public static byte[] IntToAscii(int dec)
        {
            byte[] by = new byte[2];
            int division = dec / 16;
            int mod = dec % 16;
            by[0] = (byte)GetAscii(division);
            by[1] = (byte)GetAscii(mod);
            return by;
        }

        private static int GetAscii(int dec)
        {
            if (dec <= 9)
                dec += 48;
            else if (dec <= 15)
                dec += 55;
            return dec;
        }

        public static int AsciiToInt(int ascii)
        {
            if (ascii >= 48 && ascii <= 57)
            {
                ascii -= 48;
            }
            else if (ascii >= 65 && ascii <= 90)
            {
                ascii -= 55;
            }
            else if (ascii >= 97 && ascii <= 122)
            {
                ascii -= 87;
            }
            return ascii;
        }

        public static int HexToInt(params int[] hexs)
        {
            if (hexs.Length % 2 != 0)
            {
                throw new Exception("数组长度不正确。");
            }
            int[] values = new int[hexs.Length / 2];
            for (int i = 0, j = 0; i < hexs.Length; i += 2, j++)
            {
                values[j] = (AsciiToInt(hexs[i]) * 16) + (AsciiToInt(hexs[i + 1]) % 16);
            }
            int ret = 0;
            if (values.Length > 1)
            {
                for (int i = 0; i < values.Length; i += 2)
                {
                    ret += (values[i] * 256) + (values[i + 1] % 256);
                }
            }
            else
            {
                ret = values[0];
            }
            return ret;
        }

        #endregion 进制转换

        #region 二进制数据

        /// <summary>
        /// 取整数的某一位
        /// </summary>
        /// <param name="num">整数</param>
        /// <param name="mask">要取的位置索引，自右至左为0-7</param>
        /// <returns></returns>
        public static int GetIntegerSomeBit(int num, int mask)
        {
            return num >> mask & 1;
        }

        /// <summary>
        /// 将整数的某位置为0或1
        /// </summary>
        /// <param name="num">整数</param>
        /// <param name="mask">整数的某位</param>
        /// <param name="flag">是否置1，True表示置1，Flase表示置0</param>
        public static int SetIntegeSomeBit(int num, int mask, bool flag)
        {
            if (flag)
            {
                num |= (0x1 << mask);
            }
            else
            {
                num &= ~(0x1 << mask);
            }
            return num;
        }

        public static int GetBitToInt(int num, int index, int count)
        {
            int value = 0;
            for (int i = 0; i < count; i++)
            {
                value += (GetIntegerSomeBit(num, index + i) * (int)Math.Pow(2, i));
            }
            return value;
        }

        #endregion  二进制数据

        #region 异或校验

        public static int Xor(byte[] by)
        {
            return Xor(by, 0, by.Length);
        }

        public static int Xor(byte[] by, int index, int count)
        {
            int result = 0;
            for (int i = index; i < count; i++)
            {
                result = result ^ by[i];
            }
            return result;
        }

        public static int Xor(List<byte> bylist)
        {
            return Xor(bylist.ToArray(), 0, bylist.Count);
        }

        public static int Xor(List<byte> bylist, int index, int count)
        {
            return Xor(bylist.ToArray(), index, count);
        }

        public static int Xor(int value1, int value2)
        {
            byte[] by = new byte[2] { (byte)value1, (byte)value2 };
            return Xor(by, 0, by.Length);
        }

        public static int Xor(string str)
        {
            byte[] by = System.Text.Encoding.ASCII.GetBytes(str);
            return Xor(by);
        }

        #endregion  异或校验

        #region MD5加密

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string Md532(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            MD5 md5 = MD5.Create();
            return HashAlgorithmBase(md5, value, encoding);
        }

        /// <summary>
        /// 加权MD5加密
        /// </summary>
        public static string Md532(string value, string salt)
        {
            return salt == null ? Md532(value) : Md532((value + "『" + salt + "』"));
        }

        #endregion

        #region SHA 加密

        /// <summary>
        /// SHA1 加密
        /// </summary>
        public static string Sha1(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 加密
        /// </summary>
        public static string Sha256(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            SHA256 sha256 = new SHA256Managed();
            return HashAlgorithmBase(sha256, value, encoding);
        }

        /// <summary>
        /// SHA512 加密
        /// </summary>
        public static string Sha512(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            SHA512 sha512 = new SHA512Managed();
            return HashAlgorithmBase(sha512, value, encoding);
        }

        #endregion

        #region HMAC 加密

        /// <summary>
        /// HmacSha1 加密
        /// </summary>
        public static string HmacSha1(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA1 hmacSha1 = new HMACSHA1(keyStr);
            return HashAlgorithmBase(hmacSha1, value, encoding);
        }

        /// <summary>
        /// HmacSha256 加密
        /// </summary>
        public static string HmacSha256(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA256 hmacSha256 = new HMACSHA256(keyStr);
            return HashAlgorithmBase(hmacSha256, value, encoding);
        }

        /// <summary>
        /// HmacSha384 加密
        /// </summary>
        public static string HmacSha384(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA384 hmacSha384 = new HMACSHA384(keyStr);
            return HashAlgorithmBase(hmacSha384, value, encoding);
        }

        /// <summary>
        /// HmacSha512 加密
        /// </summary>
        public static string HmacSha512(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACSHA512 hmacSha512 = new HMACSHA512(keyStr);
            return HashAlgorithmBase(hmacSha512, value, encoding);
        }

        /// <summary>
        /// HmacMd5 加密
        /// </summary>
        public static string HmacMd5(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACMD5 hmacMd5 = new HMACMD5(keyStr);
            return HashAlgorithmBase(hmacMd5, value, encoding);
        }

        /// <summary>
        /// HmacRipeMd160 加密
        /// </summary>
        public static string HmacRipeMd160(string value, string keyVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            byte[] keyStr = encoding.GetBytes(keyVal);
            HMACRIPEMD160 hmacRipeMd160 = new HMACRIPEMD160(keyStr);
            return HashAlgorithmBase(hmacRipeMd160, value, encoding);
        }

        #endregion

        #region AES 加密解密

        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="value">待加密字段</param>  
        /// <param name="keyVal">密钥值</param>  
        /// <param name="ivVal">加密辅助向量</param> 
        /// <returns></returns>  
        public static string AesStr(string value, string keyVal, string ivVal)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            byte[] btKey = FormatByte(keyVal, encoding); // keyVal.FormatByte(encoding);
            byte[] btIv = FormatByte(ivVal, encoding); // ivVal.FormatByte(encoding);
            byte[] byteArray = encoding.GetBytes(value);
            string encrypt;
            Rijndael aes = Rijndael.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(btKey, btIv), CryptoStreamMode.Write))
                {
                    cStream.Write(byteArray, 0, byteArray.Length);
                    cStream.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(mStream.ToArray());
                }
            }
            aes.Clear();
            return encrypt;
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="value">待加密字段</param>  
        /// <param name="keyVal">密钥值</param>  
        /// <param name="ivVal">加密辅助向量</param>  
        /// <returns></returns>  
        public static string UnAesStr(string value, string keyVal, string ivVal)
        {
            var encoding = Encoding.UTF8;
            byte[] btKey = FormatByte(keyVal, encoding); // keyVal.FormatByte(encoding);
            byte[] btIv = FormatByte(ivVal, encoding); // ivVal.FormatByte(encoding);
            byte[] byteArray = Convert.FromBase64String(value);
            string decrypt;
            Rijndael aes = Rijndael.Create();
            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(btKey, btIv), CryptoStreamMode.Write))
                {
                    cStream.Write(byteArray, 0, byteArray.Length);
                    cStream.FlushFinalBlock();
                    decrypt = encoding.GetString(mStream.ToArray());
                }
            }
            aes.Clear();
            return decrypt;
        }

        /// <summary>  
        /// AES Byte类型 加密  
        /// </summary>  
        /// <param name="data">待加密明文</param>  
        /// <param name="keyVal">密钥值</param>  
        /// <param name="ivVal">加密辅助向量</param>  
        /// <returns></returns>  
        public static byte[] AesByte(byte[] data, string keyVal, string ivVal)
        {
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(keyVal.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(ivVal.PadRight(bVector.Length)), bVector, bVector.Length);
            byte[] cryptograph;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bVector), CryptoStreamMode.Write))
                    {
                        cStream.Write(data, 0, data.Length);
                        cStream.FlushFinalBlock();
                        cryptograph = mStream.ToArray();
                    }
                }
            }
            catch
            {
                cryptograph = null;
            }
            return cryptograph;
        }

        /// <summary>  
        /// AES Byte类型 解密  
        /// </summary>  
        /// <param name="data">待解密明文</param>  
        /// <param name="keyVal">密钥值</param>  
        /// <param name="ivVal">加密辅助向量</param> 
        /// <returns></returns>  
        public static byte[] UnAesByte(byte[] data, string keyVal, string ivVal)
        {
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(keyVal.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(ivVal.PadRight(bVector.Length)), bVector, bVector.Length);
            byte[] original;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream(data))
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Read))
                    {
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            byte[] buffer = new byte[1024];
                            int readBytes;
                            while ((readBytes = cStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                originalMemory.Write(buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return original;
        }

        #endregion

        #region DES 加密解密

        /// <summary>
        /// DES 加密
        /// </summary>
        public static string Des(string value, string keyVal, string ivVal)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(value);
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
            catch { return "转换出错！"; }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        public static string UnDes(string value, string keyVal, string ivVal)
        {
            try
            {
                string[] sInput = value.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return "解密出错！"; }
        }

        #endregion

        #region BASE64 加密解密

        /// <summary>
        /// BASE64 加密
        /// </summary>
        /// <param name="value">待加密字段</param>
        /// <returns></returns>
        public static string Base64(string value)
        {
            var btArray = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(btArray, 0, btArray.Length);
        }

        /// <summary>
        /// BASE64 解密
        /// </summary>
        /// <param name="value">待解密字段</param>
        /// <returns></returns>
        public static string UnBase64(string value)
        {
            var btArray = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(btArray);
        }

        #endregion

        #region Base64加密解密
        /// <summary>
        /// Base64加密 可逆
        /// </summary>
        /// <param name="value">待加密文本</param>
        /// <returns></returns>
        public static string Base64Encrypt(string value)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(value));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="ciphervalue">密文</param>
        /// <returns></returns>
        public static string Base64Decrypt(string ciphervalue)
        {
            return System.Text.Encoding.Default.GetString(System.Convert.FromBase64String(ciphervalue));
        }
        #endregion

        #region 内部方法

        /// <summary>
        /// 转成数组
        /// </summary>
        private static byte[] Str2Bytes(string source)
        {
            source = source.Replace(" ", "");
            byte[] buffer = new byte[source.Length / 2];
            for (int i = 0; i < source.Length; i += 2) buffer[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        private static string Bytes2Str(IEnumerable<byte> source, string formatStr = "{0:X2}")
        {
            StringBuilder pwd = new StringBuilder();
            foreach (byte btStr in source) { pwd.AppendFormat(formatStr, btStr); }
            return pwd.ToString();
        }

        private static byte[] FormatByte(string strVal, Encoding encoding)
        {
            return encoding.GetBytes(Base64(strVal).Substring(0, 16).ToUpper());
        }

        /// <summary>
        /// HashAlgorithm 加密统一方法
        /// </summary>
        private static string HashAlgorithmBase(HashAlgorithm hashAlgorithmObj, string source, Encoding encoding)
        {
            byte[] btStr = encoding.GetBytes(source);
            byte[] hashStr = hashAlgorithmObj.ComputeHash(btStr);
            return Bytes2Str(hashStr);
        }

        #endregion

        #region Json

        public static string SerializeObject<T>(IList<T> ilist)
        {
            return JsonConvert.SerializeObject(ilist);
        }

        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType<T>(json, anonymousTypeObject);
        }


        /// <summary>
        /// List转成json 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    PropertyInfo[] pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (int j = 0; j < pi.Length; j++)
                    {
                        Type type;
                        object o = pi[j].GetValue(list[i], null);
                        string v = string.Empty;
                        if (o != null)
                        {
                            type = o.GetType();
                            v = o.ToString();
                        }
                        else
                        {
                            type = typeof(string);
                        }

                        Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(v, type));

                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        /// <summary>
        /// 序列化集合对象
        /// </summary>
        public static string JsonSerializerByArrayData<T>(T[] tArray)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T[]));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, tArray);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary>
        /// 序列化单个对象
        /// </summary>
        public static string JsonSerializerBySingleData<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary> 
        /// 反序列化单个对象
        /// </summary> 
        public static T JsonDeserializeBySingleData<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式  
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary> 
        /// 反序列化集合对象
        /// </summary> 
        public static T[] JsonDeserializeByArrayData<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式  
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T[]));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T[] arrayObj = (T[])ser.ReadObject(ms);
            return arrayObj;
        }

        /// <summary> 
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串 
        /// </summary> 
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>  
        /// 将时间字符串转为Json时间 
        /// </summary> 
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }

        /// <summary>
        /// List转成json 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson<T>(list, obj.GetType().Name);
        }

        /// <summary> 
        /// 对象转换为Json字符串 
        /// </summary> 
        /// <param name="jsonObject">对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(object jsonObject)
        {
            try
            {
                StringBuilder jsonString = new StringBuilder();
                jsonString.Append("{");
                PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                    if (objectValue == null)
                    {
                        continue;
                    }
                    StringBuilder value = new StringBuilder();
                    if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    else if (objectValue is string)
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    else if (objectValue is IEnumerable)
                    {
                        value.Append(ToJson((IEnumerable)objectValue));
                    }
                    else
                    {
                        value.Append("\"" + objectValue.ToString() + "\"");
                    }
                    jsonString.Append("\"" + propertyInfo[i].Name + "\":" + value + ","); ;
                }
                return jsonString.ToString().TrimEnd(',') + "}";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson(item) + ",";
            }
            if (jsonString.Length > 1)
            {
                jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            }
            else
            {
                jsonString = "[]";
            }
            return jsonString + "]";
        }

        /// <summary> 
        /// 普通集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }

        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            if (jsonString.Length == 1)
            {
                return "[]";
            }
            return jsonString.ToString();
        }

        /// <summary>
        /// DataTable转成Json 
        /// </summary>
        /// <param name="jsonName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j] is DBNull ? string.Empty : dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        /// <summary> 
        /// DataReader转换为Json 
        /// </summary> 
        /// <param name="dataReader">DataReader对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IDataReader dataReader)
        {
            try
            {
                StringBuilder jsonString = new StringBuilder();
                jsonString.Append("[");

                while (dataReader.Read())
                {
                    jsonString.Append("{");
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        Type type = dataReader.GetFieldType(i);
                        string strKey = dataReader.GetName(i);
                        string strValue = dataReader[i].ToString();
                        jsonString.Append("\"" + strKey + "\":");
                        strValue = StringFormat(strValue, type);
                        if (i < dataReader.FieldCount - 1)
                        {
                            jsonString.Append(strValue + ",");
                        }
                        else
                        {
                            jsonString.Append(strValue);
                        }
                    }
                    jsonString.Append("},");
                }
                if (!dataReader.IsClosed)
                {
                    dataReader.Close();
                }
                jsonString.Remove(jsonString.Length - 1, 1);
                jsonString.Append("]");
                if (jsonString.Length == 1)
                {
                    return "[]";
                }
                return jsonString.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 
        /// DataSet转换为Json 
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    case '\v':
                        sb.Append("\\v"); break;
                    case '\0':
                        sb.Append("\\0"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string StringFormat(string str, Type type)
        {
            if (type != typeof(string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }
            else if (type == typeof(byte[]))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(Guid))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }

        #endregion Json

        #region 文件夹
          
        public static void OpenWindowDirectory(string directory)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + Path.GetFullPath(directory);
            System.Diagnostics.Process.Start(psi);
        }

        #endregion 文件夹
    }
}
