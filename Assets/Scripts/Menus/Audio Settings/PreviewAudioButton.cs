using GLEAMoscopeVR.Interaction;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GLEAMoscopeVR.Settings;

namespace GLEAMoscopeVR.Audio
{
    public class PreviewAudioButton : MonoBehaviour, IActivatable
    {
        public AudioClip AudioClipEnglish;
        public AudioClip AudioClipItalian;
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

        private AudioClip selectLanguageClip()
        {
            switch(SettingsManager.Instance.CurrentLanguageSetting)
            {
                case LanguageSetting.English:
                    return AudioClipEnglish;
                case LanguageSetting.Italian:
                    return AudioClipItalian;
                default:
                    return null;
            }
        }
        
        public bool CanActivate()
        {
            return !(AudioSource.clip == selectLanguageClip() && AudioSource.isPlaying);
        }

        public void Activate()
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }

            AudioSource.clip = selectLanguageClip();
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

            Assert.IsNotNull(AudioClipEnglish, $"<b>[{GetType().Name} - {gameObject.name}]</b> english audio clip has not been assigned.");
            Assert.IsNotNull(AudioClipItalian, $"<b>[{GetType().Name} - {gameObject.name}]</b> italian audio clip has not been assigned.");
            Assert.IsNotNull(AudioSource, $"<b>[{GetType().Name} - {gameObject.name}]</b> audio source has not been assigned.");

            Assert.IsNotNull(BorderImage, $"<b>[{GetType().Name} - {gameObject.name}]</b> border image has not been assigned.");
        }
    }
}