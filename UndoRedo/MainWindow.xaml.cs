using System.ComponentModel.Design;
using System.Windows;
using System.Linq;
using UndoRedo.Tests;

namespace UndoRedo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            //var trackingManager = new TrackingManager();

            //var model1 = new TestModel(trackingManager);
            //var model2 = new TestModel(trackingManager);

            //model1.Name = "je suis un model";
            //model2.Name = "un petit truc";

            //trackingManager.LastActions.Clear();

            //model1.Name = "hello";
            //model1.Name = "blabla";

            ////model1.Value = 123;

            //var revertAction = trackingManager.LastActions.Reverse().ToList();
            //trackingManager.IsAction = true;

            //foreach (var item in revertAction)
            //    item.Revert();

            //foreach (var item in trackingManager.LastActions)
            //    item.Apply();

            //var testObject = new ObjectReflexion();

        }
    }
}
