using System;
using System.Windows;
using System.Windows.Interop;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Returns a pointer to handler
        /// </summary>
        /// <param name="window">Window</param>
        /// <returns>A pointer to the handler</returns>
        public static IntPtr GetWindowHandler(this Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            return helper.Handle;
        }
    }
}
