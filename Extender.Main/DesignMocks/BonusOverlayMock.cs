using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using Extender.Main.Models;
using Extender.Main.ViewModels;
using GalaSoft.MvvmLight;


namespace Extender.Main.DesignMocks
{
    public class BonusOverlayMock : ViewModelBase
    {
        public BonusOverlayMock()
        {
            BonusItems = new BonusItemsObservableCollection
            {
                new BonusItem(new Point(0, 0), new Size(1000, 100)),
                new BonusItem(new Point(100, 0), new Size(1000, 100)),
                new BonusItem(new Point(200, 0), new Size(1000, 100)),
                new BonusItem(new Point(0, 100), new Size(1000, 100)),
                new BonusItem(new Point(100, 100), new Size(1000, 100)),
                new BonusItem(new Point(200, 100), new Size(1000, 100)),
                new BonusItem(new Point(0, 200), new Size(1000, 100)),
                new BonusItem(new Point(100, 200), new Size(1000, 100)),
                new BonusItem(new Point(200, 200), new Size(1000, 100))
            };
        }

        public BonusItem AttackLocation => new BonusItem(new Point(300, 100), new Size(1000, 100));
        public BonusItemsObservableCollection BonusItems { get; set; }
    }
}
