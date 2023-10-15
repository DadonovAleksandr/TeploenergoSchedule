using System;

namespace TeploenergoSchedule.Model.FileInfo;

internal enum FileStateEnum
{
    None,
    Processing,
    CorrectedSuccess,
    CorrectedWithWarning,
    Error
}