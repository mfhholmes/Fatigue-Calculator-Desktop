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
        private calculation currentCalc;
        private Storage currentStore;

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
            loadLookup();
        }
        public identityPage(calculation passedCalc)
        {
            InitializeComponent();
            // set up values from calculation
            currentCalc = passedCalc;
            currentStore = new Storage();
            loadLookup();
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
                        currentCalc.currentInputs.identity = this.txtName.Text;
                        break;
                    }
                case lookupType.dynamic:
                    {
                        // check if it's a valid name or not
                        if (checkLookup(txtName.Text, true))
                        {
                            // matched, and already in the control, let's go
                            currentCalc.currentInputs.identity = this.txtName.Text;
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
                            this.txtName.Text = currentStore.getLookup(txtName.Text);
                            currentCalc.currentInputs.identity = this.txtName.Text;
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
            if (currentStore.isNewPerson(this.txtName.Text))
            {
                disclaimerPage disclaimer = new disclaimerPage(currentCalc);
                this.NavigationService.Navigate(disclaimer);
            }
            else
            {
                shiftPage next = new shiftPage(currentCalc);
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
        private void loadLookup()
        {

#if (Multiuser || Unprotected || DEBUG)
            // get the lookup type from the settings
            string ltype = Properties.Settings.Default.IDLookupType;
            switch (ltype.ToLower())
            {
                case "dynamic":
                    {
                        if (currentStore.loadIDLookupList())
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
                        if (currentStore.loadIDLookupList())
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
            // check the flag
            switch (lookup)
            {
                case lookupType.dynamic:
                    {
                        int matches = currentStore.doLookup(text);
                        if (matches == 1)
                        {
                            txtName.Text = currentStore.getLookup(text);
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
                            if (currentStore.doLookup(text) == 1)
                            {
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
