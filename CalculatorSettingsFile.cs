using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;


namespace Fatigue_Calculator_Desktop
{
    public class CalculatorSettingsFile : Fatigue_Calculator_Desktop.ICalculatorSettings
    {
        /// <summary>
        /// Default filename for the settings file
        /// </summary>
        const string DEFAULT_SETTINGS_FILENAME = "Fatigue Calculator Desktop.exe.config";

        /// <summary>
        /// the filename of the settings file
        /// </summary>
        string _fileName = "";
        public string fileName
        {
            get { return _fileName; }
            set 
            {
                if(File.Exists(value))
                   _fileName = value; 
            }
        }

        private string _validationError = "";
        public string lastValidationError
        {
            get { return _validationError; }
        }
        
        /// <summary>
        /// true if the settings file is loaded
        /// </summary>
        bool _isLoaded = false;
        public bool isLoaded
        {
            get { return _isLoaded; }
        }

        private XmlDocument _settings;

        /// <summary>
        /// constructor that finds the default file and loads it if the filename isn't specified
        /// </summary>
        public CalculatorSettingsFile()
        {
            if (_fileName.Length != 0)
            {
                LoadFile();
            }
            else
            {
                // check to see whether we've got a valid filename in the app path
                string currentPath = checkPath(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(this.GetType()).Location));
                if (currentPath.Length > 0)
                {
                    LoadFile(currentPath);
                    return;
                }
                // nope, so try the application start path
                currentPath = checkPath(System.Windows.Application.Current.StartupUri.LocalPath);
                if(currentPath.Length > 0)
                {
                    LoadFile(currentPath);
                    return;
                }
            }
        }

