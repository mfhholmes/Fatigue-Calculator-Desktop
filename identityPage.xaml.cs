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
    /// Interaction logic for identityPage.xaml
    /// </summary>
    public partial class identityPage : Page
    {
        private calculation _currentCalc;
        private IidentityLookup _idLookup;
        private bool _warned = false;

        public identityPage()
        {
            InitializeComponent();
            _idLookup = new identityLookupFactory().getLookup();
        }
        public identityPage(calculation passedCalc)
        {
            InitializeComponent();
            // set up values from calculation
            _currentCalc = passedCalc;
            _idLookup = new identityLookupFactory().getLookup();
        }

        public bool skipPage
        {
            get
            {
                if(!_idLookup.displayPage)
                {
                    // skipping the page, so set the default identity
                    _currentCalc.currentInputs.identity = _idLookup.validate("");
                    return true;
                }
                return false;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //go back to welcome screen
            this.NavigationService.GoBack();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if(_warned)
            {
                // just move to the next screen, we've done all this once
                shiftPage next = new shiftPage(_currentCalc);
                this.NavigationService.Navigate(next);
            }
            // get the match from the lookup
            identity valid = _idLookup.validate(txtName.Text);
            _currentCalc.currentInputs.identity = valid;
            if(valid==null || !valid.isValid)
            {
                match.Text = "That name or ID does not match a valid identity.";
                match.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            // now check if this is their first calculation, and if so, give them the Disclaimer
            ILogService log = new logFile();
            log.setLogURL(Config.ConfigSettings.settings.logServiceUrl);

            if(log.isIdentityOnLog(valid))
            {
                // check if they've done a calculation in the last 12 hours
                DateTime? lastLog = log.lastLogEntryForUser(valid);
                if ((lastLog != null) && (lastLog > (DateTime.Now - new TimeSpan(12,0,0))))
                {
                    // yeah, the last calculation was within the last 12 hours so display a warning
                    bdrWarning.Visibility = System.Windows.Visibility.Visible;
                    _warned = true;
                    return;
                }
                shiftPage next = new shiftPage(_currentCalc);
                this.NavigationService.Navigate(next);
            }
            else
            {
                disclaimerPage disclaimer = new disclaimerPage(_currentCalc);
                this.NavigationService.Navigate(disclaimer);
            }

        }

        private void Keyboard_Click(object sender, RoutedEventArgs e)
        {
            Button key = (Button)sender;
            bool doPopulate = true;
            if (key.Content.ToString() == "Back")
            {
                txtName.Text = txtName.Text.Substring(0, txtName.Text.Length - 1);
                doPopulate = false;
            }
            else if (key.Content.ToString() == "Reset")
            {
                txtName.Text = "";
                doPopulate = false;
            }
            else if (key.Content.ToString() == "Space")
            {
                txtName.Text += " ";
                doPopulate = true;
            }
            else
            {
                txtName.Text = txtName.Text + key.Content.ToString();
                doPopulate = true;
            }
            checkLookup(txtName.Text, doPopulate);

        }
        private bool checkLookup(string text, bool populateOnBest = true)
        {
            match.Visibility = System.Windows.Visibility.Visible;
            int numMatches = _idLookup.getMatchCount(text);
            if (numMatches == 1)
            {
                if (populateOnBest)
                {
                    txtName.Text = _idLookup.getBestMatch(text);
                    txtName.CaretIndex = txtName.Text.Length;
                }
                match.Text = "valid identity match";
                return true;
            }
            if (numMatches == 0)
                match.Text = "no matches";
            else
                match.Text = "multiple matches";
            return false;
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = checkLookup(txtName.Text + e.Text);
        }

    }
}
