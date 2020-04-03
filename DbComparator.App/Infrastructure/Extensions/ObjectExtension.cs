using System;
using System.Windows;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// For Window From Template
        /// </summary>
        /// <param name="element">Object</param>
        /// <param name="action">Action delegate</param>
        public static void ForWindowFromTemplate(this object element, Action<Window> action)
        {
            Window window = ((FrameworkElement)element).TemplatedParent as Window;
            if (window != null)
            {
                action(window);
            }
        }
    }
}
