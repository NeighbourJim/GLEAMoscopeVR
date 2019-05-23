using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunsetController : MonoBehaviour
{
    // There are two publicly available methods - StartSunset() and ResetSky(). 
    // For testing in Unity, press 'P' to call StartSunset() and press 'R' to call ResetSky().

    [SerializeField] private Transform fullSkyImagery;
    [SerializeField] private Transform visibleSpectrum;
    [SerializeField] private Light sun;
    [SerializeField] private Material sunsetSkyboxMat;
    [SerializeField] private float fadeTime = 6;
    [SerializeField] private float fadeInPoint = 0.2f;
    [SerializeField] private float exposureStart = 3f;
    [SerializeField] private Color sunColorStart = new Color(0.274f, 0.055f, 0.000f, 1.000f);
    [SerializeField] private Color sunColorEnd = new Color(0.039f, 0.039f, 0.078f, 1.000f);
    [SerializeField] private int sunRotationXStart = 150;
    [SerializeField] private int sunRotationXEnd = 180;
    [SerializeField] private int moonRotationXStart = 0;
    [SerializeField] private int moonRotationXEnd = 90;
    private Material sphereMaterial;
    private Color currentColor;
    private float exposureEnd;
    private float sunRotationXCurrent;
    private float moonRotationXCurrent;
    private float exposureCurrent;
    private float currentTransparency;
    private float elapsedTime;
    private bool sunHasSet;
    private bool cycleHasCompleted;

    private void Awake()
    {
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
        sun.transform.eulerAngles = new Vector3(sunRotationXStart, 0, 0);
    }

    // Starts the sunset and eventual fade in of visible spectrum.
    public void StartSunset()
    {
        StartCoroutine(FadeExposure(exposureStart, exposureEnd));
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
            currentColor = Color.Lerp(sunColorStart, sunColorEnd, Mathf.Clamp01(elapsedTime / fadeTime));
            sunRotationXCurrent = Mathf.Lerp(sunRotationXStart, sunRotationXEnd, elapsedTime / fadeTime);
            sun.transform.eulerAngles = new Vector3(sunRotationXCurrent, transform.eulerAngles.y, transform.eulerAngles.z);
            sun.color = Color.Lerp(sunColorStart, sunColorEnd, elapsedTime / fadeTime);
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

    private void OnApplicationQuit()
    {
        SetSkyboxExposure();
        ZeroSpectrumTransparency();
    }

}
