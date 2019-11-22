using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Bogus;
using UndoRedo.Base;
using Xce.TrackingItem;
using Xce.TrackingItem.TestModel.Base;

namespace UndoRedo
{
    public abstract class TestModelEditionViewModel : PropertyObject, IDisposable
    {
        public abstract void Dispose();
    }

    public class TestModelEditionViewModel<TDriver, TCar, TAddr> : TestModelEditionViewModel
        where TDriver : Driver<TCar, TAddr>
        where TCar : Car
        where TAddr : Address
    {
        private readonly IList<TrackingManager> trackingManagers;
        private readonly ITrackingManagerProvider managerProvider;
        private string logDetails;

        public IList<StopTrackingScope> stopTrackingScopes;

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

            StartNewScopeCommand = new RelayCommand(StartTrackingScope, () => scopes == null);
            StopNewScopeCommand = new RelayCommand(StopTrackingScope, () => scopes != null);

            StopTrackingCommand = new RelayCommand(DisableTracking, () => stopTrackingScopes == null);
            StartTrackingCommand = new RelayCommand(EnableTracking, () => stopTrackingScopes != null);

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

        public ICommand StopTrackingCommand { get; }
        public ICommand StartTrackingCommand { get; }

        public GeneratorPropertiesModel GeneratorProperties { get; }

        public string LogDetails
        {
            get => logDetails;
            set => Set(ref logDetails, value);
        }

        private void ApplySeed()
        {
            if (GeneratorProperties.Seed != 0)
                Randomizer.Seed = new Random(GeneratorProperties.Seed);
        }

        private void GenerateFakeAddresses() => GenerateFake(FakerProviders.GetFakerAddress<TAddr>(), GeneratorProperties.AddressNumber);


        private void GenerateFakeCars() => GenerateFake(FakerProviders.GetFakerCar<TCar>(), GeneratorProperties.CarNumber);

        private void GenerateFakeDrivers() => 
            GenerateFake(FakerProviders.getFakerDriver<TDriver, TCar, TAddr>(GeneratorProperties.CarNumber, GeneratorProperties.AddressNumber), GeneratorProperties.DriverNumber);

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

        private void CleanUp<T>(ref IList<T> collection) where T : IDisposable
        {
            if (collection == null)
                return;

            foreach (var item in collection)
                item.Dispose();

            collection = null;
        }
        private void StopTrackingScope() => CleanUp(ref scopes);

        private void StartTrackingScope() => scopes = trackingManagers.Select(x => x.NewScope()).ToList();

        private void DisableTracking() => stopTrackingScopes = trackingManagers.Select(x => new StopTrackingScope(x)).ToList();
        
        private void EnableTracking() => CleanUp(ref stopTrackingScopes);

        public override void Dispose()
        {
            EnableTracking();
            StopTrackingScope();
            managerProvider.Clear();
        }
    }
}
