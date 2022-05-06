using LoL_Assist_WAPP.ViewModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Threading.Tasks;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Net;
using LoLA.Utils;
using System.IO;
using System;
using LoLA;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for ConfigPanel.xaml
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        private readonly Border backDrop = new Border();
        ConfigViewModel ConfigViewModel = new ConfigViewModel();
        public ConfigPanel(Border border)
        {
            InitializeComponent();
            backDrop = border;
            DataContext = ConfigViewModel;
        }

        void Close()
        {
            Utils.Animation.FadeOut(backDrop);
            Utils.Animation.Margin(this, ConfigModel.marginOpen, ConfigModel.marginClose);
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Close();

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void CheckForUpdatesLink_Click(object sender, RoutedEventArgs e) 
        {
            CheckForUpdatesContent.Text = "Checking for updates...";
            await Update.Start();
            CheckForUpdatesContent.Text = "Check for updates";
        }

        private void clearCacheBtn_Click(object sender, RoutedEventArgs e)
        {
            MsgBox exitMsg = new MsgBox("By clicking 'Yes' Champions folder which contains images and data for the the champion will be deleted permanently. Do you want to continue this action?", 230, 130);
            exitMsg.Margin = new Thickness(0, Height, 0, 0);
            MainGrid.Children.Add(exitMsg);
            Grid.SetRowSpan(exitMsg, 2);
            exitMsg.Decided += delegate (bool result)
            {
                var path = Global.libraryFolder + "\\Champions";
                if (result && Directory.Exists(path))
                    Directory.Delete(path, true);

                Utils.Animation.FadeOut(BackDrop, 0.13);
                Utils.Animation.Margin(exitMsg, ConfigModel.marginOpen, new Thickness(0, Height, 0, 0), 0.13);
            };
            Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.marginOpen, 0.13);
        }

        private void Animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.Margin(element, from, to, time);
        }

        private void resetConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            MsgBox exitMsg = new MsgBox("By clicking 'Yes' LoL Assist config will be reset to the default value. Do you want to continue this action?", 230, 130);
            exitMsg.Margin = new Thickness(0, Height, 0, 0);
            MainGrid.Children.Add(exitMsg);
            Grid.SetRowSpan(exitMsg, 2);
            exitMsg.Decided += delegate (bool result)
            {
                if (result)
                {
                    ConfigModel.config = new ConfigModel.Config();
                    ConfigModel.SaveConfig();
                    ConfigViewModel.Update();
                }

                Utils.Animation.FadeOut(BackDrop, 0.13);
                Utils.Animation.Margin(exitMsg, ConfigModel.marginOpen, new Thickness(0, Height, 0, 0), 0.13);
            };
            Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.marginOpen, 0.13);
        }
    }
}
