using System.Drawing;
using Extender.Main.Messages;
using Extender.Main.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using WinApiWrapper.Interfaces;


namespace Extender.Main.Models
{
    public class ExtenderSettings : ObservableObject
    {
        private bool _isStarted;
        private IWinApiWindow _gameWindow;
        private Point _clickPoint;
        private readonly SettingsRepository _settingsRepository;


        public ExtenderSettings()
        {
            _settingsRepository = new SettingsRepository("settings.json");
            IsStarted = false;
        }

        public BonusItemsObservableCollection BonusItemsObservableCollection => _settingsRepository.BonusItems;
        public readonly object BonusItemsLocker = new object();

        public Size WindowSize
        {
            get { return _settingsRepository.WindowSize; }
            set
            {
                _settingsRepository.WindowSize = value;
                RaisePropertyChanged(() => WindowSize);
            }
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

        public long AttackDelay
        {
            get { return _settingsRepository.AttackDelay; }
            set
            {
                _settingsRepository.AttackDelay = value;
                RaisePropertyChanged(() => AttackDelay);
            }
        }

        public long BonusDelay
        {
            get { return _settingsRepository.BonusDelay; }
            set
            {
                _settingsRepository.BonusDelay = value;
                RaisePropertyChanged(() => BonusDelay);
            }
        }

        public bool IsAttackEnabled
        {
            get { return _settingsRepository.IsAttackEnabled; }
            set
            {
                _settingsRepository.IsAttackEnabled = value;
                RaisePropertyChanged(() => IsAttackEnabled);
            }
        }

        public bool IsBonusEnabled
        {
            get { return _settingsRepository.IsBonusEnabled; }
            set
            {
                _settingsRepository.IsBonusEnabled = value;
                RaisePropertyChanged(() => IsBonusEnabled);
            }
        }

        public IWinApiWindow GameWindow
        {
            get { return _gameWindow; }
            set
            {
                _gameWindow = value;
                RaisePropertyChanged(() => GameWindow);
                Messenger.Default.Send(new GameWindowChangedMessage(_gameWindow));
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


        public void Save()
        {
            _settingsRepository.SaveToDisk();
        }
    }
}
