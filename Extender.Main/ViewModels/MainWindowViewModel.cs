using System;
using System.Drawing;
using System.Windows.Input;
using Extender.Main.Models;
using Extender.Main.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using WinApiWrapper.Interfaces;

namespace Extender.Main.ViewModels
{
    // TODO: KG - Add a list that shows the Bonuses.
    // TODO: KG - Show Bonuses Position on screen and make them editable/deletable.
    // TODO: KG - Add a list that shows the Bonuses.
    // TODO: KG - Display the information related to the Game Window.
    // TODO: KG - Allow changing the Override Key.
    // TODO: KG - Save Configuration like last GameWindow Size and Position.

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ExtenderRunner _extenderRunner;
        private readonly ExtenderSettings _settings;
        
        private string _startStopCurrentLabel;

        private ICommand _startStopCommand;
        private bool _isStartStopEnabled;

        public MainWindowViewModel()
        {
            _settings = new ExtenderSettings();
            _settings.GameWindowChanged += OnGameWindowChanged;
            _extenderRunner = new ExtenderRunner(_settings);
            SetStartStopLabel(false);
        }

        public BonusItemsObservableCollection BonusItems => _settings.BonusItemsObservableCollection;

        public long ClickDelay
        {
            get { return _settings.ClickDelay; }
            set { _settings.ClickDelay = value; }
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


        private void OnGameWindowChanged(IWinApiWindow gameWindow)
        {
            IsStartStopEnabled = _settings.IsStarted || gameWindow != null;
        }
    }
}