using System;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public enum Direction
    {
        left,
        right
    }

    public class ChangeLanguageButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public Direction direction = Direction.left;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = true;

        CanvasGroup _canvasGroup;
        Collider _collider;

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            if(this is IActivatable activatable && activatable.CanActivate())
            {
                int[] langs = (int[])Enum.GetValues(typeof(LanguageSetting));
                if (direction == Direction.left)
                {
                    if ((int)SettingsManager.Instance.CurrentLanguageSetting == langs[0])
                    {
                        SettingsManager.Instance.SetLanguageSetting((LanguageSetting)langs[langs.Length - 1]);
                    }
                    else
                    {
                        SettingsManager.Instance.SetLanguageSetting(SettingsManager.Instance.CurrentLanguageSetting - 1);
                    }
                }
                else
                {
                    if ((int)SettingsManager.Instance.CurrentLanguageSetting == langs[langs.Length-1])
                    {
                        SettingsManager.Instance.SetLanguageSetting((LanguageSetting)langs[0]);
                    }
                    else
                    {
                        SettingsManager.Instance.SetLanguageSetting(SettingsManager.Instance.CurrentLanguageSetting + 1);
                    }
                }
            }
        }

        void Awake()
        {
            SetAndCheckReferences();
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            canActivate = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _collider.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");
        }
    }
}
