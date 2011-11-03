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

        private enum lookupType
        {
            none,
            dynamic,
            single
        }
        private lookupType lookup;

        private struct person
        {
            public int IDnumber;
            public string Name;
        }
        private person[] people;

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
                        shiftPage next = new shiftPage(currentCalc);
                        this.NavigationService.Navigate(next);
                        return;
                    }
                case lookupType.dynamic:
                    {
                        // check if it's a valid name or not
                        if (checkLookup(txtName.Text, true))
                        {
                            // matched, and already in the control, let's go
                            currentCalc.currentInputs.identity = this.txtName.Text;
                            shiftPage next = new shiftPage(currentCalc);
                            this.NavigationService.Navigate(next);
                            return;
                        }
                        else
                        {
                            // didn't match...no go
                            //TODO: we should maybe tell them?
                            return;
                        }
                    }
                case lookupType.single:
                    {
                        // check if it's a valid name or not
                        if (checkLookup(txtName.Text, true))
                        {
                            // matched, but not in the control
                            this.txtName.Text = getLookup(txtName.Text);
                            currentCalc.currentInputs.identity = this.txtName.Text;
                            shiftPage next = new shiftPage(currentCalc);
                            this.NavigationService.Navigate(next);
                            return;
                        }
                        else
                        {
                            // didn't match...no go
                            //TODO: we should maybe tell them?
                            return;
                        }

                    }
                default:
                    {
                        //TODO: some kind of error here?
                        return;
                    }
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

#if Multiuser
            // get the lookup type from the settings
            string ltype = Properties.Settings.Default.IDLookupType;
            switch (ltype.ToLower())
            {
                case "dynamic":
                    {
                        lookup = lookupType.dynamic;
                        break;
                    }
                case "none":
                    {
                        lookup= lookupType.none;
                        break;
                    }
                case "single":
                    {
                        lookup= lookupType.single;
                        break;
                    }
                default:
                    {
                        lookup= lookupType.none;
                        break;
                    }
            }
            {
                // get the filename from the settings
                string filename = Properties.Settings.Default.IdentityLookupFile;
                if (filename.Length == 0)
                {
                    lookup= lookupType.none;
                    return;
                }
                // check for special paths using  the storage class
                Storage storage = new Storage();
                filename = storage.getPath(filename);
                // see if the file exists
                System.IO.FileInfo file = new System.IO.FileInfo(new System.Uri(filename).LocalPath);
                if (!file.Exists)
                {
                    lookup= lookupType.none;
                    return;
                }

                // if so, attempt to load it
                System.IO.FileStream fstream = file.OpenRead();

                // iterate through the list and set up the array
                System.IO.StreamReader reader = new System.IO.StreamReader(fstream);
                string personData = "";
                person data;
                System.Collections.ArrayList newPeople = new System.Collections.ArrayList();
                while (!reader.EndOfStream)
                {
                    personData = reader.ReadLine();
                    data = splitString(personData.Split(','));
                    newPeople.Add(data);
                }
                reader.Close();
                // transfer the people to the array
                people = new person[newPeople.Count];
                for (int i = 0; i < newPeople.Count; i++) people[i] = (person)newPeople[i];
                // and we're done
            }
            //catch
            //{
            //    // failed somewhere along the line
            //}
#else
            lookup = lookupType.none;
            return;
#endif
            //try
        }
        private person splitString(string[] data)
        {
            person result = new person();
            result.IDnumber = Convert.ToInt16(data[0]);
            result.Name = data[1].ToUpper();
            return result;
        }
        private bool checkLookup(string text, bool isFinal)
        {
            // check the flag
            switch (lookup)
            {
                case lookupType.dynamic:
                    {
                        int matches = doLookup(text);
                        if (matches == 1)
                        {
                            txtName.Text = getLookup(text);
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
                            if (doLookup(text) == 1)
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
        private int doLookup(string text)
        {
            // check the array and see how many names or numbers match in the file
            int result;
            if(int.TryParse(text, out result))
            {
                result = 0;
                // numbers
                for (int i = 0; i < people.Length; i++)
                {
                    if (people[i].IDnumber.ToString().Substring(0,Math.Min(text.Length, people[i].IDnumber.ToString().Length)).ToUpper() == text.ToUpper())
                    {
                        result++;
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < people.Length; i++)
                {
                    if (people[i].Name.Substring(0, Math.Min(text.Length, people[i].Name.Length)).ToUpper() == text.ToUpper())
                    {
                        result++;
                    }
                }
            }
            return result;

        }
        private string getLookup(string text)
        {
            // same as before, only we just return the first match
            // check the array and see how many names or numbers match in the file
            int result;
            if(int.TryParse(text, out result))
            {
                result = 0;
                // numbers
                for (int i = 0; i < people.Length; i++)
                {
                    if (people[i].IDnumber.ToString().Substring(0, Math.Min(text.Length, people[i].IDnumber.ToString().Length)).ToUpper() == text.ToUpper())
                    {
                        return people[i].IDnumber.ToString();
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < people.Length; i++)
                {
                    if (people[i].Name.Substring(0, Math.Min(text.Length, people[i].Name.Length)).ToUpper() == text.ToUpper())
                    {
                        return people[i].Name ;
                    }
                }
            }
            return "";

        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = checkLookup(e.Text,false);
        }

    }
}
