using Xce.UndoRedo.ViewModels;

namespace Xce.UndoRedo.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
