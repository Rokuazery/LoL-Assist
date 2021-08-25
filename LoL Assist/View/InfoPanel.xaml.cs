using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : UserControl
    {
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public InfoPanel()
        {
            InitializeComponent();
            Version.Text = $"Version {version} - Windows App Edition";
        }

        private void CloseBtn_Clicked(object sender, MouseButtonEventArgs e) => Utils.Animation.Out(this);
    }
}
