using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;

namespace Dashboard.Common
{
    public class WebConfigHelper
    {
        public static List<AppSettingItem> GetAllAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var result = new List<AppSettingItem>();

            foreach (string key in appSettings.AllKeys)
            {
                result.Add(new AppSettingItem
                {
                    Key = key,
                    Value = appSettings[key]
                });
            }

            return result.OrderBy(x => x.Key).ToList();
        }

        public static void SaveAppSetting(string key, string value)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = config.AppSettings.Settings;

            if (appSettings[key] != null)
            {
                appSettings[key].Value = value;
            }
            else
            {
                appSettings.Add(key, value);
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void DeleteAppSetting(string key)
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var appSettings = config.AppSettings.Settings;

            if (appSettings[key] != null)
            {
                appSettings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public static bool AppSettingExists(string key)
        {
            return ConfigurationManager.AppSettings[key] != null;
        }
    }

    public class AppSettingItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

