using GLEAMoscopeVR.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StartCreditsButton : MonoBehaviour, IActivatable, IHideableUI
{
    [SerializeField] private float activationTime = 2f;
    [SerializeField] private bool canActivate = false;

    #region References
    CanvasGroup _canvasGroup;
    Collider _collider;

    StartScreenManager _startManager;
    CreditsController _creditsController = null;
    SoundEffects _soundEffects;
    #endregion


    CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
    Collider IHideableUI.Collider => _collider;

    float IActivatable.ActivationTime => activationTime;
    bool IActivatable.IsActivated => false;

    #region Unity Methods

    void Awake()
    {
        SetAndCheckReferences();
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
        _startManager.ShowCreditsCanvas();
        _creditsController.StartCredits();
    }

    void IActivatable.Deactivate() { }

    public void SetVisibleAndInteractableState(bool visible)
    {
        canActivate = visible;
        _canvasGroup.alpha = visible ? 1 : 0;
        _collider.enabled = visible;
    }

    #region Debugging
    private void SetAndCheckReferences()
    {
        _canvasGroup = GetComponentInParent<CanvasGroup>();
        Assert.IsNotNull(_canvasGroup, $"<b>[CreditsButton]</b> has no Canvas Group component in parent.");

        _collider = GetComponent<Collider>();
        Assert.IsNotNull(_collider, $"<b>[CreditsButton]</b> has no collider component.");

        _startManager = FindObjectOfType<StartScreenManager>();
        Assert.IsNotNull(_startManager, $"[CreditsButton] {gameObject.name} cannot find StartScreenManager in the scene.");

        _creditsController = FindObjectOfType<CreditsController>();
        Assert.IsNotNull(_startManager, $"[CreditsButton] {gameObject.name} cannot find StartScreenManager in the scene.");
    }
    #endregion
}
