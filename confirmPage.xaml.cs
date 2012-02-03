using System;
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
    /// Interaction logic for confirmPage.xaml
    /// </summary>
    public partial class confirmPage : Page
    {
        private calculation currentCalc;

        public confirmPage()
        {
            InitializeComponent();
        }
        public confirmPage(calculation passedCalc)
        {
            InitializeComponent();
            currentCalc = passedCalc;
            lblShift.Text = "Your shift runs from " + currentCalc.currentInputs.shiftStart.ToString("ddd h:mm tt") + " to " + currentCalc.currentInputs.shiftEnd.ToString("ddd h:mm tt");
            lblSleep.Text = "You slept for "+ currentCalc.currentInputs.sleep24.ToString() +" hours in the last 24 hours, "+ currentCalc.currentInputs.sleep48.ToString() +" hours in the last 48 hours, and have been awake for "+ currentCalc.currentInputs.hoursAwake.ToString() +" hours";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            //load the calculation settings from the config settings
            currentCalc.currentPresets.lowThreshold = Properties.Settings.Default.lowThreshold;
            currentCalc.currentPresets.highThreshold = Properties.Settings.Default.highThreshold;
            currentCalc.currentInputs.deviceId = Properties.Settings.Default.DeviceId;
            currentCalc.currentPresets.defaultRosterLength = 72;
            
            // so do the calculation!
            currentCalc.doCalc();

            //log the calculation
            ILogService log = new logFile();
            log.setLogURL(Properties.Settings.Default.LogServiceURL);
            if (log.isValid)
            {
                currentCalc.logged = currentCalc.logCalc(log);
            }
            else
            {
                //failed to log the calculation...let's just make sure the calculator knows it
                currentCalc.logged = false;
            }

            // on to the results
            resultsPage next = new resultsPage(currentCalc);
            this.NavigationService.Navigate(next);
        }

        private void btnSleep_Click(object sender, RoutedEventArgs e)
        {
            // easy, just go back a page
            this.NavigationService.GoBack();

        }

        private void btnShift_Click(object sender, RoutedEventArgs e)
        {
            // harder, need to go back to the last shift page
            // lets try removing a back entry and then going back
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.GoBack();
        }
    }
}
