using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBus.Logging
{
  internal interface ILogger
  {
    /// <summary>
    /// Set delegate for logging
    /// </summary>
    /// <param name="logAction"></param>
    void LogAction(Action<string> logAction);
    void Log(string message);
  }
}
