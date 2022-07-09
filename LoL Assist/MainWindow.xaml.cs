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
        public MainWindow()
        {
            InitializeComponent();

            ConfigPanel = new View.ConfigPanel(BackDrop);
            InfoPanel = new View.InfoPanel(BackDrop);

            ConfigPanel.Margin = ConfigModel.r_MarginClose;
            InfoPanel.Margin = ConfigModel.r_MarginClose;

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
        }

        private void Animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.Margin(element, from, to, time);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e) => Animate(ConfigPanel, ConfigModel.r_MarginClose, ConfigModel.r_MarginOpen);
        private void InfoBtn_Click(object sender, RoutedEventArgs e) => Animate(InfoPanel, ConfigModel.r_MarginClose, ConfigModel.r_MarginOpen);
    }
}
