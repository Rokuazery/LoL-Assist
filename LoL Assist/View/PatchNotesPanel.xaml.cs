using LoL_Assist_WAPP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for PatchNotesPanel.xaml
    /// </summary>
    public partial class PatchNotesPanel : UserControl
    {
        private readonly Border backDrop = new Border();
        public PatchNotesPanel(Border border)
        {
            InitializeComponent();
            backDrop = border;
            Visibility = Visibility.Collapsed;
            Opacity = 0;

            if (!ConfigModel.config.DoNotShowPatch)
                Open();
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Close();
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        void Close()
        {
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.FadeOut(this);
        }

        async void Open()
        {
            await Task.Delay(550);
            Utils.Animation.FadeIn(backDrop);
            Utils.Animation.FadeIn(this, 0.5);
        }

        private void GithubLink_RequestNavigate(object sender, RequestNavigateEventArgs e) => Process.Start(e.Uri.ToString());
    }
}
