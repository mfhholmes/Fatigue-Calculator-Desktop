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
            // get the match from the lookup
            identity valid = new identity(_idLookup.validate(txtName.Text));
            _currentCalc.currentInputs.identity = valid.Name;
            if(!valid.isValid)
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
            if (key.Content.ToString() == "Back") txtName.Text = txtName.Text.Substring(0, txtName.Text.Length - 1);
            else if (key.Content.ToString() == "Reset") txtName.Text = "";
            else if (key.Content.ToString() == "Space") txtName.Text += " ";
            else txtName.Text = txtName.Text + key.Content.ToString();
            checkLookup(txtName.Text);

        }
        private bool checkLookup(string text)
        {
            match.Visibility = System.Windows.Visibility.Visible;
            int numMatches = _idLookup.getMatchCount(text);
            if (numMatches == 1)
            {
                txtName.Text = _idLookup.getBestMatch(text);
                txtName.CaretIndex = txtName.Text.Length;
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
