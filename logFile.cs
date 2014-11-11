using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace Fatigue_Calculator_Desktop
{
	public class logFile : ILogService
	{
		private string _logFileName = "";
		private bool _isValid = false;
		private Exception _lastError;
		private List<logEntry> _logEntries = new List<logEntry>();

		#region "interface implementation"

		bool ILogService.isValid
		{
			get { return _isValid; }
		}

		bool ILogService.setLogURL(string logURL)
		{
			_logFileName = Utilities.checkFileName(logURL);
			if (_logFileName.Length > 0)
			{
				int entriesRead = readFile();
				if (entriesRead > 0)
				{
					_isValid = true;
					return true;
				}
				else
				{
					// interesting...this could be a specifier for a new log file so valid with no entries
					// or it could be an attempt to open an existing invalid log file
					// guess we could check if it exists
					_isValid = true;
					return false;
				}
			}
			else
			{
				_lastError = new Exception("invalid log file specified");
				_isValid = false;
				return false;
			}
		}

		List<logEntry> ILogService.logEntries
		{
			get
			{
				if (_isValid)
					return _logEntries;
				else
					return new List<logEntry>();
			}
		}

		bool ILogService.AddLogEntry(logEntry newEntry)
		{
			return addEntry(newEntry);
		}

		Exception ILogService.lastError
		{
			get
			{
				if (_lastError != null)
					return _lastError;
				else
					return new Exception("no error reported");
			}
		}

		DateTime? ILogService.lastLogEntryForUser(identity user)
		{
			// check for valid log
			if (!_isValid)
				return null;
			// get all matching entries
			if (user.Name.Length > 0)
			{
				DateTime? last = null;
				foreach (logEntry match in _logEntries.Where<logEntry>(entry => entry.Identity == user.Name))
				{
					if ((last == null) || (match.dateTimeDone > last))
					{
						last = match.dateTimeDone;
					}
				}
				return last;
			}
			return null;
		}

		bool ILogService.isIdentityOnLog(identity user)
		{
			// check for valid log
			if (!_isValid)
				return false;
			// check for name
			int matches = 0;
			if (user.Name.Length > 0)
			{
				matches = _logEntries.Count<logEntry>(entry => entry.Identity == user.Name);
				if (matches > 0)
					return true;
			}
			// no matches, so check for id
			if (user.Id.Length > 0)
			{
				matches = _logEntries.Count<logEntry>(entry => entry.Identity == user.Id);
				if (matches > 0)
					return true;
			}
			// no matches, so check for identity string
			string ident = user.ToString();
			if (ident.Length > 0)
			{
				matches = _logEntries.Count<logEntry>(entry => entry.Identity == ident);
				if (matches > 0)
					return true;
			}

			// no matches, so no matches
			return false;
		}

		#endregion "interface implementation"

		/// <summary>
		/// blank constructor that does nothing except set isValid to false
		/// </summary>
		public logFile()
		{
			// nothing to do here
		}

		/// <summary>
		/// opens a log file. Private member as new logfiles should create new objects.
		/// </summary>
		/// <param name="filename">the filename of the log file to open</param>
		/// <returns>-1 if logfile is invalid, otherwise the count of valid log entries in the file</returns>
		private int readFile()
		{
			//quick check on the filename
			string checkedFile = Utilities.checkFileName(_logFileName);
			FileInfo tempcopy = null;
			if (checkedFile.Length == 0)
			{
				_lastError = new Exception("checkFileName returned zero length");
				return -1;
			}

			// check the file actually exists
			FileInfo file = new FileInfo(checkedFile);
			if (!file.Exists)
			{
				_lastError = new Exception("file.Exists returned false");
				return -1;
			}

			// try and open the file
			System.IO.FileStream fstream;
			try
			{
				fstream = file.OpenRead();
			}
			catch (Exception err)
			{
				// failed to open the file

				// wonder if we can create a copy of the file and read that instead?
				tempcopy = file.CopyTo(file.Directory.FullName + "tempcopy.csv");
				if (tempcopy.Exists)
				{
					try
					{
						fstream = tempcopy.OpenRead();
					}
					catch (Exception err2)
					{
						// nope, couldn't open the copy
						_lastError = new Exception("Failed to create copy of the log file: " + err2.Message);
						return -1;
					}
				}
				else
				{
					// nope, couldn't copy the file
					_lastError = err;
					return -1;
				}
			}

			// clear down the list and repopulate
			_logEntries.Clear();
			logEntry entry;
			// iterate through the list
			//TODO: investigate if there's a better way than this...reading the entire log file into memory seems like it's not going to scale at all...
			System.IO.StreamReader reader = new System.IO.StreamReader(fstream);
			while (!reader.EndOfStream)
			{
				// parse the log line and add the result to the collection if valid
				entry = new logEntry(reader.ReadLine());
				if (entry.isValid)
					_logEntries.Add(entry);
			}

			// kill the temp copy if we had to set it up
			if (tempcopy != null)
				tempcopy.Delete();

			reader.Close();
			fstream.Close();
			return _logEntries.Count;
		}

		/// <summary>
		/// adds a new entry to the current logfile
		/// </summary>
		/// <param name="newEntry"></param>
		/// <returns></returns>
		private bool addEntry(logEntry newEntry)
		{
			bool createdFile = false;
			// before we get into file handling and stuff, let's just check we've got a valid entry
			if (!newEntry.isValid)
				return false;

			// ok, now it's worth opening the file
			FileInfo log = new FileInfo(_logFileName);
			StreamWriter writer;
			//check the file was valid when we were created
			if (!_isValid)
			{
				return false;
			}
			else
			{
				try
				{
					// check it exists
					if (log.Exists)
					{
						// it's valid and exists so let's open it
						writer = new StreamWriter(log.Open(FileMode.Append, FileAccess.Write, FileShare.Read));
					}
					else
					{
						try
						{
							// it's valid but doesn't exist yet so let's create it and write the headers
							writer = log.CreateText();
							writer.WriteLine(newEntry.headers);
							createdFile = true;
						}
						catch (Exception err)
						{
							// can't create it (or write the headers)
							// guess it's not valid after all
							_lastError = err;
							_isValid = false;
							return false;
						}
					}
				}
				catch
				{
					// problem opening the file, try this instead:
					writer = log.AppendText();
				}
			}
			// we have an open file let's write to it
			string output = newEntry.makeOutputLine();
			writer.WriteLine(output);
			writer.Close();
			// if we created the file, try and set the permissions on it
			if (createdFile)
			{
				try
				{
					FileSystemAccessRule newRule = new FileSystemAccessRule("Everyone", FileSystemRights.Modify, AccessControlType.Allow);
					FileSecurity fileSec = new FileSecurity(log.FullName, AccessControlSections.Group);
					fileSec.AddAccessRule(newRule);
					File.SetAccessControl(log.FullName, fileSec);
				}
				catch
				{
					//something went wrong, probably insufficient rights to change it
					// nae bother, just move on...
				}
			}
			return true;
		}

		public LogType thisLogType
		{
			get { return LogType.local; }
		}
	}
}