using System.Drawing;

namespace Extender.Main.Models
{
    internal class BonusItem
    {
        public Point Position { get; set; }
        public Size WindowSize { get; set; }

        public BonusItem(Point position, Size windowSize)
        {
            Position = position;
            WindowSize = windowSize;
        }
    }
}