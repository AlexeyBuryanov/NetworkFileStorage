using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Client.Model;
using Client.Windows;
using RemotingLib;

namespace Client
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private BasicProps _basic;
        private MainWindow _mainWindow;
        private bool _isConnect = false;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        } // OnPropertyChanged


        public ApplicationViewModel(MainWindow mainWindow)
        {
            StatusText = "Добро пожаловать";

            _mainWindow = mainWindow;
            _basic = mainWindow.Basic;

            //***** СОЗДАНИЕ КОМАНД *****//
            ExitCommand           = new RelayCommand(Exit);
            MinimizeCommand       = new RelayCommand(Minimize);
            AboutCommand          = new RelayCommand(About);
            BackCommand           = new RelayCommand(Back);
            GoToFolderCommand     = new RelayCommand(GoToFolder);
            GoToUserFolderCommand = new RelayCommand(GoToUserFolder);
            CreateFolderCommand   = new RelayCommand(CreateFolder);
            DeleteCommand         = new RelayCommand(Delete);
            RefreshCommand        = new RelayCommand(Refresh);

            _isConnect = _basic.ConnectWithServer();
            if (_isConnect) {
                StatusText = $"Подключено к {_basic.LoginData.HostName}:{_basic.LoginData.Port}";
            } else {
                StatusText = "Не удаётся подключиться к серверу. Сервер отверг запрос на подключение.";
            } // if-else
        } // ApplicationViewModel


        //******************** СВОЙСТВА ДЛЯ СВЯЗИ С ИНТЕРФЕЙСОМ ***********************//

        /// <summary>Текстовка в статус-баре</summary>
        private string _statusText;
        public string StatusText {
            get { return _statusText; }
            private set {
                _statusText = value;
                OnPropertyChanged("StatusText");
            } // set
        } // StatusText


        //********************************* КОМАНДЫ ***********************************//

        /// <summary>Выход</summary>
        private RelayCommand _exitCommand;
        public RelayCommand ExitCommand {
            get { return _exitCommand; }
            set { _exitCommand = value; }
        } // ExitCommand

        /// <summary>Свернуть</summary>
        private RelayCommand _minimizeCommand;
        public RelayCommand MinimizeCommand {
            get { return _minimizeCommand; }
            set { _minimizeCommand = value; }
        } // MinimizeCommand

        /// <summary>О программе</summary>
        private RelayCommand _aboutCommand;
        public RelayCommand AboutCommand {
            get { return _aboutCommand; }
            set { _aboutCommand = value; }
        } // AboutCommand

        /// <summary>К предыдущему каталогу</summary>
        private RelayCommand _backCommand;
        public RelayCommand BackCommand {
            get { return _backCommand; }
            set { _backCommand = value; }
        } // BackCommand

        /// <summary>Перейти в папку</summary>
        private RelayCommand _goToFolderCommand;
        public RelayCommand GoToFolderCommand {
            get { return _goToFolderCommand; }
            set { _goToFolderCommand = value; }
        } // GoToFolderCommand

        /// <summary>Перейти в корневую папку пользователя</summary>
        private RelayCommand _goToUserFolderCommand;
        public RelayCommand GoToUserFolderCommand {
            get { return _goToUserFolderCommand; }
            set { _goToUserFolderCommand = value; }
        } // GoToUserFolderCommand

        /// <summary>Создать папку</summary>
        private RelayCommand _createFolderCommand;
        public RelayCommand CreateFolderCommand {
            get { return _createFolderCommand; }
            set { _createFolderCommand = value; }
        } // CreateFolderCommand

        /// <summary>Удалить</summary>
        private RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand {
            get { return _deleteCommand; }
            set { _deleteCommand = value; }
        } // DeleteCommand

        /// <summary>Обновить</summary>
        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand {
            get { return _refreshCommand; }
            set { _refreshCommand = value; }
        } // RefreshCommand


        //**************************** РЕАЛИЗАЦИИ КОМАНД ******************************//

        /// <summary>Выход</summary>
        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        } // Exit


        /// <summary>Свернуть</summary>
        private void Minimize(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        } // Exit


        /// <summary>О программе</summary>
        private void About(object obj)
        {
            AboutWindow win = new AboutWindow(Application.Current.MainWindow);
            win.ShowDialog();
        } // About


        /// <summary>К предыдущему каталогу</summary>
        private void Back(object obj)
        {
            if (string.IsNullOrWhiteSpace(_basic.PathBack)) {
                return;
            } else {
                _basic.Path = _basic.PathBack;
                _basic.ShowFilesAndDirs(_basic.PathBack);
            } // if-else
        } // Back


        /// <summary>Перейти в папку</summary>
        private void GoToFolder(object obj)
        {
            if (_basic.DataGridMain.SelectedItem == null) {
                return;
            } else if ((_basic.DataGridMain.SelectedItem as FileProps).Extension == "Папка с файлами") {
                _basic.PathBack = _basic.Path;
                _basic.Path = (_basic.DataGridMain.SelectedItem as FileProps).FullName;
                _basic.ShowFilesAndDirs(_basic.Path);
            } // if-else
        } // GoToFolder


        /// <summary>Перейти в корневую папку пользователя</summary>
        private void GoToUserFolder(object obj)
        {
            _basic.ShowFilesAndDirs($@"D:\\Storage\{_basic.LoginData.UserName}");
        } // GoToUserFolder


        /// <summary>Создать папку</summary>
        private void CreateFolder(object obj)
        {
            DialogInputDirName win = new DialogInputDirName(_mainWindow);
            win.ShowDialog();
            _basic.Proxy.CreateDir(_basic.Path + "\\" + win.subdir);
            _basic.ShowFilesAndDirs(_basic.Path);
        } // CreateFolder


        /// <summary>Удалить</summary>
        private void Delete(object obj)
        {
            if (_basic.DataGridMain.SelectedItem == null) {
                MessageBox.Show("Выберите, что Вы хотите удалить", "Удаление",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            } else if ((_basic.DataGridMain.SelectedItem as FileProps).Extension == "Папка с файлами") {
                System.Windows.Forms.DialogResult dr =
                    System.Windows.Forms.MessageBox.Show("Эта операция необратима. " +
                    "Файлы внутри папки (если они есть) также будут удалены. " +
                    $"Удалить папку \"{(_basic.DataGridMain.SelectedItem as FileProps).Name}?\"",
                    "Удаление папки",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes) {
                    _basic.Proxy.DeleteDir((_basic.DataGridMain.SelectedItem as FileProps).FullName);
                    _basic.ShowFilesAndDirs(_basic.Path);
                } // if
            } else if ((_basic.DataGridMain.SelectedItem as FileProps).Extension != "Папка с файлами") {
                System.Windows.Forms.DialogResult dr =
                    System.Windows.Forms.MessageBox.Show("Эта операция необратима. " +
                    $"Удалить файл \"{(_basic.DataGridMain.SelectedItem as FileProps).Name}\"?",
                    "Удаление файла",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes) {
                    _basic.Proxy.DeleteFile((_basic.DataGridMain.SelectedItem as FileProps).FullName);
                    _basic.ShowFilesAndDirs(_basic.Path);
                } // if
            } // else-if
        } // Delete


        /// <summary>Обновить</summary>
        private void Refresh(object obj)
        {
            _basic.ShowFilesAndDirs($@"{_basic.Path}");
        } // GoToUserFolder
    } // class ApplicationViewModel
} // Client