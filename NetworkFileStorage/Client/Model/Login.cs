using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.Model
{
    /// <summary>
    /// Класс, объекты которого будут храниться в БД
    /// </summary>
    public class Login : INotifyPropertyChanged
    {
        public int Id { get; set; }


        private string _userName;
        public string UserName {
            get { return _userName; }
            set {
                _userName = value;
                OnPropertyChanged("UserName");
            } // set
        } // UserName


        private string _password;
        public string Password {
            get { return _password; }
            set {
                _password = value;
                OnPropertyChanged("Password");
            } // set
        } // Password


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        } // OnPropertyChanged
    } // Logins
} // Client.Model