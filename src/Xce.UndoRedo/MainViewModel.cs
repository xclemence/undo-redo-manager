using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xce.UndoRedo.Base;
using Xce.UndoRedo.Tests;
using Xce.UndoRedo.Models.DataSet;
using Xce.UndoRedo.Models.Demo;
using Xce.UndoRedo.Models.Fody;
using Xce.UndoRedo.Models.ItemSave;
using Xce.UndoRedo.Models.Multi;
using Xce.UndoRedo.Models.PropertySave;

namespace Xce.UndoRedo
{
    public class MainViewModel : PropertyObject
    {
        public readonly static string DemoModeKey = "DemoMode";
        public readonly static string PropertyModeKey = "PropertyMode";
        public readonly static string ItemModeKey = "ItemMode";
        public readonly static string DataSetModeKey = "DataSetMode";
        public readonly static string AllModeKey = "AllMode";
        public readonly static string FodyModeKey = "FodyMode";

        private readonly GeneratorPropertiesModel generatorProperties = new GeneratorPropertiesModel();

        private readonly IDictionary<string, Func<TestModelEditionViewModel>> modes;

        private TestModelEditionViewModel currentTestModelEdition;
        private string currentMode;


        public MainViewModel()
        {
            modes = new Dictionary<string, Func<TestModelEditionViewModel>>()
            {
                [DemoModeKey] = () => new TestModelEditionViewModel<DriverDemo, CarDemo, AddressDemo>(DemoTrackingManagerProvider.Instance, generatorProperties),
                [PropertyModeKey] = () => new TestModelEditionViewModel<DriverProperty, CarProperty, AddressProperty>(PropertyTrackingManagerProvider.Instance, generatorProperties),
                [ItemModeKey] = () => new TestModelEditionViewModel<DriverItem, CarItem, AddressItem>(ItemTrackingManagerProvider.Instance, generatorProperties),
                [DataSetModeKey] = () => new TestModelEditionViewModel<DriverDataSet, CarDataSet, AddressDataSet>(DataSetTrackingManagerProvider.Instance, generatorProperties),
                [AllModeKey] = () => new TestModelEditionViewModel<DriverMulti, CarMulti, AddressMulti>(MultiTrackingManagerProvider.Instance, generatorProperties),
                [FodyModeKey] = () => new TestModelEditionViewModel<DriverFody, CarFody, AddressFody>(FodyTrackingManagerProvider.Instance, generatorProperties)
            };

            SetMode(DemoModeKey);

            SetModeCommand = new AsyncCommand<string>(SetMode, x => x != currentMode);

            OpenTestPerfCommand = new AsyncCommand(OpenTestPerf);
        }

        private void OpenTestPerf()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                var window = new SetterPerformanceWindow();
                window.Show();
            });
        }

        public ICommand OpenTestPerfCommand { get; }

        private void SetMode(string mode) 
        {
            currentMode = mode;

            CurrentTestModelEdition?.Dispose();
            CurrentTestModelEdition = modes[mode]();
        }

        public string CurrentMode
        {
            get => currentMode;
            set => Set(ref currentMode, value);
        }
        public TestModelEditionViewModel CurrentTestModelEdition
        {
            get => currentTestModelEdition;
            set => Set(ref currentTestModelEdition, value);
        }
        public ICommand SetModeCommand { get; }
    }
}

