using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TranslationTextChanger : MonoBehaviour
{
    private TextMeshProUGUI textfieldUGUI = null;
    private TextMeshPro textfieldGeneric = null;
    LanguageSetting CurrentLanguage = LanguageSetting.English;

    [Tooltip("The text this object will display when in English mode"), TextArea(3, 10)]
    [SerializeField] private string englishText;
    [Tooltip("The text this object will display when in Italian mode"), TextArea(3, 10)]
    [SerializeField] private string italianText;

    private void ChangeText()
    {
        if (CurrentLanguage != SettingsManager.Instance.CurrentLanguageSetting)
        {
            CurrentLanguage = SettingsManager.Instance.CurrentLanguageSetting;
            switch (CurrentLanguage)
            {
                case LanguageSetting.English:
                    if (textfieldUGUI != null)
                        textfieldUGUI.text = englishText;
                    else
                        textfieldGeneric.text = englishText;
                    break;
                case LanguageSetting.Italian:
                    if (textfieldUGUI != null)
                        textfieldUGUI.text = italianText;
                    else
                        textfieldGeneric.text = italianText;
                    break;
            }
        }
    }

    private void Awake()
    {
        SetAndCheckReferences();
    }

    private void LateUpdate()
    {
        ChangeText();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<TeleportToSceneEvent>(_ => ChangeText());
        EventManager.Instance.AddListener<VideoClipEndedEvent>(_ => ChangeText());        
        EventManager.Instance.AddListener<LanguageSettingChangedEvent>(_ => ChangeText());
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<TeleportToSceneEvent>(_ => ChangeText());
        EventManager.Instance.AddListener<VideoClipEndedEvent>(_ => ChangeText());
        EventManager.Instance.RemoveListener<LanguageSettingChangedEvent>(_ => ChangeText());
    }

    private void SetAndCheckReferences()
    {
        textfieldUGUI = GetComponent<TextMeshProUGUI>();
        textfieldGeneric = GetComponent<TextMeshPro>();
        Assert.IsFalse(textfieldUGUI == null && textfieldGeneric == null, $"<b>[{GetType().Name} - {gameObject.name}]</b> Translation Text Changer has no text field.");
    }
}
