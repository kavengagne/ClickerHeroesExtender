using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Extender.Main.Helpers;
using Extender.Main.Models;
using Extender.Main.ViewModels;
using WinApiWrapper.Enums;
using WinApiWrapper.Interfaces;
using WinApiWrapper.Unsafe;
using WinApiWrapper.Wrappers;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Extender.Main.Classes
{
    internal class ExtenderRunner
    {
        private readonly BonusKeeper _bonusKeeper;
        private readonly ExtenderSettings _settings;

        private readonly IWinApiMouse _mouse;
        private readonly Stopwatch _bonusStopwatch;
        private bool _canRecordBonus;

        public ExtenderRunner(ExtenderSettings settings, BonusKeeper bonusKeeper)
        {
            _settings = settings;
            _bonusKeeper = bonusKeeper;
            _mouse = new WinApiMouse();
            _bonusStopwatch = new Stopwatch();
        }

        [STAThread]
        public bool Start()
        {
            _canRecordBonus = true;
            if (Initialize())
            {
                _bonusStopwatch.Restart();
                Task.Run((Action)RunEnemyClicker);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            _canRecordBonus = false;
            _bonusStopwatch.Reset();
        }
        

        private bool Initialize()
        {
            if (FindGameWindow())
            {
                AdjustWindowSize();
                FindClickLocation();
                return true;
            }
            return false;
        }

        private bool FindGameWindow()
        {
            var gameWindowList = WinApiWindow.EnumWindows(IsGameWindow).ToList();
            _settings.GameWindow = gameWindowList.FirstOrDefault();
            if (_settings.GameWindow != null)
            {
                Debug.WriteLine("FindGameWindow: {0} ({1} x {2})", _settings.GameWindow.Title,
                                _settings.GameWindow.ClientSize.Width, _settings.GameWindow.ClientSize.Height);
                return true;
            }
            return false;
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
            // TODO: KG - Save Client Size to Configuration File
            // TODO: KG - Read Client Size from Configuration File
            // TODO: KG - Display Client Size in UI (Allow to change it)
            var rect = new NativeMethods.Structs.RECT(0, 0, _settings.GameWindow.ClientSize.Width,
                                                      _settings.GameWindow.ClientSize.Height);
            var styles = NativeMethods.User32.GetWindowLong(_settings.GameWindow.Hwnd, NativeMethods.Enums.GWL.GWL_STYLE);
            var exStyles = NativeMethods.User32.GetWindowLong(_settings.GameWindow.Hwnd,
                                                              NativeMethods.Enums.GWL.GWL_EXSTYLE);
            var hasMenu = NativeMethods.User32.GetMenu(_settings.GameWindow.Hwnd);
            if (NativeMethods.User32.AdjustWindowRectEx(ref rect, (uint)styles, hasMenu != IntPtr.Zero, (uint)exStyles))
            {
                _settings.GameWindow.Size = new Rectangle(
                    _settings.GameWindow.Size.X, _settings.GameWindow.Size.Y, rect.Right - rect.Left,
                    rect.Bottom - rect.Top);
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
            while (_settings.IsStarted)
            {
                if (!_mouse.IsRightButtonDown)
                {
                    if (BonusClickDelayExpired())
                    {
                        foreach (var bonus in _bonusKeeper)
                        {
                            ClickAtPoint(bonus.Position);
                        }
                    }
                    ClickAtPoint(_settings.ClickPoint);
                }
                else if (CanRecordBonusItem())
                {
                    RecordBonusItem();
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(_settings.ClickDelay));
            }
        }

        private bool BonusClickDelayExpired()
        {
            var isExpired = _bonusStopwatch.ElapsedMilliseconds >= _settings.BonusDelay;
            if (isExpired)
            {
                _bonusStopwatch.Restart();
            }
            return isExpired;
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

            if (_bonusKeeper.Add(bonusItem))
            {
                ClickAtPoint(bonusItem.Position);
            }
        }

        private void ClickAtPoint(Point point)
        {
            _mouse.PerformClick(WinApiMouseButton.Left, _settings.GameWindow.Hwnd, point);
        }
    }
}
