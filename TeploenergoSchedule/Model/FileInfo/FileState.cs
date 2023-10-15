using System;
using System.ComponentModel;

namespace TeploenergoSchedule.Model.FileInfo;

internal class FileState : INotifyPropertyChanged
{
    private FileStateEnum _state;

     public string Path { get; set; }
     public FileStateEnum State 
    {
        get => _state;
        set
        {
            _state = value;
            NotifyPropertyChanged(nameof(State));
        }
    }

    public FileState(string filePath)
    {
        Path = filePath;
        State = FileStateEnum.None;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged(string p)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }


    public override string ToString() => Path;

}