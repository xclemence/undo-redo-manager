using System.Windows.Input;
using Xce.TrackingItem;
using Xce.TrackingItem.Extensions;
using Xce.UndoRedo.Base;

namespace Xce.UndoRedo.ViewModels
{
    public class TrackingItemsViewModel : PropertyObject
    {
        private readonly TrackingManager trackingManager;

        private string mainScopeItems;
        private string currentScopeItems;

        public TrackingItemsViewModel(TrackingManager trackingManager)
        {
            this.trackingManager = trackingManager;
            RefreshCommand = new AsyncCommand(RefreshScreen);
        }

        public ICommand RefreshCommand { get; }

        public string MainScopeItems
        {
            get => mainScopeItems;
            set => Set(ref mainScopeItems, value);
        }

        public string CurrentScopeItems
        {
            get => currentScopeItems;
            set => Set(ref currentScopeItems, value);
        }

        private void RefreshScreen()
        {
            CurrentScopeItems = trackingManager.CurrentScope.LastActions.ContentToString();
            MainScopeItems = trackingManager.BaseScope.LastActions.ContentToString();
        }
    }
}
