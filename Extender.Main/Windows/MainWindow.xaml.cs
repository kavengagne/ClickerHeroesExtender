using System.Windows;
using Extender.Main.Classes;
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
    }
}
