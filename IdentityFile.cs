using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;

namespace Fatigue_Calculator_Desktop
{
    public class identityFile : IIdentityService
    {
        string _idFile = "";
        bool _isValid = false;
        List<identity> _identities = new List<identity>();
        Exception _lastError = null;

        public identityFile()
        {
            // do nothing at this point
        }

        public bool SetIdentityListSource(string sourceURL)
        {

            // check the file
            string checkedFile = Utilities.checkFileName(sourceURL);
            // check if this file is the same as any existing one
            if (checkedFile == _idFile)
            {
                // check if we've loaded it already
                if (_isValid)
                    return true;
            }
            else
            {
                _idFile = checkedFile;
            }
            if (_idFile.Length == 0)
            {
                _isValid = false;
                return false;
            }
            // check it exists
            FileInfo file = new FileInfo(_idFile);
            if (file.Exists)
            {
                // yeah so read it in
                _identities.Clear();
                _isValid = readFile();
            }
            else
            {
                // can still be valid, just not written it yet
                _isValid = true;
                _identities.Clear();
            }
            return _isValid;
        }

        public List<identity> LookUpId(string Id)
        {
            string id = Id.ToUpper();
            if (!_isValid) return
                new List<identity>();
            List<identity> result = new List<identity>();
            foreach (identity matchedId in _identities.Where(ident => ident.Id.IndexOf(id) == 0))
            {
                result.Add(matchedId);
            }
            return result;
        }

        public List<identity> LookUpName(string Name)
        {
            string name = Name.ToUpper();
            if (!_isValid) return
                new List<identity>();
            List<identity> result = new List<identity>();
            foreach (identity matchedId in _identities.Where(ident => ident.Name.IndexOf(name) == 0))
            {
                result.Add(matchedId);
            }
            return result;
        }

        public List<identity> ExactMatchId(string Id)
        {
            string id = Id.ToUpper();
            if (!_isValid) return
                new List<identity>();
            List<identity> result = new List<identity>();
            foreach (identity matchedId in _identities.Where(ident => ident.Id == id))
            {
                result.Add(matchedId);
            }
            return result;
        }

        public List<identity> ExactMatchName(string Name)
        {
            string name = Name.ToUpper();
            if (!_isValid) return
                new List<identity>();
            List<identity> result = new List<identity>();
            foreach (identity matchedId in _identities.Where(ident => ident.Name == name))
            {
                result.Add(matchedId);
            }
            return result;
        }

        public List<identity> IdentityList(bool noduplicates = true)
        {
            if (!_isValid) return
                new List<identity>();
            List<identity> result = new List<identity>();
            foreach (identity matchedId in _identities.Distinct())
            {
                result.Add(matchedId);
            }
            return result;

        }

        /// <summary>
        /// reads the log file into the identity list
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        private bool readFile()
        {
            // assume this has been checked when set, so before getting this far
            FileInfo file = new FileInfo(_idFile);
            StreamReader reader;
            try
            {
                reader = new StreamReader(file.OpenRead());
            }
            catch (Exception err)
            {
                // can't open file for reading
                _lastError = err;
                return false;
            }
            string line = "";
            identity newId;
            _identities.Clear();
            try
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    newId = new identity(line);
                    // only add valid identities to the list
                    if (newId.isValid)
                        _identities.Add(newId);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                // problem somewhere
                reader.Close();
                _lastError = err;
                return false;
            }
            return true;
        }

        public bool AddNewIdentity(identity newId)
        {
            // check the new identity is valid...don't want to add crap to a good file
            if (!newId.isValid)
            {
                _lastError = new Exception("Attempted to add an invalid identity to the identity file");
                return false;
            }
            // check we've got a good file and a valid read
            if (!((_idFile.Length > 0) && _isValid))
            {
                return false;
            }
            // open the file for append access
            FileInfo file = new FileInfo(_idFile);
            StreamWriter writer = StreamWriter.Null;
            try
            {
                writer = file.AppendText();
                writer.WriteLine(newId.ToString());
                writer.Close();
            }
            catch (Exception err)
            {
                writer.Close();
                _lastError = err;
                return false;
            }
            return true;
        }

