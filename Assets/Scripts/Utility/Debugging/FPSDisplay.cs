using UnityEngine;
using TMPro;

namespace GLEAMoscopeVR.Utility.Debugging
{
    /// <summary>
    /// Displays the scenes current frames per second.
    /// Replaces VRFPSCounter.cs from VR Standard Assets
    /// </summary>
    public class FPSDisplay : MonoBehaviour
    {
        private const float smoothingCoef = 0.1f;   // This is used to smooth out the displayed fps.
        private float deltaTime;                    // This is the smoothed out time between frames.

        TextMeshProUGUI _fpsText;                   // Reference to the component that displays the fps.
        
        void Start ()
        {
            _fpsText = GetComponent<TextMeshProUGUI> ();
        }

        void Update ()
        {
            // This line has the effect of smoothing out delta time.
            deltaTime += (Time.deltaTime - deltaTime) * smoothingCoef;
            
            // The frames per second is the number of frames this frame (one)
            // divided by the time for this frame (delta time).
            float fps = 1.0f / deltaTime;

            // Set the displayed value of the fps to be an integer.
            _fpsText.text = Mathf.FloorToInt (fps) + " fps";

            // Turn the fps display on and off using the F key.
            if (Input.GetKeyDown (KeyCode.F))
            {
                _fpsText.enabled = !_fpsText.enabled;
            }
        }
    }
}