using System.Linq;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

public class FadeAssetController : MonoBehaviour
{
    //TEMP SCRIPT THIS NEEDS TO BE RE IMPLEMENTED PROPERLY

    [SerializeField]
    bool explorationMode = true;

    [Header("Exploration")]
    public Renderer[] renderers = { };

    [Header("Passive")]
    [SerializeField]
    private Renderer platformRenderer = null;
    [SerializeField]
    private Renderer logoRenderer = null;

    [Header("Debugging")]
    [SerializeField] private ExperienceMode previousMode = ExperienceMode.Exploration;
    [SerializeField] private ExperienceMode currentMode = ExperienceMode.Introduction;

    private CameraBlink cameraBlink;

    #region References
    ExperienceModeController _modeController;
    #endregion


    void Start()
    {
        SetAndCheckReferences();

        GetRenderers();
        SetInitialRendererState();
    }

    private void SetInitialRendererState()//todo replace once persisting mode
    {
        platformRenderer.enabled = false;
        logoRenderer.enabled = false;
    }

    private void GetRenderers()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void HandleExperienceModeChanged()//todo: check logic
    {
        currentMode = _modeController.CurrentMode;

        if (currentMode == previousMode) return;

        switch (currentMode)
        {
            case ExperienceMode.Exploration when previousMode == ExperienceMode.Introduction:
            case ExperienceMode.Introduction when previousMode == ExperienceMode.Exploration:
                return;
            default:
                break;
        }
        
        cameraBlink.EyeClosed.AddListener(UpdateEnvironmentRenderers);
        cameraBlink.Blink();
    }

    private void UpdateEnvironmentRenderers()
    {
        var enableEnvironment = _modeController.CurrentMode != ExperienceMode.Passive;

        SetEnvironmentRendererState(enableEnvironment);
        SetPlatformAndLogoRendererState(!enableEnvironment);

        previousMode = currentMode;
        cameraBlink.EyeClosed.RemoveListener(UpdateEnvironmentRenderers);
    }

    private void SetPlatformAndLogoRendererState(bool enable)
    {
        platformRenderer.enabled = enable;
        logoRenderer.enabled = enable;
    }

    private void SetEnvironmentRendererState(bool enable)
    {
        renderers.ToList().ForEach(r => r.enabled = enable);
    }

    private void SetAndCheckReferences()
    {
        _modeController = FindObjectOfType<ExperienceModeController>().Instance;
        Assert.IsNotNull(_modeController, $"[FadeAssetController] does not have a reference to the experience mode controller.");
        previousMode = _modeController.CurrentMode;
        _modeController.OnExperienceModeChanged += HandleExperienceModeChanged;
        Assert.IsNotNull(platformRenderer, $"[FadeAssetController] does not have a reference to the platform renderer.");

        cameraBlink = Camera.main.GetComponentInChildren<CameraBlink>();
        Assert.IsNotNull(cameraBlink, $"[FadeAssetController] cannot find CameraBlink component in main camera children.");
    }
}

/*
void UpdateProxReticleObjAlpha()
        {
            if (reticleObject != null)
            {

                reticleObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, reticleAlpha / 255f);

                if (doesReticleHideOnNoHit &&
                    currentCentreHitObject == null && currentProxHitObject == null)
                {
                    reticleObject.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    reticleObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
*/

/*
 IEnumerator FadeUp(GameObject sphere)
        {
            ToggleRendererState(sphere, false);//MM - sets renderer active
            fadingUp = true;
            
            for (float i = 0; i < 1.05f; i += 0.025f)
            {
                sphere.GetComponent<Renderer>().material.SetFloat("_Transparency", i);
                yield return new WaitForSeconds(0.05f);
            }

            fadingUp = false;
        }

        /// <summary>
        /// Simple co-routine to fade the alpha down on a sky sphere.
        /// </summary>
        /// <param name="sphere">The game object to be faded. The shader for the object must have a property named "_Transparency" tied to the alpha channel!</param>
        /// <returns></returns>
        IEnumerator FadeDown(GameObject sphere)
        {
            fadingDown = true;
            for (float i = 0; i < 1.05f; i += 0.025f)
            {
                sphere.GetComponent<Renderer>().material.SetFloat("_Transparency", 1f - i);
                yield return new WaitForSeconds(0.05f);
            }

            ToggleRendererState(sphere, true);//MM - sets renderer inactive
            fadingDown = false;
        }
*/

/* Removed 27/05/19 (MM)
public void StartFade()
{
    explorationMode = !explorationMode;

    if(explorationMode)
    {
        if (meshObjects != null)
        {
            foreach (GameObject mesh in meshObjects)
            {
                StartCoroutine(FadeUp(mesh));
            }
        }
    }
    else
    {
        platformRenderer.enabled = true;
        //print("Fade down");
        if (meshObjects != null)
        {
            foreach (GameObject mesh in meshObjects)
            {
                StartCoroutine(FadeDown(mesh));
            }
        }
    }
}

IEnumerator FadeUp(GameObject pObject)
{
    for(float i = 0; i < 1f; i += 0.023809f)
    {
        pObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, i);
        yield return new WaitForSeconds(0.05f);
    }
    platformRenderer.enabled = false;
}

IEnumerator FadeDown(GameObject pObject)
{
    for (float i = 0f; i <= 1f; i += 0.023809f)
    {
        //print("into Fade down");
        pObject.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1-i);
        yield return new WaitForSeconds(0.05f);
    }
}
*/
