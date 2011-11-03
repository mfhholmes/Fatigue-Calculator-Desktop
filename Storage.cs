using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fatigue_Calculator_Desktop
{
    class Storage
    {
        public string getPath(string path)
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
