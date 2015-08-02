using System.Windows.Input;

namespace Extender.Main.DesignMocks
{
    public class MainWindowViewModelMock
    {
        public string StartStopCurrentLabel => "Start";

        public long ClickDelay { get; set; } = 1000;

        public ICommand StartStopCommand { get; set; }

        public bool IsStartStopEnabled => true;
    }
}