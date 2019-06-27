using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine.UI;

public class VoiceSelectButton : MonoBehaviour, IActivatable, IHideableUI
{
    [SerializeField] private float activationTime = 2f;
    [SerializeField] private bool canActivate = false;

    #region References
    CanvasGroup _canvasGroup;
    Collider _collider;

    SettingsManager _settingsManager;
    SoundEffects _soundEffects;
    #endregion


    CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
    Collider IHideableUI.Collider => _collider;

    float IActivatable.ActivationTime => activationTime;
    bool IActivatable.IsActivated => false;

    [SerializeField] VoiceoverSetting voiceover = VoiceoverSetting.Female;
    [SerializeField] Color activeColour;
    [SerializeField] Color inactiveColour;
    Image selfBorderImage;
    [SerializeField] VoiceSelectButton otherButton = null;

    #region Unity Methods

    void Awake()
    {
        SetAndCheckReferences();
        if(_settingsManager.UserSettings.VoiceSetting == voiceover)
        {
            SetActiveColour();
        }
        else
        {
            SetInactiveColour();
        }
    }

    void Start()
    {

    }

    #endregion


    bool IActivatable.CanActivate()
    {
        return canActivate;
    }

    void IActivatable.Activate()
    {
        _settingsManager.SetVoiceSetting(voiceover);
        SetActiveColour();
        otherButton.SetInactiveColour();
    }

    void IActivatable.Deactivate() { }

    public void SetVisibleAndInteractableState(bool visible)
    {
        canActivate = visible;
        _canvasGroup.alpha = visible ? 1 : 0;
        _collider.enabled = visible;
    }

    public void SetActiveColour()
    {
        selfBorderImage.color = activeColour;
        canActivate = false;
    }

    public void SetInactiveColour()
    {
        selfBorderImage.color = inactiveColour;
        canActivate = true;
    }

    #region Debugging
    private void SetAndCheckReferences()
    {
        _canvasGroup = GetComponentInParent<CanvasGroup>();
        Assert.IsNotNull(_canvasGroup, $"<b>[VoiceSelectButton]</b> has no Canvas Group component in parent.");

        _collider = GetComponent<Collider>();
        Assert.IsNotNull(_collider, $"<b>[VoiceSelectButton]</b> has no collider component.");

        _settingsManager = FindObjectOfType<SettingsManager>();
        Assert.IsNotNull(_settingsManager, $"[VoiceSelectButton] {gameObject.name} cannot find SettingsManager in the scene.");

        selfBorderImage = GetComponent<Image>();
        Assert.IsNotNull(selfBorderImage, $"<b>[VoiceSelectButton]</b> has no Image component (How?).");

        Assert.IsNotNull(otherButton, $"[VoiceSelectButton] {gameObject.name} does not have its opposite button set.");
    }
    #endregion

}
