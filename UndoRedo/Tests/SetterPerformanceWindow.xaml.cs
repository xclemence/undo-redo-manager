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
            var synthese = results.ToList().Select(x => $"{x.test}\t: {x.duration}").Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");

            System.Windows.Application.Current.Dispatcher.Invoke(() => analyseText.Text = synthese);
        }
    }
}
