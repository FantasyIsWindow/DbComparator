using System;
using System.Windows;

namespace DbComparator.App.Infrastructure.Extensions
{
    public static class ObjectExtension
    {
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
