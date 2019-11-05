namespace GLEAMoscopeVR.Events
{
    namespace GLEAMoscopeVR.Events
    {
        public class PassiveBlinkSettingChanged : GlobalEvent
        {
            public bool BlinkInPassiveMode { get; }

            public PassiveBlinkSettingChanged(bool blinkInPassiveMode, string eventMessage = "") : base(eventMessage)
            {
                BlinkInPassiveMode = blinkInPassiveMode;
            }
        }
    }
}