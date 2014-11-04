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
    /// Interaction logic for sleepPage.xaml
    /// </summary>
    public partial class sleepPage : Page
    {
        private calculation currentCalc;
        private bool helpShown = true;

        public sleepPage()
        {
            InitializeComponent();
        }

        public sleepPage(calculation passedCalc)
        {
            InitializeComponent();
            currentCalc = passedCalc;
            // TODO: might need to work out the mins better to find the nearest 15 mins 'cos of double rounding errors
            spnSleep24Hours.Value  = (int)currentCalc.currentInputs.sleep24;
            spnSleep24Mins.Value = (int)((currentCalc.currentInputs.sleep24 - spnSleep24Hours.Value) * 60.0);
            spnSleep48Hours.Value = (int)currentCalc.currentInputs.sleep48;
            spnSleep48Mins.Value = (int)((currentCalc.currentInputs.sleep48 - spnSleep48Hours.Value) * 60.0);
            spnHoursAwake.Value = (int)currentCalc.currentInputs.hoursAwake;
            spnMinsAwake.Value = (int)((currentCalc.currentInputs.hoursAwake - spnMinsAwake.Value) * 60.0);
            sleep24Text.Text = "How many hours of sleep have you had in the last 24 hours from " + DateTime.Now.ToString("HH:mm") + "?";
            sleep48Text.Text = "How many hours of sleep have you had in the last 48 hours from " + DateTime.Now.ToString("HH:mm") + "?";
            hoursAwakeText.Text = "How many hours since you last woke, as of " + DateTime.Now.ToString("HH:mm") + "?";
            btnHelp_Click(this, new RoutedEventArgs());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //back to the shift page
            this.NavigationService.GoBack();

        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            // save the values
            currentCalc.currentInputs.sleep24 = spnSleep24Hours.Value + (spnSleep24Mins.Value / 60.0);
            currentCalc.currentInputs.sleep48 = spnSleep48Hours.Value + (spnSleep48Mins.Value / 60.0);
            currentCalc.currentInputs.hoursAwake = spnHoursAwake.Value + (spnMinsAwake.Value / 60.0);

            // forward to the confirmation page
            confirmPage next = new confirmPage(currentCalc);
            this.NavigationService.Navigate(next);
        }

        private void spnSleep24Hours_ValueChanged(object sender, RoutedEventArgs e)
        {
            // must have slept as least as much in the last 48 hours as we did in the last 24
            spnSleep48Hours.minValue = spnSleep24Hours.Value;
            // can't have slept more in the last 48 hours than we did in the last 24 + 24
            spnSleep48Hours.maxValue = 24 + spnSleep24Hours.Value;

            if ((spnSleep24Hours.Value == spnSleep48Hours.Value) && (spnSleep24Mins.Value > spnSleep48Mins.Value)) spnSleep48Mins.Value = spnSleep24Mins.Value;

            // check for max hours, then set mins to 0
            if (spnSleep24Hours.Value == spnSleep24Hours.maxValue) spnSleep24Mins.Value = 0;

        }

        private void spnSleep48Hours_ValueChanged(object sender, RoutedEventArgs e)
        {
            //can't have slept more in the last 24 hours than we have in the last 48
            //spnSleep24.maxValue = spnSleep48.Value;
            if ((spnSleep24Hours.Value == spnSleep48Hours.Value) && (spnSleep24Mins.Value > spnSleep48Mins.Value)) spnSleep48Mins.Value = spnSleep24Mins.Value;

            if (spnSleep48Hours.Value == spnSleep48Hours.maxValue) spnSleep48Mins.Value = 0;
        }

        private void spnHoursAwake_ValueChanged(object sender, RoutedEventArgs e)
        {
            //not sure about this...we might have to involve shift times in this too
            //spnSleep24.maxValue = 24 - spnHoursAwake.Value;
            if (spnHoursAwake.Value == spnHoursAwake.maxValue) spnMinsAwake.Value = 0;
        }

        private void spnSleep24Mins_ValueChanged(object sender, RoutedEventArgs e)
        {
            if ((spnSleep24Hours.Value == spnSleep48Hours.Value) && (spnSleep24Mins.Value > spnSleep48Mins.Value)) spnSleep48Mins.Value = spnSleep24Mins.Value;
        }

        private void spnSleep48Mins_ValueChanged(object sender, RoutedEventArgs e)
        {
            if ((spnSleep24Hours.Value == spnSleep48Hours.Value) && (spnSleep24Mins.Value > spnSleep48Mins.Value)) spnSleep48Mins.Value = spnSleep24Mins.Value;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            if (helpShown)
            {
                sleep24label.Inlines.Remove(sleep24Help);
                sleep48label.Inlines.Remove(sleep48Help);
                hoursAwakelabel.Inlines.Remove(hoursAwakeHelp);
                sleep24label.Inlines.Remove(LB1);
                sleep48label.Inlines.Remove(LB2);
                hoursAwakelabel.Inlines.Remove(LB3);
                helpShown = false;
            }
            else
            {
                sleep24label.Inlines.Add(LB1);
                sleep48label.Inlines.Add(LB2);
                hoursAwakelabel.Inlines.Add(LB3);
                sleep24label.Inlines.Add(sleep24Help);
                sleep48label.Inlines.Add(sleep48Help);
                hoursAwakelabel.Inlines.Add(hoursAwakeHelp);
                helpShown = true;
            }

        }

    }
}
