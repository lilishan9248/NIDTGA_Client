using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DHCPv6
{
    public class ConfigHelper
    {
        public static string fileName = System.IO.Path.GetFileName(Application.ExecutablePath);

        public static bool AddSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(fileName);
            config.AppSettings.Settings.Add(key, value);
            config.Save();
            return true;
        }

        public static string GetSetting(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(fileName);
            if (config.AppSettings.Settings[key] == null)
            {
                return null;
            }
            string value = config.AppSettings.Settings[key].Value;
            return value;
        }

        public static bool UpdateSetting(string key, string newValue)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(fileName);
            config.AppSettings.Settings[key].Value = newValue;
            config.Save();
            return true;
        }
    }
}