using UnityEngine;

namespace GLEAMoscopeVR.Events
{
    /// <summary>
    /// Base class for all events handled by <see cref="EventManager"/>.
    /// </summary>
    public class GlobalEvent
    {
        protected string _logPrefix = "";
        protected string _logMessage = "";

        public string LogMessage => _logMessage;

        public GlobalEvent(string eventMessage = "")
        {
            GenerateLogMessage(eventMessage);
        }

        protected void GenerateLogMessage(string eventMessage)
        {
            _logPrefix = $"<b>[{GetType().Name}]</b>";
            _logMessage = $"{_logPrefix} {eventMessage}";
            Debug.Log(_logMessage);
        }
    }
}