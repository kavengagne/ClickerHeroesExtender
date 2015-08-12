using System.Drawing;

namespace Extender.Main.Models
{
    public class SettingsModel
    {
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

        public BonusItemsObservableCollection BonusItems { get; set; }

        public bool IsAttackEnabled { get; set; }

        public long AttackDelay { get; set; }
        
        public bool IsBonusEnabled { get; set; }

        public long BonusDelay { get; set; }
    }
}