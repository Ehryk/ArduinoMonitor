using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoMonitor
{
    public enum EventType
    {
        ApplicationStart,
        Initialized,
        ApplicationStop,
        ApplicationPause,
        ApplicationContinue,
        ConfigurationChanged,

        LowThresholdCrossed,
        HighThresholdCrossed,
        NormalityRestored,

        EmailSent,
        EmailFailure,

        ReadFailure,

        Error
    }
}
