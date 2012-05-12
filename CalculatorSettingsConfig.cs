using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Fatigue_Calculator_Desktop.Config;

namespace Fatigue_Calculator_Desktop
{
    class CalculatorSettingsConfig : ICalculatorSettings
    {
        string _validationError = "";

        bool ICalculatorSettings.ChangeSetting(string Key, string Value)
        {
            // check it's valid
            bool result = validateSetting(Key, Value);
            // nope, so exit here
            if (!result)
                return false;
            // then cast it back to the correct type depending on the type of the setting
            bool success = false;
            if (Properties.Settings.Default[Key].GetType() == typeof(string))
            {
                Properties.Settings.Default[Key] = Value;
                success = true;
            }
            
            if (Properties.Settings.Default[Key].GetType() == typeof(double))
            {
                double res = 0;
                if (double.TryParse(Value, out res))
                {
                    Properties.Settings.Default[Key] = res;
                    success = true;
                }
            }
            if (Properties.Settings.Default[Key].GetType() == typeof(int))
            {
                int res = 0;
                if (int.TryParse(Value, out res))
                {
                    Properties.Settings.Default[Key] = res;
                    success = true;
                }
            }
            if (Properties.Settings.Default[Key].GetType() == typeof(DateTime))
            {
                DateTime res;
                if (DateTime.TryParse(Value, out res))
                {
                    Properties.Settings.Default[Key] = res;
                    success = true;
                }
            }

            //save the settings
            if(success)
                Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            return success;

        }

        string ICalculatorSettings.GetSetting(string Key)
        {
            if(Properties.Settings.Default[Key] != null)
                return Properties.Settings.Default[Key].ToString();
            else
            {
                _validationError = "Setting " + Key + " doesn't exist in the config file";
                return "";
            }
        }

        Dictionary<string, string> ICalculatorSettings.GetSettings()
        {
            Dictionary <string,string> result = new Dictionary<string,string>();
            // check if the values have been loaded yet
            
            foreach(SettingsPropertyValue setting in Properties.Settings.Default.PropertyValues)
            {
                result.Add(setting.Name, setting.PropertyValue.ToString());
            }
            return result;
        }

        string ICalculatorSettings.lastValidationError
        {
            get { return _validationError; }
        }

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
                        if (value == "open" || value == "closed" || value == "fixed" || value == "none") return true;
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
                        return "The fatigue score that marks the point at which Moderate fatigue risk becomes High fatigue risk.. The default value is recommended by the Centre for Sleep Research. Do not change this value without conducting a Fatigue Risk Assessment first.";
                    }
                case "highThreshold":
                    {
                        return "The fatigue score that marks the point at which High fatigue risk becomes Extreme fatigue risk.. The default value is recommended by the Centre for Sleep Research. Do not change this value without conducting a Fatigue Risk Assessment first.";
                    }
                case "defaultRosterLength":
                    {
                        return "The number of hours that the calculation engine will calculate fatigue for by default. Do not change this value unless instructed to by a support technician.";
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
