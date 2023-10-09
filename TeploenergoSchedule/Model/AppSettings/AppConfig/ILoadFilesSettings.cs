using Config.Net;

namespace TeploenergoSchedule.Model.AppSettings.AppConfig;

public interface ILoadFilesSettings
{
    [Option(DefaultValue = "")]
    string DefaultFolderPath { get; set; }
}