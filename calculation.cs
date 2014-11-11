using System;
using System.Windows.Media;

namespace Fatigue_Calculator_Desktop
{
	public class calculation
	{
		/// <summary>
		/// blank constructor just loads the presets
		/// </summary>
		public calculation()
		{
			//load the presets on startup
			this.LoadPresets();
		}

		private bool _logged;

		/// <summary>
		/// logged indicates whether the calculation has been logged or not
		/// </summary>
		public bool logged
		{
			get { return _logged; }
			set { _logged = value; }
		}

		/// <summary>
		/// 4 levels of fatigue risk
		/// </summary>
		public enum fatigueLevels
		{
			Low,
			Moderate,
			High,
			Extreme
		}

		/// <summary>
		/// simple enum-to-int converter
		/// </summary>
		/// <param name="level">enum fatigue risk level</param>
		/// <returns>int equivalent of the enum fatigue risk level</returns>
		public int levelToNumber(fatigueLevels level)
		{
			switch (level)
			{
				case calculation.fatigueLevels.Low:
					return 1;
				case calculation.fatigueLevels.Moderate:
					return 2;
				case calculation.fatigueLevels.High:
					return 3;
				case calculation.fatigueLevels.Extreme:
					return 4;
			}
			return 0;
		}

		/// <summary>
		/// int to enum converter for fatigue risk levels
		/// </summary>
		/// <param name="level">int representation of fatigue level</param>
		/// <returns>enum for that level of fatigue risk</returns>
		public fatigueLevels levelFromNumber(int level)
		{
			switch (level)
			{
				case 1:
					return fatigueLevels.Low;
				case 2:
					return calculation.fatigueLevels.Moderate;
				case 3:
					return calculation.fatigueLevels.High;
				case 4:
					return calculation.fatigueLevels.Extreme;
			}
			return fatigueLevels.Low;
		}

		/// <summary>
		/// converter of fatigue level to brush colour
		/// </summary>
		/// <param name="level">fatigue risk leve</param>
		/// <returns>Brush for that fatigue level</returns>
		public Brush getColourForLevel(calculation.fatigueLevels level)
		{
			//TODO: may have to look this up in the theme
			switch (level)
			{
				case calculation.fatigueLevels.Low:
					return Brushes.Green;
				case calculation.fatigueLevels.Moderate:
					return Brushes.Orange;
				case calculation.fatigueLevels.High:
					return Brushes.Red;
				case calculation.fatigueLevels.Extreme:
					return Brushes.Black;
			}
			return Brushes.White;
		}

		/// <summary>
		/// logs the current calculation to the appropriate log service
		/// </summary>
		/// <param name="logger">the log service to use (usually file or web service)</param>
		/// <returns>true if log operation was successful, false otherwise</returns>
		public bool logCalc(ILogService logger)
		{
			// check for a valid log service first
			if (!logger.isValid)
				return false;
			// create the new entry
			logEntry newEntry = new logEntry();
			// presets
			newEntry.AlgorithmVersion = this.currentPresets.algorithmVersion.ToString();
			newEntry.lowThreshold = this.currentPresets.lowThreshold;
			newEntry.highThreshold = this.currentPresets.highThreshold;
			// inputs
			newEntry.DeviceId = this.currentInputs.deviceId;
			newEntry.Identity = this.currentInputs.identity.logString;
			newEntry.shiftStart = this.currentInputs.shiftStart;
			newEntry.shiftEnd = this.currentInputs.shiftEnd;
			newEntry.sleep24 = this.currentInputs.sleep24;
			newEntry.sleep48 = this.currentInputs.sleep48;
			newEntry.hoursAwake = this.currentInputs.hoursAwake;
			// outputs
			newEntry.currentScore = this.currentOutputs.currentScore;
			newEntry.CurrentLevel = this.currentOutputs.currentLevel.ToString();
			newEntry.dateDone = this.currentOutputs.calcDone.Date;
			newEntry.timeDone = this.currentOutputs.calcDone.TimeOfDay;
			newEntry.becomesModerate = this.currentOutputs.calcDone + this.currentOutputs.becomesModerate;
			newEntry.becomesHigh = this.currentOutputs.calcDone + this.currentOutputs.becomesHigh;
			newEntry.becomesExtreme = this.currentOutputs.calcDone + this.currentOutputs.becomesExtreme;

			//add the new entry to the logger and bug out
			_logged = logger.AddLogEntry(newEntry);
			return _logged;
		}

