using System.Windows;
using System.Windows.Input;
using Extender.Main.Classes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace Extender.Main.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ExtenderRunner _extenderRunner;
        private readonly BonusKeeper _bonusKeeper;
        private readonly ExtenderSettings _settings;

        
        private string _startStopCurrentLabel;

        private ICommand _startStopCommand;

        public MainWindowViewModel()
        {
            _bonusKeeper = new BonusKeeper("bonuses.json");
            _settings = new ExtenderSettings();
            _extenderRunner = new ExtenderRunner(_settings, _bonusKeeper);
            SetStartStopLabel(false);
        }

        public bool IsStarted
        {
            get { return _settings.IsStarted; }
            set
            {
                _settings.IsStarted = value;
                SetStartStopLabel(value);
            }
        }

        public double ClickDelay
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

        public ICommand StartStopCommand
            => _startStopCommand ?? (_startStopCommand = new RelayCommand(StartStop, CanStartStop));

        private bool CanStartStop()
        {
            return true;
        }

        private void StartStop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                _extenderRunner.Stop();
            }
            else
            {
                IsStarted = true;
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
    }
}