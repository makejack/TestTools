using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConfig;
using System.IO;

namespace Bll
{
    public class ConfigManager
    {
        private static string ConfigFileName = "Confing.cfg";

        public static string GetString(string root, string element)
        {
            Configuration settings = GetConfiguration();
            return settings[root][element].StringValue;
        }

        public static int GetInt(string root, string element)
        {
            Configuration settings = GetConfiguration();
            return settings[root][element].IntValue;
        }

        public static void Set(string root, string element, string value)
        {
            Configuration settings = GetConfiguration();
            settings[root][element].StringValue = value;
            Save(settings);
        }

        public static void Set(string root, string element, int value)
        {
            Configuration settings = GetConfiguration();
            settings[root][element].IntValue = value;
            Save(settings);
        }

        private static Configuration GetConfiguration()
        {
            return File.Exists(ConfigFileName) ? Configuration.LoadFromBinaryFile(ConfigFileName) : new Configuration();
        }

        private static void Save(Configuration settings)
        {
            settings.SaveToBinaryFile(ConfigFileName);
        }
    }
}
