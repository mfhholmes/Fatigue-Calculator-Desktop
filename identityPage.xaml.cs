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
            identity valid = _idLookup.validate(txtInput.Text);
            _currentCalc.currentInputs.identity = valid;
            if(valid==null || !valid.isValid)
            {
                lblMatch.Text = "That name or ID does not match a valid identity.";
                lblMatch.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            // now check if this is their first calculation, and if so, give them the Disclaimer
            ILogService log = LogFactory.createLog(Config.ConfigSettings.settings.logServiceUrl);
            
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
            handleInput(key.Content.ToString());
        }
        private void handleInput(string input)
        {
            if (input== "Back")
            {
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            }
            else if (input == "Reset")
            {
                txtInput.Text = "";
            }
            else if (input == "Space")
            {
                txtInput.Text += " ";
            }
            else if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-'1234567890 ".Contains(input))
            {
                txtInput.Text = txtInput.Text + input.ToUpper();
            }
            checkLookup(txtInput.Text);
        }
        private bool checkLookup(string text)
        {
            lblMatch.Visibility = System.Windows.Visibility.Visible;
            int numMatches = _idLookup.getMatchCount(text);
            if (numMatches == 1)
            {
                txtMatch.Text = _idLookup.getBestMatch(text).Substring(text.Length).ToUpper();
                txtInput.Text = text;
                lblMatch.Text = "valid identity match";
                return true;
            }
            if (numMatches == 0)
            {
                lblMatch.Text = "no matches";
                txtMatch.Text = "";
            }
            else
            {
                lblMatch.Text = "multiple matches";
                txtMatch.Text = "";
            }
            return false;
        }

        private void page_TextInput(object sender, TextCompositionEventArgs e)
        {
            handleInput(e.Text);
        }


    }
}
