using System.Windows.Input;
using Xce.TrackingItem;

namespace UndoRedo
{
    public class MainViewModel
    {
        private readonly TrackingManager trackingManager;

        private bool scopeEnabled;

        private TrackingScope scope;

        public MainViewModel()
        {
            trackingManager = new TrackingManager();

            Model = new TestModel(trackingManager);

            Undo = new RelayCommand(trackingManager.Revert, () => trackingManager.CanRevert);
            Redo = new RelayCommand(trackingManager.Remake, () => trackingManager.CanRemake);

            StartScope = new RelayCommand(StartTrackingScope, () => !scopeEnabled);
            StopScope = new RelayCommand(StopTrackingScope, () => scopeEnabled);
        }

        public TestModel Model { get; set; } 

        public ICommand Undo { get; }
        public ICommand Redo { get; }
        public ICommand StartScope { get; }
        public ICommand StopScope { get; }

        private void StopTrackingScope()
        {
            scopeEnabled = false;
            scope.Dispose();
            scope = null;
        }

        private void StartTrackingScope()
        {
            scopeEnabled = true;
            scope = trackingManager.NewScope();
        }
    }
}