        /// <summary>
        /// changes all existing identities in the file that match the old identity parameter to the new identity
        /// </summary>
        /// <param name="oldIdent">the identity to change</param>
        /// <param name="newIdent">the identity to replace them with</param>
        /// <returns>true if anything changed</returns>
        public bool ChangeIdentity(identity oldIdent, identity newIdent)
        {
            if (!newIdent.isValid || !oldIdent.isValid)
            {
                _lastError = new Exception("Attempt to change identity using invalid identity parameters");
                return false;
            }
            // check we've got a good file and a valid read
            if (!((_idFile.Length > 0) && _isValid))
            {
                return false;
            }

            // easiest way to do this will be to read the whole file in, edit it in memory, then write it out to a new file, then swap the files

            readFile();

            // better lock the original file while we do this
            FileInfo loglock = new FileInfo(_idFile);
            loglock.IsReadOnly = true;

            //find the matching identities
            foreach (identity possible in _identities)
            {
                if (possible.Equals(oldIdent))
                {
                    // match, so change it
                    possible.Id = newIdent.Id;
                    possible.Name = newIdent.Name;
                    possible.ResearchApproved = newIdent.ResearchApproved;
                }
            }
            // now write the whole list to a new file
            string newFilename = _idFile + ".tmp";
            if (!writeFile(newFilename))
                return false;
            FileInfo newFile = new FileInfo(newFilename);
            // now replace the contents of the old file with the new file
            loglock.IsReadOnly = false;
            newFile.Replace(_idFile, _idFile + ".bck");
            // and delete the new file
            newFile.Delete();
            // set permissions on the file
            setPermissions(_idFile);
            // and reload the list
            return readFile();
        }
        public bool deleteIdentity(identity toDelete)
        {
            if (!toDelete.isValid)
            {
                _lastError = new Exception("Attempt to delete identity using invalid identity parameters");
                return false;
            }
            // check we've got a good file and a valid read
            if (!((_idFile.Length > 0) && _isValid))
            {
                return false;
            }

            // easiest way to do this will be to read the whole file in, edit it in memory, then write it out to a new file, then swap the files
            readFile();

            // better lock the original file while we do this
            FileInfo loglock = new FileInfo(_idFile);
            loglock.IsReadOnly = true;

            //find the matching identities and store them in a separate list so we can delete them once we've finished iterating
            List<identity> forDeletion = new List<identity>();
            foreach (identity possible in _identities)
            {
                if (possible.Equals(toDelete))
                {
                    // match, so add it to the kill list
                    forDeletion.Add(possible);
                }
            }
            // now kill the ones marked for death
            foreach (identity dead in forDeletion)
            {
                _identities.Remove(dead);
            }
            // unlock the file as we're ready to write it
            loglock.IsReadOnly = false;
            // then write the file over the old one
            if (!writeFile(_idFile))
                return false;
            // and reload the list
            return readFile();
        }

        /// <summary>
        /// writes the current identity list to a file
        /// </summary>
        /// <param name="newFilename">the name of the file to write the list to</param>
        /// <returns>true if the file was written successfully</returns>
        private bool writeFile(string newFilename)
        {
            FileInfo newFile = new FileInfo(newFilename);
            // check if it exists. If so, delete it
            if (newFile.Exists)
                newFile.Delete();

            StreamWriter writer = StreamWriter.Null;
            try
            {
                writer = newFile.AppendText();
            }
            catch (Exception err)
            {
                writer.Close();
                _lastError = err;
                return false;
            }
            try
            {
                foreach (identity newId in _identities)
                    writer.WriteLine(newId.ToString());
                writer.Close();
                setPermissions(newFilename);
            }
            catch (Exception err)
            {
                writer.Close();
                newFile.Delete();
                _lastError = err;
                return false;
            }
            return true;
        }
        private bool setPermissions(string filename)
        {
            // try and set the permissions on the new file
            try
            {
                FileSystemAccessRule newRule = new FileSystemAccessRule("Everyone", FileSystemRights.Modify, AccessControlType.Allow);
                FileSecurity fileSec = new FileSecurity(filename, AccessControlSections.Group);
                fileSec.AddAccessRule(newRule);
                File.SetAccessControl(filename, fileSec);
                return true;
            }
            catch
            {
                //something went wrong, probably insufficient rights to change it
                // nae bother, just move on...
                return false;
            }

        }



    }
    }