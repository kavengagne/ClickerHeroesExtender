using System;
using System.Drawing;
using Extender.Main.Enums;
using Extender.Main.Helpers;
using Extender.Main.Messages;
using Extender.Main.Models;
using GalaSoft.MvvmLight.Messaging;
using WinApiWrapper.Enums;
using WinApiWrapper.Interfaces;
using WinApiWrapper.Native.Enums;
using WinApiWrapper.Native.Methods;
using WinApiWrapper.Native.Structs;
using WinApiWrapper.Wrappers;


namespace Extender.Main.Services
{
    internal class ExtenderRunner
    {
        private readonly ExtenderSettings _settings;
        private readonly RunnerDispatcher _dispatcher;

        private readonly IWinApiMouse _mouse;

        public ExtenderRunner(ExtenderSettings settings)
        {
            _settings = settings;
            _mouse = new WinApiMouse();

            _dispatcher = new RunnerDispatcher();
            _dispatcher.Add(DispatcherItemId.BonusFish, RunBonusClicker, _settings.BonusDelay, false);
            _dispatcher.Add(DispatcherItemId.MainClick, RunEnemyClicker, _settings.AttackDelay, false);
            _dispatcher.Add(DispatcherItemId.FindGameWindow, FindGameWindow, 1000);
            _dispatcher.Add(DispatcherItemId.DelayWatcher, RunDelayWatcher, 1000);
            _dispatcher.Add(DispatcherItemId.WindowWatcher, RunWindowWatcher, 2000);
            _dispatcher.Add(DispatcherItemId.SaveSettings, SaveSettings, 2000);
        }


        public bool Start()
        {
            _settings.IsStarted = true;
            if (Initialize())
            {
                _dispatcher.Resume(DispatcherItemId.BonusFish);
                _dispatcher.Resume(DispatcherItemId.MainClick);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            _settings.IsStarted = false;
            _dispatcher.Pause(DispatcherItemId.BonusFish);
            _dispatcher.Pause(DispatcherItemId.MainClick);
        }


        private bool Initialize()
        {
            if (_settings.GameWindow != null)
            {
                AdjustWindowSize();
                return true;
            }
            return false;
        }

        private void FindGameWindow()
        {
            IWinApiWindow window = WinApiWindow.Find("ApolloRuntimeContentWindow", "Clicker Heroes");
            if (window == null)
            {
                _settings.GameWindow = null;
            }
            else if (_settings.GameWindow != window)
            {
                _settings.GameWindow = window;
            }
            FindClickLocation();
        }

        private void FindClickLocation()
        {
            if (_settings.GameWindow == null)
            {
                _settings.ClickPoint = Point.Empty;
                return;
            }

            var clickPointRatio = new PointF(3 / 4f, 1 / 2f);
            var areaSize = SizeHelper.GetGameAreaRectangle(_settings.GameWindow.ClientSize);

            _settings.ClickPoint = new Point((int)(areaSize.Width * clickPointRatio.X + areaSize.X),
                                             (int)(areaSize.Height * clickPointRatio.Y + areaSize.Y));
        }

        private void AdjustWindowSize()
        {
            if (_settings.WindowSize.IsEmpty)
            {
                _settings.WindowSize = new Size(
                    _settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height);
            }

            var windowRect = new RECT(
                0, 0, _settings.WindowSize.Width, _settings.WindowSize.Height);

            var styles = User32.GetWindowLong(_settings.GameWindow.Hwnd, GetWindowLong.GWL_STYLE);
            var exStyles = User32.GetWindowLong(
                _settings.GameWindow.Hwnd, GetWindowLong.GWL_EXSTYLE);
            var hasMenu = User32.GetMenu(_settings.GameWindow.Hwnd);

            if (User32.AdjustWindowRectEx(ref windowRect, (uint)styles, hasMenu != IntPtr.Zero,
                                                        (uint)exStyles))
            {
                _settings.GameWindow.Size = new Rectangle(
                    _settings.GameWindow.Size.X, _settings.GameWindow.Size.Y,
                    windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top);
            }
        }

        private void RunEnemyClicker()
        {
            // TODO: KG - Change key here when override key will be modifiable
            if (!_mouse.IsRightButtonDown && _settings.IsAttackEnabled)
            {
                ClickAtPoint(_settings.ClickPoint);
            }
        }

        private void RunDelayWatcher()
        {
            _dispatcher.ChangeDelay(DispatcherItemId.MainClick, _settings.AttackDelay);
            _dispatcher.ChangeDelay(DispatcherItemId.BonusFish, _settings.BonusDelay);
        }

        private void RunWindowWatcher()
        {
            Messenger.Default.Send(new GameWindowChangedMessage(_settings.GameWindow));
        }

        private void SaveSettings()
        {
            _settings.Save();
        }

        private void RunBonusClicker()
        {
            if (!_mouse.IsRightButtonDown && _settings.IsBonusEnabled)
            {
                lock (_settings.BonusItemsLocker)
                {
                    foreach (var bonus in _settings.BonusItemsObservableCollection)
                    {
                        ClickAtPoint(bonus.Position);
                    }
                }
            }
        }

        private void ClickAtPoint(Point point)
        {
            if (_settings.GameWindow != null)
            {
                _mouse.PerformClick(WinApiMouseButton.Left, point, _settings.GameWindow.Hwnd);
            }
        }
    }
}
