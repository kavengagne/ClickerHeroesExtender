using System;
using System.Drawing;
using System.Windows.Input;
using Extender.Main.Messages;
using Extender.Main.Models;
using Extender.Main.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace Extender.Main.ViewModels
{
    // TODO: KG - Add a list that shows the Bonuses.
    // TODO: KG - Show Bonuses Position on screen and make them editable/deletable.
    // TODO: KG - Add a list that shows the Bonuses.
    // TODO: KG - Allow changing the Override Key.
    // TODO: KG - Save Configuration like last GameWindow Size and Position.

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ExtenderRunner _extenderRunner;
        private readonly ExtenderSettings _settings;
        
        private string _startStopCurrentLabel;

        private ICommand _startStopCommand;
        private bool _isStartStopEnabled;
        private string _windowTitle;
        private IntPtr _windowHandle;
        private Size _windowSize;


        public MainWindowViewModel()
        {
            RegisterForGameWindowChangedMessages();

            _settings = new ExtenderSettings();
            _extenderRunner = new ExtenderRunner(_settings);

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

        public ICommand StartStopCommand
            => _startStopCommand ?? (_startStopCommand = new RelayCommand(StartStop, CanStartStop));

        public void Exit()
        {
            _extenderRunner.Stop();
        }

        private bool CanStartStop()
        {
            return true;
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

        private void SetStartStopLabel(bool isEnabled)
        {
            StartStopCurrentLabel = isEnabled ?  "Stop" : "Start";
        }


        private void RegisterForGameWindowChangedMessages()
        {
            Messenger.Default.Register<GameWindowChangedMessage>(this, GameWindowChangedMessageHandler);
        }

        private void GameWindowChangedMessageHandler(GameWindowChangedMessage message)
        {
            IsStartStopEnabled = _settings.IsStarted || message.GameWindow != null;

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