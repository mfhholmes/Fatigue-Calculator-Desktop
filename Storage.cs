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

        private class logEntry
        {
            public string DateDone { get; set; }
            public string TimeDone { get; set; }
            public string DeviceId { get; set; }
            public string Identity { get; set; }
            public string ShiftStart { get; set; }
            public string ShiftEnd { get; set; }
            public string sleep24 { get; set; }
            public string sleep48 { get; set; }
            public string hoursAwake { get; set; }
            public string lowThreshold { get; set; }
            public string highThreshold { get; set; }
            public string currentScore { get; set; }
            public string currentLevel { get; set; }
            public string algorithmVersion { get; set; }

            public logEntry(string logLine)
            {
                try
                {
                    string[] entries = logLine.Split(',');
                    if (entries.Length == 14)
                    {
                        DateDone = entries[0];
                        TimeDone = entries[1];
                        DeviceId = entries[2];
                        Identity = entries[3];
                        ShiftStart = entries[4];
                        ShiftEnd = entries[5];
                        sleep24 = entries[6];
                        sleep48 = entries[7];
                        hoursAwake = entries[8];
                        lowThreshold = entries[9];
                        highThreshold = entries[10];
                        currentScore = entries[11];
                        currentLevel = entries[12];
                        algorithmVersion = entries[13];
                    }
                    else
                    {
                        // there's an issue with earlier versions missing the identity line
                        DateDone = entries[0];
                        TimeDone = entries[1];
                        DeviceId = entries[2];
                        Identity = "";
                        ShiftStart = entries[3];
                        ShiftEnd = entries[4];
                        sleep24 = entries[5];
                        sleep48 = entries[6];
                        hoursAwake = entries[7];
                        lowThreshold = entries[8];
                        highThreshold = entries[9];
                        currentScore = entries[10];
                        currentLevel = entries[11];
                        algorithmVersion = entries[12];
                    }
                }
                catch
                {
                    // must be another bad thing
                    DateDone = "";
                    TimeDone = "";
                    DeviceId = "";
                    Identity = "";
                    ShiftStart = "";
                    ShiftEnd = "";
                    sleep24 = "";
                    sleep48 = "";
                    hoursAwake = "";
                    lowThreshold = "";
                    highThreshold = "";
                    currentScore = "";
                    currentLevel = "";
                    algorithmVersion = "";

                }
            }
        }

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
                filename = storage.getPath(filename);
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
            string logfile = getPath(Properties.Settings.Default.logfile);
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
                entry = parseLog(line);
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
        private logEntry parseLog(string logLine)
        {
            logEntry result = new logEntry(logLine);
            return result;
        }
        private string getPath(string path)
        {
            string result;
            if (path.IndexOf("%") > -1)
            {
                // special path is the bit between the %'s
                string special;
                special = path.Substring(path.IndexOf("%") + 1);
                special = special.Substring(0, special.IndexOf("%"));
                special = lookupSpecial(special);
                result = path.Substring(0, path.IndexOf("%")) + special + "\\" + path.Substring(path.LastIndexOf("%") + 1);
            }
            else
            {
                result = path;
            }
            return result;
        }
        private string lookupSpecial(string special)
        {
            string result;
            switch (special)
            {
                //TODO: add more special paths
                case "app.path":
                    {
                        result = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                        break;
                    }
                case "documents":
                    {
                        result = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments, Environment.SpecialFolderOption.Create);
                        break;
                    }
                default:
                    {
                        result = "";
                        break;
                    }

            }
            return result;
        }
        public bool logCalc(calculation currentCalc)
        {

            // get the logfile path
            string logfile = getPath(Properties.Settings.Default.logfile);
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
            // just check first in case we need to write the headers
            bool writeHeaders = false;
            if (!log.Exists) writeHeaders = true;
            StreamWriter writer = new StreamWriter(log.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
            if (writeHeaders)
            {
                writer.WriteLine("Date,Time,DeviceID,UserID,ShiftStart,ShiftEnd,Sleep24,Sleep48,HoursAwake,LowThreshold,HighThreshold,Score,Level,version");
            }
            string output = "";
            output += currentCalc.currentOutputs.calcDone.ToString("dd/MMM/yyyy") + ",";
            output += currentCalc.currentOutputs.calcDone.ToString("hh:mm:ss") + ",";
            output += currentCalc.currentInputs.deviceId+",";
            output += currentCalc.currentInputs.identity + ",";
            output += currentCalc.currentInputs.shiftStart.ToString("dd/MMM/yyyy hh:mm:ss") + ",";
            output += currentCalc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy hh:mm:ss") + ",";
            output += currentCalc.currentInputs.sleep24 + ",";
            output += currentCalc.currentInputs.sleep48 + ",";
            output += currentCalc.currentInputs.hoursAwake + ",";
            output += currentCalc.currentPresets.lowThreshold + ",";
            output += currentCalc.currentPresets.highThreshold + ",";
            output += currentCalc.currentOutputs.currentScore.ToString()  + ",";
            output += currentCalc.currentOutputs.currentLevel.ToString() + ",";
            output += currentCalc.currentPresets.algorithmVersion.ToString();
            writer.WriteLine(output);
            writer.Close();
            return true;
        }
    }
}
