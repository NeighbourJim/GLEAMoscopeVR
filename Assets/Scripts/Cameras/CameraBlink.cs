using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraBlink : MonoBehaviour
{
    [Header("Eye Closing")]
    [Tooltip("The amount transparency is incremented by.")]
    public float Increment = 0.066f;
    [Tooltip("Seconds between incrementing transparency.")]
    public float IncrementWait = 0.01f;
    [Header("Eye Closed")]
    [Tooltip("How many seconds the 'eye' remains shut for once fading to black.")]
    public float SecondsToStayBlack = 0.5f;

    [Header("Unity Events")]
    [Tooltip("Invoked when the eye begins to close.")]
    public UnityEvent BlinkStart;
    [Tooltip("Invoked when the eye is fully closed.")]
    public UnityEvent EyeClosed;
    [Tooltip("Invoked when the eye has closed and opened again.")]
    public UnityEvent BlinkEnded;

    Material fadeMaterial;

    /// <summary>
    /// Get the material on the object to modify.
    /// Material must use the Inverted Normals shader.
    /// </summary>
    private void OnEnable()
    {
        fadeMaterial = gameObject.GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Disable the mesh renderer if transparancy is at 0, to save performance
    /// </summary>
    public void Update()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = (fadeMaterial.GetFloat("_Transparency") > 0f);
    }

    /// <summary>
    /// Activates the blink effect.
    /// </summary>
    public void Blink()
    {
        StopCoroutine(BlinkRoutine());
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()        
    {
        BlinkStart.Invoke();

        fadeMaterial.SetFloat("_Transparency", 0f);
        for (float i = 0; i <= 1f; i += Increment)
        {
            fadeMaterial.SetFloat("_Transparency", i);
            yield return new WaitForSeconds(IncrementWait);
        }
        fadeMaterial.SetFloat("_Transparency", 1f);
        EyeClosed.Invoke();

        yield return new WaitForSeconds(SecondsToStayBlack);

        for (float i = 1; i >= 0f; i -= Increment)
        {
            fadeMaterial.SetFloat("_Transparency", i);
            yield return new WaitForSeconds(IncrementWait);
        }
        fadeMaterial.SetFloat("_Transparency", 0f);

        BlinkEnded.Invoke();
    }
}
