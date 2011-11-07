using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool startFullScreen = false;
            bool windowless = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i].ToUpper() == "/FULLSCREEN")
                {
                    startFullScreen = true;
                }
                if (e.Args[i].ToUpper() == "/WINDOWLESS")
                {
                    windowless = true;
                }
            }

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            if (startFullScreen)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            if (windowless)
            {
                mainWindow.WindowStyle = WindowStyle.None;
            }
            mainWindow.Show();
        }
    }
}