		/// <summary>
		/// calculates the fatigue level for a given score and threshold set
		/// </summary>
		/// <param name="lowThreshold">the point at which Moderate fatigue risk becomes High</param>
		/// <param name="highThreshold">the point at which High fatigue risk becomes Extreme</param>
		/// <param name="score">the score to check</param>
		/// <returns>the enum for the appropriate fatigue risk level for this score using these thresholds</returns>
		public fatigueLevels calcFatigueLevel(int lowThreshold, int highThreshold, int score)
		{
			// simple comparison of levels and score
			if (score == 0) return fatigueLevels.Low;
			else if (score < lowThreshold) return fatigueLevels.Moderate;
			else if (score < highThreshold) return fatigueLevels.High;
			else return fatigueLevels.Extreme;
		}

		/// <summary>
		/// structure to hold the inputs for a calculation. Not validated
		/// </summary>
		public struct inputs
		{
			public identity identity;
			public string deviceId;
			public DateTime shiftStart;
			public DateTime shiftEnd;
			public double sleep24;
			public double sleep48;
			public double hoursAwake;
		}

		public inputs currentInputs;

		/// <summary>
		/// validation of current input set
		/// </summary>
		/// <returns>true if valid, false if not</returns>
		public bool areInputsValid()
		{
			// checks the inputs and returns true if they're valid
			// identity can be anything
			// shift must be positive in duration
			if (this.currentInputs.shiftEnd < this.currentInputs.shiftStart) return false;
			// sleep24 must be less than sleep48
			if (this.currentInputs.sleep24 > this.currentInputs.sleep48) return false;
			// sleep48 can't be more than sleep 24 + 24
			if (this.currentInputs.sleep48 > (this.currentInputs.sleep24 + 24)) return false;
			// apparently we're all good
			return true;
		}

		/// <summary>
		/// struct to hold the outputs from the calculation algorithm. not validated
		/// </summary>
		public struct outputs
		{
			public DateTime calcDone;
			public int currentScore;
			public fatigueLevels currentLevel;
			public TimeSpan becomesModerate;
			public TimeSpan becomesHigh;
			public TimeSpan becomesExtreme;
			public TimeSpan shiftStart;
			public TimeSpan shiftEnd;
			public int rosterLength;
			public rosterItem[] roster;
		}

		public outputs currentOutputs;

		/// <summary>
		/// performs a fatigue calculation on the current inputs struct and puts the results in the current outputs struct
		/// </summary>
		/// <returns>true if the calculation was successful, false if a problem occurred</returns>
		public bool doCalc()
		{
			//do the calculation and return true if done, false if not
			try
			{
				// grab the device
				//this.currentInputs.deviceId = Properties.Settings.Default.DeviceId;
				// check the inputs
				if (!this.areInputsValid()) return false;
				// calculate the roster
				if (this.currentOutputs.rosterLength == 0)
					this.currentOutputs.rosterLength = this.currentPresets.defaultRosterLength;
				this.currentOutputs.roster = this.calcRoster(this.currentOutputs.rosterLength, this.currentPresets.lowThreshold, this.currentPresets.highThreshold, this.currentInputs.sleep24, this.currentInputs.sleep48, this.currentInputs.hoursAwake);
				// current score is roster at 0
				this.currentOutputs.currentScore = this.currentOutputs.roster[0].score;
				this.currentOutputs.currentLevel = this.currentOutputs.roster[0].level;
				// now the times
				this.currentOutputs.calcDone = DateTime.Now;
				this.currentOutputs.shiftStart = this.currentInputs.shiftStart - this.currentOutputs.calcDone;
				this.currentOutputs.shiftEnd = this.currentInputs.shiftEnd - this.currentOutputs.calcDone;
				// set the becomes times for the levels already reached, and work out which levels we've go to find
				this.currentOutputs.becomesModerate = new TimeSpan(0, 0, 0);
				this.currentOutputs.becomesHigh = new TimeSpan(0, 0, 0);
				this.currentOutputs.becomesExtreme = new TimeSpan(0, 0, 0);
				fatigueLevels nextLevel = fatigueLevels.Moderate;
				if (this.currentOutputs.currentLevel == fatigueLevels.Moderate) nextLevel = fatigueLevels.High;
				if (this.currentOutputs.currentLevel == fatigueLevels.High) nextLevel = fatigueLevels.Extreme;
				for (int i = 1; i < this.currentOutputs.rosterLength; i++)
				{
					if (this.currentOutputs.roster[i].level != this.currentOutputs.roster[i - 1].level)
					{
						switch (nextLevel)
						{
							case fatigueLevels.Moderate:
								this.currentOutputs.becomesModerate = new TimeSpan(i, 0, 0);
								nextLevel = fatigueLevels.High;
								break;

							case fatigueLevels.High:
								this.currentOutputs.becomesHigh = new TimeSpan(i, 0, 0);
								nextLevel = fatigueLevels.Extreme;
								break;

							case fatigueLevels.Extreme:
								this.currentOutputs.becomesExtreme = new TimeSpan(i, 0, 0);
								// enough, we can stop now
								i = this.currentOutputs.rosterLength;
								break;
						}
					}
				}

				return true;
			}
			catch
			{
				// bad thing happened
				return false;
			}
		}

