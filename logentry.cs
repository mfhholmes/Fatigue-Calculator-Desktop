using System;

namespace Fatigue_Calculator_Desktop
{
	public class logEntry
	{
		private string default_date_format = "dd/MMM/yyyy";
		private string default_time_format = "HH:mm";
		private string default_timespan_format = @"hh\:mm";

		public string headers
		{
			get { return "Date,Time,Device,User,Shift Start,Shift End,Sleep 24,Sleep 48,Hours Awake,Low Threshold,High Threshold,Fatigue Risk Score,Fatigue Risk Level,Algorithm Version,Risk Becomes Moderate,Risk Becomes High,Risk Becomes Extreme"; }
		}

		private DateTime? _dateDone = null;

		public string DateDone
		{
			get
			{
				if (_dateDone != null)
					return ((DateTime)_dateDone).ToString(default_date_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_dateDone = null;
				if (DateTime.TryParse(value, out result))
					_dateDone = result;
				else
					dateDone = null;
				validate();
			}
		}

		public DateTime? dateDone
		{
			get
			{
				return _dateDone;
			}
			set
			{
				_dateDone = value;
				validate();
			}
		}

		private TimeSpan? _timeDone = null;

		public string TimeDone
		{
			get
			{
				if (_timeDone != null)
					return ((TimeSpan)_timeDone).ToString(default_timespan_format);
				else
					return "";
			}
			set
			{
				TimeSpan result;
				_timeDone = null;
				if (TimeSpan.TryParse(value, out result))
					_timeDone = result;
				else
					_timeDone = null;
				validate();
			}
		}

		public TimeSpan? timeDone
		{
			get
			{
				return _timeDone;
			}
			set
			{
				_timeDone = value;
			}
		}

		public DateTime dateTimeDone
		{
			get
			{
				if (_isValid && (_dateDone != null) && (_timeDone != null))
				{
					return (DateTime)(_dateDone + _timeDone);
				}
				else
				{
					// guess we do the best we can with what we've got
					// no point returning the time by itself
					if ((_dateDone != null) && (_timeDone != null))
						return (DateTime)(_dateDone + _timeDone);
					else
						// don't have a choice. Have to return a valid DateTime value, don't have one to return, have to return Now
						return DateTime.Now;
				}
			}
		}

		private string _deviceId = "";

		public string DeviceId
		{
			get { return _deviceId; }
			set { _deviceId = value; validate(); }
		}

		private string _identity = "";

		public string Identity
		{
			get { return _identity; }
			set { _identity = value; validate(); }
		}

		private DateTime? _shiftStart;

		public string ShiftStart
		{
			get
			{
				if (_shiftStart != null)
					return ((DateTime)_shiftStart).ToString(default_date_format + " " + default_time_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_shiftStart = null;
				if (DateTime.TryParse(value, out result))
					_shiftStart = result;
				else
					_shiftStart = null;
				validate();
			}
		}

		public DateTime? shiftStart
		{
			get
			{
				return _shiftStart;
			}
			set
			{
				_shiftStart = value;
				validate();
			}
		}

		private DateTime? _shiftEnd;

		public string ShiftEnd
		{
			get
			{
				if (_shiftEnd != null)
					return ((DateTime)_shiftEnd).ToString(default_date_format + " " + default_time_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_shiftEnd = null;
				if (DateTime.TryParse(value, out result))
					_shiftEnd = result;
				else
					_shiftEnd = null;
				validate();
			}
		}

		public DateTime? shiftEnd
		{
			get
			{
				return _shiftEnd;
			}
			set
			{
				_shiftEnd = value;
				validate();
			}
		}

		private double _sleep24 = 0; // not sure if we shouldn't default to -1 to be sure

		public string Sleep24
		{
			get
			{
				return _sleep24.ToString();
			}
			set
			{
				double result;
				_sleep24 = 0;
				if (double.TryParse(value, out result))
					_sleep24 = result;
				else
					_sleep24 = 0;
				validate();
			}
		}

		public double sleep24
		{
			get { return _sleep24; }
			set { _sleep24 = value; validate(); }
		}

		private double _sleep48 = 0;

		public string Sleep48
		{
			get
			{
				return _sleep48.ToString();
			}
			set
			{
				double result;
				_sleep48 = 0;
				if (double.TryParse(value, out result))
					_sleep48 = result;
				else
					_sleep48 = 0;
				validate();
			}
		}

		public double sleep48
		{
			get { return _sleep48; }
			set { _sleep48 = value; validate(); }
		}

		private double _hoursAwake = 0;

		public string HoursAwake
		{
			get
			{
				return _hoursAwake.ToString();
			}
			set
			{
				double result;
				_hoursAwake = 0;
				if (double.TryParse(value, out result))
					_hoursAwake = result;
				else
					_hoursAwake = 0;
				validate();
			}
		}

		public double hoursAwake
		{
			get { return _hoursAwake; }
			set { _hoursAwake = value; validate(); }
		}

		private int _lowThreshold = 0;

		public string LowThreshold
		{
			get
			{
				return _lowThreshold.ToString();
			}
			set
			{
				int result;
				_lowThreshold = 0;
				if (int.TryParse(value, out result))
					_lowThreshold = result;
				else
					_lowThreshold = 0;
				validate();
			}
		}

		public int lowThreshold
		{
			get { return _lowThreshold; }
			set { _lowThreshold = value; validate(); }
		}

		private int _highThreshold = 0;

		public string HighThreshold
		{
			get
			{
				return _highThreshold.ToString();
			}
			set
			{
				int result;
				_highThreshold = 0;
				if (int.TryParse(value, out result))
					_highThreshold = result;
				else
					_highThreshold = 0;
				validate();
			}
		}

		public int highThreshold
		{
			get { return _highThreshold; }
			set { _highThreshold = value; }
		}

		private int _currentScore = 0;

		public string CurrentScore
		{
			get
			{
				return _currentScore.ToString();
			}
			set
			{
				int result;
				_currentScore = 0;
				if (int.TryParse(value, out result))
					_currentScore = result;
				else
					_currentScore = 0;
				validate();
			}
		}

		public int currentScore
		{
			get { return _currentScore; }
			set { _currentScore = value; validate(); }
		}

		private string _currentLevel = "";

		public string CurrentLevel
		{
			get { return _currentLevel; }
			set { _currentLevel = value; validate(); }
		}

		private string _algorithmVersion = "";

		public string AlgorithmVersion
		{
			get { return _algorithmVersion; }
			set { _algorithmVersion = value; validate(); }
		}

		private DateTime? _becomesModerate = null;

		public string BecomesModerate
		{
			get
			{
				if (_becomesModerate != null)
					return ((DateTime)_becomesModerate).ToString(default_date_format + " " + default_time_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_becomesModerate = null;
				if (DateTime.TryParse(value, out result))
					_becomesModerate = result;
				else
					_becomesModerate = null;
				validate();
			}
		}

		public DateTime? becomesModerate
		{
			get { return _becomesModerate; }
			set { _becomesModerate = value; validate(); }
		}

		private DateTime? _becomesHigh = null;

		public string BecomesHigh
		{
			get
			{
				if (_becomesHigh != null)
					return ((DateTime)_becomesHigh).ToString(default_date_format + " " + default_time_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_becomesHigh = null;
				if (DateTime.TryParse(value, out result))
					_becomesHigh = result;
				else
					_becomesHigh = null;
				validate();
			}
		}

		public DateTime? becomesHigh
		{
			get { return _becomesHigh; }
			set { _becomesHigh = value; validate(); }
		}

		private DateTime? _becomesExtreme = null;

		public string BecomesExtreme
		{
			get
			{
				if (_becomesExtreme != null)
					return ((DateTime)_becomesExtreme).ToString(default_date_format + " " + default_time_format);
				else
					return "";
			}
			set
			{
				DateTime result;
				_becomesExtreme = null;
				if (DateTime.TryParse(value, out result))
					_becomesExtreme = result;
				else
					_becomesExtreme = null;
				validate();
			}
		}

		public DateTime? becomesExtreme
		{
			get { return _becomesExtreme; }
			set { _becomesExtreme = value; validate(); }
		}

		private bool _isValid = false;

		public bool isValid
		{
			get { return _isValid; }
		}

		/// <summary>
		/// checks that we have valid entries for all the entries required, and sets the isValid flag appropriately
		/// </summary>
		/// <returns>true if valid, false if not</returns>
		public bool validate()
		{
			// real simple list of checks for null or missing entries.
			// not validating the actual entries here, that should be done at source
			_isValid = false;
			if (_algorithmVersion.Length == 0) return false;
			if (_becomesExtreme == null) return false;
			if (_becomesHigh == null) return false;
			if (_becomesModerate == null) return false;
			if (_currentLevel.Length == 0) return false;
			if (_currentScore < 0) return false;
			if (_dateDone == null) return false;
			if (_deviceId.Length == 0) return false;
			if (_highThreshold < 1) return false;
			if (_hoursAwake < 0) return false;
			if (_identity.Length == 0) return false;
			if (_lowThreshold < 1) return false;
			if (_shiftEnd == null) return false;
			if (_shiftStart == null) return false;
			if (_sleep24 < 0) return false;
			if (_sleep48 < 0) return false;
			if (_timeDone == null) return false;
			_isValid = true;
			return true;
		}

		/// <summary>
		/// blank constructor to allow the calculator to construct its output without a log line
		/// </summary>
		public logEntry()
		{
			_isValid = false;
		}

		/// <summary>
		/// parses a log entry from a log file into its component values
		/// </summary>
		/// <param name="logLine">string containing the log entry from the log file</param>
		public logEntry(string logLine)
		{
			const int VERSION_1_ENTRIES = 13;
			const int VERSION_2_ENTRIES = 14;
			const int VERSION_3_ENTRIES = 17;
			string[] entries = logLine.Split(',');

			switch (entries.Length)
			{
				case VERSION_1_ENTRIES:
					{
						_isValid = parseVersion1(entries);
						break;
					}
				case VERSION_2_ENTRIES:
					{
						_isValid = parseVersion2(entries);
						break;
					}
				case VERSION_3_ENTRIES:
					{
						_isValid = parseVersion3(entries);
						break;
					}
				default:
					{
						// not a valid log length
						_isValid = false;
						break;
					}
			}
		}

		/// <summary>
		/// parses version 1 of the log entry
		/// </summary>
		/// <param name="entries"></param>
		/// <returns></returns>
		private bool parseVersion1(string[] entries)
		{
			try
			{
				// there's an issue with earlier versions missing the identity line
				DateDone = entries[0];
				TimeDone = entries[1];
				DeviceId = entries[2];
				Identity = "Unknown";
				ShiftStart = entries[3];
				ShiftEnd = entries[4];
				Sleep24 = entries[5];
				Sleep48 = entries[6];
				HoursAwake = entries[7];
				LowThreshold = entries[8];
				HighThreshold = entries[9];
				CurrentScore = entries[10];
				CurrentLevel = entries[11];
				AlgorithmVersion = entries[12];
				// missing the times that the risk levels switch, so calculate the values
				return calculateRiskLevelChanges() && _isValid;
			}
			catch
			{
				//not valid
				return false;
			}
		}

		private bool parseVersion2(string[] entries)
		{
			try
			{
				DateDone = entries[0];
				TimeDone = entries[1];
				DeviceId = entries[2];
				Identity = entries[3];
				if (Identity.Length == 0) Identity = "Unknown";
				ShiftStart = entries[4];
				ShiftEnd = entries[5];
				Sleep24 = entries[6];
				Sleep48 = entries[7];
				HoursAwake = entries[8];
				LowThreshold = entries[9];
				HighThreshold = entries[10];
				CurrentScore = entries[11];
				CurrentLevel = entries[12];
				AlgorithmVersion = entries[13];
				// missing the times that the risk levels switch, so calculate the values
				return calculateRiskLevelChanges() && _isValid;
			}
			catch
			{
				return false;
			}
		}

		private bool parseVersion3(string[] entries)
		{
			try
			{
				// there's an issue with earlier versions missing the identity line
				DateDone = entries[0];
				TimeDone = entries[1];
				DeviceId = entries[2];
				Identity = entries[3];
				ShiftStart = entries[4];
				ShiftEnd = entries[5];
				Sleep24 = entries[6];
				Sleep48 = entries[7];
				HoursAwake = entries[8];
				LowThreshold = entries[9];
				HighThreshold = entries[10];
				CurrentScore = entries[11];
				CurrentLevel = entries[12];
				AlgorithmVersion = entries[13];
				BecomesModerate = entries[14];
				BecomesHigh = entries[15];
				BecomesExtreme = entries[16];
				return _isValid;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// creates a calculation object and recreates the original calcualtions to work out when the risk levels change
		/// </summary>
		private bool calculateRiskLevelChanges()
		{
			try
			{
				calculation calc = new calculation();
				// load in the values
				calc.currentOutputs.calcDone = (DateTime)dateTimeDone;
				calc.currentInputs.shiftStart = (DateTime)_shiftStart;
				calc.currentInputs.shiftEnd = (DateTime)_shiftEnd;
				calc.currentInputs.sleep24 = _sleep24;
				calc.currentInputs.sleep48 = _sleep48;
				calc.currentInputs.hoursAwake = _hoursAwake;
				calc.currentPresets.lowThreshold = _lowThreshold;
				calc.currentPresets.highThreshold = _highThreshold;
				calc.currentPresets.defaultRosterLength = 24;
				// do the calc
				if (calc.doCalc())
				{
					// fetch the levels
					becomesModerate = (dateTimeDone + calc.currentOutputs.becomesModerate);
					becomesHigh = (dateTimeDone + calc.currentOutputs.becomesHigh);
					becomesExtreme = (dateTimeDone + calc.currentOutputs.becomesExtreme);
				}
				else
				{
					return false;
				}
				return true;
			}
			catch //(Exception e)
			{
				// a problem with one of the inputs probably
				// just mark this as invalid and move on
				return false;
			}
		}

		/// <summary>
		/// build the output line from the values in the object
		/// </summary>
		/// <returns></returns>
		public string makeOutputLine()
		{
			if (!validate())
				return "";
			string output = "";
			output += DateDone + ",";
			output += TimeDone + ",";
			output += DeviceId + ",";
			output += Identity + ",";
			output += ShiftStart + ",";
			output += ShiftEnd + ",";
			output += Sleep24 + ",";
			output += Sleep48 + ",";
			output += HoursAwake + ",";
			output += LowThreshold + ",";
			output += HighThreshold + ",";
			output += CurrentScore + ",";
			output += CurrentLevel + ",";
			output += AlgorithmVersion + ",";
			output += BecomesModerate + ",";
			output += BecomesHigh + ",";
			output += BecomesExtreme;
			return output;
		}
	}
}