using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extender.Main.Models;
using Newtonsoft.Json;

namespace Extender.Main.Classes
{
    internal class BonusKeeper : IEnumerable<BonusItem>
    {
        private readonly string _jsonFileName;
        private List<BonusItem> _bonusItems;

        public BonusKeeper(string jsonFileName)
        {
            _bonusItems = new List<BonusItem>();

            _jsonFileName = jsonFileName;
            if (!File.Exists(_jsonFileName))
            {
                Save();
            }
            Load();
        }

        public bool Add(BonusItem bonusItem)
        {
            if (!Exists(bonusItem))
            {
                _bonusItems.Add(bonusItem);
                Save();
                return true;
            }
            return false;
        }

        public void Load()
        {
            var jsonContentString = File.ReadAllText(_jsonFileName);
            _bonusItems = JsonConvert.DeserializeObject<List<BonusItem>>(jsonContentString);
        }

        public void Save()
        {
            var jsonContentString = JsonConvert.SerializeObject(_bonusItems);
            File.WriteAllText(_jsonFileName, jsonContentString);
        }

        public bool Exists(BonusItem bonusItem)
        {
            return _bonusItems.Any(b => (b.Position == bonusItem.Position && b.WindowSize == bonusItem.WindowSize) ||
                IsEquivalentPosition(b, bonusItem));
        }

        private static bool IsEquivalentPosition(BonusItem bonusItemA, BonusItem bonusItemB)
        {
            var posA = bonusItemA.Position;
            var sizeA = bonusItemA.WindowSize;

            var posB = bonusItemB.Position;
            var sizeB = bonusItemB.WindowSize;
            return false;
        }

        #region IEnumerable Implementation
        public IEnumerator<BonusItem> GetEnumerator()
        {
            return _bonusItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion IEnumerable Implementation
    }
}
