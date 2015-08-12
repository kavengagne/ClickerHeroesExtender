using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Extender.Main.Enums;
using Extender.Main.Helpers;
using Extender.Main.Messages;
using Extender.Main.Models;
using GalaSoft.MvvmLight.Messaging;
using WinApiWrapper.Enums;
using WinApiWrapper.Interfaces;
using WinApiWrapper.Unsafe;
using WinApiWrapper.Wrappers;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Extender.Main.Services
{
    internal class ExtenderRunner
    {
        private readonly ExtenderSettings _settings;
        private readonly RunnerDispatcher _dispatcher;

        private readonly IWinApiMouse _mouse;
        private bool _canRecordBonus;

        public ExtenderRunner(ExtenderSettings settings)
        {
            _settings = settings;
            _mouse = new WinApiMouse();

            _dispatcher = new RunnerDispatcher();
            _dispatcher.Add(DispatcherItemId.BonusFish, RunBonusClicker, _settings.BonusDelay, false);
            _dispatcher.Add(DispatcherItemId.MainClick, RunEnemyClicker, _settings.AttackDelay, false);
            _dispatcher.Add(DispatcherItemId.FindGameWindow, FindGameWindow, 1000);
            _dispatcher.Add(DispatcherItemId.Watcher, RunWatcher, 1000);
            _dispatcher.Add(DispatcherItemId.SaveSettings, SaveSettings, 2000);
        }

        
        public bool Start()
        {
            _canRecordBonus = true;
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
            _canRecordBonus = false;
            _settings.IsStarted = false;
            _dispatcher.Pause(DispatcherItemId.BonusFish);
            _dispatcher.Pause(DispatcherItemId.MainClick);
        }
        

        private bool Initialize()
        {
            if (_settings.GameWindow != null)
            {
                AdjustWindowSize();
                FindClickLocation();
                return true;
            }
            return false;
        }

        private void FindGameWindow()
        {
            if (_settings.GameWindow == null ||
                !WinApiWindow.EnumWindows(w => w.Hwnd == _settings.GameWindow.Hwnd).Any())
            {
                var gameWindowList = WinApiWindow.EnumWindows(IsGameWindow).ToList();
                _settings.GameWindow = gameWindowList.FirstOrDefault();
                if (_settings.GameWindow != null)
                {
                    Debug.WriteLine("FindGameWindow: {0} ({1} x {2})", _settings.GameWindow.Title,
                                    _settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height);
                }
            }
        }

        private static bool IsGameWindow(IWinApiWindow window)
        {
            bool isGameWindow = false;
            ExceptionWrapper.TrySafe<Exception>(
                () => isGameWindow = window.IsDesktopWindow
                                     && !String.IsNullOrWhiteSpace(window.Title)
                                     && window.Title.Contains("Clicker Heroes")
                                     && window.ClassName.Contains("ApolloRuntimeContentWindow"),
                ex => MessageBox.Show(ex.ToString()));
            return isGameWindow;
        }

        private void AdjustWindowSize()
        {
            // TODO: KG - Display Client Size in UI (Allow to change it)

            if (_settings.WindowSize.IsEmpty)
            {
                _settings.WindowSize = new Size(
                    _settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height);
            }

            var windowRect = new NativeMethods.Structs.RECT(
                0, 0, _settings.WindowSize.Width, _settings.WindowSize.Height);

            var styles = NativeMethods.User32.GetWindowLong(_settings.GameWindow.Hwnd, NativeMethods.Enums.GWL.GWL_STYLE);
            var exStyles = NativeMethods.User32.GetWindowLong(
                _settings.GameWindow.Hwnd, NativeMethods.Enums.GWL.GWL_EXSTYLE);
            var hasMenu = NativeMethods.User32.GetMenu(_settings.GameWindow.Hwnd);

            if (NativeMethods.User32.AdjustWindowRectEx(ref windowRect, (uint) styles, hasMenu != IntPtr.Zero, (uint) exStyles))
            {
                _settings.GameWindow.Size = new Rectangle(
                    _settings.GameWindow.Size.X, _settings.GameWindow.Size.Y,
                    windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top);
            }
        }

        private void FindClickLocation()
        {
            // TODO: KG - Should call every 5 seconds or so.
            if (_settings.GameWindow != null)
            {
                var size = _settings.GameWindow.ClientSize;
                _settings.ClickPoint = new Point((int)(size.Width * (3 / 4d)), (int)(size.Height * (1 / 2d)));
                Debug.WriteLine("Windows is at X: {0}, Y: {1}, Width: {2}, Height: {3}", size.X, size.Y, size.Width,
                                size.Height);
                Debug.WriteLine("Click Point is at {0} x {1}", _settings.ClickPoint.X, _settings.ClickPoint.Y);
            }
        }

        private void RunEnemyClicker()
        {
            if (!_mouse.IsRightButtonDown)
            {
                ClickAtPoint(_settings.ClickPoint);
            }
            else if (CanRecordBonusItem())
            {
                 RecordBonusItem();
            }
        }

        private void RunWatcher()
        {
            _dispatcher.ChangeDelay(DispatcherItemId.MainClick, _settings.AttackDelay);
            _dispatcher.ChangeDelay(DispatcherItemId.BonusFish, _settings.BonusDelay);
        }

        private void SaveSettings()
        {
            _settings.Save();
        }

        private void RunBonusClicker()
        {
            if (!_mouse.IsRightButtonDown)
            {
                foreach (var bonus in _settings.BonusItemsObservableCollection)
                {
                    ClickAtPoint(bonus.Position);
                }
            }
        }

        private bool CanRecordBonusItem()
        {
            var isRecordKeyDown = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                isRecordKeyDown = Keyboard.IsKeyDown(Key.LeftAlt);
            });

            if (isRecordKeyDown)
            {
                if (_canRecordBonus)
                {
                    _canRecordBonus = false;
                    return true;
                }
            }
            else
            {
                _canRecordBonus = true;
            }
            return false;
        }

        private void RecordBonusItem()
        {
            var mousePosition = _mouse.GetClientPosition(_settings.GameWindow);
            Debug.WriteLine(
                "Recorded Position [Mouse (X: {0}, Y: {1}) - Window (W: {2}, H: {3})]",
                mousePosition.X, mousePosition.Y,
                _settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height);

            var bonusItem = new BonusItem(
                new Point(mousePosition.X, mousePosition.Y),
                new Size(_settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height));

            if (_settings.BonusItemsObservableCollection.Add(bonusItem))
            {
                ClickAtPoint(bonusItem.Position);
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
