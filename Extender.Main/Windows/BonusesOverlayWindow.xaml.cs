using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using Extender.Main.Helpers;
using Extender.Main.Models;
using Extender.Main.ViewModels;
using WinApiWrapper.Native.Delegates;
using WinApiWrapper.Native.Enums;
using WinApiWrapper.Native.Methods;
using WinApiWrapper.Native.Structs;
using Size = System.Drawing.Size;


namespace Extender.Main.Windows
{
    public partial class BonusesOverlayWindow
    {
        private readonly ExtenderSettings _settings;
        private IntPtr _windowHook;
        private GCHandle _windowHookGcHandle;
        private bool _isDragging;
        private ToggleButton _capturedItem;

        public BonusesOverlayWindow(ExtenderSettings settings)
        {
            _settings = settings;

            InitializeComponent();

            Left = -10000;
            IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnIsVisibleChanged(object sender,
                                        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!IsVisible)
            {
                DisableOverlay();
            }
            else
            {
                EnableOverlay();
            }
        }

        private void DisableOverlay()
        {
            User32.UnhookWinEvent(_windowHook);
            _windowHookGcHandle.Free();
        }

        private void EnableOverlay()
        {
            SetOverlayPosition();
            SetEventHooks();
        }

        private void SetOverlayPosition()
        {
            var overlayHwnd = new WindowInteropHelper(this).Handle;
            var size = SizeHelper.GetGameAreaRectangle(_settings.GameWindow.ClientSize);

            var pt = new POINT { x = size.X, y = size.Y };
            User32.ClientToScreen(_settings.GameWindow.Hwnd, ref pt);

            User32.SetWindowPos(overlayHwnd, IntPtr.Zero, pt.x, pt.y, size.Width, size.Height,
                                SetWindowPosFlags.FrameChanged | SetWindowPosFlags.IgnoreZOrder);
        }

        private void SetEventHooks()
        {
            WinEventProc eventHandler = LocationChangedCallback;
            _windowHookGcHandle = GCHandle.Alloc(eventHandler);
            _windowHook = User32.SetWinEventHook(
                AccessibleEvents.LocationChange, AccessibleEvents.LocationChange, IntPtr.Zero, eventHandler
                , 0, 0, SetWinEventHookParameter.WINEVENT_OUTOFCONTEXT);
        }

        private void LocationChangedCallback(IntPtr winEventHookHandle, AccessibleEvents accEvent, IntPtr windowHandle,
                                             int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            if (accEvent == AccessibleEvents.LocationChange && windowHandle == _settings.GameWindow.Hwnd)
            {
                SetOverlayPosition();
            }
        }

        private void BonusItem_OnMouseMove(object sender, MouseEventArgs e)
        {
            var item = sender as ToggleButton;
            if (_isDragging && item != null && item.Equals(_capturedItem))
            {
                Point canvPosToWindow = canvas.TransformToAncestor(this).Transform(new Point(0, 0));

                var upperlimit = canvPosToWindow.Y + (item.Height / 2);
                var lowerlimit = canvPosToWindow.Y + canvas.ActualHeight - (item.Height / 2);

                var leftlimit = canvPosToWindow.X + (item.Width / 2);
                var rightlimit = canvPosToWindow.X + canvas.ActualWidth - (item.Width / 2);

                var absmouseXpos = e.GetPosition(this).X;
                var absmouseYpos = e.GetPosition(this).Y;

                if ((absmouseXpos > leftlimit && absmouseXpos < rightlimit)
                    && (absmouseYpos > upperlimit && absmouseYpos < lowerlimit))
                {
                    var margin = new Thickness(e.GetPosition(canvas).X,
                                               e.GetPosition(canvas).Y, 0, 0);
                    item.SetValue(MarginProperty, margin);
                }
            }
        }

        private void BonusItem_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (button != null)
            {
                _capturedItem?.ReleaseMouseCapture();
                if (button.IsChecked.GetValueOrDefault())
                {
                    _capturedItem = button;
                    button.CaptureMouse();
                    _isDragging = true;
                }
                else
                {
                    button.ReleaseMouseCapture();
                    _isDragging = false;
                }
            }
            e.Handled = true;
        }

        private void Window_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            var clientSize = _settings.GameWindow.ClientSize;

            var item = new BonusItem(new System.Drawing.Point((int)mousePosition.X, (int)mousePosition.Y),
                                     new Size(clientSize.Width, clientSize.Height));

            var viewModel = DataContext as BonusOverlayViewModel;
            lock (_settings.BonusItemsLocker)
            {
                viewModel?.BonusItems.Add(item);
            }
        }

        private void DeleteBonusItem_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var item = button?.DataContext as BonusItem;
            if (item != null)
            {
                var viewModel = DataContext as BonusOverlayViewModel;
                lock (_settings.BonusItemsLocker)
                {
                    viewModel?.BonusItems.Remove(item);
                }
            }
        }
    }
}
