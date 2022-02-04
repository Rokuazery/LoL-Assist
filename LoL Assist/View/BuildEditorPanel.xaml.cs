using LoL_Assist_WAPP.Model;
using LoL_Assist_WAPP.ViewModel;
using System;
using System.Windows.Controls;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for BuildEditorPanel.xaml
    /// </summary>
    public partial class BuildEditorPanel : UserControl
    {
        public BuildEditorPanel()
        {
            InitializeComponent();
            DataContext = new BuildEditorViewModel();
        }

        private void CloseBtn_Clicked(object sender, System.Windows.Input.MouseButtonEventArgs e) => Utils.Animation.FadeOut(this);
    }
}
