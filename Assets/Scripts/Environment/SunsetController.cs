using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SunsetController : MonoBehaviour
{
    // There are two publicly available methods - StartSunset() and ResetSky(). 
    // For testing in Unity, press 'P' to call StartSunset() and press 'R' to call ResetSky().

    [SerializeField] private Transform fullSkyImagery;
    [SerializeField] private Transform visibleSpectrum;
    [SerializeField] private Material[] billboardMaterials;
    [SerializeField] private Light sun;
    [SerializeField] private Material sunsetSkyboxMat;
    [SerializeField] private float fadeTime = 6;
    [SerializeField] private float fadeInPoint = 0.2f;
    [SerializeField] private float exposureStart = 3f;
    [SerializeField] private Color sunColorEnd = new Color(0.745f, 0.745f, 0.980f, 1.000f);
    [SerializeField] private Color billboardsColorStart = new Color(0.274f, 0.129f, 0.086f, 1.000f);
    [SerializeField] private Color billboardsColorEnd = new Color(0.745f, 0.745f, 0.980f, 1.000f);
    [SerializeField] private int sunRotationXStart = 150;
    [SerializeField] private int sunRotationXEnd = 180;
    [SerializeField] private int moonRotationXStart = 40;
    [SerializeField] private int moonRotationXEnd = 90;
    public UnityEvent SunIsSetting;
    private Color sunColorStart;
    private Material sphereMaterial;
    private Color sunColorCurrent;
    private Color billboardsColorCurrent;
    private float exposureEnd;
    private float sunRotationXCurrent;
    private float moonRotationXCurrent;
    private float exposureCurrent;
    private float currentTransparency;
    private float elapsedTime;
    private bool sunHasSet;
    private bool cycleHasCompleted;
    public bool SunsetCompleted => cycleHasCompleted;

    private void Start()
    {
        sunColorStart = sun.color;
        ResetSky();
    }

    // Resets the skybox to the sunset material and resets other necessary values for calling StartSunset().
    public void ResetSky()
    {
        cycleHasCompleted = false;
        sunHasSet = false;
        SetSkyboxExposure();
        ZeroSpectrumTransparency();
        elapsedTime = 0f;
        currentTransparency = 0;
        exposureCurrent = exposureStart;
        exposureCurrent = RenderSettings.skybox.GetFloat("_Exposure");
        sun.color = sunColorStart;
        foreach (Material mat in billboardMaterials)
        {
            mat.color = billboardsColorStart;
        }
        sun.transform.eulerAngles = new Vector3(sunRotationXStart, 0, 0);
    }

    // Starts the sunset and eventual fade in of visible spectrum.
    public void StartSunset()
    {
        StartCoroutine(FadeExposure(exposureStart, exposureEnd));
        SunIsSetting.Invoke();
    }

    private void Update()
    {
        WaitForInput();

        if (sunHasSet && !cycleHasCompleted)
        {
            FadeInfullSkyImagery();
        }
    }

    // Press P to start sunset. Press R to reset sky.
    private void WaitForInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartSunset();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSky();
        }
    }

    IEnumerator FadeExposure(float expStart, float expEnd)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            if (exposureCurrent <= fadeInPoint & !cycleHasCompleted)
            {
                sunHasSet = true;
            }

            elapsedTime += Time.deltaTime;
            exposureCurrent = Mathf.Lerp(expStart, expEnd, Mathf.Clamp01(elapsedTime / fadeTime));
            sunColorCurrent = Color.Lerp(sunColorStart, sunColorEnd, Mathf.Clamp01(elapsedTime / fadeTime * 0.5f));
            billboardsColorCurrent = Color.Lerp(billboardsColorStart, billboardsColorEnd, Mathf.Clamp01(elapsedTime / fadeTime * 1.2f));
            sunRotationXCurrent = Mathf.Lerp(sunRotationXStart, sunRotationXEnd, elapsedTime / fadeTime * 0.5f);
            if (exposureCurrent > fadeInPoint)
            {
                sun.transform.eulerAngles = new Vector3(sunRotationXCurrent, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            sun.color = Color.Lerp(sunColorStart, sunColorEnd, elapsedTime / fadeTime);
            foreach (Material mat in billboardMaterials)
            {
                mat.color = billboardsColorCurrent;
            }
            RenderSettings.skybox.SetFloat("_Exposure", exposureCurrent);
            yield return new WaitForEndOfFrame();
        }

        elapsedTime = 0f;
    }

    private void FadeInfullSkyImagery()
    {
        if (exposureCurrent < fadeInPoint && currentTransparency < 1)
        {
            elapsedTime += Time.deltaTime;
            currentTransparency = Mathf.Lerp(0, 1, Mathf.Clamp01(elapsedTime / fadeTime));
            sphereMaterial.SetFloat("_Transparency", currentTransparency);
            moonRotationXCurrent = Mathf.SmoothStep(moonRotationXStart, moonRotationXEnd, elapsedTime / fadeTime);
            sun.transform.eulerAngles = new Vector3(moonRotationXCurrent, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            Debug.Log("Day/Night Cycle Complete");
            
            cycleHasCompleted = true;
        }
    }

    private void ZeroSpectrumTransparency()
    {
        foreach (Transform spectrum in fullSkyImagery)
        {
            if (spectrum.GetComponent<Renderer>() != null)
            {
                spectrum.GetComponent<Renderer>().enabled = true;
                sphereMaterial = spectrum.GetComponent<Renderer>().material;
                sphereMaterial.SetFloat("_Transparency", 0);
            }
        }

        sphereMaterial = visibleSpectrum.GetComponent<Renderer>().material;
    }

    private void SetSkyboxExposure()
    {
        RenderSettings.skybox.SetFloat("_Exposure", exposureStart);
    }

    private void SetBillboardColor()
    {
        foreach (Material mat in billboardMaterials)
        {
            mat.color = billboardsColorStart;
        }
    }

    private void OnApplicationQuit()
    {
        SetSkyboxExposure();
        SetBillboardColor();
        ZeroSpectrumTransparency();
    }

}