		/// <summary>
		/// struct to hold the presets. Presets usually loaded from config file but may be specified as a constant
		/// </summary>
		public struct presets
		{
			public int lowThreshold;
			public int highThreshold;
			public int defaultRosterLength;
			public int algorithmVersion;
		}

		public presets currentPresets;

		public bool LoadPresets()
		{
			this.currentPresets.defaultRosterLength = 72;
			this.currentPresets.lowThreshold = Config.ConfigSettings.settings.lowThreshold;
			this.currentPresets.highThreshold = Config.ConfigSettings.settings.highThreshold;
			this.currentPresets.algorithmVersion = 1;
			return true;
		}

		#region "Roster Calculation"

		/// <summary>
		/// struct to hold a single roster item (a single hour's fatigue score and level)
		/// </summary>
		public struct rosterItem
		{
			public int hour;
			public int score;
			public fatigueLevels level;
		}

		/// <summary>
		/// calculates the roster of fatigue risk scores
		/// </summary>
		/// <param name="numItems">number of hours to calculate for</param>
		/// <param name="lowThreshold">the Moderate-to-High risk transition score to use</param>
		/// <param name="highThreshold">the High-To-Extreme risk transition score to use</param>
		/// <param name="sleep24">number of hours sleep had in the last 24 hours</param>
		/// <param name="sleep48">number of hours sleep had in the last 48 hours</param>
		/// <param name="hoursAwake">number of hours awake since the last sleep period</param>
		/// <returns>array of rosterItems representing the fatigue risk roster</returns>
		public rosterItem[] calcRoster(int numItems, int lowThreshold, int highThreshold, double sleep24, double sleep48, double hoursAwake)
		{
			// calculate the scores for a whole roster, and give fatigue levels as determined by the fatigue levels
			rosterItem[] result = new rosterItem[numItems];
			//iterate the roster
			for (int i = 0; i < numItems; i++)
			{
				// hour is just the offset (we could just use the index, but this is better as it allows subsets of the full roster to still make sense)
				result[i].hour = i;
				// calculate the score for this hour, adding the hour to the hours awake. The sleep24 and sleep48 don't change as they're specified from the start of the shift.
				result[i].score = calcScore(sleep24, sleep48, hoursAwake + i);
				// check the fatigue level for this score
				result[i].level = calcFatigueLevel(lowThreshold, highThreshold, result[i].score);
			}
			return result;
		}

		#endregion "Roster Calculation"

		#region "Single Score Calculation"

		/// <summary>
		/// calculation of a single score
		/// </summary>
		/// <param name="sleep24">the hours of sleep had in the last 24 hours</param>
		/// <param name="sleep48">the hours of sleep had in the last 48 hours</param>
		/// <param name="hoursAwake">the hours since the last sleep period</param>
		/// <returns>fatigue score represented as a single integer</returns>
		public int calcScore(double sleep24, double sleep48, double hoursAwake)
		{
			//Algorithm property of the Centre for Sleep Research and licensed to MB Solutions
			//unauthorised use of this algorithm is a breach of copyright
			int result = 0;
			result = (int)(ScoreX(sleep24) + ScoreY(sleep48) + ScoreZ(hoursAwake, sleep48));
			return result;
		}

		private int ScoreX(double sleep24)
		{
			//Algorithm property of the Centre for Sleep Research and licensed to MB Solutions
			//unauthorised use of this algorithm is a breach of copyright
			if (sleep24 > 5) return 0;
			return (int)(20.0 - (sleep24 * 4.0));
		}

		private int ScoreY(double sleep48)
		{
			//Algorithm property of the Centre for Sleep Research and licensed to MB Solutions
			//unauthorised use of this algorithm is a breach of copyright
			//if (sleep48 < 6) return 12;
			if (sleep48 > 12) return 0;
			return (int)(24.0 - (sleep48 * 2.0));
		}

		private int ScoreZ(double hoursAwake, double sleep48)
		{
			//Algorithm property of the Centre for Sleep Research and licensed to MB Solutions
			//unauthorised use of this algorithm is a breach of copyright
			int result = (int)(hoursAwake - sleep48);
			if (result < 0) result = 0;
			return result;
		}

		#endregion "Single Score Calculation"
	}
}