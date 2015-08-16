using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
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
            // TODO: KG - Cleanup before exit, like timer and mouse states and overlay, etc...
            var mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel?.Exit();
        }
    }
}
