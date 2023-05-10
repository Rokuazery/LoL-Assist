using LoL_Assist_WAPP.Utils;
using LoLA;
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
    /// Interaction logic for SpellView.xaml
    /// </summary>
    public partial class SpellView : UserControl
    {
        public SpellView()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty SpellProperty =
        DependencyProperty.Register("Spell", typeof(Spell), typeof(SpellView), new PropertyMetadata(null, OnSpellChanged));

        public Spell Spell
        {
            get { return (Spell)GetValue(SpellProperty); }
            set { SetValue(SpellProperty, value); }
        }


        public static void OnSpellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spellPreviewControl = (SpellView)d;
            if (spellPreviewControl.Spell == null)
            {
                spellPreviewControl.Visibility = Visibility.Collapsed;
                return;
            }
            spellPreviewControl.Visibility = Visibility.Visible;
            spellPreviewControl.SetFirstSpellName();
            spellPreviewControl.SetFirstSpellImage();
            spellPreviewControl.SetFSecondSpellName();
            spellPreviewControl.SetSecondSpellImage();
        }

        public void SetFirstSpellImage()
        {
            if (string.IsNullOrEmpty(Spell.First)) return;

            First.ImageSource = new BitmapImage(new Uri(Helper.ImageSrc(Spell.First)));
        }

        public void SetFirstSpellName()
        {
            if (string.IsNullOrEmpty(Spell.First)) return;

            FirstSpellName.Text = DataConverter.SpellIdToSpellName(Spell.First);
        }

        public void SetSecondSpellImage()
        {
            if (string.IsNullOrEmpty(Spell.Second)) return;

            Second.ImageSource = new BitmapImage(new Uri(Helper.ImageSrc(Spell.Second)));
        }

        public void SetFSecondSpellName()
        {
            if (string.IsNullOrEmpty(Spell.Second)) return;

            SecondSpellName.Text = DataConverter.SpellIdToSpellName(Spell.Second);
        }
    }
}
