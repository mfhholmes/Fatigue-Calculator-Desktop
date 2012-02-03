using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fatigue_Calculator_Desktop
{
    static public class Utilities
    {
        /// <summary>
        /// Removes references to the special %-% paths and replaces them with the appropriate path
        /// </summary>
        /// <param name="pathToParse">the path that needs to be parsed</param>
        /// <returns>the correct path, checked for validity</returns>
        public static string parsePath(string pathToParse)
        {
            string result;
            if (pathToParse.IndexOf("%") > -1)
            {
                // special path is the bit between the %'s
                string special;
                special = pathToParse.Substring(pathToParse.IndexOf("%") + 1);
                // quick check just in case it's a mess and there's no second %
                if (special.IndexOf("%") < 0)
                {
                    // malformed special, return blank
                    return "";
                }
                special = special.Substring(0, special.IndexOf("%"));
                special = lookupSpecial(special);
                if (special.Length == 0)
                {
                    // if the special was detected but invalid, then the path can't be valid
                    return "";
                }
                if(Uri.IsWellFormedUriString(special,UriKind.RelativeOrAbsolute)) 
                    special = new Uri(special).LocalPath;
                result = pathToParse.Substring(0, pathToParse.IndexOf("%")) + special + @"\";
                result += pathToParse.Substring(pathToParse.LastIndexOf("%") + 1);
                //remove any duplicate slashes
                while (result.IndexOf(@"\\") > 0)
                {
                    result = result.Replace(@"\\", @"\");
                }
            }
            else
            {
                result = pathToParse;
            }

            // ok, so now we need to just pull the directory path so we can check if it exists
            // check for '.' to see if we've got a file or a directory
            if (result.IndexOf(@".") > -1)
                result = System.IO.Path.GetDirectoryName(result);

            // validate the path before returning, as all the paths we deal with should already exist
            try
            {
                System.IO.DirectoryInfo finalPath = new System.IO.DirectoryInfo(result);
                if (finalPath.Exists )
                {
                    return finalPath.FullName;
                }
                else
                    return "";
            }
            catch
            {
                //invalid path
                return "";
            }
        }

        public static string lookupSpecial(string special)
        {
            string result;
            switch (special.ToLower())
            {
                case "app.path":
                case "apppath":
                case "app_path":
                    {
                        result = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                        if (result.Substring(0, 6) == "file:\\") result = result.Substring(6);
                        break;
                    }
                case "app_data":
                case "app.data":
                case "appdata":
                    {
                        result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.DoNotVerify);
                        break;
                    }
                case "my documents":
                case "my_documents":
                    {
                        result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.DoNotVerify);
                        break;
                    }
                case "documents":
                    {
                        result = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments, Environment.SpecialFolderOption.DoNotVerify);
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

        /// <summary>
        /// checks whether a path and file name is valid, allowing for special paths and other weirdnesses
        /// </summary>
        /// <param name="filename">filename to check</param>
        /// <returns>empty string if not valid, proper complete path if valid</returns>
        public static string checkFileName(string filename)
        {
            string checkedPath = Utilities.parsePath(filename);
            if (checkedPath.Length == 0)
            {
                // nope, the path is bogus
                return "";
            }
            // get the filename from the input and add it to the path
            string justfile = "";
            if (filename.IndexOf(@"\") == -1)
            {
                //it's possible, if the path contains a special path
                if (filename.IndexOf(@"%") == -1)
                {
                    // no special path and no directory separator. I call bullshit
                    return "";
                }
                else
                {
                    // the filename must be the bit after the special then
                    justfile = filename.Substring(filename.LastIndexOf(@"%") + 1);
                }
            }
            else
            {
                justfile = filename.Substring(filename.LastIndexOf(@"\") + 1);
            }
            // check for invalid file characters in the filename
            foreach (char checkchar in System.IO.Path.GetInvalidFileNameChars())
            {
                // crap filename, so just bomb out here
                if(justfile.IndexOf(checkchar)>-1) 
                    return "";
            }

            // compile new full filename, allowing for the various options of how they specified the dir separators
            string fullfilename = "";
            if (checkedPath.EndsWith(@"\"))
            {
                if (justfile.StartsWith(@"\"))
                {
                    fullfilename = checkedPath + justfile.Substring(1);
                }
                else
                {
                    fullfilename = checkedPath + justfile;
                }
            }
            else
            {
                if (justfile.StartsWith(@"\"))
                {
                    fullfilename = checkedPath + justfile;
                }
                else
                {
                    fullfilename = checkedPath + @"\" + justfile;
                }
            }

            return fullfilename;
        }
    }
}
