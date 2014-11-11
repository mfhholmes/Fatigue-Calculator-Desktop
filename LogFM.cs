using System;
using System.Collections.Generic;

namespace Fatigue_Calculator_Desktop
{
	internal class LogFM : ILogService
	{
		private string _url = "";
		private Exception _lastError = null;

		bool ILogService.setLogURL(string logURL)
		{
			_url = logURL;
			return true;
		}

		List<logEntry> ILogService.logEntries
		{
			get
			{
				// need to think this one through more...maybe pass in a selection criteria?
				// also, security... how do we check that the user has the authority to see other people's results?
				return new List<logEntry>();
			}
		}

		bool ILogService.AddLogEntry(logEntry newEntry)
		{
			try
			{
				//FatigueManager.FatigueCalculatorDataSoapClient FM = new FatigueManager.FatigueCalculatorDataSoapClient();
				//FatigueManager.LogCalcRequestBody body = new FatigueManager.LogCalcRequestBody(newEntry.dateTimeDone,
				//                newEntry.DeviceId,
				//                newEntry.Identity,
				//                newEntry.shiftStart ?? default(DateTime),
				//                newEntry.shiftEnd ?? default(DateTime),
				//                newEntry.sleep24,
				//                newEntry.sleep48,
				//                newEntry.hoursAwake,
				//                newEntry.lowThreshold,
				//                newEntry.highThreshold,
				//                newEntry.currentScore,
				//                newEntry.CurrentLevel,
				//                newEntry.AlgorithmVersion,
				//                newEntry.becomesModerate ?? default(DateTime),
				//                newEntry.becomesHigh ?? default(DateTime),
				//                newEntry.becomesExtreme ?? default(DateTime));
				//FatigueManager.LogCalcRequest request = new FatigueManager.LogCalcRequest(body);
				//FatigueManager.LogCalcResponse response = FM.LogCalc(request);

				FatigueManager.FatigueCalculatorData FM = new FatigueManager.FatigueCalculatorData();
				FM.Url = Config.ConfigSettings.settings.logServiceUrl;
				// work around the local/server time problem
				DateTime done = new DateTime(newEntry.dateTimeDone.Year, newEntry.dateTimeDone.Month, newEntry.dateTimeDone.Day, newEntry.dateTimeDone.Hour, newEntry.dateTimeDone.Minute, newEntry.dateTimeDone.Second);
				FM.LogCalc(done, true,
												newEntry.DeviceId,
												newEntry.Identity,
												newEntry.shiftStart ?? default(DateTime), true,
												newEntry.shiftEnd ?? default(DateTime), true,
												newEntry.sleep24, true,
												newEntry.sleep48, true,
												newEntry.hoursAwake, true,
												newEntry.lowThreshold, true,
												newEntry.highThreshold, true,
												newEntry.currentScore, true,
												newEntry.CurrentLevel,
												newEntry.AlgorithmVersion,
												newEntry.becomesModerate ?? default(DateTime), true,
												newEntry.becomesHigh ?? default(DateTime), true,
												newEntry.becomesExtreme ?? default(DateTime), true);

				return true;
			}
			catch
			{
				// just report that we failed
				return false;
			}
		}

		bool ILogService.isValid
		{
			get
			{
				try
				{
					const int meaningOfLife = 42;
					FatigueManager.FatigueCalculatorData FM = new FatigueManager.FatigueCalculatorData();
					FM.Url = _url;
					int result = 0;
					bool resultsent = false;
					FM.TestConnect(meaningOfLife, true, out result, out resultsent);
					return (result == meaningOfLife);

					//FatigueManager.FatigueCalculatorDataSoapClient FM = new FatigueManager.FatigueCalculatorDataSoapClient();
					//FatigueManager.TestConnectResponse response = FM.TestConnect(new FatigueManager.TestConnectRequest(meaningOfLife));
					//return (response.TestConnectResult == meaningOfLife );
				}
				catch (Exception err)
				{
					_lastError = err;
					return false;
				}
			}
		}

		Exception ILogService.lastError
		{
			get { return _lastError; }
		}

		bool ILogService.isIdentityOnLog(identity user)
		{
			try
			{
				FatigueManager.FatigueCalculatorData FM = new FatigueManager.FatigueCalculatorData();
				FM.Url = _url;
				bool result = false;
				bool resultspecified = false;
				FM.IsIdentityOnLog(user.Name, out result, out resultspecified);
				if (resultspecified)
					return result;
				else
					return false;

				//FatigueManager.FatigueCalculatorDataSoapClient FM = new FatigueManager.FatigueCalculatorDataSoapClient();
				//FatigueManager.IsIdentityOnLogResponse response = FM.IsIdentityOnLog(new FatigueManager.IsIdentityOnLogRequest(new FatigueManager.IsIdentityOnLogRequestBody(user.Name)));
				//return response.Body.IsIdentityOnLogResult;
			}
			catch (Exception err)
			{
				_lastError = err;
				return false;
			}
		}

		DateTime? ILogService.lastLogEntryForUser(identity user)
		{
			try
			{
				FatigueManager.FatigueCalculatorData FM = new FatigueManager.FatigueCalculatorData();
				FM.Url = _url;
				DateTime? result;
				bool resultspecified = false;
				FM.lastLogEntryForUser(user.Name, out result, out resultspecified);
				if (resultspecified)
					return result;
				else
					return null;

				//FatigueManager.FatigueCalculatorDataSoapClient FM = new FatigueManager.FatigueCalculatorDataSoapClient();
				//FatigueManager.lastLogEntryForUserResponse response = FM.lastLogEntryForUser(new FatigueManager.lastLogEntryForUserRequest(new FatigueManager.lastLogEntryForUserRequestBody(user.Name)));
				//return response.Body.lastLogEntryForUserResult;
			}
			catch (Exception err)
			{
				_lastError = err;
				return null;
			}
		}

		public LogType thisLogType
		{
			get { return LogType.remote; }
		}
	}
}