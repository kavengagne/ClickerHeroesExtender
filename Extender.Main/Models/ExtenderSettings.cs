using System;
using System.Drawing;
using GalaSoft.MvvmLight;
using WinApiWrapper.Interfaces;

namespace Extender.Main.Models
{
    public class ExtenderSettings : ObservableObject
    {
        private bool _isStarted;
        private long _clickDelay;
        private int _bonusDelay;
        private IWinApiWindow _gameWindow;
        private Point _clickPoint;

        public ExtenderSettings()
        {
            BonusItemsObservableCollection = new BonusItemsObservableCollection("bonuses.json");
            IsStarted = false;
            ClickDelay = 1000;
            BonusDelay = 5000;
        }

        public BonusItemsObservableCollection BonusItemsObservableCollection { get; set; }

        public bool IsStarted
        {
            get { return _isStarted; }
            set
            {
                _isStarted = value;
                RaisePropertyChanged(() => IsStarted);
            }
        }

        public long ClickDelay
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

        public event Action<IWinApiWindow> GameWindowChanged;

        public IWinApiWindow GameWindow
        {
            get { return _gameWindow; }
            set
            {
                _gameWindow = value;
                RaisePropertyChanged(() => GameWindow);
                OnGameWindowChanged(value);
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

        protected virtual void OnGameWindowChanged(IWinApiWindow gameWindow)
        {
            GameWindowChanged?.Invoke(gameWindow);
        }
    }
}