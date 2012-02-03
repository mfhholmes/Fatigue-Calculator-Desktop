using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fatigue_Calculator_Desktop
{
    class Storage
    {
        private struct person
        {
            public int IDnumber;
            public string Name;
        }
        private person[] people;

        private person splitString(string[] data)
        {
            person result = new person();
            result.IDnumber = Convert.ToInt16(data[0]);
            result.Name = data[1].ToUpper();
            return result;
        }

        public bool loadIDLookupList()
        {
            try
            {
                // get the filename from the settings
                string filename = Properties.Settings.Default.IdentityLookupFile;
                if (filename.Length == 0)
                {
                    return false;
                }
                // check for special paths using  the storage class
                Storage storage = new Storage();
                filename = Utilities.parsePath(filename);
                // see if the file exists
                System.IO.FileInfo file = new System.IO.FileInfo(new System.Uri(filename).LocalPath);
                if (!file.Exists)
                {
                    return false;
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
                return true;
            }
            catch
            {
                // failed somewhere along the line
                return false;
            }

        }
        public int doLookup(string text)
        {
            // check the array and see how many names or numbers match in the file
            // is it a name or a number?
            int result;
            if (int.TryParse(text, out result))
            {
                result = 0;
                // numbers
                for (int i = 0; i < people.Length; i++)
                {
                    if (people[i].IDnumber.ToString().Substring(0, Math.Min(text.Length, people[i].IDnumber.ToString().Length)).ToUpper() == text.ToUpper())
                    {
                        result++;
                    }
                }

            }
            else
            {
                // name
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
        public string getLookup(string text)
        {
            // same as before, only we just return the first match
            // check the array and see how many names or numbers match in the file
            int result;
            if (int.TryParse(text, out result))
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
                        return people[i].Name;
                    }
                }
            }
            return "";

        }
        public bool isNewPerson(string name)
        {
            // load up the log results
            // get the logfile path
            string logfile = Utilities.parsePath(Properties.Settings.Default.LogServiceURL);
            // open the file and create it if it doesn't exist
            FileInfo log;
            try
            {
                log = new FileInfo(logfile);
            }
            catch
            {
                // invalid log file path
                return false;
            }
            // if there's no log yet then this is the first run
            if (!log.Exists) return true;

            // iterate through each row and look for the name
            StreamReader reader = new StreamReader(log.OpenRead());
            string line;
            logEntry entry;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                entry = new logEntry();
                if (entry.Identity == name)
                {
                    // if we find it, return true
                    reader.Close();
                    return false;
                }
            }
            // if we reach the end of the file, we didn't find it
            return true;
        }
        
    }
}
