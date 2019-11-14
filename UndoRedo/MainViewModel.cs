using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.ItemSave;
using Xce.TRackingItem.TestModel.Multi;
using Xce.TRackingItem.TestModel.PropertySave;

namespace UndoRedo
{
    public class MainViewModel : PropertyObject
    {
        public static string PropertyModeKey = "PropertyMode";
        public static string ItemModeKey = "ItemMode";
        public static string AllModeKey = "AllyMode";

        //private readonly TrackingManager trackingManager;

        //private bool scopeEnabled;

        //private TrackingScope scope;

        public IDictionary<string, Func<TestModelEditionViewModel>> modes = new Dictionary<string, Func<TestModelEditionViewModel>>()
        {
            [PropertyModeKey] = () => new TestModelEditionViewModel<DriverProperty, CarProperty, AddressProperty>(PropertyTrackingManagerProvider.Instance),
            [ItemModeKey] = () => new TestModelEditionViewModel<DriverItem, CarItem, AddressItem>(ItemTrackingManagerProvider.Instance),
            [AllModeKey] = () => new TestModelEditionViewModel<DriverMulti, CarMulti, AddressMulti>(MultiTrackingManagerProvider.Instance)
        };


        public MainViewModel()
        {
            SetMode(PropertyModeKey);

            //trackingManager = MultiTrackingManagerProvider.Instance.GetTrackingManagers().Last();

            //Undo = new RelayCommand(trackingManager.Revert, () => trackingManager.CanRevert);
            //Redo = new RelayCommand(trackingManager.Remake, () => trackingManager.CanRemake);

            //StartScope = new RelayCommand(StartTrackingScope, () => !scopeEnabled);
            //StopScope = new RelayCommand(StopTrackingScope, () => scopeEnabled);

            SetModeCommand = new RelayCommand<string>(SetMode, x => x != currentMode);
        }

        private void SetMode(string mode) 
        {
            currentMode = mode;
            CurrentTestModelEdition = modes[mode]();
        }

        private string currentMode;
        public string CurrentMode
        {
            get => currentMode;
            set => Set(ref currentMode, value);
        }


        private TestModelEditionViewModel currentTestModelEdition;
        public TestModelEditionViewModel CurrentTestModelEdition
        {
            get => currentTestModelEdition;
            set => Set(ref currentTestModelEdition, value);
        }


        public TestModel Model { get; set; } 

        public ICommand SetModeCommand { get; }

        //public ICommand Undo { get; }
        //public ICommand Redo { get; }
        //public ICommand StartScope { get; }
        //public ICommand StopScope { get; }

        //private void StopTrackingScope()
        //{
        //    scopeEnabled = false;
        //    scope.Dispose();
        //    scope = null;
        //}

        //private void StartTrackingScope()
        //{
        //    scopeEnabled = true;
        //    scope = trackingManager.NewScope();
        //}
    }
}

