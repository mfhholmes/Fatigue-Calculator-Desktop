using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fatigue_Calculator_Desktop
{
    public interface ILogService
    {
        bool setLogURL(string logURL);
        List<logEntry> logEntries{get;}
        bool AddLogEntry(logEntry newEntry);
        bool isValid { get; }
        Exception lastError{get;}
        bool isIdentityOnLog(identity user);
    }
}
