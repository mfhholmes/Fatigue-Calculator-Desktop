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
using System.Windows.Threading;



namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for shiftPage.xaml
    /// </summary>
    public partial class shiftPage : Page
    {
        private calculation currentCalc;

        public shiftPage()
        {
            InitializeComponent();
        }
        public shiftPage(calculation passedCalc)
        {
            InitializeComponent();
            // set up the values of the current page with the values in the current calc
            currentCalc = passedCalc;
            spnStartHour.Value = currentCalc.currentInputs.shiftStart.Hour;
            spnStartMin.Value = currentCalc.currentInputs.shiftStart.Minute;
            spnEndHour.Value = currentCalc.currentInputs.shiftEnd.Hour;
            spnEndMin.Value = currentCalc.currentInputs.shiftEnd.Minute;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //go back to the identity screen
            this.NavigationService.GoBack();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            
            // work out the times
            DateTime shiftStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, spnStartHour.Value, spnStartMin.Value,0);
            DateTime shiftEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, spnEndHour.Value, spnEndMin.Value, 0);
            
            // check for overnight shift, add a day to shift end if so
            if (shiftEnd < shiftStart) shiftEnd += new TimeSpan(1, 0, 0, 0);
            
            // add them to the calculator inputs
            currentCalc.currentInputs.shiftStart = shiftStart;
            currentCalc.currentInputs.shiftEnd = shiftEnd;

            //go forward to the hours slept page
            sleepPage next = new sleepPage(currentCalc);
            this.NavigationService.Navigate(next);
        }



    }
}
