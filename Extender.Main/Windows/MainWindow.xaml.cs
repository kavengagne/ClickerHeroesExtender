using Extender.Main.ViewModels;

namespace Extender.Main.Windows
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel?.Exit();
        }
    }
}
