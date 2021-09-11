using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;

namespace LoL_Assist_WAPP.View
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>
    public partial class IconButton : UserControl
    {
        public delegate void ClickedHendler(object sender, MouseButtonEventArgs e);
        public event ClickedHendler Clicked;

        public Geometry IconGeometry
        {
            get
            {
                return (Geometry)GetValue(IconGeometryProperty);
            }
            set
            {
                SetValue(IconGeometryProperty, value);
            }
        }

        public Stretch IconStretch
        {
            get
            {
                return (Stretch)GetValue(IconStretchProperty);
            }
            set
            {
                SetValue(IconStretchProperty, value);
            }
        }

        public Color OnHoverColor
        {
            get
            {
                return (Color)GetValue(OnHoverColorPropery);
            }
            set
            {
                SetValue(OnHoverColorPropery, value);
            }
        }

        public Color StaticColor
        {
            get
            {
                return (Color)GetValue(StaticColorProperty);
            }
            set
            {
                SetValue(StaticColorProperty, value);
            }
        }

        public static readonly DependencyProperty OnHoverColorPropery = DependencyProperty.Register("OnHoverColor", typeof(Color), typeof(IconButton), new FrameworkPropertyMetadata(Color.FromRgb(255, 255, 255)));
        public static readonly DependencyProperty StaticColorProperty = DependencyProperty.Register("StaticColor", typeof(Color), typeof(IconButton), new FrameworkPropertyMetadata(null));

        readonly DependencyPropertyDescriptor StaticColorPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(StaticColorProperty, typeof(IconButton));

        public static readonly DependencyProperty IconStretchProperty = DependencyProperty.Register("IconStretch", typeof(Stretch), typeof(IconButton), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.Register("IconGeometry", typeof(Geometry), typeof(IconButton), new FrameworkPropertyMetadata(null));

        readonly DependencyPropertyDescriptor IconStretchPrpertyDescriptor = DependencyPropertyDescriptor.FromProperty(IconStretchProperty, typeof(IconButton));
        readonly DependencyPropertyDescriptor IconGeometryPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(IconGeometryProperty, typeof(IconButton));

        public IconButton()
        {
            InitializeComponent();

            IconGeometryPropertyDescriptor.AddValueChanged(this, IconGeometryChanged);
            IconStretchPrpertyDescriptor.AddValueChanged(this, IconStretchChanged);
            StaticColorPropertyDescriptor.AddValueChanged(this, StaticColorChanged);

            StaticColor = Color.FromRgb(114, 117, 122);

            DataContext = this;
        }

        private void StaticColorChanged(object sender, EventArgs e)
        {
            IconPath.Fill = new SolidColorBrush(StaticColor);
        }

        private void IconStretchChanged(object sender, EventArgs e)
        {
            IconPath.Stretch = IconStretch;
            IconPathPressed.Stretch = IconStretch;
        }

        private void IconGeometryChanged(object sender, EventArgs e)
        {
            IconPath.Data = IconGeometry;
            IconPathPressed.Data = IconGeometry;
        }

        private void ButtonGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((Grid)sender).CaptureMouse();
            IconPathPressed.Visibility = Visibility.Visible;
        }

        private void ButtonGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Grid)sender).ReleaseMouseCapture();
            IconPathPressed.Visibility = Visibility.Hidden;

            if (IsMouseOver) Clicked?.Invoke(this, e);
        }

        private void ButtonGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                IconPathPressed.Visibility = Visibility.Hidden;
                IconPath.Fill = new SolidColorBrush(StaticColor);
            }
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonGrid.Width = Width;
            ButtonGrid.Height = Height;
        }
    }
}
