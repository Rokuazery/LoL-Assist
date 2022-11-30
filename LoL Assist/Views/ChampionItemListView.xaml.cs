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
    /// Interaction logic for ChampionItemListView.xaml
    /// </summary>
    public partial class ChampionItemListView: UserControl
    {
        public static readonly DependencyProperty IncomingItemProperty =
    DependencyProperty.Register("IncomingItem", typeof(object), typeof(ChampionItemListView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object IncomingItem
        {
            get { return GetValue(IncomingItemProperty); }
            set { SetValue(IncomingItemProperty, value); }
        }

        public static readonly DependencyProperty RemovedItemProperty =
            DependencyProperty.Register("RemovedItem", typeof(object), typeof(ChampionItemListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object RemovedItem
        {
            get { return GetValue(RemovedItemProperty); }
            set { SetValue(RemovedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemDropCommandProperty =
            DependencyProperty.Register("ItemDropCommand", typeof(ICommand), typeof(ChampionItemListView),
                new PropertyMetadata(null));

        public ICommand ItemDropCommand
        {
            get { return (ICommand)GetValue(ItemDropCommandProperty); }
            set { SetValue(ItemDropCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemRemovedCommandProperty =
            DependencyProperty.Register("ItemRemovedCommand", typeof(ICommand), typeof(ChampionItemListView),
                new PropertyMetadata(null));

        public ICommand ItemRemovedCommand
        {
            get { return (ICommand)GetValue(ItemRemovedCommandProperty); }
            set { SetValue(ItemRemovedCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemInsertedCommandProperty =
            DependencyProperty.Register("ItemInsertedCommand", typeof(ICommand), typeof(ChampionItemListView),
                new PropertyMetadata(null));

        public ICommand ItemInsertedCommand
        {
            get { return (ICommand)GetValue(ItemInsertedCommandProperty); }
            set { SetValue(ItemInsertedCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertedItemProperty =
            DependencyProperty.Register("InsertedItem", typeof(object), typeof(ChampionItemListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object InsertedItem
        {
            get { return GetValue(InsertedItemProperty); }
            set { SetValue(InsertedItemProperty, value); }
        }

        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(object), typeof(ChampionItemListView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object TargetItem
        {
            get { return GetValue(TargetItemProperty); }
            set { SetValue(TargetItemProperty, value); }
        }


        public ChampionItemListView()
        {
            InitializeComponent();
        }

        private void ChampionsItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed 
            && sender is FrameworkElement frameworkElement)
            {
                object item = frameworkElement.DataContext;

                DragDropEffects dragDropResult = DragDrop.DoDragDrop(frameworkElement,
                    new DataObject(DataFormats.Serializable, item),
                    DragDropEffects.Move);

                if (dragDropResult == DragDropEffects.None)
                {
                    addItem(item);
                }
            }
        }

        private void ChampionsItem_DragOver(object sender, DragEventArgs e)
        {
            if (ItemInsertedCommand?.CanExecute(null) ?? false)
            {
                if (sender is FrameworkElement element)
                {
                    TargetItem = element.DataContext;
                    InsertedItem = e.Data.GetData(DataFormats.Serializable);
                    ItemInsertedCommand?.Execute(null);
                }
            }
        }

        private void Champions_DragOver(object sender, DragEventArgs e)
        {
            object item = e.Data.GetData(DataFormats.Serializable);
            addItem(item);
        }

        private void addItem(object item)
        {
            if (ItemDropCommand?.CanExecute(null) ?? false)
            {
                IncomingItem = item;
                ItemDropCommand?.Execute(null);
            }
        }

        private void Champions_DragLeave(object sender, DragEventArgs e)
        {
            HitTestResult result = VisualTreeHelper.HitTest(listViewItems, e.GetPosition(listViewItems));

            if (result == null)
            {
                if (ItemRemovedCommand?.CanExecute(null) ?? false)
                {
                    RemovedItem = e.Data.GetData(DataFormats.Serializable);
                    ItemRemovedCommand?.Execute(null);
                }
            }
        }
    }
}
