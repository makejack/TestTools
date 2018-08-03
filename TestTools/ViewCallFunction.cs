using Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace TestTools
{
    public class ViewCallFunction
    {
        /// <summary>
        /// 消息提示框
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewAlert(string msg)
        {
            msg = msg.Replace("\r"," ").Replace("\n"," ");
            Main.GetMain.ExecuteJavascript($"ViewAlert('{msg}')");
        }

        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="icon"></param>
        public static void ViewMessage(string msg, string icon)
        {
            Main.GetMain.ExecuteJavascript($"ViewMessage('{msg}','{icon}')");
        }

        /// <summary>
        /// 关闭加载层
        /// </summary>
        public static void ViewCloseLoading()
        {
            Main.GetMain.ExecuteJavascript("CloseLoading()");
        }

        /// <summary>
        /// 端口数量变化时消息
        /// </summary>
        /// <param name="serialNames"></param>
        public static void ViewSerialPortCountChanged(List<string> serialNames)
        {
            string json = Utility.SerializeObject<string>(serialNames);
            Main.GetMain.ExecuteJavascript($"ViewSerailCountChanged('{json}')");
        }

        /// <summary>
        /// 端口变化时消息
        /// </summary>
        /// <param name="port"></param>
        public static void ViewSerialPortChanged(SerialPortEx port)
        {
            string json = Utility.JsonSerializerBySingleData<SerialPortEx>(port);
            Main.GetMain.ExecuteJavascript($"ViewSerialPortChanged('{json}')");
        }

        /// <summary>
        /// 连接错误消息
        /// </summary>
        public static void ViewConnectionFailedMessage()
        {
            Main.GetMain.ExecuteJavascript("ViewConnectionFailedMessage()");
        }

        /// <summary>
        /// 显示读卡的定距卡信息
        /// </summary>
        /// <param name="info"></param>
        public static void ViewDisplayReadCardInfo(CardInfo info)
        {
            string json = Utility.JsonSerializerBySingleData<CardInfo>(info);
            Main.GetMain.ExecuteJavascript($"ViewDisplayCardInfo('{json}')");
        }

        /// <summary>
        /// 定距卡发行结束
        /// </summary>
        public static void ViewIssueOver(CardInfo info)
        {
            string json = Utility.JsonSerializerBySingleData<CardInfo>(info);
            Main.GetMain.ExecuteJavascript($"ViewIssueOver('{json}')");
        }

        /// <summary>
        /// 解锁副卡
        /// </summary>
        public static void ViewRemoveLock(List<CardInfo> infos)
        {
            string json = Utility.JsonSerializerByArrayData(infos.ToArray());
            Main.GetMain.ExecuteJavascript($"ViewViceRemoveLock('{json}')");
        }

        /// <summary>
        /// 人员通道定距卡发行结束
        /// </summary>
        /// <param name="info"></param>
        public static void ViewIssuePersonnelOver(CardInfo info)
        {
            string json = "";
            if (info != null)
            {
                json = Utility.JsonSerializerBySingleData<CardInfo>(info);
            }
            Main.GetMain.ExecuteJavascript($"ViewPersonnelIssueOver('{json}')");
        }

        /// <summary>
        /// 显示批量完成的内容
        /// </summary>
        /// <param name="info"></param>
        public static void ViewDisplayBatchContent(CardInfo info, int index)
        {
            string json = Utility.JsonSerializerBySingleData(info);
            Main.GetMain.ExecuteJavascript($"ViewDisplayBatchContent('{json}','{index}')");
        }

        /// <summary>
        /// 批量结束
        /// </summary>
        internal static void ViewBatchOver(int count)
        {
            Main.GetMain.ExecuteJavascript($"ViewBatchOver('{count}')");
        }

        internal static void ViewLoseOver()
        {
            Main.GetMain.ExecuteJavascript("ViewLoseOver()");
        }

        /// <summary>
        /// 定距加密
        /// </summary>
        public static void ViewEncryptMessage(string msg)
        {
            Main.GetMain.ExecuteJavascript($"ViewEncryptMessage('{msg}')");
        }

        /// <summary>
        /// 定距加密结束
        /// </summary>
        public static void ViewEncryptOver()
        {
            Main.GetMain.ExecuteJavascript("ViewEncryptOver()");
        }

        /// <summary>
        /// 无线测试消息提示
        /// </summary>
        /// <param name="msg"></param>
        public static void ViewWirelessMessage(string msg)
        {
            Main.GetMain.ExecuteJavascript($"ViewWirelessMessage('{msg}')");
        }

        /// <summary>
        /// 无线测试操作结束
        /// </summary>
        public static void ViewWirelessOver()
        {
            Main.GetMain.ExecuteJavascript("ViewWirelessOver()");
        }

        /// <summary>
        /// 客户编号下载结束
        /// </summary>
        internal static void ViewDownloadOver(bool result)
        {
            Main.GetMain.ExecuteJavascript($"ViewDownloadOver('{result}')");
        }

        /// <summary>
        /// 客户编号下载超时
        /// </summary>
        internal static void ViewDownloadOverTime()
        {
            Main.GetMain.ExecuteJavascript("ViewDownloadOverTime()");
        }

    }
}
