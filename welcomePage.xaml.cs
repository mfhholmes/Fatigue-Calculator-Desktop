﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for welcomePage.xaml
    /// </summary>
    public partial class welcomePage : Page
    {
        public welcomePage()
        {
            InitializeComponent();

        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            // need to create a new calculation object every time we leave the welcome page
            calculation newCalc = new calculation();
            // next page is the identity page, depending on licence type

#if Singleuser
            newCalc.currentInputs.identity = Properties.Settings.Default.FixedID;
            // skip identity page and on to shift
            shiftPage nextShift = new shiftPage(newCalc);
            this.NavigationService.Navigate(nextShift);
#else
            identityPage next = new identityPage(newCalc);
            this.NavigationService.Navigate(next);
#endif
        }

    }
}
