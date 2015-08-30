using System;
using System.Windows.Input;
using Extender.Main.Messages;
using Extender.Main.Models;
using Extender.Main.Services;
using Extender.Main.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Size = System.Drawing.Size;


namespace Extender.Main.ViewModels
{
    // TODO: KG - Allow changing GameWindow Size from UI.
    // TODO: KG - Allow changing the Override Key.
    // TODO: KG - Calculate Overlay and Bonus Size/Position ratios when the GameWindow is zooming

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ExtenderRunner _extenderRunner;
        private readonly ExtenderSettings _settings;

        private readonly BonusesOverlayWindow _bonusesOverlay;


        private string _startStopCurrentLabel;

        private ICommand _startStopCommand;
        private ICommand _showBonusesOverlayCommand;

        private bool _isStartStopEnabled;
        private string _windowTitle;
        private IntPtr _windowHandle;
        private Size _windowSize;
        private bool _isShowingBonusesOverlay;
        private bool _isBonusesOverlayEnabled;


        public MainWindowViewModel()
        {
            RegisterForGameWindowChangedMessages();

            _settings = new ExtenderSettings();
            _extenderRunner = new ExtenderRunner(_settings);

            _bonusesOverlay = new BonusesOverlayWindow(_settings)
            {
                DataContext = new BonusOverlayViewModel(_settings)
            };

            SetStartStopLabel(false);
        }


        public BonusItemsObservableCollection BonusItems => _settings.BonusItemsObservableCollection;

        public long AttackDelay
        {
            get { return _settings.AttackDelay; }
            set
            {
                _settings.AttackDelay = value;
                RaisePropertyChanged(() => AttackDelay);
            }
        }

        public long BonusDelay
        {
            get { return _settings.BonusDelay; }
            set
            {
                _settings.BonusDelay = value;
                RaisePropertyChanged(() => BonusDelay);
            }
        }

        public bool IsAttackEnabled
        {
            get { return _settings.IsAttackEnabled; }
            set
            {
                _settings.IsAttackEnabled = value;
                RaisePropertyChanged(() => IsAttackEnabled);
            }
        }

        public bool IsBonusEnabled
        {
            get { return _settings.IsBonusEnabled; }
            set
            {
                _settings.IsBonusEnabled = value;
                RaisePropertyChanged(() => IsBonusEnabled);
            }
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            private set
            {
                _windowTitle = value;
                RaisePropertyChanged(() => WindowTitle);
            }
        }

        public IntPtr WindowHandle
        {
            get { return _windowHandle; }
            private set
            {
                _windowHandle = value;
                RaisePropertyChanged(() => WindowHandle);
            }
        }

        public Size WindowSize
        {
            get { return _windowSize; }
            private set
            {
                _windowSize = value;
                RaisePropertyChanged(() => WindowSize);
            }
        }

        public string StartStopCurrentLabel
        {
            get { return _startStopCurrentLabel; }
            set
            {
                _startStopCurrentLabel = value;
                RaisePropertyChanged(() => StartStopCurrentLabel);
            }
        }

        public bool IsStartStopEnabled
        {
            get { return _isStartStopEnabled; }
            set
            {
                _isStartStopEnabled = value;
                RaisePropertyChanged(() => IsStartStopEnabled);
            }
        }

        public bool IsBonusesOverlayEnabled
        {
            get { return _isBonusesOverlayEnabled; }
            set
            {
                _isBonusesOverlayEnabled = value;
                RaisePropertyChanged(() => IsBonusesOverlayEnabled);
            }
        }


        public ICommand StartStopCommand
            => _startStopCommand ?? (_startStopCommand = new RelayCommand(StartStop));

        public ICommand ShowBonusesOverlayCommand =>
            _showBonusesOverlayCommand ?? (_showBonusesOverlayCommand = new RelayCommand(ShowBonusesOverlay));
        

        public void Exit()
        {
            _extenderRunner.Stop();
        }

        private void StartStop()
        {
            if (_settings.IsStarted)
            {
                SetStartStopLabel(false);
                _extenderRunner.Stop();
            }
            else
            {
                SetStartStopLabel(true);
                if (!_extenderRunner.Start())
                {
                    StartStop();
                }
            }
        }

        private void ShowBonusesOverlay()
        {
            if (_isShowingBonusesOverlay)
            {
                _bonusesOverlay.Hide();
            }
            else
            {
                _bonusesOverlay.Show();
            }
            _isShowingBonusesOverlay = !_isShowingBonusesOverlay;
        }


        private void SetStartStopLabel(bool isEnabled)
        {
            StartStopCurrentLabel = isEnabled ? "Stop" : "Start";
        }


        private void RegisterForGameWindowChangedMessages()
        {
            Messenger.Default.Register<GameWindowChangedMessage>(this, GameWindowChangedMessageHandler);
        }

        private void GameWindowChangedMessageHandler(GameWindowChangedMessage message)
        {
            IsStartStopEnabled = _settings.IsStarted || message.GameWindow != null;
            IsBonusesOverlayEnabled = message.GameWindow != null;

            if (message.GameWindow == null)
            {
                WindowHandle = IntPtr.Zero;
                WindowTitle = String.Empty;
                WindowSize = Size.Empty;
            }
            else
            {
                WindowHandle = message.GameWindow.Hwnd;
                WindowTitle = message.GameWindow.Title;

                var size = message.GameWindow.ClientSize;
                WindowSize = new Size(size.Width, size.Height);
            }
        }
    }
}
