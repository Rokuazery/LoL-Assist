using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly View.InfoPanel InfoPanel;
        private readonly View.ConfigPanel ConfigPanel;
        private readonly View.PatchNotesPanel PatchNotesPanel;

        private BuildEditorWindow buildEditorWindow;
        public MainWindow()
        {
            InitializeComponent();

            var MatchFoundPanel = new View.MatchFoundPanel(Me);
            ConfigPanel = new View.ConfigPanel(BackDrop);
            InfoPanel = new View.InfoPanel(BackDrop);
            PatchNotesPanel = new View.PatchNotesPanel(BackDrop);

            ConfigPanel.Margin = ConfigModel.marginClose;
            InfoPanel.Margin = ConfigModel.marginClose;

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);
            Grid.SetRowSpan(MatchFoundPanel, 2);
            Grid.SetRowSpan(PatchNotesPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
            MainGrid.Children.Add(MatchFoundPanel);
            MainGrid.Children.Add(PatchNotesPanel);
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

        private void ConnectionStatus_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            if (ConnectionStatus.Text == "Disconnected")
            {
                GMode.Visibility = Visibility.Hidden;
                wStatus.Visibility = Visibility.Visible;
                ChampionContainer.Visibility = Visibility.Hidden;
                ConnectionStatus.Foreground = new SolidColorBrush(Color.FromRgb(231, 72, 86));
            }
            else
            {
                GMode.Visibility = Visibility.Visible;
                wStatus.Visibility = Visibility.Hidden;
                ChampionContainer.Visibility = Visibility.Visible;
                ConnectionStatus.Foreground = (SolidColorBrush)Application.Current.Resources["FontPrimaryBrush"];
            }
        }
    }
}
