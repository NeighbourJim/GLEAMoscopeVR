using GLEAMoscopeVR.Spectrum;

namespace GLEAMoscopeVR.Events
{
    public class SpriteWavelengthChangedEvent : GlobalEvent
    {
        public Wavelength Wavelength { get; }
        
        public SpriteWavelengthChangedEvent(Wavelength wavelength, string eventMessage = "") : base(eventMessage)
        {
            Wavelength = wavelength;
        }
    }
}