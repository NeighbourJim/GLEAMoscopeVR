using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [Header("Canvas Objects")]
    [Tooltip("The canvases to fade through. They will fade in order.")]
    [SerializeField] private Canvas[] canvases;
    [Header("Fade Durations")]
    [Tooltip("The time in seconds it will take for a canvas to fade up")]
    [SerializeField] private float FadeUpTime = 1.5f;
    [Tooltip("The time in seconds it will take for a canvas to fade down")]
    [SerializeField] private float FadeDownTime = 1.5f;
    [Tooltip("The time in seconds between a canvas fading down and the next fading up.")]
    [SerializeField] private float WaitTime = 0.5f;
    [Tooltip("The time in seconds a canvas will remain fully visible before fading to the next.")]
    [SerializeField] private float AppearTime = 5f;

    Coroutine credits = null;
    bool fading = false;

    private void Start()
    {
        Assert.IsTrue(canvases.Length > 0, "Credits canvas array not set!");
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            StartCredits();
        }
    }

    public void StartCredits()
    {
        credits = StartCoroutine(Credits());
    }

    public void StopCredits()
    {
        StopCoroutine(credits);
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    IEnumerator Credits()
    {
        Coroutine fadeUp = null;
        Coroutine fadeDown = null;
        for(int i = 0; i < canvases.Length; i++)
        {
           
            if(i > 0)
            {
                if (fadeDown != null)
                {
                    StopCoroutine(fadeDown);
                }
                fadeDown = StartCoroutine(FadeCanvasDown(canvases[i-1]));
            }
            yield return new WaitUntil(() => !fading);
            yield return new WaitForSeconds(WaitTime);
            if (i < canvases.Length)
            {
                if (fadeUp != null)
                {
                    StopCoroutine(fadeUp);
                }
                fadeUp = StartCoroutine(FadeCanvasUp(canvases[i]));
            }
            yield return new WaitUntil(() => !fading);
            yield return new WaitForSeconds(AppearTime);
        }
        yield return null;
    }

    IEnumerator FadeCanvasUp(Canvas c)
    {
        float elapsed = 0;
        fading = true;
        while (elapsed < FadeUpTime)
        {
            c.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, (elapsed / FadeUpTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }

    IEnumerator FadeCanvasDown(Canvas c)
    {
        float elapsed = 0;
        fading = true;
        while (elapsed < FadeDownTime)
        {
            c.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, (elapsed / FadeDownTime));
            elapsed += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }
}
