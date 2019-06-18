using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    // Real programmers repeat code.
    // Real programmers repeat code.

    [SerializeField] private AudioSource audioSourceWind;
    [SerializeField] private AudioSource audioSourceCicadas;
    [SerializeField] private float maxVolumeWind = 0.5f;
    [SerializeField] private float maxVolumeCicadas = 0.4f;
    [SerializeField] private float fadeInSpeedWind = 0.1f;
    [SerializeField] private float fadeInSpeedCicadas = 0.1f;
    private bool windIsBlowing;
    private bool cicadasAreChirping;
    private float volumeBoostWind;
    private float volumeBoostCicadas;

    public void StartWind()
    {
        audioSourceWind.Play();
        windIsBlowing = true;
    }

    public void StartCicadas()
    {
        audioSourceCicadas.Play();
        cicadasAreChirping = true;
    }

    public void StopWind()
    {
        audioSourceWind.Stop();
        windIsBlowing = false;
        audioSourceWind.volume = 0;
    }

    public void StopCicadas()
    {
        audioSourceCicadas.Stop();
        cicadasAreChirping = false;
        audioSourceWind.volume = 0;
    }

    private void CheckForWind()
    {
        if (windIsBlowing && audioSourceWind.volume != maxVolumeWind)
        {
            if (audioSourceWind.volume < maxVolumeWind)
            {
                audioSourceWind.volume = Mathf.Lerp(0, maxVolumeWind, volumeBoostWind);
                volumeBoostWind += fadeInSpeedWind * Time.deltaTime;
            }
            else
            {
                audioSourceWind.volume = maxVolumeWind;
                volumeBoostWind = 0;
            }
        }
    }

    private void CheckForCicadas()
    {
        if (cicadasAreChirping && audioSourceCicadas.volume != maxVolumeCicadas)
        {
            if (audioSourceCicadas.volume < maxVolumeCicadas)
            {
                audioSourceCicadas.volume = Mathf.Lerp(0, maxVolumeCicadas, volumeBoostCicadas);
                volumeBoostCicadas += fadeInSpeedCicadas * Time.deltaTime;
            }
            else
            {
                audioSourceCicadas.volume = maxVolumeCicadas;
                volumeBoostCicadas = 0;
            }
        }
    }

    private void Update()
    {
        CheckForWind();
        CheckForCicadas();
    }
}
