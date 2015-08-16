using System;
using System.Drawing;
using System.Windows.Input;

namespace Extender.Main.DesignMocks
{
    public class MainWindowViewModelMock
    {
        public string StartStopCurrentLabel => "Start";

        public long AttackDelay { get; set; } = 1000;

        public ICommand StartStopCommand { get; set; }

        public ICommand ShowBonusesOverlayCommand { get; set; }

        public bool IsStartStopEnabled => true;

        public long BonusDelay { get; set; } = 5000;

        public bool IsAttackEnabled { get; set; } = true;

        public bool IsBonusEnabled { get; set; } = true;

        public string WindowTitle { get; set; } = "Clicker Heroes";

        public IntPtr WindowHandle { get; set; } = new IntPtr(1234523123);

        public Size WindowSize { get; set; } = new Size(1152, 690);

        public bool IsShowingBonusesOverlay { get; set; } = false;
    }
}