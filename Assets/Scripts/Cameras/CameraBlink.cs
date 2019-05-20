using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraBlink : MonoBehaviour
{
    [Header("Eyelid Movement")]
    [Tooltip("How many seconds it takes for the eye to close.")]
    public float EyeCloseTime = 0.3f;
    [Tooltip("How many seconds it takes for the eye to open. Should generally == EyeCloseTime.")]
    public float EyeOpenTime = 0.3f;
    [Header("Eye Closed")]
    [Tooltip("How many seconds the 'eye' remains shut for once fading to black.")]
    public float EyeRemainClosedTime = 0.5f;

    [Header("Unity Events")]
    [Tooltip("Invoked when the eye begins to close.")]
    public UnityEvent BlinkStart;
    [Tooltip("Invoked when the eye is fully closed.")]
    public UnityEvent EyeClosed;
    [Tooltip("Invoked when the eye has closed and opened again.")]
    public UnityEvent BlinkEnded;

    Material fadeMaterial;
    Coroutine currentBlink = null;


    /// <summary>
    /// Get the material on the object to modify.
    /// Material must use the Inverted Normals shader.
    /// </summary>
    private void OnEnable()
    {
        fadeMaterial = gameObject.GetComponent<Renderer>().material;
        BlinkStart.AddListener(() => ToggleRenderer(true));
        BlinkEnded.AddListener(() => ToggleRenderer(false));
    }


    /// <summary>
    /// Activates the blink effect using editor set values.
    /// </summary>
    public void Blink()
    {
        if(currentBlink != null)
        {
            StopCoroutine(currentBlink);
        }        
        currentBlink = StartCoroutine(BlinkRoutine());
    }

    /// <summary>
    /// Activates the blink effect using passed parameters.
    /// </summary>
    /// <param name="eye_close_time">Seconds for the eyelid to close.</param>
    /// <param name="eye_open_time">Seconds for the eyelid to open.</param>
    /// <param name="eye_remain_closed_time">Seconds for the eye to remain closed.</param>
    public void Blink(float eye_close_time, float eye_open_time, float eye_remain_closed_time)
    {
        if (currentBlink != null)
        {
            StopCoroutine(currentBlink);
        }
        currentBlink = StartCoroutine(BlinkRoutine(eye_close_time,eye_open_time,eye_remain_closed_time));
    }

    void ToggleRenderer(bool value)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = value;
    }

    /// <summary>
    /// Default Blink co-routine, uses the values set in the editor.
    /// </summary>
    /// <returns></returns>
    IEnumerator BlinkRoutine()
    {
        float elapsed = 0;

        BlinkStart.Invoke();
        while (elapsed < EyeCloseTime)
        {
            fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(0f, 1f, (elapsed / EyeCloseTime)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        EyeClosed.Invoke();
        yield return new WaitForSeconds(EyeRemainClosedTime);
        elapsed = 0f;
        while (elapsed < EyeCloseTime)
        {
            fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(1f, 0f, (elapsed / EyeOpenTime)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        BlinkEnded.Invoke();
    }

    /// <summary>
    /// Override Blink Co-Routine. Can take in values for a specific blink time.
    /// </summary>
    /// <param name="eye_close_time">Seconds for the eyelid to close.</param>
    /// <param name="eye_open_time">Seconds for the eyelid to open.</param>
    /// <param name="eye_remain_closed_time">Seconds for the eye to remain closed.</param>
    /// <returns></returns>
    IEnumerator BlinkRoutine(float EyeCloseTime, float EyeOpenTime, float EyeRemainClosedTime)
    {
        float elapsed = 0;

        BlinkStart.Invoke();

        while (elapsed < EyeCloseTime)
        {
            fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(0f, 1f, (elapsed / EyeCloseTime)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        EyeClosed.Invoke();
        yield return new WaitForSeconds(this.EyeRemainClosedTime);
        elapsed = 0f;
        while (elapsed < EyeCloseTime)
        {
            fadeMaterial.SetFloat("_Transparency", Mathf.Lerp(1f, 0f, (elapsed / EyeOpenTime)));
            elapsed += Time.deltaTime;
            yield return null;
        }

        BlinkEnded.Invoke();
    }
}
