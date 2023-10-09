﻿using Config.Net;
using System;
using System.IO;

namespace TeploenergoSchedule.Model.AppSettings.AppConfig
{
    public interface IAppConfig
    {
        ILoadFilesSettings LoadFilesSettings { get; set; }
    }

    public static class AppConfig
    {
        internal static IAppConfig GetConfigFromDefaultPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configPath = Path.Combine(appDataPath, $"{AppConst.Get().AppName}", "settings.json");
            return new ConfigurationBuilder<IAppConfig>()
                .UseJsonFile(configPath)
                .Build();
        }
    }
}