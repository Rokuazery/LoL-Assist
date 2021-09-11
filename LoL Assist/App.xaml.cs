using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));
            base.OnStartup(e);
        }
    }
}
