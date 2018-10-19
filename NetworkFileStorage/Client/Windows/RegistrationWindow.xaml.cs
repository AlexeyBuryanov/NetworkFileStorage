using System.Windows;
using Client.Model;

namespace Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public Login Login { get; private set; }

        public RegistrationWindow()
        {
            InitializeComponent();
        } // RegistrationWindow


        private void ButtonRegistr_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxUserName.Text) ||
                !string.IsNullOrWhiteSpace(PasswordBoxPassword.Password)) {

                Login login = new Login {
                    UserName = TextBoxUserName.Text,
                    Password = PasswordBoxPassword.Password
                };

                Login = login;

                DialogResult = true;
            } else {
                TextBlockWarning.Text = "Введите логин и пароль";
                TextBlockWarning.Visibility = Visibility.Visible;
            } // if-else
        } // ButtonRegistr_Click
    } // RegistrationWindow
} // Client.Windows