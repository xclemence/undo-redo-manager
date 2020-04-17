using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Runtime.CompilerServices;
using System;

namespace Xce.UndoRedo.Base
{
    public class PropertyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Set<T>(ref T field, T value, [CallerMemberName] string CallerMemberNameAttribute = "")
        {
            if (field == null && value == null)
                return false;

            if (field != null && field.Equals(value))
                return false;

            field = value;

            NotifyPropertyChanged(CallerMemberNameAttribute);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CallerMemberNameAttribute));

            return true;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string CallerMemberNameAttribute = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CallerMemberNameAttribute));
    }

    public class ItemManager : PropertyObject
    {
        private object selectedItem;

        public object SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, value);
        }
    }

    public class SelectionManager : PropertyObject
    {
        private readonly Action onSelectionChanged;
        private object selectedItem;

        public SelectionManager(Action onSelectionChanged) =>
            this.onSelectionChanged = onSelectionChanged ?? throw new ArgumentNullException(nameof(onSelectionChanged));

        internal IList<ItemManager> ItemManagers { get; } = new List<ItemManager>();

        public object SelectedItem
        {
            get => selectedItem;
            set => Set(ref selectedItem, value);
        }

        public object this[int key]
        {
            get => ItemManagers[key].SelectedItem;
            set
            {
                for (int i = 0; i < ItemManagers.Count; ++i)
                    ItemManagers[i].SelectedItem = null;

                ItemManagers[key].SelectedItem = value;


                SelectedItem = value == null ? null : ItemManagers[key];

                onSelectionChanged?.Invoke();
            }
        }

        public int AddNewItem()
        {
            ItemManagers.Add(new ItemManager());

            return ItemManagers.Count - 1;
        }

        public void Clear()
        {
            ItemManagers.Clear();
            SelectedItem = null;
        }
    }

    /// <summary>
    /// Interaction logic for EditionPanel.xaml
    /// </summary>
    public partial class EditionPanel : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty EditObjectProperty =
                DependencyProperty.Register(nameof(EditObject), typeof(object), typeof(EditionPanel), new PropertyMetadata(null, OnObjectChanged));

        public event PropertyChangedEventHandler PropertyChanged;


        public EditionPanel()
        {
            InitializeComponent();

             SelectionManager = new SelectionManager(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectionManager))));
        }

        public object EditObject
        {
            get { return (object)GetValue(EditObjectProperty); }
            set { SetValue(EditObjectProperty, value); }
        }

        public SelectionManager SelectionManager { get; }

        private static void OnObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is EditionPanel control))
                return;

            control.mainPanel.Children.Clear();
            control.mainPanel.DataContext = e.NewValue;
            control.SelectionManager.Clear();

            control.mainGroup.Header = control.GetMainGroupName();

            if (e.NewValue == null)
                return;

            var properties = e.NewValue.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (item.PropertyType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(item.PropertyType))
                    control.AddCollectionItem(item, e.NewValue);
                else
                    control.AddPropertyItem(item);
            }
        }

        private void AddCollectionItem(PropertyInfo item, object parent)
        {
            if (!item.CanRead)
                return;

            var dataGrid = new DataGrid { HeadersVisibility = DataGridHeadersVisibility.Column };

            dataGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(item.Name));
            
            var index = SelectionManager.AddNewItem();

            var selectionBinding = new Binding($"{nameof(SelectionManager)}[{index}]")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(EditionPanel), 1),
                Mode = BindingMode.TwoWay
            };

            int? value = null;

            try
            {
                dynamic collection = item.GetMethod.Invoke(parent, null);
                value = collection.Count;
            }
            catch
            {
                // Do nothing
            }
            
            dataGrid.SetBinding(Selector.SelectedItemProperty, selectionBinding);

            var group = new GroupBox 
            { 
                Header = $"{item.Name} ({value}",
                Content = dataGrid,
            };

            mainPanel.Children.Add(group);
        }

        private void AddPropertyItem(PropertyInfo item)
        {
            if (!item.CanWrite || !item.CanRead)
                return;

            if (item.SetMethod.IsPrivate)
                return;

            var panel = new StackPanel { Orientation = Orientation.Horizontal };

            panel.Children.Add(new TextBlock { Text = item.Name });
            var textbox = new TextBox();
            textbox.SetBinding(TextBox.TextProperty, new Binding(item.Name));
            panel.Children.Add(textbox);

            mainPanel.Children.Add(panel);
        }

        private string GetMainGroupName() => EditObject?.GetType().Name;
    }
}
