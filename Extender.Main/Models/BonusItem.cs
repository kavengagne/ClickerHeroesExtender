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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((BonusItem)obj);
        }

        protected bool Equals(BonusItem other)
        {
            return Position.Equals(other.Position) && WindowSize.Equals(other.WindowSize);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ WindowSize.GetHashCode();
            }
        }
    }
}
