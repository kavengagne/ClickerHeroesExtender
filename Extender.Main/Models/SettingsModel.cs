using System.Drawing;
using GalaSoft.MvvmLight;


namespace Extender.Main.Models
{
    public class SettingsModel : ObservableObject
    {
        private BonusItemsObservableCollection _bonusItems;

        public SettingsModel()
        {
            WindowSize = new Size();
            BonusItems = new BonusItemsObservableCollection();

            IsAttackEnabled = true;
            AttackDelay = 1000;

            IsBonusEnabled = true;
            BonusDelay = 5000;
        }


        public Size WindowSize { get; set; }

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