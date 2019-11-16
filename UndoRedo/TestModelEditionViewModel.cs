using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Bogus;
using Xce.TrackingItem;
using Xce.TrackingItem.TestModel.Base;

namespace UndoRedo
{
    public class TestModelEditionViewModel : PropertyObject {  }

    public class TestModelEditionViewModel<TDriver, TCar, TAddr> : TestModelEditionViewModel
        where TDriver : Driver<TCar, TAddr>
        where TCar : Car
        where TAddr : Address
    {
        private readonly IList<TrackingManager> trackingManagers;
        private readonly ITrackingManagerProvider managerProvider;
        private string logDetails;

        private bool scopeEnabled;

        private IList<TrackingScope> scopes;

        public TestModelEditionViewModel(ITrackingManagerProvider managerProvider, GeneratorPropertiesModel generatorProperties)
        {
            GeneratorProperties = generatorProperties;
            trackingManagers = managerProvider.GetTrackingManagers().ToList();
            this.managerProvider = managerProvider;


            UndoCommand = new RelayCommand(managerProvider.RevertMulti, () => trackingManagers.First().CanRevert);
            RedoCommand = new RelayCommand(managerProvider.RemakeMulti, () => trackingManagers.First().CanRemake);

            UndoAllCommand = new RelayCommand(managerProvider.RevertAlltMulti, () => trackingManagers.First().CanRevert);
            RedoAllCommand = new RelayCommand(managerProvider.RemakeAllMulti, () => trackingManagers.First().CanRemake);

            GenerateDriversCommand = new RelayCommand(GenerateFakeDrivers);
            GenerateCarsCommand = new RelayCommand(GenerateFakeCars);
            GenerateAddressesCommand = new RelayCommand(GenerateFakeAddresses);

            StartNewScopeCommand = new RelayCommand(StartTrackingScope, () => !scopeEnabled);
            StopNewScopeCommand = new RelayCommand(StopTrackingScope, () => scopeEnabled);

            RefreshLogCommand = new RelayCommand(RefreshLogs);
        }

        private EditionModel model;
        
        public EditionModel Model
        {
            get => model;
            set => Set(ref model, value);
        }

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand UndoAllCommand { get; }
        public ICommand RedoAllCommand { get; }

        public ICommand StartNewScopeCommand { get; set; }
        public ICommand StopNewScopeCommand { get; set; }

        public ICommand GenerateDriversCommand { get; }
        public ICommand GenerateCarsCommand { get; }
        public ICommand GenerateAddressesCommand { get; }
        public ICommand RefreshLogCommand { get; }

        public GeneratorPropertiesModel GeneratorProperties { get; }

        private void ApplySeed()
        {
            if (GeneratorProperties.Seed != 0)
                Randomizer.Seed = new Random(GeneratorProperties.Seed);
        }

        private void GenerateFakeAddresses() => GenerateFake(FakerProviders.GetFakerAddress<TAddr>(), GeneratorProperties.AddressNumber);


        private void GenerateFakeCars() => GenerateFake(FakerProviders.GetFakerCar<TCar>(), GeneratorProperties.CarNumber);

        private void GenerateFakeDrivers() => 
            GenerateFake(FakerProviders.getFakerDriver<TDriver, TCar, TAddr>(), GeneratorProperties.DriverNumber);

        private void GenerateFake<T>(Faker<T> faker, int itemNumber)
            where T: class
        {
            managerProvider.Clear();

            ApplySeed();

            UpdateModel(new List<object>(faker.Generate(itemNumber)));
        }

        private void UpdateModel(IList<object> items)
        {
            Model = new EditionModel { Items = items };
        }
                
        private void RefreshLogs() 
        {
            var builder = new StringBuilder();

            foreach (var item in trackingManagers)
                builder.AppendLine(GenerateTrackingLog(item));

            LogDetails = builder.ToString();
        }

        private string GenerateTrackingLog(TrackingManager manager)
        {
            var builder = new StringBuilder();

            builder.AppendLine("---------------------------------------------");
            builder.AppendLine($"Op number : { manager.CurrentScope.Logs.Count}");

            var operations = manager.CurrentScope.Logs.GroupBy(x => new { x.Type, x.Operation }).ToList();

            foreach (var op in operations)
            {
                builder.AppendLine($"{op.Key.Type} - {op.Key.Operation} :");
                builder.AppendLine($"\t Number: { op.Count() }");
                builder.AppendLine($"\t Total: { new TimeSpan(op.Sum(r => r.Time.Ticks)) }");
                builder.AppendLine($"\t Avg: { TimeSpan.FromMilliseconds(op.Average(r => r.Time.TotalMilliseconds)) }");
                builder.AppendLine($"\t Min: { op.Min(x => x.Time) }");
                builder.AppendLine($"\t Max: { op.Max(x => x.Time) }");
            }

            return builder.ToString();
        }

        public string LogDetails
        {
            get => logDetails;
            set => Set(ref logDetails, value);
        }

        private void StopTrackingScope()
        {

            scopeEnabled = false;

            if (scopes == null)
                return;

            foreach (var item in scopes)
                item.Dispose();

            scopes = null;
        }

        private void StartTrackingScope()
        {
            scopeEnabled = true;
            scopes = trackingManagers.Select(x => x.NewScope()).ToList();
        }
    }
}
