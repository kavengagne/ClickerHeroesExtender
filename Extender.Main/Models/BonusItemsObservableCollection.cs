using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Extender.Main.Models
{
    // TODO: KG - Change this class to use a Repository to wrap the Json file access.
    public class BonusItemsObservableCollection : ObservableCollection<BonusItem>
    {
        private readonly string _jsonFileName;

        public BonusItemsObservableCollection(string jsonFileName)
        {
            _jsonFileName = jsonFileName;
            if (!File.Exists(_jsonFileName))
            {
                Save();
            }
            Load();
        }

        public new bool Add(BonusItem bonusItem)
        {
            if (!Exists(bonusItem))
            {
                base.Add(bonusItem);
                Save();
                return true;
            }
            return false;
        }

        public void Load()
        {
            var jsonContentString = File.ReadAllText(_jsonFileName);
            var bonuses = JsonConvert.DeserializeObject<List<BonusItem>>(jsonContentString);

            Clear();
            foreach (var bonusItem in bonuses)
            {
                Add(bonusItem);
            }
        }

        public void Save()
        {
            var jsonContentString = JsonConvert.SerializeObject(this);
            File.WriteAllText(_jsonFileName, jsonContentString);
        }

        public bool Exists(BonusItem bonusItem)
        {
            return this.Any(b => (b.Position == bonusItem.Position && b.WindowSize == bonusItem.WindowSize) ||
                                 IsEquivalentPosition(b, bonusItem));
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
