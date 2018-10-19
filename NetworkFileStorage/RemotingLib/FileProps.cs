using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotingLib
{
    [Serializable]
    public class FileProps
    {
        public FileProps() {}
        public FileProps(string Name, string FullName, DateTime CreationTime, string Extension, long Length)
        {
            this.Name = Name;
            this.FullName = FullName;
            this.CreationTime = CreationTime;
            this.Extension = Extension;
            this.Length = Length;
        } // FileProps


        // Имя (файла/каталога)
        public string Name { get; set; }
        // Полный путь
        public string FullName { get; set; }
        // Дата создания
        public DateTime CreationTime { get; set; }
        // Тип
        public string Extension { get; set; }
        // Размер
        public long Length { get; set; }
    } // FileProps
} // ClassLib