using System.Collections;
using GLEAMoscopeVR;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using GLEAMoscopeVR.SubtitleSystem;
using TMPro;

public class SubtitleDisplayer : MonoBehaviour
{
    public GameObject SubtitleText;
    [SerializeField] SubtitleData currentSubQueue;
    [SerializeField] bool playing = false;
    
    public bool IsDisplaying => playing;

    int SubListCounter = 0;
    Coroutine SubtitleRoutine;
    

    public void SetSubQueue(SubtitleData receiveSub)
    {
        currentSubQueue = receiveSub;
    }

    private void Update()
    {
        if (currentSubQueue != null && !playing)
        {
            SubtitleRoutine = StartCoroutine(DisplaySubtitle(currentSubQueue));
        }
    }

    IEnumerator DisplaySubtitle(SubtitleData subtitle)
    {
        playing = true;
        SubListCounter = 0;

        yield return new WaitForSecondsRealtime(subtitle.startDelay);
        
        while (SubListCounter < subtitle.subtitle.Count)
        {
            float delay;
            delay = FindObjectOfType<SettingsManager>().UserSettings.VoiceSetting == VoiceoverSetting.Female ? subtitle.delayLengthF[SubListCounter] : subtitle.delayLengthM[SubListCounter];
            //delay = StateKeeper.Instance.CurrentVoiceoverSetting == VoiceoverSetting.Female ? subtitle.delayLengthF[SubListCounter] : subtitle.delayLengthM[SubListCounter];
            SubtitleText.GetComponent<TextMeshPro>().text = subtitle.subtitle[SubListCounter];
            SubListCounter++;
            yield return new WaitForSecondsRealtime(delay);
        }
        
        SubListCounter = 0;
        SubtitleText.GetComponent<TextMeshPro>().text = subtitle.subtitle[SubListCounter];
        playing = false;
        currentSubQueue = null;
        yield return new WaitForEndOfFrame();     
    }
}