        /// <summary>
        /// checks whether a path contains a valid config file
        /// </summary>
        /// <param name="path">the path to check</param>
        /// <returns>the full path and filename for the config file found</returns>
        public string checkPath(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    FileInfo[] results = dir.GetFiles(DEFAULT_SETTINGS_FILENAME);
                    if (results.Length > 0)
                    {
                        return results[0].FullName;
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Loads a settings file, either the one specified in the fileName property or passed as a param
        /// </summary>
        /// <param name="passedFileName">optional, the file name of the settings file to load</param>
        /// <returns>true if the file was loaded</returns>
        public bool LoadFile(string passedFileName = "not_passed")
        {
            // check if we were passed a file to load
            if (passedFileName != "not_passed")
            { 
                // check if the passed file name is valid
                if (File.Exists(passedFileName))
                {
                    _fileName = passedFileName;
                }
                else
                {
                    // passed an invalid file, so refuse to load it
                    return false;
                }
            }
            // check if we've got a file at all...
            if (!File.Exists(_fileName))
            {
                // nope, so let's go find one
                if (!FindFile())
                {
                    // can't find a settings file on any drive...
                    return false;
                }
            }

            // open the config file
            try
            {
                _settings = new XmlDocument();
                _settings.Load(new XmlTextReader(_fileName));
            }
            catch(Exception e)
            {
                // failed
                //EventLog.WriteEntry("Fatigue Calculator Admin Utility", "Failed to load XML config file: " + _fileName + ". Error Reported as: " + e.Message);
                _validationError = "Failed to load file. " + e.Message;
                return false;
            }
            // let them know we did it
            _isLoaded = true;
            return true;
        }

        /// <summary>
        /// searches the local drives of the machine to find the settings file
        /// assumed the file is the default filename as specified in the constant
        /// </summary>
        /// <returns>true if a settings file is found</returns>
        public bool FindFile()
        {
            //TODO: shove this into a background worker thread 
            string[] result;
            // iterate drives
            foreach (DriveInfo drive in DriveInfo.GetDrives().Where(x => x.IsReady == true))
            {
                // let's just check if we've got any directories starting with 'program' and search them first, try to find the program files dir where fatige calc is probably installed
                foreach (string dir in Directory.GetDirectories(drive.RootDirectory.FullName, "Program*" , SearchOption.TopDirectoryOnly))
                {
                    result = Directory.GetFiles(dir, DEFAULT_SETTINGS_FILENAME, SearchOption.AllDirectories);
                    if (result.Length > 0)
                    {
                        // got one
                        _fileName = result[0];
                        return true;
                    }
                }
                // default search - everything! This might take a while lol
                result = Directory.GetFiles(drive.RootDirectory.FullName, DEFAULT_SETTINGS_FILENAME, SearchOption.AllDirectories);
                if (result.Length > 0)
                {
                    // got one
                    _fileName = result[0];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a Dictionary of the settings in the config file if it has been opened
        /// </summary>
        /// <returns>a dictionary object with the setting name in the key values and the setting value in the value values</returns>
        public Dictionary<string,string> GetSettings()
        {
            if (!isLoaded)
            {
                if (!LoadFile(_fileName))
                {
                    return default(Dictionary<string, string>);
                }
            }
            // ok, settings file loaded, let's iterate that bitch!

            // iterate the settings and copy the values
            Dictionary<string, string> result = new Dictionary<string,string>();
            // config file structure is pretty simple
            // <configuration>
            //      <configSections>
            //      <applicationSettings>
            //          <Fatigue_Calculator_Desktop.Properties.Settings>
            //              <setting name serializeAs="type">
            //                  <value>
            foreach (XmlNode node in _settings.DocumentElement.GetElementsByTagName("setting"))
            {
                result.Add(node.Attributes.GetNamedItem("name").InnerXml, node.FirstChild.InnerXml);
            }
            return result;
        }

        /// <summary>
        /// changes a setting in the loaded config file
        /// </summary>
        /// <param name="Key">the name of the setting to change</param>
        /// <param name="Value">the new value for the setting</param>
        /// <returns></returns>
        public bool ChangeSetting(string Key, string Value)
        {
            int tryCount = 0;
            bool tryAgain = true;
            while (tryAgain)
            {
                tryCount++;
                tryAgain = false;
                try
                {
                    if (validateSetting(Key, Value))
                    {
                        _settings.DocumentElement.SelectSingleNode("//setting[@name='" + Key + "']/value").InnerXml = Value;
                        _settings.Save(_fileName);
                    }
                    else
                        return false;
                }
                catch (Exception e)
                {
                    // if we've already done this, then let's not do it again
                    if (tryCount > 1)
                    {
                        _validationError = "Saving the setting failed. " + e.Message;
                        return false;
                    }
                    else
                    {
                        // go again
                        tryAgain = true;
                    }
                    // try setting the file access permissions
                    // check for the file being read only
                    FileInfo checkFile = new FileInfo(_fileName);
                    if (checkFile.IsReadOnly)
                        checkFile.IsReadOnly = false;
                    // now check for write permission
                    FileSecurity fileSec = checkFile.GetAccessControl();
                    SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                    fileSec.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, AccessControlType.Allow));
                    try
                    {
                        checkFile.SetAccessControl(fileSec);
                    }
                    catch (Exception err)
                    {
                        // nope, can't set permissions
                        _validationError = "Setting file permissions on the settings file failed. " + err.Message;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// returns a single setting from the config file
        /// </summary>
        /// <param name="Key">the key of the setting to return</param>
        /// <returns>empty if the setting doesn't exist, otherwise the value of the specified setting</returns>
        public string GetSetting(string Key)
        {
            XmlNode settingNode = _settings.DocumentElement.SelectSingleNode("//setting[@name='" + Key + "']/value");
            if (settingNode != null)
                return settingNode.InnerText;
            else
                return "";
        }

        /// <summary>
        /// validates a config setting and populates the lastValidationError property if not valid
        /// </summary>
        /// <param name="Key">key name for the setting</param>
        /// <param name="Value">new value of the setting</param>
        /// <returns>true if the setting is valid, false if not</returns>
        public bool validateSetting(string Key, string Value)
        {
            const int LOW_THRESHOLD_MAX = 24;
            const int HIGH_THRESHOLD_MAX = 48;
            const int HIGH_THRESHOLD_MIN = 4;
            const int ROSTER_MIN = 24;
            const int ROSTER_MAX = 72;
            const int MAX_STRING_LENGTH = 50;
            switch (Key)
            {
                case "lowThreshold":
                    {
                        return validateInt(Key, Value, LOW_THRESHOLD_MAX, 0);
                    }
                case "highThreshold":
                    {
                        return validateInt(Key, Value, HIGH_THRESHOLD_MAX, HIGH_THRESHOLD_MIN);
                    }
                case "defaultRosterLength":
                    {
                        return validateInt(Key, Value, ROSTER_MAX, ROSTER_MIN);
                    }
                case "IdentityLookupFile":
                    {
                        return validateFile(Key, Value, true);
                    }
                case "LogServiceURL":
                    {
                        return validateFile(Key, Value, false);
                    }
                case "DeviceId":
                    {
                        // no validation required, can be anything, but length needs to be checked
                        if(Value.Length <= MAX_STRING_LENGTH)
                            return true;
                        _validationError = "Settings must be less than " + MAX_STRING_LENGTH.ToString() + " characters long";
                        return false;
                    }
                case "IDLookupType":
                    {
                        // can only be one of three values, not case-sensitive
                        string value = Value.ToLower();
                        if (value == "open" || value == "closed" || value == "fixed" || value == "none" ) return true;
                        _validationError = "IDLookupType can only be one of the four values 'Open', 'Closed', 'Fixed' or 'None'";
                        return false;
                    }
                case "FixedID":
                    {
                        // no validation required, can be anything, but length needs to be checked
                        if (Value.Length <= MAX_STRING_LENGTH)
                            return true;
                        _validationError = "Settings must be less than " + MAX_STRING_LENGTH.ToString() + " characters long";
                        return false;
                    }
                default:
                    {
                        // some retard created a new setting and forgot to update the admin utility
                        return true;
                    }
            }
        }

        /// <summary>
        /// performs a basic integer and min/max validation
        /// </summary>
        /// <param name="Key">the name of the setting to validate</param>
        /// <param name="Value">the value to be validated</param>
        /// <param name="max">the maximum allowed value</param>
        /// <param name="min">the minimum allowed value</param>
        /// <returns>true if valid, false if not, and populates the validationError property if invalid</returns>
        private bool validateInt(string Key, string Value, int max, int min)
        {
            // must be numeric
            int result;
            if (!int.TryParse(Value, out result))
            {
                _validationError = Key + " must be a valid number";
                return false;
            }
            else
            {
                if (result > max)
                {
                    _validationError = Key + " cannot be larger than " + max.ToString();
                    return false;
                }
                else
                {
                    if (result < min)
                    {
                        _validationError = Key + " cannot be smaller than " + min.ToString();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }

        /// <summary>
        /// validates a file/path setting
        /// </summary>
        /// <param name="Key">the name of the setting</param>
        /// <param name="Value">the value to be validated</param>
        /// <param name="mustExist">true if the file must already exist, false if not</param>
        /// <returns>true if the filepath is valid, false if not</returns>
        private bool validateFile(string Key, string Value, bool mustExist)
        {
            try
            {
                FileInfo file = new FileInfo(Utilities.parsePath(Value));
                if (mustExist)
                {
                    if (!file.Exists)
                    {
                        _validationError = "the file doesn't exist at that location";
                        return false;
                    }
                }
                return true;
            }
            catch //(Exception e)
            {
                // bad file path, or some other reason
                _validationError = "that location isn't valid";
                return false;
            }
        }
        /// <summary>
        /// provides a description for the setting specified by Key
        /// </summary>
        /// <param name="Key">key name for the setting</param>
        /// <returns>string description of the setting</returns>
        public string settingDescription(string Key)
        {
            // nothing fancy here... every time we make any changes to the settings we'll need to change code, so a simple switch will do here
            switch (Key)
            {
                case "lowThreshold":
                    {
                        return "The fatigue score that marks the point at which Moderate fatigue risk becomes High fatigue risk.";
                    }
                case "highThreshold":
                    {
                        return "The fatigue score that marks the point at which High fatigue risk becomes Extreme fatigue risk.";
                    }
                case "defaultRosterLength":
                    {
                        return "The number of hours that the calculation engine will calculate fatigue for by default.";
                    }
                case "IdentityLookupFile":
                    {
                        return "The location and name of the file that stores the valid identities or so-far-used identities for the system.";
                    }
                case "LogServiceURL":
                    {
                        return "The location and name of the file that stores the results of the calculations.";
                    }
                case "DeviceId":
                    {
                        return "The name of the PC, kiosk or device that the calculator is being run on.";
                    }
                case "IDLookupType":
                    {
                        return "Four possible values: None (anything goes), Fixed (uses the FixedId setting), Open(new names added to the store), Closed(only names in the store)";
                    }
                case "FixedID":
                    {
                        return "If the ID Lookup Type is Fixed, this is the identity used for all calculations.";
                    }
                default:
                    {
                        // some retard created a new setting and forgot to update the admin utility
                        return "Unkown setting. Please contact Support";
                    }
            }
        }

    }
}
