using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    /// <summary>
    /// Данные подключения текущего юзера
    /// </summary>
    public class LoginData
    {
        public string HostName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
    } // LoginData
} // Client.Model