using LoL_Assist_WAPP.ViewModel;
using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows.Input;
using LoL_Assist_WAPP.View;
using System.Windows;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for BuildEditorWindow.xaml
    /// </summary>
    public partial class BuildEditorWindow : Window
    {
        BuildEditorViewModel viewModel = new BuildEditorViewModel();
        public BuildEditorWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Close();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(viewModel.SelectedBuildName))
            {
                MsgBox exitMsg = new MsgBox($"Are you sure you want to delete '{viewModel.SelectedBuildName}'?", 260, 160);
                exitMsg.Margin = new Thickness(0, Height, 0, 0);
                MainGrid.Children.Add(exitMsg);
                Grid.SetRowSpan(exitMsg, 3);
                exitMsg.Decided += delegate (bool result)
                {
                    if (result)
                        viewModel.DeleteConfigExecute();

                    Utils.Animation.FadeOut(BackDrop, 0.13);
                    Utils.Animation.Margin(exitMsg, ConfigModel.marginOpen, new Thickness(0, Height, 0, 0), 0.13);
                };
                Animate(exitMsg, new Thickness(0, Height, 0, 0), ConfigModel.marginOpen, 0.13);
            }
        }

        private void Animate(FrameworkElement element, Thickness from, Thickness to, double time = 0.2)
        {
            Utils.Animation.FadeIn(BackDrop, time);
            Utils.Animation.Margin(element, from, to, time);
        }
    }
}
