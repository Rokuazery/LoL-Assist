using System.Windows.Controls;
using LoL_Assist_WAPP.Model;
using System.Windows;
using System;
using LoLA;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Global.Config.debug = false;
            Console.Title = "LoL Assist - Debug Console";
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

            if (!Global.Config.debug)
            {
                var handle = Utils.GetConsoleWindow();
                Utils.ShowWindow(handle, Utils.SW_HIDE);
            }
            base.OnStartup(e);
        }
    }
}
