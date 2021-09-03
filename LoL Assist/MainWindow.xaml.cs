using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using LoL_Assist_WAPP.ViewModel;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            var MatchFoundPanel = new View.MatchFoundPanel(Me);

            Grid.SetRowSpan(MatchFoundPanel, 2);
            MainGrid.Children.Add(MatchFoundPanel);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void cStatus_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if(cStatus.Text == "Not Connected!")
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
        private void Info_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.FadeIn(InfPanel);
        private void SettingsBtn_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.FadeIn(ConfPanel);
    }
}
