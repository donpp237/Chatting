using System.Windows.Input;

using ChatClient.Models;

namespace ChatClient.ViewModels
{
    class ClientViewModel : BindingHelper.Helper
    {
        #region Binding

        public string UserName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

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

        public ICommand ConnectCommand => GetCommand((obj) => Connect());

        public ICommand SendCommand => GetCommand((obj) => Send());


        #endregion

        Client m_client;

        public ClientViewModel()
        {
            m_client = new Client();
            m_client.ReceivedEvent += ReceivedMessage;
        }

        private void Connect()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                System.Windows.MessageBox.Show("대화명을 입력하세요!");
                return;
            }

            m_client.Connect(UserName, "10.10.83.189", 5000);
        }

        private void Send()
        {
            if (string.IsNullOrEmpty(UserName) == true)
                UserName = "None";

            m_client.Send($"[{UserName}]: {SendContent}");
            ReceivedMessage($"[Me]: {SendContent}");

            SendContent = "";
        }

        private void ReceivedMessage(string message)
        {
            Chat += $"\n{message}";
        }
    }
}
