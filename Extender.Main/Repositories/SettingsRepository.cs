using System.ComponentModel;
using System.Drawing;
using System.IO;
using Extender.Main.Models;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Extender.Main.Repositories
{
    public class SettingsRepository : ObservableObject
    {
        private readonly string _jsonFileName;
        private SettingsModel _settingsModel;
        private bool _isSaveRequired;


        public SettingsRepository(string jsonFileName)
        {
            _jsonFileName = jsonFileName;
            _settingsModel = new SettingsModel();

            Initialize();
        }


        public Size WindowSize
        {
            get { return _settingsModel.WindowSize; }
            set
            {
                _settingsModel.WindowSize = value;
                RaisePropertyChanged(() => WindowSize);
                _isSaveRequired = true;
            }
        }

        public BonusItemsObservableCollection BonusItems => _settingsModel.BonusItems;
        
        public bool IsAttackEnabled
        {
            get { return _settingsModel.IsAttackEnabled; }
            set
            {
                _settingsModel.IsAttackEnabled = value;
                RaisePropertyChanged(() => IsAttackEnabled);
                _isSaveRequired = true;
            }
        }

        public bool IsBonusEnabled
        {
            get { return _settingsModel.IsBonusEnabled; }
            set
            {
                _settingsModel.IsBonusEnabled = value;
                RaisePropertyChanged(() => IsBonusEnabled);
                _isSaveRequired = true;
            }
        }

        public long AttackDelay
        {
            get { return _settingsModel.AttackDelay; }
            set
            {
                _settingsModel.AttackDelay = value;
                RaisePropertyChanged(() => AttackDelay);
                _isSaveRequired = true;
            }
        }

        public long BonusDelay
        {
            get { return _settingsModel.BonusDelay; }
            set
            {
                _settingsModel.BonusDelay = value;
                RaisePropertyChanged(() => BonusDelay);
                _isSaveRequired = true;
            }
        }


        public void SaveToDisk()
        {
            if (_isSaveRequired)
            {
                _isSaveRequired = false;
                var jsonContentString = JsonConvert.SerializeObject(_settingsModel, Formatting.Indented);
                File.WriteAllText(_jsonFileName, jsonContentString);
            }
        }

        public void LoadFromDisk()
        {
            var jsonContentString = File.ReadAllText(_jsonFileName);
            _settingsModel = JsonConvert.DeserializeObject<SettingsModel>(jsonContentString);
            foreach (var item in _settingsModel.BonusItems)
            {
                item.PropertyChanged += OnPropertyChangedEventHandler;
            }
        }


        private void Initialize()
        {
            if (!File.Exists(_jsonFileName))
            {
                SaveToDisk();
            }
            LoadFromDisk();
            SetupCollectionChangedHandler();
        }

        private void SetupCollectionChangedHandler()
        {
            _settingsModel.BonusItems.CollectionChanged += (sender, args) =>
            {
                _isSaveRequired = true;
                if (args.NewItems != null)
                {
                    foreach (INotifyPropertyChanged newItem in args.NewItems)
                    {
                        newItem.PropertyChanged += OnPropertyChangedEventHandler;
                    }
                }
                if (args.OldItems != null)
                {
                    foreach (INotifyPropertyChanged oldItem in args.OldItems)
                    {
                        oldItem.PropertyChanged -= OnPropertyChangedEventHandler;
                    }
                }
            };
        }

        private void OnPropertyChangedEventHandler(object o, PropertyChangedEventArgs eventArgs)
        {
            _isSaveRequired = true;
        }
    }
}