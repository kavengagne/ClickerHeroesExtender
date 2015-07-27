using System.Drawing;
using GalaSoft.MvvmLight;
using WinApiWrapper.Interfaces;

namespace Extender.Main.ViewModels
{
    public class ExtenderSettings : ObservableObject
    {
        private bool _isStarted;
        private double _clickDelay;
        private int _bonusDelay;
        private IWinApiWindow _gameWindow;
        private Point _clickPoint;

        public ExtenderSettings()
        {
            IsStarted = false;
            ClickDelay = 1000;
            BonusDelay = 5000;
        }

        public bool IsStarted
        {
            get { return _isStarted; }
            set
            {
                _isStarted = value;
                RaisePropertyChanged(() => IsStarted);
            }
        }

        public double ClickDelay
        {
            get { return _clickDelay; }
            set
            {
                _clickDelay = value;
                RaisePropertyChanged(() => ClickDelay);
            }
        }

        public int BonusDelay
        {
            get { return _bonusDelay; }
            set
            {
                _bonusDelay = value;
                RaisePropertyChanged(() => BonusDelay);
            }
        }

        public IWinApiWindow GameWindow
        {
            get { return _gameWindow; }
            set
            {
                _gameWindow = value;
                RaisePropertyChanged(() => GameWindow);
            }
        }

        public Point ClickPoint
        {
            get { return _clickPoint; }
            set
            {
                _clickPoint = value;
                RaisePropertyChanged(() => ClickPoint);
            }
        }
    }
}