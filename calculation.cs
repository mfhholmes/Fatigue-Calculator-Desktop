using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Fatigue_Calculator_Desktop
{
    public class calculation
    {
        public calculation()
        {
            //load the presets on startup
            this.LoadPresets();
        }
        private bool _logged;
        public bool logged 
        {
            public get {return _logged;}
            public set { _logged = value;}
        }
        // fatigue levels enum to indicate the 4 fatigue levels
        public enum fatigueLevels
        {
            Low ,
            Moderate,
            High,
            Extreme
        }
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
        public Brush getColourForLevel(calculation.fatigueLevels level)
        {
            //TODO: may have to look this up in the resource directory
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

        public fatigueLevels calcFatigueLevel(int lowThreshold, int highThreshold, int score)
        {
            // simple comparison of levels and score
            if (score == 0) return fatigueLevels.Low;
            else if (score < lowThreshold) return fatigueLevels.Moderate;
            else if (score < highThreshold) return fatigueLevels.High;
            else return fatigueLevels.Extreme;
        }
        // struct to remember the inputs so far
        public struct inputs
        {
            public string identity ;
            public string deviceId;
            public DateTime shiftStart;
            public DateTime shiftEnd;
            public double sleep24;
            public double sleep48;
            public double hoursAwake;
        }
        public inputs currentInputs;
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

        // struct to remember the outputs so far
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
        public bool doCalc()
        {
            //do the calculation and return true if done, false if not
            try
            {
                // grab the device
                this.currentInputs.deviceId = Properties.Settings.Default.DeviceId;
                // check the inputs
                if (!this.areInputsValid()) return false;
                // calculate the roster
                if(this.currentOutputs.rosterLength == 0) this.currentOutputs.rosterLength = this.currentPresets.defaultRosterLength;
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

        // struct to remember presets
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
            this.currentPresets.defaultRosterLength = Properties.Settings.Default.defaultRosterLength;
            this.currentPresets.lowThreshold = Properties.Settings.Default.lowThreshold;
            this.currentPresets.highThreshold = Properties.Settings.Default.highThreshold;
            this.currentPresets.algorithmVersion = 1;
            return true;
        }
        #region "Roster Calculation"
        // roster item to hold the details of a single hour's calculation
        public struct rosterItem
        {
            public int hour;
            public int score;
            public fatigueLevels level;
        }

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
        
        #endregion
        #region "Single Score Calculation"
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
        #endregion
    }
}
