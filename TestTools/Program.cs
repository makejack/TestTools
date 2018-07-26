using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NetDimension.NanUI;

namespace TestTools
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Bootstrap.Load())
            {
                Bootstrap.RegisterAssemblyResources(System.Reflection.Assembly.GetExecutingAssembly(), "www");

                Application.Run(new Main());
            }
        }
    }
}
