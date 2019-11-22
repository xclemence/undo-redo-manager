//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.Linq;

//namespace Xce.TrackingItem
//{
//    public class ListItem
//    {
//        public int Id { get; set; }
//    }

//    public class BaseTestModel : ModelBase
//    {
//        private int value;
//        private string name;

//        public BaseTestModel(TrackingManager trackingManager) 
//            : base(trackingManager)
//        {
//            Items = new ObservableCollection<ListItem>();
//            Items.CollectionChanged += OnItemsCollectionChanged<ListItem>;
//        }

//        ~BaseTestModel()
//        {
//            Items.CollectionChanged -= OnItemsCollectionChanged<ListItem>;
//        }

//        public int Value
//        {
//            get => value;
//            set => SetProperty(this, ref this.value, value);
//        }

//        public string Name
//        {
//            get => name;
//            set => SetProperty(this, ref name, value);
//        }

//        public ObservableCollection<ListItem> Items { get; set; }

//        private void OnItemsCollectionChanged<TItem>(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            if(e.NewItems != null) 
//                OnAddItem(sender as ObservableCollection<TItem>, e.NewItems.Cast<TItem>().ToList(), e.NewStartingIndex);

//            if(e.OldItems != null)
//                OnRemoveItem(sender as ObservableCollection<TItem>, e.OldItems.Cast<TItem>().ToList(), e.OldStartingIndex);
//        }
//    }
//}
