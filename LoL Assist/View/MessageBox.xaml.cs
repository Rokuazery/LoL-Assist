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
