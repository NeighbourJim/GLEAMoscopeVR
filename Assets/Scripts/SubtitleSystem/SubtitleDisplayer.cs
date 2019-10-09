using System.Collections;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using GLEAMoscopeVR.SubtitleSystem;
using TMPro;

public class SubtitleDisplayer : MonoBehaviour
{
    public GameObject SubtitleText;
    public GameObject SubtitleTextBG;

    [Space]
    [SerializeField] private SubtitleData currentSubtitleQueue;
    [SerializeField] private bool isDisplaying = false;
    
    private int subListCounter = 0;

    Coroutine subtitleRoutine;

    public bool IsDisplaying => isDisplaying;

    public void SetSubtitleQueue(SubtitleData receiveSub)
    {
        if (!SettingsManager.Instance.ShowSubtitles)
        {
#if UNITY_EDITOR
            Debug.LogError($"<b>[{GetType().Name}]</b> Trying to show subtitles when user setting is disabled.");
#endif
            return;
        }
        currentSubtitleQueue = receiveSub;
    }

    private void Update()
    {
        if (currentSubtitleQueue != null && !isDisplaying)
        {
            subtitleRoutine = StartCoroutine(DisplaySubtitle(currentSubtitleQueue));
        }
    }

    public void StopSubtitles()
    {
        if (isDisplaying && subtitleRoutine != null)
        {
            StopCoroutine(subtitleRoutine);
            ResetSubtitleDisplayState();
        }
    }

    IEnumerator DisplaySubtitle(SubtitleData subtitle)
    {
        isDisplaying = true;
        subListCounter = 0;

        yield return new WaitForSecondsRealtime(subtitle.startDelay);
        
        while (subListCounter < subtitle.subtitle.Count)
        {
            var delay = SettingsManager.Instance.CurrentVoiceoverSetting == VoiceoverSetting.Female ? subtitle.delayLengthF[subListCounter] : subtitle.delayLengthM[subListCounter];
            SubtitleText.GetComponent<TextMeshPro>().text = subtitle.subtitle[subListCounter];
            SubtitleTextBG.GetComponent<TextMeshPro>().text = "<font=\"Arial SDF\"><mark=#000000DC>" + subtitle.subtitle[subListCounter];
            subListCounter++;
            yield return new WaitForSecondsRealtime(delay);
        }
        
        ResetSubtitleDisplayState();
        yield return new WaitForEndOfFrame();     
    }

    private void ResetSubtitleDisplayState()
    {
        subListCounter = 0;
        SubtitleText.GetComponent<TextMeshPro>().text = string.Empty;
        SubtitleTextBG.GetComponent<TextMeshPro>().text = string.Empty;
        isDisplaying = false;
        currentSubtitleQueue = null;
    }
}
