using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using RemotingLib;

namespace Client.Model
{
    /// <summary>
    /// Основные свойства и методы, которые приходится 
    /// выносить и в главном окне (для обработки событий), и в ViewModel 
    /// для взаимодействия с ними в рамках паттерна MVVM
    /// </summary>
    public class BasicProps
    {
        public List<FileProps> ServerFS { get; set; }
        public List<FileProps> Files { get; set; }
        public List<FileProps> Dirs { get; set; }
        public string Path { get; set; }
        public string PathBack { get; set; }
        public LoginData LoginData { get; set; }
        public System.Windows.Controls.DataGrid DataGridMain { get; set; }

        public MyMessageObject Proxy { get; set; }
        public CallBackClient Client { get; set; }

        public BasicProps()
        {
            Path = null;
            PathBack = null;
            LoginData = new LoginData();
            ServerFS = new List<FileProps>();
            Files = new List<FileProps>();
            Dirs = new List<FileProps>();
            DataGridMain = new System.Windows.Controls.DataGrid();
        } // BasicProp


        /// <summary>Соединение с сервером</summary>
        public bool ConnectWithServer()
        {
            BinaryClientFormatterSinkProvider bcfs = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider bsfs = new BinaryServerFormatterSinkProvider {
                TypeFilterLevel = TypeFilterLevel.Full
            };

            IDictionary id = new Hashtable {
                ["port"] = 0,
                ["typeFilterLevel"] = TypeFilterLevel.Full,
                ["name"] = Guid.NewGuid().ToString()
            };

            TcpChannel tcc = new TcpChannel(id, bcfs, bsfs);
            ChannelServices.RegisterChannel(tcc, false);

            string url = $@"tcp://{LoginData.HostName}:{LoginData.Port}/RemoteMsgObj";
            object RemoteObj = Activator.GetObject(typeof(MyMessageObject), url);
            Proxy = (MyMessageObject)RemoteObj;

            try {
                Client = new CallBackClient();
                Proxy.SetClients(Client);
                Path = $@"D:\\Storage\{LoginData.UserName}";
                Proxy.CreateDir(Path);
                ShowFilesAndDirs(Path);
                return true;
            } catch (SocketException) {
                return false;
            } // try-catch
        } // ConnectWithServer
        

        /// <summary>Показать файлы и папки</summary>
        public void ShowFilesAndDirs(string path)
        {
            ServerFS.Clear();
            Files.Clear();
            Dirs.Clear();
            DataGridMain.ItemsSource = null;

            GetFileList(path);
            GetDirList(path);

            DataGridMain.ItemsSource = ServerFS;
        } // ShowFilesAndDirs


        /// <summary>Получить список папок
        /// Для взаимодействия с методом используется поле в главном окне</summary>
        public void GetDirList(string path)
        {
            var temp = Proxy.GetDirectoryList(path);

            foreach (FileProps item in temp) {
                Dirs.Add(item);
            } // foreach

            foreach (FileProps item in Dirs) {
                FileProps prop = new FileProps {
                    Name = item.Name,
                    FullName = item.FullName,
                    CreationTime = item.CreationTime,
                    Extension = item.Extension,
                    Length = item.Length
                };

                ServerFS.Add(prop);
            } // foreach
        } // GetDirList


        /// <summary>Получить список папок
        /// Для взаимодействия с методом используется поле в главном окне</summary>
        public void GetFileList(string path)
        {
            var temp = Proxy.GetFileList(path);

            foreach (var item in temp) {
                Files.Add(item);
            } // foreach

            foreach (FileProps item in Files) {
                FileProps prop = new FileProps {
                    Name = item.Name,
                    FullName = item.FullName,
                    CreationTime = item.CreationTime,
                    Extension = item.Extension,
                    Length = ((item.Length / 1024) / 1024)
                };

                ServerFS.Add(prop);
            } // foreach
        } // GetFileList


        /// <summary>Грузит файл клиента в MemoryStream</summary>
        public MemoryStream FileLoadToMemory(string source)
        {
            try {
                using (FileStream _sourceFile = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    byte[] buffer = new byte[_sourceFile.Length];
                    _sourceFile.Read(buffer, 0, buffer.Length);

                    MemoryStream memory = new MemoryStream();
                    memory.Write(buffer, 0, buffer.Length);

                    return memory;
                } // using
            } catch (Exception ex) {
                MessageBox.Show($"Метод FileLoadToMemory()\n\n{ex.Message}",
                    "Ошибка в загрузке файла в память", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            } // try-catch
        } // FileLoadToMemory


        /// <summary>Сжимает файл из MemoryStream</summary>
        public MemoryStream CompressFileFromMemory(MemoryStream memory)
        {
            try {
                MemoryStream compressMemory = new MemoryStream();
                byte[] buffer = memory.ToArray();

                using (GZipStream compressedzipStream = new GZipStream(compressMemory, CompressionMode.Compress, true)) {
                    compressedzipStream.Write(buffer.ToArray(), 0, buffer.ToArray().Length);
                } // using

                return compressMemory;
            } catch (Exception ex) {
                MessageBox.Show($"Метод CompressFileFromMemory()\n\n{ex.Message}",
                    "Ошибка в сжатии файла", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            } // try-catch
        } // CompressFileFromMemory


        /// <summary>Преобразует сжатый файл из MemoryStream и сохраняет</summary>
        public void GetCompressFileFromMemory(MemoryStream compressMemory, string fileName, string destPath)
        {
            try {
                using (FileStream destFile = File.Create($"{destPath}\\{fileName}")) {
                    compressMemory.WriteTo(destFile);
                    compressMemory.Close();
                } // using
            } catch (Exception ex) {
                MessageBox.Show($"Метод GetCompressFileFromMemory()\n\n{ex.Message}",
                    "Ошибка в разборе файла", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } // try-catch
        } // GetCompressFileFromMemory
    } // BasicProp
} // Client.Model