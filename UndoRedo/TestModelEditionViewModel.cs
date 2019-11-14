using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Bogus;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

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

        public TestModelEditionViewModel(ITrackingManagerProvider managerProvider)
        {
            trackingManagers = managerProvider.GetTrackingManagers().ToList();
            this.managerProvider = managerProvider;

            Undo = new RelayCommand(managerProvider.RevertMulti, () => trackingManagers.First().CanRevert);
            Redo = new RelayCommand(managerProvider.RemakeMulti, () => trackingManagers.First().CanRemake);

            UndoAllCommand = new RelayCommand(managerProvider.RevertAlltMulti, () => trackingManagers.First().CanRevert);
            RedoAllCommand = new RelayCommand(managerProvider.RemakeAllMulti, () => trackingManagers.First().CanRemake);

            GenerateDrivers = new RelayCommand(GenerateFakeDrivers);
            GenerateCars = new RelayCommand(GenerateFakeCars);
            GenerateAddresses = new RelayCommand(GenerateFakeAddresses);

            RefreshLogCommand = new RelayCommand(RefreshLogs);
        }

        private EditionModel model;
        
        public EditionModel Model
        {
            get => model;
            set => Set(ref model, value);
        }

        public ICommand Undo { get; }
        public ICommand Redo { get; }

        public ICommand UndoAllCommand { get; }
        public ICommand RedoAllCommand { get; }

        //public ICommand StartScope { get; }
        //public ICommand StopScope { get; }

        public ICommand GenerateDrivers { get; }
        public ICommand GenerateCars { get; }
        public ICommand GenerateAddresses { get; }
        public ICommand RefreshLogCommand { get; }

        public int DriverNumber { get; set; } = 50;
        public int CarNumber { get; set; } = 100;
        public int AddressNumber { get; set; } = 100;

        public int Seed { get; set; }

        private void ApplySeed()
        {
            if (Seed != 0)
                Randomizer.Seed = new Random(Seed);
        }

        private void GenerateFakeAddresses() => GenerateFake(FakerProviders.GetFakerAddress<TAddr>(), AddressNumber);


        private void GenerateFakeCars() => GenerateFake(FakerProviders.GetFakerCar<TCar>(), CarNumber);

        private void GenerateFakeDrivers() => 
            GenerateFake(FakerProviders.getFakerDriver<TDriver, TCar, TAddr>(), DriverNumber);

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
