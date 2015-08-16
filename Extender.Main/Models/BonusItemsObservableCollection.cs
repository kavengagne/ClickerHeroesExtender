using System.Collections.ObjectModel;


namespace Extender.Main.Models
{
    public class BonusItemsObservableCollection : ObservableCollection<BonusItem>
    {
        public new bool Add(BonusItem bonusItem)
        {
            if (!Contains(bonusItem))
            {
                base.Add(bonusItem);
                return true;
            }
            return false;
        }

        private static bool IsEquivalentPosition(BonusItem bonusItemA, BonusItem bonusItemB)
        {
            // TODO: KG - Calculate equivalent positions (we are resizing the window for now...)
            var posA = bonusItemA.Position;
            var sizeA = bonusItemA.WindowSize;

            var posB = bonusItemB.Position;
            var sizeB = bonusItemB.WindowSize;
            return false;
        }
    }
}
