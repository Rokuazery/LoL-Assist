using LoL_Assist_WAPP.ViewModels;
using System.Windows.Controls;
using LoL_Assist_WAPP.Models;
using System.Windows;
using System;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Views.InfoPanel InfoPanel;
        private readonly Views.ConfigPanel ConfigPanel;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            ConfigPanel = new Views.ConfigPanel(BackDrop);
            InfoPanel = new Views.InfoPanel(BackDrop);

            ConfigPanel.Margin = ConfigModel.r_MarginClose;
            InfoPanel.Margin = ConfigModel.r_MarginClose;

            Grid.SetRowSpan(InfoPanel, 2);
            Grid.SetRowSpan(ConfigPanel, 2);

            MainGrid.Children.Add(InfoPanel);
            MainGrid.Children.Add(ConfigPanel);
        }

        private void animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.Margin(element, from, to, time);
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e) => animate(InfoPanel, ConfigModel.r_MarginClose, ConfigModel.r_MarginOpen);
        private void OptionsBtn_Click(object sender, RoutedEventArgs e) => animate(ConfigPanel, ConfigModel.r_MarginClose, ConfigModel.r_MarginOpen);
    }
}
