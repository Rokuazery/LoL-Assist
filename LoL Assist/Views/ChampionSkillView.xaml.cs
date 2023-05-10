using LoLA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LoL_Assist_WAPP.Views
{
    /// <summary>
    /// Interaction logic for ChampionSkillView.xaml
    /// </summary>
    public partial class ChampionSkillView : UserControl
    {
        public ChampionSkillView()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty ChampionSkillProperty =
        DependencyProperty.Register("ChampionSkill", typeof(ChampionSkill), typeof(ChampionSkillView), new PropertyMetadata(null, OnChampionSkillChanged));

        public ChampionSkill ChampionSkill
        {
            get { return (ChampionSkill)GetValue(ChampionSkillProperty); }
            set { SetValue(ChampionSkillProperty, value); }
        }

        public static void OnChampionSkillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var championSkillControl = (ChampionSkillView)d;
            if (championSkillControl.ChampionSkill == null)
            {
                championSkillControl.Visibility = Visibility.Collapsed;
                return;
            }

            championSkillControl.Visibility = Visibility.Visible;
            championSkillControl.P1.Text = championSkillControl.ChampionSkill.Priority[0].ToString();
            championSkillControl.P2.Text = championSkillControl.ChampionSkill.Priority[1].ToString();
            championSkillControl.P3.Text = championSkillControl.ChampionSkill.Priority[2].ToString();
            championSkillControl.Order.ItemsSource = championSkillControl.ChampionSkill.Order;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0) 
                scrollviewer.LineRight();
            else
                scrollviewer.LineLeft();
            e.Handled = true;
        }
    }
}
