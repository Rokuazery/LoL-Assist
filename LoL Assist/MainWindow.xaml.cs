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
        private View.InfoPanel InfoPanel;
        private View.ConfigPanel ConfigPanel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            var MatchFoundPanel = new View.MatchFoundPanel(Me);
            ConfigPanel = new View.ConfigPanel(BackDrop);
            InfoPanel = new View.InfoPanel(BackDrop);

            ConfigPanel.Margin = ConfigM.marginClose;
            InfoPanel.Margin = ConfigM.marginClose;

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);
            Grid.SetRowSpan(MatchFoundPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
            MainGrid.Children.Add(MatchFoundPanel);
        }

        private void cStatus_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (cStatus.Text == "Disconnected.")
            {
                gMode.Visibility = Visibility.Hidden;
                ChampionContainer.Visibility = Visibility.Hidden;
                cStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
            }
            else
            {
                gMode.Visibility = Visibility.Visible;
                ChampionContainer.Visibility = Visibility.Visible;
                cStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Environment.Exit(69);
        private void MinimzieBtn_Clicked(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Info_Clicked(object sender, MouseButtonEventArgs e)
        {
            Utils.Animation.FadeIn(BackDrop);
            Utils.Animation.Margin(InfoPanel, ConfigM.marginClose, ConfigM.marginOpen, 0.2);
        }
        private void SettingsBtn_Clicked(object sender, MouseButtonEventArgs e) 
        {
            Utils.Animation.FadeIn(BackDrop);
            Utils.Animation.Margin(ConfigPanel, ConfigM.marginClose, ConfigM.marginOpen);
        }

        private void Me_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
