using System;
using System.Collections;
using GLEAMoscopeVR.Utility.Extensions;
using GLEAMoscopeVR.Utility.Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GLEAMoscopeVR.Spectrum
{
    public class WavelengthStateController : GenericSingleton<WavelengthStateController>
    {
        #region Variable Declarations

        [Header("Game Objects")]
        [SerializeField]
        [Tooltip("Array of game objects for the wavelength spheres. Length should be 6.")]
        GameObject[] spheres = null;

        [Header("Fade")]
        [SerializeField]
        [Tooltip("The duration of the fade between spheres.")]
        float fadeTime = 1f;

        [SerializeField, Tooltip("Index of the spectrum sphere that is opaque when the app starts.")]
        private int initialSphereIndex = 2;

        [Header("GUI Objects")]
        [SerializeField]
        [Tooltip("The UI slider representing the relative position on the spectrum.")]
        Slider wavelengthSlider = null;

        [SerializeField]
        [Tooltip("The TextMesh object that displays the current wavelength.")]
        TextMeshProUGUI wavelengthLabel = null;

        

        CameraBlink cameraBlink = null;
        UnityAction eyeClosedAction = null;

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
        [SerializeField]
        Wavelengths currentWavelength = Wavelengths.Visible;
        public Wavelengths CurrentWavelength => currentWavelength;
        
        #endregion

        public event Action OnWavelengthChanged;

        #region Unity Methods
        void Start()
        {
            cameraBlink = FindObjectOfType<CameraBlink>();
            SetInitialRendererState();
            UpdateWavelengthLabel(currentWavelength);
        }
        #endregion

        #region Renderer State
        /// <summary>
        /// Sets state of sphere renderers when the app loads.
        /// </summary>
        private void SetInitialRendererState()
        {
            foreach (var sphere in spheres)
            {
                ToggleRendererState(sphere, sphere != spheres[initialSphereIndex]);
            }
        }

        /// <summary>
        /// Enables renderers for spheres with opaque materials.
        /// Disables renderers for spheres with transparent materials.
        /// </summary>
        /// <param name="sphere">Game object for which renderer state is to be updated</param>
        /// <param name="isTransparent">Transparent / opaque state of sphere material.</param>
        private void ToggleRendererState(GameObject sphere, bool isTransparent)
        {
            sphere.GetComponent<Renderer>().enabled = !isTransparent;
        }
        #endregion

        #region State Management

        /// <summary>
        /// Calls ChangeState() to move the state forward (longer wavelengths, e.g Visible -> Infrared)
        /// Does nothing if fadingUp or fadingDown are true, or if at the final state
        /// </summary>
        public void StateForward()
        {
            Debug.Log("Forward!");
            if (!fadingUp && !fadingDown && currentWavelength != Wavelengths.Radio)
            {
                ChangeState(currentWavelength + 1);
            }
        }

        /// <summary>
        /// Calls ChangeState() to move the state backward (shorter wavelengths, e.g Visible -> X-Ray)
        /// Does nothing if fadingUp or fadingDown are true, or if at the first state
        /// </summary>
        public void StateBackward()
        {
            Debug.Log("Backward!");
            if (!fadingUp && !fadingDown && currentWavelength != Wavelengths.Gamma)
            {
                ChangeState(currentWavelength - 1);
            }
        }

        /// <summary>
        /// Quickly transition to a wavelength state with a blink effect.
        /// </summary>
        /// <param name="destinationState">The state to transition to quickly.</param>
        public void StateQuick(int destinationState)
        {
            if (!fadingUp && !fadingDown && destinationState != (int)currentWavelength)
            {
                eyeClosedAction = () => ChangeStateQuick((Wavelengths)destinationState);
                cameraBlink.EyeClosed.AddListener(eyeClosedAction);
                cameraBlink.Blink();
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

            currentWavelength = destinationState;
            OnWavelengthChanged?.Invoke();
        }

        /// <summary>
        /// Quickly change to a specific wavelength with a blink.
        /// Selects which wavelength to be fading 'from' based on the direction the user would be moving.
        /// </summary>
        /// <param name="destinationState"></param>
        void ChangeStateQuick(Wavelengths destinationState)
        {
            if (destinationState != currentWavelength)
            {
                // which direction the destination is relative to current state. true = left, false = right
                bool direction = (currentWavelength > destinationState); 

                cameraBlink.EyeClosed.RemoveListener(eyeClosedAction);

                switch (destinationState)
                {
                    case Wavelengths.Gamma:
                        wavelengthSlider.value = 0;
                        FadeSpecificSpheres(Wavelengths.Gamma, Wavelengths.XRay);
                        break;
                    case Wavelengths.XRay:
                        wavelengthSlider.value = 20;
                        if (direction)
                            FadeSpecificSpheres(Wavelengths.XRay, Wavelengths.Visible);
                        else
                            FadeSpecificSpheres(Wavelengths.XRay, Wavelengths.Gamma);
                        break;
                    case Wavelengths.Visible:
                        wavelengthSlider.value = 40;
                        if (direction)
                            FadeSpecificSpheres(Wavelengths.Visible, Wavelengths.Infrared);
                        else
                            FadeSpecificSpheres(Wavelengths.Visible, Wavelengths.XRay);
                        break;
                    case Wavelengths.Infrared:
                        wavelengthSlider.value = 60;
                        if (direction)
                            FadeSpecificSpheres(Wavelengths.Infrared, Wavelengths.Microwave);
                        else
                            FadeSpecificSpheres(Wavelengths.Infrared, Wavelengths.Visible);
                        break;
                    case Wavelengths.Microwave:
                        wavelengthSlider.value = 80;
                        if (direction)
                            FadeSpecificSpheres(Wavelengths.Microwave, Wavelengths.Radio);
                        else
                            FadeSpecificSpheres(Wavelengths.Microwave, Wavelengths.Infrared);
                        break;
                    case Wavelengths.Radio:
                        wavelengthSlider.value = 100;
                        FadeSpecificSpheres(Wavelengths.Radio, Wavelengths.Microwave);
                        break;
                }
                UpdateWavelengthLabel(destinationState);
                currentWavelength = destinationState;
                OnWavelengthChanged?.Invoke();
            }
        }

        /// <summary>
        /// Starts the coroutines for fading the sky sphere alpha
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void FadeSpheres(Wavelengths state)
        {
            StartCoroutine(FadeDown(spheres[(int)currentWavelength]));
            StartCoroutine(FadeUp(spheres[(int)state]));
        }

        void FadeSpecificSpheres(Wavelengths fadeUp, Wavelengths fadeDown)
        {
            foreach(GameObject sphere in spheres)
            {
                sphere.GetComponent<Renderer>().material.SetFloat("_Transparency", 0f);
            }
            spheres[(int)fadeDown].GetComponent<Renderer>().material.SetFloat("_Transparency", 1f);
            StartCoroutine(FadeDown(spheres[(int)fadeDown]));
            StartCoroutine(FadeUp(spheres[(int)fadeUp]));
        }

        /// <summary>
        /// Starts the coroutines for animating the slider's handle.
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void AnimateSlider(Wavelengths state)
        {
            if (state > currentWavelength)
            {
                StartCoroutine(SliderUp());
            }
            else if (state < currentWavelength)
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
            ToggleRendererState(sphere, false);//MM - sets renderer active
            fadingUp = true;
            float elapsed = 0;
            Material fadeMaterial = sphere.GetComponent<Renderer>().material;

            while (elapsed < fadeTime)
            {
                fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(0f, 1f, (elapsed / fadeTime)));
                elapsed += Time.deltaTime;
                yield return null;
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
            float elapsed = 0;
            Material fadeMaterial = sphere.GetComponent<Renderer>().material;

            while (elapsed < fadeTime)
            {
                fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(1f, 0f, (elapsed / fadeTime)));
                elapsed += Time.deltaTime;
                yield return null;
            }        

            ToggleRendererState(sphere, true);//MM - sets renderer inactive
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
            wavelengthLabel.text = state.GetDescription();
        }

        #endregion

        #endregion

        #region Not Used
        // <summary>
        // Returns a boolean value that determines whether the scene can be reset based on it's current state.
        // </summary>
        //public bool CanReset()
        //    => !fadingUp
        //       && !fadingDown
        //       && currentWavelength != Wavelengths.Visible;
        #endregion
    }
}