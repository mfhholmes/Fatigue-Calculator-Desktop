﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            bool admin = false;
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
                if (e.Args[i].ToUpper() == "/ADMIN")
                {
                    admin = true;
                }

            }

            // work out the start page depending what mode we're in
            Page startPage;
            if (admin)
            {
                ICalculatorSettings settings = new CalculatorSettingsFile();
                startPage = new adminMenuPage(settings);
            }
            else
            {
                startPage = new welcomePage();
            }
            // create the main window
            MainWindow mainWindow = new MainWindow(startPage);

            // then work out if we're fullscreen or windowless
            if (startFullScreen)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            if (windowless)
            {
                mainWindow.WindowStyle = WindowStyle.None;
            }
            // and.....go!
            mainWindow.Show();
        }
    }
}
