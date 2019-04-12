using System.Collections;
using GLEAMoscopeVR.Utility.Extensions;
using GLEAMoscopeVR.Utility.Management;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GLEAMoscopeVR.Wavelengths
{
    public class SphereFadeControl : GenericSingleton<SphereFadeControl>
    {
        #region Variable Declarations

        [Header("Game Objects")]
        [SerializeField]
        [Tooltip("Array of game objects for the wavelength spheres. Length should be 6.")]
        GameObject[] spheres = null;

        [Header("GUI Objects")]
        [SerializeField]
        [Tooltip("The UI slider representing the relative position on the spectrum.")]
        Slider wavelengthSlider = null;

        [SerializeField] [Tooltip("The TextMesh object that displays the current wavelength.")]
        TextMeshProUGUI wavelengthLabel = null;

        /// <summary>
        /// Flag to prevent input when actively fading a sphere up
        /// </summary>
        bool fadingUp = false;

        /// <summary>
        /// Flag to prevent input when actively fading a sphere down
        /// </summary>
        bool fadingDown = false;

        /// <summary>
        /// State variable holding which wavelength is currently enabled - Defaults to Wavelengths.Visible
        /// </summary>
        Wavelengths currentState = Wavelengths.Visible;

        #endregion

        #region State Management

        /// <summary>
        /// Returns a boolean value that determines whether the scene can be reset based on it's current state.
        /// </summary>
        public bool CanReset()
            => !fadingUp
               && !fadingDown
               && currentState != Wavelengths.Visible;

        /// <summary>
        /// Calls ChangeState() to move the state forward (longer wavelengths, e.g Visible -> Infrared)
        /// Does nothing if fadingUp or fadingDown are true, or if at the final state
        /// </summary>
        public void StateForward()
        {
            Debug.Log("Forward!");
            if (!fadingUp && !fadingDown && currentState != Wavelengths.Radio)
            {
                ChangeState(currentState + 1);
            }
        }

        /// <summary>
        /// Calls ChangeState() to move the state backward (shorter wavelengths, e.g Visible -> X-Ray)
        /// Does nothing if fadingUp or fadingDown are true, or if at the first state
        /// </summary>
        public void StateBackward()
        {
            Debug.Log("Backward!");
            if (!fadingUp && !fadingDown && currentState != Wavelengths.Gamma)
            {
                ChangeState(currentState - 1);
            }
        }

        /// <summary>
        /// Transition to a new state.
        /// Calls FadeSpheres(), AnimateSlider() and UpdateWavelengthLabel()
        /// </summary>
        /// <param name="destinationState">The Wavelengths to transition to.</param>
        void ChangeState(Wavelengths destinationState)
        {
            FadeSpheres(destinationState);
            AnimateSlider(destinationState);
            UpdateWavelengthLabel(destinationState);

            currentState = destinationState;
        }

        /// <summary>
        /// Starts the coroutines for fading the sky sphere alpha
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void FadeSpheres(Wavelengths state)
        {
            StartCoroutine(FadeDown(spheres[(int) currentState]));
            StartCoroutine(FadeUp(spheres[(int) state]));
        }

        /// <summary>
        /// Starts the coroutines for animating the slider's handle.
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void AnimateSlider(Wavelengths state)
        {
            if (state > currentState)
            {
                StartCoroutine(SliderUp());
            }
            else if (state < currentState)
            {
                StartCoroutine(SliderDown());
            }
        }

        #endregion

        #region Co-Routines

        #region Sphere Faders

        /// <summary>
        /// Simple co-routine to fade the alpha up on a sky sphere.
        /// </summary>
        /// <param name="sphere">The game object to be faded. The shader for the object must have a property named "_Transparency" tied to the alpha channel!</param>
        /// <returns></returns>
        IEnumerator FadeUp(GameObject sphere)
        {
            fadingUp = true;
            for (float i = 0; i < 1.05f; i += 0.025f)
            {
                sphere.GetComponent<Renderer>().material.SetFloat("_Transparency", i);
                yield return new WaitForSeconds(0.05f);
            }

            fadingUp = false;
        }

        /// <summary>
        /// Simple co-routine to fade the alpha down on a sky sphere.
        /// </summary>
        /// <param name="sphere">The game object to be faded. The shader for the object must have a property named "_Transparency" tied to the alpha channel!</param>
        /// <returns></returns>
        IEnumerator FadeDown(GameObject sphere)
        {
            fadingDown = true;
            for (float i = 0; i < 1.05f; i += 0.025f)
            {
                sphere.GetComponent<Renderer>().material.SetFloat("_Transparency", 1f - i);
                yield return new WaitForSeconds(0.05f);
            }

            fadingDown = false;
        }

        #endregion

        #region Slider Manipulation

        IEnumerator SliderUp()
        {
            for (int i = 0; i < 40; i++)
            {
                wavelengthSlider.value += 0.5f;
                yield return new WaitForSeconds(0.05f);
            }
        }

        IEnumerator SliderDown()
        {
            for (int i = 0; i < 40; i++)
            {
                wavelengthSlider.value -= 0.5f;
                yield return new WaitForSeconds(0.05f);
            }
        }

        #endregion

        #region UI Text Manipulation

        /// <summary>
        /// Updates the label showing the current wavelength.
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void UpdateWavelengthLabel(Wavelengths state)
        {
            // updated by MM on 28/02/2019
            wavelengthLabel.text = state.GetDescription();
            //if (state == Wavelengths.XRay)
            //{
            //    wavelengthLabel.text = "X-Ray";
            //}
            //else if (state == Wavelengths.Radio)
            //{
            //    wavelengthLabel.text = "GLEAM (Radio)";
            //}
            //else
            //{
            //    wavelengthLabel.text = state.ToString();
            //}
        }

        #endregion

        #endregion
    }
}