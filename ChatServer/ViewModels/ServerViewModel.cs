using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;

using ChatServer.Models;

namespace ChatServer.ViewModels
{
    public class ServerViewModel : BindingHelper.Helper
    {
        #region Binding

        public string Chat
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string SendContent
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public ObservableCollection<string> ConnectedClients { get; set; } = new ObservableCollection<string>();

        public ICommand SendCommand => GetCommand((obj) => Send());


        #endregion

        Server m_server;

        public ServerViewModel()
        {
            m_server = new Server();
            m_server.ReceivedEvent += ReceivedMessage;
            m_server.ConnectClientEvent += ConnectClient;
        }

        private void Send()
        {
            string sendMessage = $"[System]: {SendContent}";

            m_server.Send(sendMessage);
            ReceivedMessage(sendMessage);

            SendContent = "";
        }

        private void ConnectClient()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                ConnectedClients.Clear();

                var clients = m_server.GetConnectedClients();
                for (int idx = 0; idx < clients.Count; idx++)
                {
                    string client = clients[idx];
                    ConnectedClients.Add(client);
                }
            });
        }

        private void ReceivedMessage(string message)
        {
            Chat += $"\n{message}";
        }
    }
}
