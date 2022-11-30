using LoL_Assist_WAPP.ViewModels;
using System.Windows.Navigation;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using LoL_Assist_WAPP.Utils;
using System.Diagnostics;
using System.Windows;
using System.IO;
using LoLA;

namespace LoL_Assist_WAPP.Views
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

        void close()
        {
            Animation.FadeOut(backDrop);
            Animation.Margin(this, ConfigModel.r_MarginOpen, ConfigModel.r_MarginClose);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void CheckForUpdatesLink_Click(object sender, RoutedEventArgs e) 
        {
            CheckForUpdatesContent.Text = "Checking for updates...";
            await Updater.Start();
            CheckForUpdatesContent.Text = "Check for updates";
        }

        private void clearCacheBtn_Click(object sender, RoutedEventArgs e)
        {
            MsgBox exitMsg = new MsgBox("By clicking 'Yes' all folder in LoLA Data which contains images and data for the the champions will be deleted permanently. Do you want to continue this action?", 240, 140);
            exitMsg.Margin = new Thickness(0, Height, 0, 0);
            MainGrid.Children.Add(exitMsg);
            Grid.SetRowSpan(exitMsg, 2);
            exitMsg.Decided += delegate (bool result)
            {
                var path1 = LibInfo.r_LibFolderPath + "\\Champions";
                var path2 = LibInfo.r_LibFolderPath + "\\Custom Builds";

                if (result && Directory.Exists(path1))
                    Directory.Delete(path1, true);

                if (result && Directory.Exists(path2))
                    Directory.Delete(path2, true);


                Animation.FadeOut(BackDrop, 0.13);
                Animation.Margin(exitMsg, ConfigModel.r_MarginOpen, new Thickness(0, Height, 0, 0), 0.13);
            };
            Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.r_MarginOpen, 0.13);
        }

        private void Animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Animation.FadeIn(BackDrop, time);
            Animation.Margin(element, from, to, time);
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
                    ConfigModel.s_Config = new ConfigModel.Config();
                    ConfigModel.SaveConfig();
                    ConfigViewModel.Update();
                }

                Animation.FadeOut(BackDrop, 0.13);
                Animation.Margin(exitMsg, ConfigModel.r_MarginOpen, new Thickness(0, Height, 0, 0), 0.13);
            };
            Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.r_MarginOpen, 0.13);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e) => close();
    }
}
