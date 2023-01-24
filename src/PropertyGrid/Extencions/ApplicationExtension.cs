﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace PropertyGrid.Extencions
{
    public static class ApplicationExtension
    {
        private static IClassicDesktopStyleApplicationLifetime GetApplicationLifetime()
        {
            return Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        }


        public static Window GetMainWindow()
        {
            var desktopLifetime = GetApplicationLifetime();

            if (desktopLifetime != null)
            {
                return desktopLifetime.MainWindow;
            }

            return null;
        }

        public static IReadOnlyList<Window> GetWindows()
        {
            var desktopLifetime = GetApplicationLifetime();

            if (desktopLifetime != null)
            {
                return desktopLifetime.Windows;
            }
            return null;
        }




    }
}
