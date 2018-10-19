using System;
using System.Threading;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        public Mutex Mutex { get; set; }

        public App()
        {
            InitializeComponent();

            // Перехват необработанных исключений
            DispatcherUnhandledException += (e, arg) =>
                MessageBox.Show(arg.Exception.Message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        } // App


        // Точка входа приложения
        [STAThread]
        private static void Main()
        {
            App app = new App();
            MainWindow win = new MainWindow();
            app.Run(win);
        }  // Main


        // Запуск только одного экземпляра приложения
        private void App_Startup(object sender, StartupEventArgs e)
        {
            const string mutexName = "AppMutexUnique123"; 

            Mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew) {
                Shutdown();
            } // if
        } // Appl_Startup
    } // class App
} // NetworkFileStorage