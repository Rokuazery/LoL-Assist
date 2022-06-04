using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LoL_Assist_WAPP.ViewModel;

namespace LoL_Assist_WAPP.View
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
            var data = ((MessageBoxViewModel)DataContext);
            MsgBox exitMsg = new MsgBox(data.message, data.width, data.height);
            exitMsg.Opacity = 0;
            exitMsg.Visibility = Visibility.Collapsed;
            MsgContainer.Children.Add(exitMsg);
            exitMsg.Decided += delegate (bool result)
            {
                if (result) data.action.Invoke();
                Utils.Animation.FadeOut(this);
            };
            Utils.Animation.FadeIn(exitMsg);
        }
    }
}
