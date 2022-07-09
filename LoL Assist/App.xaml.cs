using System.Windows.Controls;
using LoL_Assist_WAPP.Utils;
using System.Diagnostics;
using System.Windows;
using System;
using LoLA;
using LoL_Assist_WAPP.Model;

namespace LoL_Assist_WAPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RuneModel.Init();
            GlobalConfig.s_Debug = true;
            //Console.Title = "LoL Assist - Debug Console";
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(int.MaxValue));

            if (!GlobalConfig.s_Debug)
            {
                var handle = Helper.GetConsoleWindow();
                Helper.ShowWindow(handle, Helper.SW_HIDE);
            }

            var proccName = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcessesByName(proccName).Length > 1)
            {
                MessageBox.Show("LoL Assist is already running!", "LoL Assist",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }
    }
}
