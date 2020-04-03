using System;
using System.Windows.Input;
using System.Windows;
using System.Runtime.InteropServices;
using DbComparator.App.Infrastructure.Extensions;

namespace DbComparator.App.Resources.Style
{
    /// <summary>
    /// Sets the direction of movement
    /// </summary>
    public enum SizingAction { Left = 1, Right, Top, TopLeft, TopRight, Bottom, BottomLeft, BottomRight };

    public partial class MechanicsMainWindow : ResourceDictionary
    {
        private const int WM_SYSCOMMAND = 0x112;

        private const int SC_SIZE = 0xF000;

        private const int SC_KEYMENU = 0xF100;


        public MechanicsMainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event clicking the window minimize button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e) => 
            sender.ForWindowFromTemplate(w => w.WindowState = WindowState.Minimized);

        /// <summary>
        /// Event clicking the window maximize button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void MaximizeBtn_Click(object sender, RoutedEventArgs e) => 
            sender.ForWindowFromTemplate(w => w.WindowState = 
                (w.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized);

        /// <summary>
        /// Event clicking the window close button
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e) => 
            CloseWindow(sender);

        /// <summary>
        /// Drag event of the window
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void Window_move(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                MaximizeBtn_Click(sender, e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w => w.DragMove());
            }
        }

        /// <summary>
        /// Event-clicking the title of the window
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void WindowTitleBarMouseMove_Click(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w =>
                {
                    if (w.WindowState == WindowState.Maximized)
                    {
                        w.BeginInit();
                        double adjustment = 40.0;
                        var mouse = e.MouseDevice.GetPosition(w);
                        var width_01 = Math.Max(w.ActualWidth - 2 * adjustment, adjustment);
                        w.WindowState = WindowState.Normal;
                        var width_02 = Math.Max(w.ActualWidth - 2 * adjustment, adjustment);
                        w.Left = (mouse.X - adjustment) * (1 - width_02 / width_01);
                        w.Top = -7;
                        w.EndInit();
                        w.DragMove();
                    }
                });
            }
        }

        /// <summary>
        /// Mouse click event on the icon
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void TitleIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                CloseWindow(sender);
            }
            else
            {
                sender.ForWindowFromTemplate(w => 
                    SendMessage(w.GetWindowHandler(), WM_SYSCOMMAND, (IntPtr)SC_KEYMENU, (IntPtr)' '));
            }
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender">Event sender</param>
        private void CloseWindow(object sender) => 
            sender.ForWindowFromTemplate(w => w.Close());

        /// <summary>
        /// Dragging the window top
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnTopSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Top);

        /// <summary>
        /// Dragging the window left
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Left);

        /// <summary>
        /// Dragging the window right
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Right);

        /// <summary>
        /// Dragging the window bottom
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnBottomSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Bottom);

        /// <summary>
        /// Dragging the window top and left
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnTopLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.TopLeft);

        /// <summary>
        /// Dragging the window top and right
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnTopRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.TopRight);

        /// <summary>
        /// Dragging the window bottom and left
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnBottomLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.BottomLeft);

        /// <summary>
        /// Dragging the window bottom and right
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void OnBottomRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.BottomRight);

        /// <summary>
        /// The movement of the window
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="action">Sizing Action</param>
        private void OnSize(object sender, SizingAction action)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w =>
                {
                    if (w.WindowState == WindowState.Normal)
                    {
                        DragSize(w.GetWindowHandler(), action);
                    }
                });
            }
        }

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <param name="Msg">Msg</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>IntPtr</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Stretch size
        /// </summary>
        /// <param name="handle">Handle</param>
        /// <param name="sizing">Sizing Action</param>
        private void DragSize(IntPtr handle, SizingAction sizing)
        {
            SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizing), IntPtr.Zero);
            SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
