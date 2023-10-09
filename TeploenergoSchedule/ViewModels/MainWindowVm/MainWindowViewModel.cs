using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using TeploenergoSchedule.Infrastructure.Commands;
using TeploenergoSchedule.Model;
using TeploenergoSchedule.Model.AppSettings.AppConfig;
using TeploenergoSchedule.Service.UserDialogService;
using TeploenergoSchedule.ViewModels.Base;
using System.Collections.ObjectModel;

namespace TeploenergoSchedule.ViewModels.MainWindowVm
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly IAppConfig _appConfig;
        private readonly IUserDialogService _userDialogService;

        private string[] _fileNames;

        /* ------------------------------------------------------------------------------------------------------------ */
        public MainWindowViewModel(IUserDialogService userDialogService)
        {
            _log.Debug($"Вызов конструктора {GetType().Name}");
            _appConfig = AppConfig.GetConfigFromDefaultPath();
            _userDialogService = userDialogService;

            FileNames = new ObservableCollection<string>();

            #region Commands
            LoadFiles = new RelayCommand(OnLoadFilesExecuted, CanLoadFilesExecute);
            #endregion
        }

        /// <summary>
        /// Действия выполняемые при закрытии основной формы
        /// </summary>
        public void OnExit()
        {
            //_projectConfigurationRepository?.Save();
        }
        /* ------------------------------------------------------------------------------------------------------------ */
        #region Commands

        #region LoadFiles
        public ICommand LoadFiles { get; }
        private void OnLoadFilesExecuted(object p)
        {
            try
            {
                var ofd = new OpenFileDialog();

                //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                ofd.Filter = "xlsx files|*.xlsx";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != true)
                    return;
                
                _fileNames = ofd.FileNames;
                FileNames.Clear();
                foreach(string file in _fileNames)
                {
                    FileNames.Add(file);    
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private bool CanLoadFilesExecute(object p) => true;
        #endregion

        #region Exit
        public ICommand Exit { get; }
        private void OnExitExecuted(object p) => Application.Current.Shutdown();
        private bool CanExitExecute(object p) => true;
        #endregion

        #endregion

        /* ------------------------------------------------------------------------------------------------------------ */

        public ObservableCollection<string> FileNames { get; set; }

        #region Window title

        private string _title = $"{AppConst.Get().AppDesciption} {ProjectVersion.Get()}";
        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion

    }
}