using LoL_Assist_WAPP.Utils;
using LoLA.Data;
using LoLA.Networking.WebWrapper.DataDragon.Data;
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
    /// Interaction logic for RuneView.xaml
    /// </summary>
    public partial class RuneView : UserControl
    {
        public RuneView()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty RuneProperty =
        DependencyProperty.Register("Rune", typeof(Rune), typeof(RuneView), new PropertyMetadata(null, OnRuneChanged));

        public Rune Rune
        {
            get { return (Rune)GetValue(RuneProperty); }
            set { SetValue(RuneProperty, value); }
        }

        public static void OnRuneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var runeViewControl = (RuneView)d;
            if (runeViewControl.Rune == null)
            {
                runeViewControl.Visibility = Visibility.Collapsed;
                return;
            }

            runeViewControl.Visibility = Visibility.Visible;
            runeViewControl.SetRuneName();
            runeViewControl.SetPrimaryPath();
            runeViewControl.SetKeystone();
            runeViewControl.SetSlot1();
            runeViewControl.SetSlot2();
            runeViewControl.SetSlot3();
            runeViewControl.SetSecondary();
            runeViewControl.SetSlot4();
            runeViewControl.SetSlot5();
        }

        public void SetRuneName()
        {
            if (string.IsNullOrEmpty(Rune.Name)) return;

            RuneName.Text = Rune.Name;
        }

        public void SetPrimaryPath()
        {
            if (Rune.PrimaryPath == 0) return;

            var primary = Converter.PathIdToName(Rune.PrimaryPath);
            Primary.Source = new BitmapImage(new Uri(Helper.ImageSrc(primary)));
        }

        public void SetKeystone()
        {
            if (Rune.Keystone == 0) return;

            var keystone = Converter.PerkIdToName(Rune.Keystone);
            Keystone.Source = new BitmapImage(new Uri(Helper.ImageSrc(keystone)));
        }

        public void SetSecondary()
        {
            if (Rune.SecondaryPath == 0) return;

            var secondary = Converter.PathIdToName(Rune.SecondaryPath);
            Secondary.Source = new BitmapImage(new Uri(Helper.ImageSrc(secondary)));
        }

        public void SetSlot1()
        {
            if (Rune.Slot1 == 0) return;

            var s1 = Converter.PerkIdToName(Rune.Slot1);
            S1.Source = new BitmapImage(new Uri(Helper.ImageSrc(s1)));
        }

        public void SetSlot2()
        {
            if (Rune.Slot2 == 0) return;

            var s2 = Converter.PerkIdToName(Rune.Slot2);
            S2.Source = new BitmapImage(new Uri(Helper.ImageSrc(s2)));
        }

        public void SetSlot3()
        {
            if (Rune.Slot3 == 0) return;

            var s3 = Converter.PerkIdToName(Rune.Slot3);
            S3.Source = new BitmapImage(new Uri(Helper.ImageSrc(s3)));
        }

        public void SetSlot4()
        {
            if (Rune.Slot4 == 0) return;

            var s4 = Converter.PerkIdToName(Rune.Slot4);
            S4.Source = new BitmapImage(new Uri(Helper.ImageSrc(s4)));
        }

        public void SetSlot5()
        {
            if (Rune.Slot5 == 0) return;

            var s5 = Converter.PerkIdToName(Rune.Slot5);
            S5.Source = new BitmapImage(new Uri(Helper.ImageSrc(s5)));
        }
    }
}
