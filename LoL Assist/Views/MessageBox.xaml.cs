using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Models;
using System.Windows;

namespace LoL_Assist_WAPP.Views
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : UserControl
    {
        public MessageBox()
        {
            InitializeComponent();
        }

        private async void msgBox_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2);
            var data = ((MessageBoxModel)DataContext);
            MsgBox msgBox = new MsgBox(data.Message, data.Width, data.Height);
            msgBox.Opacity = 0;
            msgBox.Visibility = Visibility.Collapsed;
            MsgContainer.Children.Add(msgBox);
            msgBox.Decided += delegate (bool result)
            {
                if (result) data.Action.Invoke();
                Utils.Animation.FadeOut(this);
            };
            Utils.Animation.FadeIn(msgBox);
        }
    }
}
