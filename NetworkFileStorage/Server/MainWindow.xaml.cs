using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using RemotingLib;

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            StartServer();
        } // MainWindow


        private void StartServer()
        {
            BinaryServerFormatterSinkProvider serverFormatter =
                new BinaryServerFormatterSinkProvider {
                    TypeFilterLevel = TypeFilterLevel.Full
                };

            IDictionary ht = new Hashtable {
                ["name"] = "ServerFileStorage",
                ["port"] = 32469
            };

            TcpChannel channel = new TcpChannel(ht, null, serverFormatter);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MyMessageObject), "RemoteMsgObj", WellKnownObjectMode.Singleton);
            
            CallBackClient client = new CallBackClient();
            client.OnSendClientsList += new ClientsListHandler(Msg_OnSendClientsList);
            client.userName = "Admin";
            MyMessageObject proxy = GetProxy();
            proxy.SetClients(client);

            Status.Text = "Сервер запущен. Ожидание подключений...";
        } // StartServer


        private MyMessageObject GetProxy()
        {
            return (MyMessageObject)Activator.GetObject(typeof(MyMessageObject), @"tcp://127.0.0.1:32469/RemoteMsgObj");
        } // GetProxy


        // При добавлении клиентов
        private delegate void ConnectedUsersInvoker(List<string> clients);
        private void ConnectedUsersAdd(List<string> clients)
        {
            Connected.Text = Convert.ToString(clients.Count-1);
        } // ConnectedUsersAdd
        private void Msg_OnSendClientsList(List<string> clients)
        {
            // Если нет доступа к текущему потоку
            if (!Dispatcher.CheckAccess()) {
                Dispatcher.Invoke(new ConnectedUsersInvoker(ConnectedUsersAdd), clients);
            } else {
                Connected.Text = Convert.ToString(clients.Count-1);
            } // if-else
        } // Msg_OnSendClientsList
    } // MainWindow
} // Server