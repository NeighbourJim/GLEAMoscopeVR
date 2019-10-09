using GLEAMoscopeVR.Interaction;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Audio
{
    public class PreviewAudioButton : MonoBehaviour, IActivatable
    {
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public Image BorderImage;
        
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = true;

        Coroutine playRoutine;

        #region References
        Collider _collider;
        #endregion
        
        public float ActivationTime => activationTime;
        public bool IsActivated => isActivated;
        
        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion
        
        public bool CanActivate()
        {
            return !(AudioSource.clip == AudioClip && AudioSource.isPlaying);
        }

        public void Activate()
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }

            AudioSource.clip = AudioClip;
            AudioSource.Play();
        }

        public void Deactivate(){}

        public void ToggleVisibleAndInteractableState(bool visible)
        {
            _collider.enabled = visible;
            BorderImage.enabled = visible;
            canActivate = visible;
        }

        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no collider component.");

            Assert.IsNotNull(AudioClip, $"<b>[{GetType().Name} - {gameObject.name}]</b> audio clip has not been assigned.");
            Assert.IsNotNull(AudioSource, $"<b>[{GetType().Name} - {gameObject.name}]</b> audio source has not been assigned.");

            Assert.IsNotNull(BorderImage, $"<b>[{GetType().Name} - {gameObject.name}]</b> border image has not been assigned.");
        }
    }
}