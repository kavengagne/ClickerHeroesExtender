using System;


namespace Extender.Main.Models
{
    public class DispatcherItem
    {
        public DispatcherItem(Action action, long delay, bool isActive = true)
        {
            Action = action;
            Delay = delay;
            LastRun = 0;
            IsActive = isActive;
        }

        public Action Action { get; set; }
        public long Delay { get; set; }
        public bool IsActive { get; set; }
        public long LastRun { get; set; }
    }
}
