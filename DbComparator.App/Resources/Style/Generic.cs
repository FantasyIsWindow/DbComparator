using System;
using System.Windows.Input;
using System.Windows;
using System.Runtime.InteropServices;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Infrastructure.Enums;

namespace DbComparator.App.Resources.Style
{
    public partial class Generic : ResourceDictionary
    {
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_SIZE = 0xF000;
        private const int SC_KEYMENU = 0xF100;


        public Generic()
        {
            InitializeComponent();
        }

        private void minimizeBtn_Click(object sender, RoutedEventArgs e) => 
            sender.ForWindowFromTemplate(w => w.WindowState = WindowState.Minimized);
        

        private void maximizeBtn_Click(object sender, RoutedEventArgs e) => 
            sender.ForWindowFromTemplate(w => w.WindowState = (w.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized);
        

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow(sender);
        }



        private void window_move(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                maximizeBtn_Click(sender, e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                sender.ForWindowFromTemplate(w => w.DragMove());
            }
        }

        private void windowTitleBarMouseMove_Click(object sender, MouseEventArgs e)
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
        
        private void titleIcon_Click(object sender, MouseButtonEventArgs e)
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

        private void CloseWindow(object sender) => 
            sender.ForWindowFromTemplate(w => w.Close());

        private void OnTopSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Top);

        private void OnBottomSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Bottom);

        private void OnLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Left);

        private void OnRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.Right);

        private void OnTopLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.TopLeft);

        private void OnTopRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.TopRight);

        private void OnBottomLeftSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.BottomLeft);

        private void OnBottomRightSide(object sender, MouseButtonEventArgs e) => OnSize(sender, SizingAction.BottomRight);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        private void DragSize(IntPtr handle, SizingAction sizing)
        {
            SendMessage(handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + sizing), IntPtr.Zero);
            SendMessage(handle, 514, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
