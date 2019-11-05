using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Cameras
{
    /// <summary>
    /// Adapted from <see cref="CameraBlink"/> and Unity VR Samples.
    /// This class is used to fade the entire screen to black (or any chosen colour).  
    /// It should be used to smooth out the transition between scenes or restarting of a scene.
    /// </summary>
    public class CameraBlinkCanvas : MonoBehaviour
    {
        /// <summary>
        /// Reference to the image that covers the screen.
        /// </summary>
        [Tooltip("Reference to the image that covers the screen.")]
        public Image FadeImage;

        /// <summary>
        /// The colour the image fades out to.
        /// </summary>      
        [SerializeField, Tooltip("The colour the image fades in to, and out from.")]
        public Color EyeClosedColour = Color.black;

        [Header("Eyelid Movement")]
        [Tooltip("How many seconds it takes for the eye to close.")]
        public float EyeCloseTime = 0.3f;

        [Tooltip("How many seconds it takes for the eye to open. Should generally == EyeCloseTime.")]
        public float EyeOpenTime = 0.3f;

        [Header("Eye Closed")]
        [Tooltip("How many seconds the 'eye' remains shut for once fading to black.")]
        public float EyeRemainClosedTime = 0.5f;

        [Header("Unity Events")]
        [Tooltip("Invoked when the eye begins to close.")]
        public UnityEvent BlinkStart;

        [Tooltip("Invoked when the eye is fully closed.")]
        public UnityEvent EyeClosed;

        [Tooltip("Invoked when the eye has closed and opened again.")]
        public UnityEvent BlinkEnded;

        Coroutine currentBlink = null;

        [Header("Audio")]
        public AudioClip BlinkClip;

        AudioSource _audioSource;

        void Start()
        {
            SetAndCheckReferences();
        }

        /// <summary>
        /// Get the material on the object to modify.
        /// Material must use the Inverted Normals shader.
        /// </summary>
        private void OnEnable()
        {
            BlinkStart.AddListener(() => EnableImageComponent(true));
            BlinkEnded.AddListener(() => EnableImageComponent(false));
        }

        private void EnableImageComponent(bool enable)
        {
            FadeImage.enabled = enable;
        }

        /// <summary>
        /// Activates the blink effect using editor set values.
        /// </summary>
        public void Blink()
        {
            if (currentBlink != null)
            {
                StopCoroutine(currentBlink);
            }
            currentBlink = StartCoroutine(BlinkRoutine());
        }

        /// <summary>
        /// Activates the blink effect using passed parameters.
        /// </summary>
        /// <param name="eye_close_time">Seconds for the eyelid to close.</param>
        /// <param name="eye_open_time">Seconds for the eyelid to open.</param>
        /// <param name="eye_remain_closed_time">Seconds for the eye to remain closed.</param>
        public void Blink(float eye_close_time, float eye_open_time, float eye_remain_closed_time)
        {
            if (currentBlink != null)
            {
                StopCoroutine(currentBlink);
            }

            currentBlink = StartCoroutine(BlinkRoutine(eye_close_time, eye_open_time, eye_remain_closed_time));
        }

        /// <summary>
        /// Default Blink co-routine, uses the values set in the editor.
        /// </summary>
        /// <returns></returns>
        IEnumerator BlinkRoutine()
        {
            float elapsed = 0;

            BlinkStart.Invoke();

            _audioSource.Play();

            while (elapsed < EyeCloseTime)
            {
                FadeImage.color = Color.Lerp(Color.clear, EyeClosedColour, elapsed / EyeCloseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            EyeClosed.Invoke();
            yield return new WaitForSeconds(EyeRemainClosedTime);
            elapsed = 0f;
            while (elapsed < EyeCloseTime)
            {
                FadeImage.color = Color.Lerp(EyeClosedColour, Color.clear, elapsed / EyeOpenTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            BlinkEnded.Invoke();
        }

        /// <summary>
        /// Override Blink Co-Routine. Can take in values for a specific blink time.
        /// </summary>
        /// <param name="eyeCloseTime">Seconds for the eyelid to close.</param>
        /// <param name="eyeOpenTime">Seconds for the eyelid to open.</param>
        /// <param name="eyeRemainClosedTime">Seconds for the eye to remain closed.</param>
        /// <returns></returns>
        IEnumerator BlinkRoutine(float eyeCloseTime, float eyeOpenTime, float eyeRemainClosedTime)
        {
            float elapsed = 0;

            BlinkStart.Invoke();
            _audioSource.Play();

            while (elapsed < eyeCloseTime)
            {
                FadeImage.color = Color.Lerp(Color.clear, EyeClosedColour, elapsed / eyeCloseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            EyeClosed.Invoke();
            yield return new WaitForSeconds(eyeRemainClosedTime);
            elapsed = 0f;
            while (elapsed < eyeCloseTime)
            {
                FadeImage.color = Color.Lerp(EyeClosedColour, Color.clear, elapsed / eyeOpenTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            BlinkEnded.Invoke();
        }
        private void SetAndCheckReferences()
        {
            _audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource, $"<b>[{GetType().Name}] has no Audio Source component.");
            Assert.IsNotNull(BlinkClip, $"<b>[{GetType().Name}] Blink audio clip is not assigned.");
        }
    }
}