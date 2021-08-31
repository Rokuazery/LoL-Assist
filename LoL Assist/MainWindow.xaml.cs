using LoL_Assist_WAPP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using LoLA;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Environment.Exit(69);
        private void MinimzieBtn_Clicked(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void cStatus_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            if(cStatus.Text == "Not Connected!")
            {
                ChampionContainer.Visibility = Visibility.Hidden;
                cStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
            }
            else
            {
                ChampionContainer.Visibility = Visibility.Visible; 
                cStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }

        private void Info_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.In(InfPanel);
        private void SettingsBtn_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.In(ConfPanel);
    }
}
