using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Model;
using Bll;
using Bll.Management;
using System.Reflection;

namespace TestTools
{
    public class ConfigureManagerEvh
    {

        public static void InitEvent()
        {
            Main.GetMain.GlobalObject.AddFunction("HostPostConfirmInfoCount").Execute += Host_PostConfirmInfoCount;
            Main.GetMain.GlobalObject.AddFunction("HostPostConfirmInfos").Execute += Host_PostConfirmInfos;
            Main.GetMain.GlobalObject.AddFunction("HostPostHostNumber").Execute += Host_PostHostNumber;
            Main.GetMain.GlobalObject.AddFunction("HostPostConfirmAdd").Execute += Host_PostConfirmAdd;
            Main.GetMain.GlobalObject.AddFunction("HostPostConfirmInfo").Execute += Host_PostConfirmInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostUpdateConfirmInfo").Execute += Host_PostUpdateConfirmInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDelConfirmInfo").Execute += Host_PostDelConfirmInfo;
            Main.GetMain.GlobalObject.AddFunction("HostPostDrives").Execute += Host_PostDrives;
            Main.GetMain.GlobalObject.AddFunction("HostPostExport").Execute += Host_PostExport;
        }

        private static void Host_PostConfirmInfoCount(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int count = ConfirmManager.GetCount();
                e.SetReturnValue(count);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostConfirmInfos(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int page = e.Arguments[0].IntValue;
                List<DeviceInfo> infos = ConfirmManager.GetInfos(page - 1, DefaultParams.PAGE_ROW_COUNT);
                string json = Utility.JsonSerializerByArrayData(infos.ToArray());
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostHostNumber(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int maxId = ConfirmManager.GetMaxId();
                var hostnumber = (maxId + 1) % 128;
                if (hostnumber == 0)
                {
                    hostnumber = 1;
                }
                e.SetReturnValue(hostnumber);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostConfirmAdd(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string json = e.Arguments[0].StringValue;
                DeviceInfo info = ConfirmManager.Add(json);
                // json = Utility.JsonSerializerBySingleData(info);
                // e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostConfirmInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                DeviceInfo info = ConfirmManager.ConfirmInfos[index];
                string json = Utility.JsonSerializerBySingleData(info);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostUpdateConfirmInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string json = e.Arguments[0].StringValue;
                DeviceInfo info = ConfirmManager.Update(json);
                // json = Utility.JsonSerializerBySingleData(info);
                // e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDelConfirmInfo(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                int index = e.Arguments[0].IntValue;
                DeviceInfo info = ConfirmManager.ConfirmInfos[index];
                ConfirmManager.Del(info.Did);
                ConfirmManager.ConfirmInfos.RemoveAt(index);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostDrives(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                string json = Utility.JsonSerializerByArrayData(drives);
                e.SetReturnValue(json);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }

        private static void Host_PostExport(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                string code = e.Arguments[0].StringValue;
                string json = e.Arguments[1].StringValue;
                DeviceInfo[] infos = Utility.JsonDeserializeByArrayData<DeviceInfo>(json);
                string controlEnabled = e.Arguments[2].StringValue;
                string controlPwd = e.Arguments[3].StringValue;
                string hostOldPwd = e.Arguments[4].StringValue;
                string hostPwd = e.Arguments[5].StringValue;
                string icPwd = e.Arguments[6].StringValue;

                string path = code + "\\SZCBKJ";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\OS";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!string.IsNullOrEmpty(controlEnabled))
                {
                    if (string.IsNullOrEmpty(controlPwd))
                    {
                        controlPwd = "00000000000000";
                    }
                    else
                    {
                        controlPwd = "3355AAEE" + controlPwd;
                    }
                }

                foreach (DeviceInfo info in infos)
                {
                    Dictionary<int, string> dic = new Dictionary<int, string>();
                    dic.Add(0, $"{info.HostNumber:X2}{info.FrequencyOffset:X2}{(info.CameraDetection == 0 ? $"{info.HostNumber:X8}" : info.WirelessNumber.ToString().PadLeft(8, '0'))}");
                    dic.Add(1, $"{info.CardReadDistance:X2}");
                    dic.Add(2, $"{info.ReadCardDelay:X2}");
                    dic.Add(3, "");
                    dic.Add(4, hostOldPwd + hostPwd);
                    dic.Add(5, "");
                    dic.Add(6, "0101");
                    dic.Add(7, $"{DateTime.Now:yyMMddHHmmss}");
                    dic.Add(8, $"{info.Partition:X2}");
                    dic.Add(9, $"{info.IOMouth:X2}");
                    dic.Add(10, $"{info.SAPBF:X2}");
                    dic.Add(11, controlPwd);
                    string openTheDoorModel = "FFFFFFF0";
                    if (info.OpenModel < 3)
                    {
                        switch (info.OpenModel)
                        {
                            case 0:
                                openTheDoorModel = $"{info.BrakeNumber:X6}FF";
                                break;
                            case 1:
                                openTheDoorModel = $"{info.BrakeNumber:X6}55";
                                break;
                            case 2:
                                openTheDoorModel = "FFFFFFAA";
                                break;
                        }
                    }
                    dic.Add(12, openTheDoorModel);
                    dic.Add(13, $"{info.Language:X2}");
                    dic.Add(14, "1E");
                    dic.Add(15, $"{info.Detection:X2}");
                    dic.Add(16, icPwd);
                    dic.Add(17, $"{info.FuzzyQuery:X2}");

                    StreamWriter sw = File.CreateText(path + $"\\FORM{info.HostNumber:X2}.txt");
                    try
                    {
                        foreach (KeyValuePair<int, string> item in dic)
                        {
                            int xor = Utility.Xor(item.Value);
                            sw.WriteLine($"00{item.Key:X2}<{item.Value.Length:X2},{item.Value},{xor:X2}>");
                        }
                    }
                    finally
                    {
                        sw.Close();
                    }
                }
                Utility.OpenWindowDirectory(path);
            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
                ViewCallFunction.ViewAlert(ex.Message);
            }
        }
    }
}
