using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Bll.Management;
using Model;
using TestTools.PortDataManagment;
using System.Reflection;
using System.Windows.Forms;

namespace TestTools
{
    public class UserManagerEvh
    {
        public static void InitEvent()
        {
            Main.GetMain.GlobalObject.AddFunction("HostPostDownloadNumber").Execute += Host_PostDownloadNumber;
            Main.GetMain.GlobalObject.AddFunction("HostPostDownloadTime").Execute += Host_PostDownloadTime;
            Main.GetMain.GlobalObject.AddFunction("HostPostDownloadPassword").Execute += Host_PostDownloadPassword;
            Main.GetMain.GlobalObject.AddFunction("HostPostClientInfoCount").Execute += Host_PostClientInfoCount;
            Main.GetMain.GlobalObject.AddFunction("HostPostUserInfos").Execute += Host_PostUserInfos;
            Main.GetMain.GlobalObject.AddFunction("HostPostUserInfo").Execute += Host_PostUserInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostConfirmClientNumber").Execute += Host_PostConfirmClientNumber;
            Main.GetMain.GlobalObject.AddFunction("HostPostAddUserInfo").Execute += Host_PostAddUserInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostUpdateUserInfo").Execute += Host_PostUpdateUserInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelUserInfo").Execute += Host_PostDelUserInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostCreateNumberFile").Execute += Host_PostCreateNumberFile;
            Main.GetMain.GlobalObject.AddFunction("HostPostLimitNumberInfos").Execute += Host_PostLimitNumberInfos;
            Main.GetMain.GlobalObject.AddFunction("HostPostLimitNumberInfo").Execute += Host_PostLimitNumberInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostAddNumberInfo").Execute += Host_PostAddNumberInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelNumberInfo").Execute += Host_PostDelNumberInfo;
        }
        private static void Host_PostDownloadPassword(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string oldpwd = e.Arguments[0].StringValue;
                string newpwd = e.Arguments[1].StringValue;
                byte[] by = PortAgreement.SetPersonnelHostPassword(oldpwd, newpwd);
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device2, by);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<PasswordDownload>();
                }
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDownloadNumber(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                UserInfo info = UserManager.UserInfos[index];
                byte[] bys = PortAgreement.SetPersonnelHostNumber(info.UserNumber);
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device2, bys);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<NumberDownload>();
                }
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDownloadTime(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                byte[] bys = PortAgreement.SetPersonnelHostTime();
                bool ret = SerialPortManager.WriteSerialPortData(SerialPortManager.Device2, bys);
                if (ret)
                {
                    ReceivedManager.SetReceivedFunction<TimeDownload>();
                }
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostClientInfoCount(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string strWhere = e.Arguments[0].StringValue;
                int count = UserManager.GetCount(strWhere);
                e.SetReturnValue(count);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUserInfos(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string whereContent = e.Arguments[0].StringValue;
                int page = e.Arguments[1].IntValue;
                List<UserInfo> infos = UserManager.GetInfos(whereContent, page - 1, DefaultParams.PAGE_ROW_COUNT);
                string json = Utility.JsonSerializerBySingleData(infos.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUserInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                UserInfo info = UserManager.UserInfos[index];
                string json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostConfirmClientNumber(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string strClientNumber = e.Arguments[0].StringValue;
                int clientNumber = Utility.StrToInt(strClientNumber);
                List<UserInfo> infos = UserManager.GetInfos();
                int count = infos.Where(w => w.UserNumber == clientNumber).Count();
                if (count == 0)
                {
                    if (LimitManager.NumberInfos == null)
                    {
                        LimitManager.GetInfos();
                    }
                    count = LimitManager.NumberInfos.Where(w => w.LimitNumber == clientNumber).Count();
                }
                e.SetReturnValue(count > 0);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostAddUserInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string json = e.Arguments[0].StringValue;
                UserInfo info = UserManager.Add(json);
                json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUpdateUserInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string json = e.Arguments[0].StringValue;
                int index = e.Arguments[1].IntValue;
                UserInfo info = UserManager.Update(json, index);
                json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelUserInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                UserManager.Del(index);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostCreateNumberFile(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                FolderBrowserDialog folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    UserManager.CreateNumberFile(index, folder.SelectedPath);
                }
                //string folder = e.Arguments[1].StringValue;
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostLimitNumberInfos(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                List<NumberLimit> infos = LimitManager.GetInfos();
                string json = Utility.JsonSerializerByArrayData(infos.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostLimitNumberInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                NumberLimit info = LimitManager.NumberInfos[index];
                string json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostAddNumberInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string strNumber = e.Arguments[0].StringValue;
                int number = Utility.StrToInt(strNumber);
                bool ret = LimitManager.Add(number);
                e.SetReturnValue(ret);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelNumberInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                LimitManager.Del(index);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }
    }
}
