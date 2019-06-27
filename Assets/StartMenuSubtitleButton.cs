using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine.UI;

public class StartMenuSubtitleButton : MonoBehaviour, IActivatable, IHideableUI
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

    [SerializeField] Image checkedImage;

    #region Unity Methods

    void Awake()
    {
        SetAndCheckReferences();
        SetCheckedState();
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
        _settingsManager.SetSubtitleSetting(!_settingsManager.UserSettings.ShowSubtitles);
        SetCheckedState();
    }

    void IActivatable.Deactivate() { }

    public void SetVisibleAndInteractableState(bool visible)
    {
        canActivate = visible;
        _canvasGroup.alpha = visible ? 1 : 0;
        _collider.enabled = visible;
    }

    public void SetCheckedState()
    {
        checkedImage.enabled = _settingsManager.UserSettings.ShowSubtitles;
    }

    #region Debugging
    private void SetAndCheckReferences()
    {
        _canvasGroup = GetComponentInParent<CanvasGroup>();
        Assert.IsNotNull(_canvasGroup, $"<b>[StartMenuSubtitleButton]</b> has no Canvas Group component in parent.");

        _collider = GetComponent<Collider>();
        Assert.IsNotNull(_collider, $"<b>[StartMenuSubtitleButton]</b> has no collider component.");

        _settingsManager = FindObjectOfType<SettingsManager>();
        Assert.IsNotNull(_settingsManager, $"[StartMenuSubtitleButton] {gameObject.name} cannot find SettingsManager in the scene.");

        Assert.IsNotNull(checkedImage, $"<b>[StartMenuSubtitleButton]</b> checkedImage not set.");

    }
    #endregion

}
