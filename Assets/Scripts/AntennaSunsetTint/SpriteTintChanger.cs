using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTintChanger : MonoBehaviour
{
    [SerializeField] private bool HasSunSet = false;
    [SerializeField] private float fadeTime = 6f;

    [SerializeField] private Color startTint;
    [SerializeField] private Color currentTint = new Color(255f,255f,255f,255f);
    [SerializeField] private Color targetTint;

    [SerializeField] private Material []materialsToChange = null;

    private void Awake()
    {
        currentTint = startTint;
    }

    private void OnDisable()
    {
        ResetTint();
    }

    public void StartChangeTint()
    {
        if(!HasSunSet)
        {
            StartCoroutine(ChangeTint());
        }
    }

    public void StartResetTint()
    {
        if(HasSunSet)
        {
            ResetTint();
        }
    }

    void ResetTint()
    {
        foreach (Material material in materialsToChange)
        {
            material.color = startTint;
        }
    }

    //DC 2019/05/24
    //Code snippet inspired by Brett's Sunset Controller. Altered to work with [] of materials.
    IEnumerator ChangeTint()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            currentTint = Color.Lerp(startTint, targetTint, Mathf.Clamp01(elapsedTime / fadeTime));
            currentTint.a = 1f;
            foreach (Material material in materialsToChange)
            {
                if(material != null)
                {
                    material.color = currentTint;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        HasSunSet = true;
    }
}
