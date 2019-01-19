using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterlab.EventBus.Logging
{
  internal class Logger : ILogger
  {
    private Action<string> _logAction;

    public void LogAction(Action<string> action) { _logAction = action; }

    /// <summary>
    /// No opt method unless overridden
    /// </summary>
    /// <param name="message"></param>
    public void Log(string message)
    {
      if(_logAction != null)
      {
        _logAction(message);
      }
    }
  }
}
