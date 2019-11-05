using System.Collections;
using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Utility;
using GLEAMoscopeVR.Utility.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Spectrum
{
    public class SpectrumStateController : GenericSingleton<SpectrumStateController>
    {
        private const int spectrumStateCount = 6;

        #region Variable Declarations
        [Header("Game Objects")]
        [Tooltip("Array of game objects for the wavelength spheres. Length should be 6.")]
        public GameObject[] Spheres = null;

        [Space, SerializeField, Tooltip("Index of the spectrum sphere that is opaque when the app starts.")]
        private int initialSphereIndex = 2;

        [Header("Fade")]
        [SerializeField]
        [Tooltip("The duration of the fade between spheres.")]
        private float fadeTime = 1f;
        public float FadeTime => fadeTime;

        /// <summary>
        /// Flag to prevent input when actively fading a sphere up
        /// </summary>
        private bool fadingUp = false;

        /// <summary>
        /// Flag to prevent input when actively fading a sphere down
        /// </summary>
        private bool fadingDown = false;

        /// <summary>
        /// State variable holding which wavelength is currently enabled - Defaults to Wavelengths.Visible
        /// </summary>
        [SerializeField]
        private Wavelength currentWavelength = Wavelength.Visible;
        #endregion

        public Wavelength CurrentWavelength => currentWavelength;

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            SetAndCheckReferences();
        }

        void Start()
        {
            SetInitialRendererState();
        }
        #endregion

        #region Renderer State
        /// <summary>
        /// Sets state of sphere renderers when the app loads.
        /// </summary>
        public void SetInitialRendererState()
        {
            foreach (var sphere in Spheres)
            {
                ToggleRendererState(sphere, sphere != Spheres[initialSphereIndex]);
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

        public void SetSpectrumStateToVisible()
        {
            currentWavelength = Wavelength.Visible;
            EventManager.Instance.Raise(new SpectrumStateChangedEvent(currentWavelength, $"Forced spectrum state to visible."));
        }

        /// <summary>
        /// Calls ChangeState() to move the state forward (longer wavelengths, e.g Visible -> Infrared)
        /// Does nothing if fadingUp or fadingDown are true, or if at the final state
        /// </summary>
        public void StateForward()
        {
            Debug.Log("Forward!");
            if (!fadingUp && !fadingDown && currentWavelength != Wavelength.Radio)
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
            if (!fadingUp && !fadingDown && currentWavelength != Wavelength.Gamma)
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
                ChangeStateQuick((Wavelength)destinationState);
            }
        }

        /// <summary>
        /// Return whether or not a fade is currently happening.
        /// </summary>
        /// <returns>Boolean flag representing if a transition is in effect.</returns>
        public bool IsFading() => fadingDown || fadingUp;

        /// <summary>
        /// Transition to a new state.
        /// Calls FadeSpheres(), AnimateSlider() and UpdateWavelengthLabel()
        /// </summary>
        /// <param name="destinationState">The Wavelengths to transition to.</param>
        void ChangeState(Wavelength destinationState)
        {
            FadeSpheres(destinationState);
            currentWavelength = destinationState;

            EventManager.Instance.Raise(new SpectrumStateChangedEvent(currentWavelength, $"Wavelength changed to {currentWavelength}"));
        }

        /// <summary>
        /// Quickly change to a specific wavelength with a blink.
        /// Selects which wavelength to be fading 'from' based on the direction the user would be moving.
        /// </summary>
        /// <param name="destinationState">The target state to transition to.</param>
        void ChangeStateQuick(Wavelength destinationState)
        {
            if (destinationState != currentWavelength)
            {
                StartCoroutine(FadeDown(Spheres[(int)currentWavelength]));
                StartCoroutine(FadeUp(Spheres[(int)destinationState]));

                currentWavelength = destinationState;
                EventManager.Instance.Raise(new SpectrumStateChangedEvent(currentWavelength));
            }
        }

        /// <summary>
        /// Starts the coroutines for fading the sky sphere alpha
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        void FadeSpheres(Wavelength state)
        {
            StartCoroutine(FadeDown(Spheres[(int)currentWavelength]));
            StartCoroutine(FadeUp(Spheres[(int)state]));
        }
        #endregion

        #region Sphere Faders

        /// <summary>
        /// Simple co-routine to fade the alpha up on a sky sphere.
        /// </summary>
        /// <param name="sphere">The game object to be faded. The shader for the object must have a property named "_Transparency" tied to the alpha channel!</param>
        /// <returns></returns>
        IEnumerator FadeUp(GameObject sphere)
        {
            ToggleRendererState(sphere, false);
            fadingUp = true;
            float elapsed = 0;
            Material fadeMaterial = sphere.GetComponent<Renderer>().material;

            while (elapsed < fadeTime)
            {
                fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(0f, 1f, (elapsed / fadeTime)));
                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            fadeMaterial.SetFloat("_Transparency", 1f);
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
            yield return new WaitForEndOfFrame();
            fadeMaterial.SetFloat("_Transparency", 0f);
            ToggleRendererState(sphere, true);
            fadingDown = false;
        }
        #endregion

        private void SetAndCheckReferences()
        {
            Assert.IsFalse(Spheres.IsNullOrEmpty(), $"<b>[{GetType().Name}]</b> Spheres array is null or empty.");
            Assert.IsTrue(Spheres.Length == 6, $"<b>[{GetType().Name}]</b> Spheres array length is {Spheres.Length}. Should be 6.");
            Assert.IsTrue(Spheres.Length == spectrumStateCount, $"<b>[{GetType().Name}]</b> Spheres array length is {Spheres.Length}. Should be {spectrumStateCount}.");
        }
    }
}