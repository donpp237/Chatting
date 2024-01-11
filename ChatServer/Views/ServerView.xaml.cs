using System.Windows;

namespace ChatServer.Views
{
    /// <summary>
    /// ServerView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ServerView : Window
    {
        public ServerView()
        {
            InitializeComponent();

            this.Title = "서버";
            this.Width = 500;
        }
    }
}
