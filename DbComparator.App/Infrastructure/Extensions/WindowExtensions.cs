using System;
using System.Windows;
using System.Windows.Interop;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class WindowExtensions
    {
        public static IntPtr GetWindowHandler(this Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            return helper.Handle;
        }
    }
}
