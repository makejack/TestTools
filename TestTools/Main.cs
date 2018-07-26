using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bll;
using NetDimension.NanUI;

namespace TestTools
{
    public partial class Main : WinFormium
    {

        internal static Main GetMain;

        public Main()
            : base("http://res.app.local/main.html")
        {
            InitializeComponent();
            GetMain = this;

            this.Load += OnLoadForm;

            LoadHandler.OnLoadEnd += (sender, args) =>
            {
                PortMonitor.StartMonitor();
            };

            GlobalObject.AddFunction("ShowDevTools").Execute += (sender, args) =>
            {
                Chromium.ShowDevTools();
            };

            GlobalObject.AddFunction("HostOpenAndCloseSerialDevice").Execute += (sender, args) =>
            {
                try
                {
                    var callback = args.Arguments.FirstOrDefault(e => e.IsFunction);
                    string deviceName = args.Arguments[0].StringValue;
                    SerialPortEx serial = SelectSerialDevice(deviceName);
                    string portName = args.Arguments[1].StringValue;
                    if (!serial.IsOpen)
                    {
                        SerialPortManager.OpenSerialPort(serial, portName);
                    }
                    else
                    {
                        SerialPortManager.CloseSerialPort(serial);
                    }
                    string json = Utility.JsonSerializerBySingleData(serial);
                    callback.ExecuteFunction(null, new Chromium.Remote.CfrV8Value[] { json });
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    ViewCallFunction.ViewAlert(ex.Message);
                }
            };

            GlobalObject.AddFunction("HostAutoConnectionDeviced").Execute += (sender, args) =>
            {
                try
                {
                    bool autoConnection = args.Arguments[0].BoolValue;
                    if (autoConnection)
                    {
                        ConnectionSerialDevice.Start();
                    }
                    else
                    {
                        ConnectionSerialDevice.Stop();
                    }
                    ConnectionSerialDevice.AutoConnectioinDevice = autoConnection;
                }
                catch (Exception ex)
                {
                    Log4Helper.ErrorInfo(ex.Message, ex);
                    ViewCallFunction.ViewAlert(ex.Message);
                }
            };

            CardManagerEvh.InitEvent();
            PwdManagerEvh.InitEvent();
            ConfigureManagerEvh.InitEvent();
            WirelessManagerEvh.InitEvent();
            UserManagerEvh.InitEvent();
        }

        private SerialPortEx SelectSerialDevice(string deviceName)
        {
            return SerialPortManager.Device1.SerialPortDevice.Name == deviceName ? SerialPortManager.Device1.SerialPortDevice : SerialPortManager.Device2.SerialPortDevice;
        }

        private void OnLoadForm(object sender, EventArgs e)
        {
            try
            {
                DataBaseManager.LoadFile();

                string version = ConfigManager.GetString("General", "Version");
                if (string.IsNullOrEmpty(version) || Application.ProductVersion != version)
                {
                    DataBaseManager.UpdateDataBaseStr();
                    ConfigManager.Set("General", "Version", Application.ProductVersion);
                }

                ConnectionSerialDevice.AutoConnectioinDevice = true;
                SerialPortManager.InitSerialDevice();

            }
            catch (Exception ex)
            {
                Log4Helper.ErrorInfo(ex.Message, ex);
            }
        }

    }
}
