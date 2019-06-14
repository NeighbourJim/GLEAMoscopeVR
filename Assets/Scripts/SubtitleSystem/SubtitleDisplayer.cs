using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLEAMoscopeVR.SubtitleSystem;
using TMPro;

public class SubtitleDisplayer : MonoBehaviour
{
    [SerializeField]
    GameObject TextStuff;
    Subtitle SubQueue = null;
    int SubListCounter = 0;
    Coroutine SubtitleRoutine;
    [SerializeField]
    bool IsFemaleVO = false;

    public void SetSubQueue(Subtitle receiveSub)
    {
        SubQueue = receiveSub;
    }

    private void Update()
    {
        if(SubQueue != null && SubtitleRoutine == null)
        {
            SubtitleRoutine = StartCoroutine(DisplaySubtitle(SubQueue));
        }
    }

    IEnumerator DisplaySubtitle(Subtitle Subtitle)
    {
        yield return new WaitForSecondsRealtime(Subtitle.startDelay);

        
        while (SubListCounter <= Subtitle.subtitle.Count - 1)
        {
            float delay;

            if (IsFemaleVO)
            {
                delay = Subtitle.delayLengthF[SubListCounter];
            }
            else
            {
                delay = Subtitle.delayLengthM[SubListCounter];
            }

            TextStuff.GetComponent<TextMeshProUGUI>().text = Subtitle.subtitle[SubListCounter];
            SubListCounter++;
            yield return new WaitForSecondsRealtime(delay);
        }

        SubQueue = null;
        SubListCounter = 0;
        TextStuff.GetComponent<TextMeshProUGUI>().text = "";
        SubtitleRoutine = null;
        yield return new WaitForSecondsRealtime(0);

    }
    //Fucking mediocre system honestly.
}
