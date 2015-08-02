using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extender.Main.Enums;
using Extender.Main.Models;

namespace Extender.Main.Services
{
    public class RunnerDispatcher
    {
        private readonly ConcurrentDictionary<DispatcherItemId, DispatcherItem> _items;
        private readonly Stopwatch _stopwatch;
        private bool _isRunning;

        public RunnerDispatcher()
        {
            _items = new ConcurrentDictionary<DispatcherItemId, DispatcherItem>();
            _stopwatch = Stopwatch.StartNew();
            Start();
        }

        #region Public Methods
        public void Start()
        {
            _isRunning = true;
            Task.Run((Action)RunnerLoop);
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public bool Add(DispatcherItemId id, Action action, long delay, bool isActive = true)
        {
            if (!IsActionValid(action) || !IsDelayValid(delay))
            {
                return false;
            }
            return _items.TryAdd(id, new DispatcherItem(action, delay, isActive));
        }

        public bool Remove(DispatcherItemId id)
        {
            DispatcherItem item;
            return _items.TryRemove(id, out item);
        }

        public bool ChangeAction(DispatcherItemId id, Action action)
        {
            if (!IsActionValid(action))
            {
                return false;
            }
            return ChangePropertyById(id, item => item.Action = action);
        }

        public bool ChangeDelay(DispatcherItemId id, long delay)
        {
            if (!IsDelayValid(delay))
            {
                return false;
            }
            return ChangePropertyById(id, item => item.Delay = delay);
        }

        public bool Pause(DispatcherItemId id)
        {
            return ChangePropertyById(id, item => item.IsActive = false);
        }

        public bool Resume(DispatcherItemId id)
        {
            return ChangePropertyById(id, item => item.IsActive = true);
        }
        #endregion Public Methods


        private static bool IsActionValid(Action action)
        {
            return action != null;
        }

        private static bool IsDelayValid(long delay)
        {
            return delay > 0;
        }

        private bool ChangePropertyById(DispatcherItemId id, Action<DispatcherItem> changeFunc)
        {
            DispatcherItem item;
            if (_items.TryGetValue(id, out item))
            {
                changeFunc.Invoke(item);
                return true;
            }
            return false;
        }

        private bool IsExecutionRequired(DispatcherItem item)
        {
            if (!item.IsActive)
            {
                return false;
            }
            var expectedTime = item.LastRun + item.Delay;
            return expectedTime <= _stopwatch.ElapsedMilliseconds;
        }

        private void RunnerLoop()
        {
            while (_isRunning)
            {
                foreach (var item in _items.Values.Where(IsExecutionRequired))
                {
                    item.LastRun = _stopwatch.ElapsedMilliseconds;
                    item.Action.Invoke();
                }
                Thread.Sleep(1);
            }
        }
    }
}