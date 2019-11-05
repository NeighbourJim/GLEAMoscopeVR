﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using UnityEngine.Assertions;

public class SwapPhotoOnLanguageChange : MonoBehaviour
{
    Image imageComponent;
    public Sprite englishImage;
    public Sprite italianImage;

    private void ChangeSprite()
    {
        switch(SettingsManager.Instance.CurrentLanguageSetting)
        {
            case LanguageSetting.English:
                imageComponent.sprite = englishImage;
                break;
            case LanguageSetting.Italian:
                imageComponent.sprite = italianImage;
                break;
        }
    }

    private void SetAndCheckReferences()
    {
        imageComponent = GetComponent<Image>();
        Assert.IsNotNull(imageComponent, "Image component not found.");
        Assert.IsNotNull(englishImage, "English image for this button not set.");
        Assert.IsNotNull(italianImage, "Italian image for this button not set.");
    }

    private void Awake()
    {
        SetAndCheckReferences();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<LanguageSettingChangedEvent>(_ => ChangeSprite());
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<LanguageSettingChangedEvent>(_ => ChangeSprite());
    }
}