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
        private View.ExitMsg ExitMessage;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            var MatchFoundPanel = new View.MatchFoundPanel(Me);
            ConfigPanel = new View.ConfigPanel(BackDrop);
            InfoPanel = new View.InfoPanel(BackDrop);
            ExitMessage = new View.ExitMsg(BackDrop);

            ConfigPanel.Margin = ConfigM.marginClose;
            InfoPanel.Margin = ConfigM.marginClose;
            ExitMessage.Margin = new Thickness(0, 330, 0, 0);

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);
            Grid.SetRowSpan(ExitMessage, 2);
            Grid.SetRowSpan(MatchFoundPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
            MainGrid.Children.Add(ExitMessage);
            MainGrid.Children.Add(MatchFoundPanel);
        }

        private void cStatus_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (cStatus.Text == "Disconnected.")
            {
                gMode.Visibility = Visibility.Hidden;
                wStatus.Visibility = Visibility.Visible;
                ChampionContainer.Visibility = Visibility.Hidden;
                cStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
            }
            else
            {
                gMode.Visibility = Visibility.Visible;
                wStatus.Visibility = Visibility.Hidden;
                ChampionContainer.Visibility = Visibility.Visible;
                cStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }

        private void MinimzieBtn_Clicked(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Info_Clicked(object sender, MouseButtonEventArgs e) => Animate(InfoPanel, ConfigM.marginClose, ConfigM.marginOpen);
        private void SettingsBtn_Clicked(object sender, MouseButtonEventArgs e) => Animate(ConfigPanel, ConfigM.marginClose, ConfigM.marginOpen);
        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Animate(ExitMessage, new Thickness(0, 330, 0, 0), ConfigM.marginOpen, 0.13);

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
    }
}
