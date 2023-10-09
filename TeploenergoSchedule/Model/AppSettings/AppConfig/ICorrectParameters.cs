using Config.Net;
using System;

namespace TeploenergoSchedule.Model.AppSettings.AppConfig;

public interface ICorrectParameters
{ 
    [Option(DefaultValue = "2023")]
    string YearOfApproval {  get; set; }

    [Option(DefaultValue = "2024")]
    string YearOfImplementation { get; set; }
}