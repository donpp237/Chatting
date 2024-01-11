using System.Windows;

namespace ChatClient.Views
{
    /// <summary>
    /// ClientView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClientView : Window
    {
        public ClientView()
        {
            InitializeComponent();

            this.Title = "클라이언트";
            Width = 500;
        }

        private void ConnectBtnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(userName.Text) == true)
                return;

            connectArea.IsEnabled = false;

            inputArea.IsEnabled = true;
        }
    }
}
