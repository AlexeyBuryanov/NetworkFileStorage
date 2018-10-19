/*==========================================================
**
** Логика взаимодействия для StartAppWindow.xaml
**
** Copyright(c) Alexey Bur'yanov, 2017
** <OWNER>Alexey Bur'yanov</OWNER>
** 
===========================================================*/

namespace Client.Windows
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Windows;
    using Client.Model;

    public partial class StartAppWindow : Window
    {
        public LoginContext _db;

        public StartAppWindow()
        {
            InitializeComponent();

            _db = new LoginContext();

            // Загружает данные из таблицы в локальный кэш контекста данных
            _db.Logins.Load();
        } // StartAppWindow


        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } // ButtonExit_Click


        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxUserName.Text) &&
                !string.IsNullOrWhiteSpace(PasswordBoxPassword.Password.ToString())) {

                try {
                    // Проверяем введённые данные на авторизацию
                    Login login = await _db.Logins
                        .Where(l => l.UserName == TextBoxUserName.Text && l.Password == PasswordBoxPassword.Password)
                        .FirstAsync();

                    DialogResult = true;
                } catch (Exception) {
                    TextBlockWarning.Text = "Такого пользователя не существует!";
                    TextBlockWarning.Visibility = Visibility.Visible;
                } // try-catch
            } // if
        } // ButtonLogin_Click


        private async void HyperLinkRegistration_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            RegistrationWindow win = new RegistrationWindow();
            bool? flagExit = win.ShowDialog();

            // Если регистрация юзера прошла, добавляем пользователя в базу
            if (flagExit.Value == true) {
                _db.Logins.Add(win.Login);
                int x = await _db.SaveChangesAsync();
            } // if

            ShowDialog();
        } // HyperLinkRegistration_Click
    } // StartAppWindow
} // Client