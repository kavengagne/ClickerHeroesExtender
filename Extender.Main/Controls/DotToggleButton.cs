using System.Windows;
using System.Windows.Controls.Primitives;

namespace Extender.Main.Controls
{
    public class DotToggleButton : ToggleButton
    {
        public static readonly DependencyProperty IsAttackLocationProperty = DependencyProperty.Register("IsAttackLocation", typeof(bool), typeof(DotToggleButton), new PropertyMetadata(false));
        public static readonly DependencyProperty CanRemoveProperty = DependencyProperty.Register("CanRemove", typeof(bool), typeof(DotToggleButton), new PropertyMetadata(true));

        public bool IsAttackLocation
        {
            get { return (bool)GetValue(IsAttackLocationProperty); }
            set { SetValue(IsAttackLocationProperty, value); }
        }

        public bool CanRemove
        {
            get { return (bool)GetValue(CanRemoveProperty); }
            set { SetValue(CanRemoveProperty, value); }
        }
    }
}
