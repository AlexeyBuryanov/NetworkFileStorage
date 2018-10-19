using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Client.Model;
using Client.Windows;
using RemotingLib;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Основные свойства с которыми будем работать
        public BasicProps Basic { get; set; }

        // Главное окно передаётся во ViewModel
        // Свойство Basic делается public
        // Благодаря этому имеем доступ ко всем нужным свойствам отовсюду

        public MainWindow()
        {
            Basic = new BasicProps();

            // Окно входа
            StartAppWindow win = new StartAppWindow();
            bool? flagExit = win.ShowDialog();

            if (flagExit.Value == false) {
                Application.Current.Shutdown();
            } else {
                // Если пользователь вошёл успешно
                InitializeComponent();

                SplashScreen ss = new SplashScreen(@"Images\splashScreen.png");
                ss.Show(true, true);
                ss.Close(new TimeSpan(0, 0, 2));

                Basic.LoginData = new LoginData {
                    HostName = win.TextBoxHostName.Text,
                    Port = win.TextBoxPort.Text,
                    UserName = win.TextBoxUserName.Text
                };

                Basic.DataGridMain = (DataGrid)FindName("DataGridMain");

                DataContext = new ApplicationViewModel(this);
            } // if-else
        } // MainWindow


        private void DataGridMain_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridMain.SelectedItem == null) {
                return;
            } else if ((DataGridMain.SelectedItem as FileProps).Extension == "Папка с файлами") {
                Basic.PathBack = Basic.Path;
                Basic.Path = (Basic.DataGridMain.SelectedItem as FileProps).FullName;
                Basic.ShowFilesAndDirs(Basic.Path);
            } // if-else
        } // DataGridMain_MouseDoubleClick


        private void DataGridMain_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                MemoryStream memory = new MemoryStream();
                MemoryStream memCompress = new MemoryStream();

                foreach (string path in paths) {
                    // Если это не папка действуем по стандарту
                    if (!Directory.Exists(path)) {
                        memory = Basic.FileLoadToMemory(path);
                        memCompress = Basic.CompressFileFromMemory(memory);
                        Basic.Proxy.GetCompressFileFromMemory(memCompress, Path.GetFileName(path), Basic.Path);
                    } else {
                        // TODO: Если папка
                        // Создаём папку в хранилище
                        Basic.Proxy.CreateDir(Basic.Path + "\\" + Path.GetFileName(path));

                        // Ищем в ней файлы
                        DirectoryInfo dir = new DirectoryInfo(path + "\\");
                        FileInfo[] fi = dir.GetFiles();

                        // Если что-то есть
                        if (fi.Length != 0) {
                            // Копируем в хранилище найденные файлы
                            foreach (FileInfo f in fi) {
                                memory = Basic.FileLoadToMemory(f.FullName);
                                memCompress = Basic.CompressFileFromMemory(memory);
                                Basic.Proxy.GetCompressFileFromMemory(memCompress, Path.GetFileName(f.FullName), Basic.Path + "\\" + Path.GetFileName(path) + "\\");
                            } // foreach
                        } // if

                        //// Ищем подпапки
                        //DirectoryInfo[] di = dir.GetDirectories();
                        //// Если что-то есть
                        //if (di.Length != 0) {
                        //    foreach (DirectoryInfo d in di) {
                        //        // Создаём
                        //        Basic.Proxy.CreateDir(d.FullName);

                        //        // В подпапке ищем файлы
                        //        DirectoryInfo subdir = new DirectoryInfo(d.FullName + "\\");
                        //        FileInfo[] fisubs = subdir.GetFiles();

                        //        if (fisubs.Length != 0) {
                        //            // Копируем в хранилище найденные файлы
                        //            foreach (FileInfo fisub in fisubs) {
                        //                memory = Basic.FileLoadToMemory(fisub.FullName);
                        //                memCompress = Basic.CompressFileFromMemory(memory);
                        //                Basic.Proxy.GetCompressFileFromMemory(memCompress, Path.GetFileName(fisub.FullName), Basic.Path);
                        //            } // foreach
                        //        } // if
                        //    } // foreach
                        //} // if
                    } // if-else
                } // foreach
                Basic.ShowFilesAndDirs(Basic.Path);
            } // if
        } // DataGridMain_Drop


        private void DataGridMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                DragDropEffects.Copy :
                DragDropEffects.None;
        } // DataGridMain_DragEnter


        private void DataGridMain_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridMain.SelectedItem != null) {
                // Цепляем выбранный файл
                FileProps fp = DataGridMain.SelectedItem as FileProps;

                // Декомпрессим файл на сервере (на сервере создаётся его расжатая копия)
                Basic.Proxy.DecompressFile(fp.FullName);
                // Которая в последствии и "вытаскивает" пользователь

                // Сделал так т.к. понятия не имею, как можно по средствам
                // Drag&Drop создать в месте куда перетаскивается файл сам файл.
                // Расжать файл в MemoryStream, а из MemeryStream этот файл нужно
                // как-то создать и нужен хотя бы путь места куда происходит Drop.
                // Вопрос: откуда этот путь взять?

                // Получаем файл уже у клиента посредствам перетаскивания
                // Передаваемые данные. Представляет базовую реализацию IDataObject
                DataObject data = new DataObject();

                // Коллекция строк с нашими выделенными объектами, а точнее
                // переносить будем расжатую копию
                StringCollection fileDropList = new StringCollection { fp.FullName+"~" };

                // Список перенесённых данных указывается в виде коллекции строк
                data.SetFileDropList(fileDropList);

                // Сохраняет указанные данные в этом объекте данных
                data.SetData("NFS", 0);

                // Инициирует операцию перетаскивания мышью
                DragDrop.DoDragDrop(DataGridMain, data, DragDropEffects.Copy);

                // Удаляем расжатую копию
                Basic.Proxy.DeleteFile(fp.FullName + "~");
            } // if
        } // DataGridMain_MouseDown
    } // class MainWindow
} // Client