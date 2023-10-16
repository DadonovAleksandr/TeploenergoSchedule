using System;
using System.Windows;
using System.Windows.Input;
using TeploenergoSchedule.Infrastructure.Commands;
using TeploenergoSchedule.Model;
using TeploenergoSchedule.Model.AppSettings.AppConfig;
using TeploenergoSchedule.Service.UserDialogService;
using TeploenergoSchedule.ViewModels.Base;
using System.Collections.ObjectModel;
using TeploenergoSchedule.Service;
using TeploenergoSchedule.Model.FileInfo;
using System.IO;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows.Threading;

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

            FileStates = new ObservableCollection<FileState>();
            
            _yearOfApproval = _appConfig.CorrectParameters.YearOfApproval;
            _yearOfImplementation = _appConfig.CorrectParameters.YearOfImplementation;

            #region Commands
            LoadFiles = new RelayCommand(OnLoadFilesExecuted, CanLoadFilesExecute);
            CorrectFiles = new RelayCommand(OnCorrectFilesExecuted, CanCorrectFilesExecute);
            #endregion
        }

        /// <summary>
        /// Действия выполняемые при закрытии основной формы
        /// </summary>
        public void OnExit()
        {
            if(!string.IsNullOrEmpty(_yearOfApproval))
                _appConfig.CorrectParameters.YearOfApproval = _yearOfApproval;
            if (!string.IsNullOrEmpty(_yearOfImplementation))
                _appConfig.CorrectParameters.YearOfImplementation = _yearOfImplementation;
        }

        /// <summary>
        /// Обработка файлов
        /// </summary>
        private void CorrectFilesProcess()
        {
            try
            {
                var corrector = new CorrectorNpoi(_yearOfApproval, _yearOfImplementation);
                foreach (var file in FileStates)
                {
                    corrector.Correct(file);
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                    _userDialogService.ShowInformation("Корректировка содержимого файлов выполнена!")));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
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

                if (!string.IsNullOrEmpty(_appConfig.LoadFilesSettings.DefaultFolderPath)
                    && Directory.Exists(_appConfig.LoadFilesSettings.DefaultFolderPath))
                {
                    ofd.InitialDirectory = _appConfig.LoadFilesSettings.DefaultFolderPath;
                }
                ofd.Filter = "Excel files|*.xlsx;*.xls";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != true)
                    return;
                
                _fileNames = ofd.FileNames;
                _appConfig.LoadFilesSettings.DefaultFolderPath = Path.GetDirectoryName(ofd.FileName);

                FileStates.Clear();
                foreach(string file in _fileNames)
                {
                    FileStates.Add(new(file));    
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private bool CanLoadFilesExecute(object p) => true;
        #endregion

        #region CorrectFiles
        public ICommand CorrectFiles { get; }
        private void OnCorrectFilesExecuted(object p) => Task.Run(() => CorrectFilesProcess());

        private bool CanCorrectFilesExecute(object p) => true;
        #endregion

        #region Exit
        public ICommand Exit { get; }
        private void OnExitExecuted(object p) => Application.Current.Shutdown();
        private bool CanExitExecute(object p) => true;
        #endregion

        #endregion

        /* ------------------------------------------------------------------------------------------------------------ */

        public ObservableCollection<FileState> FileStates { get; set; }

        #region year of approval

        private string _yearOfApproval;
        /// <summary>
        /// Год утверждения
        /// </summary>
        public string YearOfApproval
        {
            get => _yearOfApproval;
            set => Set(ref _yearOfApproval, value);
        }
        #endregion

        #region year of implementation

        private string _yearOfImplementation;
        /// <summary>
        /// Год выполнения
        /// </summary>
        public string YearOfImplementation
        {
            get => _yearOfImplementation;
            set => Set(ref _yearOfImplementation, value);
        }
        #endregion

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