using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TranslationTextChanger : MonoBehaviour
{
    private TextMeshProUGUI textField = null;

    [Tooltip("The text this object will display when in English mode"), TextArea(3, 10)]
    [SerializeField] private string englishText;
    [Tooltip("The text this object will display when in Italian mode"), TextArea(3, 10)]
    [SerializeField] private string italianText;

    private void ChangeText()
    {
        var CurrentLanguage = SettingsManager.Instance.CurrentLanguageSetting;
        switch (CurrentLanguage)
        {
            case LanguageSetting.English:
                textField.text = englishText;
                break;
            case LanguageSetting.Italian:
                textField.text = italianText;
                break;
        }
    }

    private void Awake()
    {
        SetAndCheckReferences();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<TeleportToSceneEvent>(_ => ChangeText());
        EventManager.Instance.AddListener<LanguageSettingChangedEvent>(_ => ChangeText());
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<TeleportToSceneEvent>(_ => ChangeText());
        EventManager.Instance.RemoveListener<LanguageSettingChangedEvent>(_ => ChangeText());
    }

    private void SetAndCheckReferences()
    {
        textField = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(textField, "Translation Text Changer is on an object without a TextMeshProUGUI component.");
    }
}
