using System.Drawing;
using GalaSoft.MvvmLight;


namespace Extender.Main.Models
{
    public class SettingsModel : ObservableObject
    {
        private BonusItemsObservableCollection _bonusItems;
        private BonusItem _attackLocation;

        public SettingsModel()
        {
            WindowSize = new Size();

            AttackLocation = new BonusItem(new Point(0, 0), new Size(0, 0));
            BonusItems = new BonusItemsObservableCollection();

            IsAttackEnabled = true;
            AttackDelay = 1000;

            IsBonusEnabled = true;
            BonusDelay = 5000;
        }


        public WindowInformation SelectedWindow { get; set; }

        public Size WindowSize { get; set; }

        public BonusItem AttackLocation
        {
            get { return _attackLocation; }
            set
            {
                _attackLocation = value;
                RaisePropertyChanged(() => AttackLocation);
            }
        }

        public BonusItemsObservableCollection BonusItems
        {
            get { return _bonusItems; }
            set
            {
                _bonusItems = value;
                RaisePropertyChanged(() => BonusItems);
            }
        }

        public bool IsAttackEnabled { get; set; }

        public long AttackDelay { get; set; }

        public bool IsBonusEnabled { get; set; }

        public long BonusDelay { get; set; }
    }
}
