using System;
using System.Collections.Generic;

namespace Fatigue_Calculator_Desktop
{
	public interface ILogService
	{
		bool setLogURL(string logURL);

		List<logEntry> logEntries { get; }

		bool AddLogEntry(logEntry newEntry);

		bool isValid { get; }

		Exception lastError { get; }

		bool isIdentityOnLog(identity user);

		DateTime? lastLogEntryForUser(identity user);

		LogType thisLogType { get; }
	}

	public enum LogType
	{
		unknown = 0,
		local = 1,
		remote = 2
	}
}