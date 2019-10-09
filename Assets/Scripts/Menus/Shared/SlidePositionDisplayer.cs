using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SlidePositionDisplayer : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] int currentPos = 0;
    [SerializeField] int maxPos = 1;
    [SerializeField] bool running = false;
    Coroutine SlideRoutine = null;

    [Header("Image Components (Children in panel)")]
    [SerializeField] Sprite emptySprite;
    [SerializeField] Sprite occupiedSprite;
    [SerializeField] List<Image> _slidePosImages = new List<Image>();
    [SerializeField] List<bool> IsShortAppearTime = new List<bool>();

    [Header("Fade duration")]
    [SerializeField] float FadeUp = 1.5f;
    [SerializeField] float FadeDown = 1.5f;
    [SerializeField] float TimeMultPosOffset = 0.5f;
    [SerializeField] float ShortAppearTime = 5f;
    [SerializeField] float LongAppearTime = 10f;

    private void Start()
    {
        SetAndCheckReferences();
    }

    public void StartSlideRoutine()
    {
        StopSlideRoutine();
        running = true;
        SlideRoutine = StartCoroutine(SlideEnumerator());
    }

    public void StopSlideRoutine()
    {
        running = false;
        if (SlideRoutine != null)
        {
            StopCoroutine(SlideRoutine);
        }

        for (int i = 0; i < maxPos + 1; i++)
        {
            _slidePosImages[i].sprite = emptySprite;
        }

        currentPos = 0;
    }

    IEnumerator SlideEnumerator()
    {
        maxPos = _slidePosImages.Count-1;

        while(running)
        {
            yield return new WaitForSecondsRealtime(FadeUp);
            //Set images based on currentPos, else set to sprite empty
            for (int i = 0; i < maxPos+1; i++)
            {
                if (i == currentPos)
                {
                    _slidePosImages[i].sprite = occupiedSprite;
                }
                else
                {
                    _slidePosImages[i].sprite = emptySprite;
                }
            }

            yield return new WaitForSecondsRealtime((IsShortAppearTime[currentPos] ? ShortAppearTime : LongAppearTime )+FadeDown);

            if (currentPos != maxPos)
            {
                currentPos++;
            }
        }
    }

    private void SetAndCheckReferences()
    {
        if (_slidePosImages.Contains(null) || _slidePosImages.Count < 1)
        {
            Assert.IsNotNull(_slidePosImages, $"<b>_slidePosImages</b> is missing an image component in the list. Searching for children images..");
            _slidePosImages = new List<Image>(GetComponentsInChildren<Image>());
            maxPos = _slidePosImages.Count - 1;

            if(IsShortAppearTime.Count == _slidePosImages.Count || IsShortAppearTime.Count < 1)
            {
                List<bool> tempBools = new List<bool>();
                foreach(Image image in _slidePosImages)
                {
                    tempBools.Add(true);
                }
                IsShortAppearTime = tempBools;
            }
        }
    }
}
