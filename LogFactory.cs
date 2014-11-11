using System;

namespace Fatigue_Calculator_Desktop
{
	internal static class LogFactory
	{
		public static ILogService createLog(string url)
		{
			ILogService result;
			// check for crap url
			if (url.Length < 5)
			{
				// invalid log reference
				// so create a default log file reference for a local file in Documents
				result = new logFile();
				result.setLogURL(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "/FatigueCalculatorLog.csv");
				return result;
			}
			// simple test for the moment, does the url start with 'http' or 'https'?
			if (url.Substring(0, 5).ToLower().Contains("http"))
			{
				result = new LogFM();
			}
			else
			{
				result = new logFile();
			}
			result.setLogURL(url);
			return result;
		}
	}
}