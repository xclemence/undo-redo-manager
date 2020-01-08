using System;
using System.Linq;
using System.Windows.Input;

namespace UndoRedo.Tests
{
    /// <summary>
    /// Interaction logic for SetterPerformanceView.xaml
    /// </summary>
    public partial class SetterPerformanceWindow
    {
        public SetterPerformanceWindow()
        {
            InitializeComponent();

            RunCommand = new AsyncCommand(OnRun);

            DataContext = this;
        }

        public ICommand RunCommand { get; }

        public uint TryNumber { get; set; } = 1000000;

        private void OnRun()
        {
            var results = new TestPerf { TryNumber = TryNumber }.RunTest();
            analyseText.Text = results.ToList().Select(x => $"{x.test}\t: {x.duration}").Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");
        }
    }
}
