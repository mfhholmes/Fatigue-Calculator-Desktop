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
        private IdentityFile _idFile;

        private enum lookupType
        {
            none,
            dynamic,
            single
        }
        private lookupType lookup;


        public identityPage()
        {
            InitializeComponent();
            loadIdentities();
        }
        public identityPage(calculation passedCalc)
        {
            InitializeComponent();
            // set up values from calculation
            _currentCalc = passedCalc;
            loadIdentities();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //go back to welcome screen
            this.NavigationService.GoBack();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            // no validation for the identity
            switch (lookup)
            {
                case lookupType.none:
                    {
                        //  no validation, free input, let's go
                        _currentCalc.currentInputs.identity = this.txtName.Text;
                        break;
                    }
                case lookupType.dynamic:
                    {
                        // check if it's a valid name or not
                        if (checkLookup(txtName.Text, true))
                        {
                            // matched, and already in the control, let's go
                            _currentCalc.currentInputs.identity = this.txtName.Text;
                            break;
                        }
                        else
                        {
                            // didn't match...no go
                            return;
                        }
                    }
                case lookupType.single:
                    {
                        // check if it's a valid name or not
                        if (checkLookup(txtName.Text, true))
                        {
                            // matched, but not in the control
                            List<identity> matches = _idFile.LookUpName(txtName.Text);
                            if (matches.Count == 1)
                                this.txtName.Text = matches.ElementAt(0).Name;
                            _currentCalc.currentInputs.identity = this.txtName.Text;
                            break;
                        }
                        else
                        {
                            // didn't match...no go
                            return;
                        }
                    }
                default:
                    {
                        //TODO: some kind of error here?
                        return;
                    }
            }
            // now check if this is their first calculation, and if so, give them the Disclaimer
            if (_idFile.LookUpName(txtName.Text).Count == 0)
            {
                disclaimerPage disclaimer = new disclaimerPage(_currentCalc);
                this.NavigationService.Navigate(disclaimer);
            }
            else
            {
                shiftPage next = new shiftPage(_currentCalc);
                this.NavigationService.Navigate(next);
            }
        }

        private void Keyboard_Click(object sender, RoutedEventArgs e)
        {
            Button key = (Button)sender;
            if (key.Content.ToString() == "Back") txtName.Text = txtName.Text.Substring(0, txtName.Text.Length - 1);
            else if (key.Content.ToString() == "Reset") txtName.Text = "";
            else if (key.Content.ToString() == "Space") txtName.Text += " ";
            else txtName.Text = txtName.Text + key.Content.ToString();
            checkLookup(txtName.Text,false);

        }
        private void loadIdentities()
        {
        _idFile = new IdentityFile();
#if (Multiuser || Unprotected || DEBUG)
            // get the lookup type from the settings
            string ltype = Properties.Settings.Default.IDLookupType;
            string filename = Properties.Settings.Default.IdentityLookupFile;
            
            switch (ltype.ToLower())
            {
                case "dynamic":
                    {
                        if ( _idFile.SetIdentityListSource(filename))
                        {
                            lookup = lookupType.dynamic;
                        }
                        else
                        {
                            lookup = lookupType.none;
                        }
                        break;
                    }
                case "none":
                    {
                        lookup= lookupType.none;
                        break;
                    }
                case "single":
                    {
                        if (_idFile.SetIdentityListSource(filename))
                        {
                            lookup = lookupType.single;
                        }
                        else
                        {
                            lookup = lookupType.none;
                        }
                        break;
                    }
                default:
                    {
                        lookup= lookupType.none;
                        break;
                    }
            }
            
#else
            lookup = lookupType.none;
            return;
#endif
            //try
        }
        private bool checkLookup(string text, bool isFinal)
        {
            identity matched;
            // check the flag
            switch (lookup)
            {
                case lookupType.dynamic:
                    {
                        int matches = _idFile.LookUpName(text).Count;
                        if (matches == 1)
                        {
                            matched = _idFile.LookUpName(text).ElementAt(0);
                            txtName.Text = matched.Name;
                            match.Visibility = System.Windows.Visibility.Visible;
                            match.Text = "Match Found";
                            return true;
                        }
                        else
                        {
                            match.Visibility = System.Windows.Visibility.Visible;
                            if (matches > 0)
                            {
                                match.Text = "Multiple Matches Found";
                            }
                            else
                            {
                                match.Text = "No Matches Found";
                            }
                            return false;
                        }
                    }
                case lookupType.none:
                    {
                        // always return false...free entry
                        match.Visibility = System.Windows.Visibility.Hidden;
                        return false;
                    }
                case lookupType.single:
                    {
                        if (isFinal)
                        {
                            int matches = _idFile.LookUpName(text).Count;
                            if (matches == 1)
                            {
                                matched = _idFile.LookUpName(text).ElementAt(0);
                                txtName.Text = matched.Name;
                                match.Visibility = System.Windows.Visibility.Visible;
                                match.Text = "Match Found";
                                return true;
                            }
                            else
                            {
                                match.Visibility = System.Windows.Visibility.Visible;
                                match.Text = "Multiple Matches Found";
                                return false;
                            }
                        }
                        else
                        {
                            match.Visibility = System.Windows.Visibility.Hidden;
                            return false;
                        }
                            
                    }
                default:
                    return false;
            }
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = checkLookup(e.Text,false);
        }

    }
}
