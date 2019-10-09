using GLEAMoscopeVR.Spectrum;

namespace GLEAMoscopeVR.Events
{
    public class SpectrumStateChangedEvent : GlobalEvent
    {
        public Wavelength Wavelength { get; }

        public SpectrumStateChangedEvent(Wavelength wavelength, string eventMessage = "") : base(eventMessage)
        {
            Wavelength = wavelength;
        }
    }
}