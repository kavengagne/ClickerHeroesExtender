using System.Drawing;
using GalaSoft.MvvmLight;

namespace Extender.Main.Models
{
    public class BonusItem : ObservableObject
    {
        private Point _position;
        private Size _windowSize;


        public BonusItem(Point position, Size windowSize)
        {
            Position = position;
            WindowSize = windowSize;
        }


        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged(() => Position);
            }
        }

        public Size WindowSize
        {
            get { return _windowSize; }
            set
            {
                _windowSize = value;
                RaisePropertyChanged(() => WindowSize);
            }
        }
    }
}