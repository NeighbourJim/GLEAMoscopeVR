﻿using System.ComponentModel;

namespace GLEAMoscopeVR.Spectrum
{
    public enum Wavelength
    {
        [Description("Gamma Ray")] Gamma,
        [Description("X-Ray")] XRay,
        [Description("Visible")] Visible,
        [Description("Far-Infrared")] Infrared,
        [Description("Microwave")] Microwave,
        [Description("Radio")] Radio
    }
}