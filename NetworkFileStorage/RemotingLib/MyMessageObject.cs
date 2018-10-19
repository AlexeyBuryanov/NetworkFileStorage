using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace RemotingLib
{
    // Типы, задающие ссылки на методы у клиента
    public delegate void ClientsListHandler(List<string> clients);

    /// <summary>
    /// Класс, хранящий информацию об одном подключенном клиенте
    /// </summary>
    public class CallBackClient : MarshalByRefObject
    {
        // Логин подключившегося клиента
        public string userName = "";

        // Поля класса, которые хранят адреса методов у клиента, эти методы вызываются с сервера
        public event ClientsListHandler OnSendClientsList;

        public void SendClientsList(List<string> clients)
        {
            // Если сетевой адрес указан, то вызвать метод у клиента
            OnSendClientsList?.Invoke(clients);
        } // SendClientsList
    } // CallBackClient


    /// <summary>
    /// Объект-прокси
    /// </summary>
    public class MyMessageObject : MarshalByRefObject
    {
        // Список подключенных к серверу клиентов
        private List<CallBackClient> _clients;
        public static Hashtable _logins;

        public MyMessageObject()
        {
            _clients = new List<CallBackClient>();
        } // MyMessageObject


        delegate void SetClientsCallback(CallBackClient client, List<string> list);
        public void DComplete(IAsyncResult res) { }
        public void SendTo(CallBackClient client, List<string> list)
        {
            if (client != null)
                client.SendClientsList(list);
        } // SendTo
        // Метод добавляет вновь связавшегося клиента в список подключенных клиентов
        public void SetClients(CallBackClient cl)
        {
            _clients.Add(cl);

            List<string> logins = new List<string>();
            foreach (CallBackClient client in _clients) {
                logins.Add(client.userName);
            } // foreach

            // Перебрать всех подлюченных клиентов
            foreach (CallBackClient client in _clients) {
                // Синхронный вызов callback-метода
                //client.SendClientsList(logins_lst);

                // Асинхронный вызов callback-метода
                SetClientsCallback d = new SetClientsCallback(SendTo);
                d.BeginInvoke(client, logins, new AsyncCallback(DComplete), null);
            } // foreach
        } // SetClients


        //*****************************************************************************//

        /// <summary>Возвращает список файлов на сервере</summary>
        /// <returns>Список объектов типа FileProps со всеми свойствами файла.
        /// Но на самом деле массив, а не список.</returns>
        public List<FileProps> GetFileList(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fi = dir.GetFiles();

            List<FileProps> res = new List<FileProps>();
            foreach (FileInfo f in fi) {
                FileProps fp = new FileProps(f.Name, f.FullName, f.CreationTime, f.Extension, f.Length);
                res.Add(fp);
            } // foreach
            return res;
        } // GetFileList


        /// <summary>Возвращает список подкаталогов текущего каталога</summary>
        /// <returns>Список объектов типа FileProps со всеми свойствами файла.
        /// Но на самом деле массив, а не список.</returns>
        public List<FileProps> GetDirectoryList(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] di = dir.GetDirectories();

            List<FileProps> res = new List<FileProps>();
            foreach (DirectoryInfo d in di) {
                FileProps fp = new FileProps(d.Name, d.FullName, d.CreationTime, "Папка с файлами", 0);
                res.Add(fp);
            } // foreach
            return res;
        } // GetFileList


        /// <summary>Создание каталога</summary>
        public void CreateDir(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists) {
                dirInfo.Create();
            } // if
        } // CreateDir


        /// <summary>Удаление каталога</summary>
        public void DeleteDir(string dirName)
        {
            try {
                DirectoryInfo dirInfo = new DirectoryInfo(dirName);
                dirInfo.Delete(true);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Удаление папки невозможно", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } // try-catch
        } // DeleteDir


        /// <summary>Удаление файла</summary>
        public void DeleteFile(string path)
        {
            try {
                if (File.Exists(path)) {
                    File.Delete(path);
                } // if
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Удаление файла невозможно", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } // try-catch
        } // DeleteDir


        /// <summary>Преобразует сжатый файл из MemoryStream и сохраняет</summary>
        public void GetCompressFileFromMemory(MemoryStream compressMemory, string fileName, string destPath)
        {
            try {
                using (FileStream destFile = File.Create($"{destPath}\\{fileName}")) {
                    compressMemory.WriteTo(destFile);
                } // using
            } catch (Exception ex) {
                MessageBox.Show($"Метод GetCompressFileFromMemory()\n\n{ex.Message}",
                    "Ошибка в разборе файла", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } // try-catch
        } // GetCompressFileFromMemory


        /// <summary>Расжимает файл сервера в MemoryStream и сохраняет в том же месте</summary>
        public void DecompressFile(string source)
        {
            try {
                using (FileStream sourceFile = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    GZipStream zipStream = new GZipStream(sourceFile, CompressionMode.Decompress);

                    MemoryStream memory = new MemoryStream();

                    int b = zipStream.ReadByte();
                    while (b != -1) {
                        memory.WriteByte((byte)b);
                        b = zipStream.ReadByte();
                    } // while

                    using (FileStream destFile = File.Create($"{source}~")) {
                        memory.WriteTo(destFile);
                    } // using
                } // using
            } catch (Exception ex) {
                MessageBox.Show($"Метод DecompressFileToMemory()\n\n{ex.Message}",
                    "Ошибка в разборе файла", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } // try-catch
        } // DecompressFile
    } // MyMessageObject
} // RemotingLib