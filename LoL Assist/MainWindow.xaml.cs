using LoL_Assist_WAPP.ViewModel;
using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;
using System;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly View.InfoPanel InfoPanel;
        private readonly View.ConfigPanel ConfigPanel;
        //private readonly View.BuildEditorPanel BuildEditorPanel;

        private BuildEditorWindow buildEditorWindow;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            var MatchFoundPanel = new View.MatchFoundPanel(Me);
            ConfigPanel = new View.ConfigPanel(BackDrop);
            InfoPanel = new View.InfoPanel(BackDrop);
            //BuildEditorPanel = new View.BuildEditorPanel();

            ConfigPanel.Margin = ConfigModel.marginClose;
            InfoPanel.Margin = ConfigModel.marginClose;

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);
            Grid.SetRowSpan(MatchFoundPanel, 2);
            //Grid.SetRowSpan(BuildEditorPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
            MainGrid.Children.Add(MatchFoundPanel);
            //MainGrid.Children.Add(BuildEditorPanel);
        }

        private void CStatus_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (cStatus.Text == "Disconnected.")
            {
                GMode.Visibility = Visibility.Collapsed;
                wStatus.Visibility = Visibility.Visible;
                ChampionContainer.Visibility = Visibility.Collapsed;
                cStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
            }
            else
            {
                GMode.Visibility = Visibility.Visible;
                wStatus.Visibility = Visibility.Collapsed;
                ChampionContainer.Visibility = Visibility.Visible;
                cStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }

        private void MinimzieBtn_Clicked(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Info_Clicked(object sender, MouseButtonEventArgs e) => Animate(InfoPanel, ConfigModel.marginClose, ConfigModel.marginOpen);
        private void SettingsBtn_Clicked(object sender, MouseButtonEventArgs e) => Animate(ConfigPanel, ConfigModel.marginClose, ConfigModel.marginOpen);
        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            View.MsgBox exitMsg = new View.MsgBox("Are you sure you want to exit LoL Assist?");
            exitMsg.Margin = new Thickness(0, Height, 0, 0);
            MainGrid.Children.Add(exitMsg);
            Grid.SetRowSpan(exitMsg, 2);
            exitMsg.Decided += delegate (bool result)
            {
                if (result) Environment.Exit(0);
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

        private void Me_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void EditBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            buildEditorWindow = new BuildEditorWindow();
            buildEditorWindow.Owner = this;
            buildEditorWindow.ShowDialog();
        }
    }
}
