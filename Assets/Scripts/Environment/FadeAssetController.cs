using System.Collections;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

public class FadeAssetController : MonoBehaviour
{
    //TEMP SCRIPT THIS NEEDS TO BE RE IMPLEMENTED PROPERLY

    [SerializeField]
    bool explortationMode = true;

    [Header("GameObjects [Meshes]")]
    [SerializeField]
    GameObject[] meshObjects = null;

    [Header("User Platform"), SerializeField]
    private Renderer platformRenderer = null;
    
    ExperienceModeController _modeController;

    void Start()
    {
        GetComponentReferences();
    }

    // Added so that the environment doesn't fade back in when the mode doesn't change
    private void HandleExperienceModeChanged()
    {
        print("Handle mode changed");
        StartFade();
    }

    public void StartFade()
    {
        explortationMode = !explortationMode;
        
        if(explortationMode)
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

    private void GetComponentReferences()
    {
        _modeController = FindObjectOfType<ExperienceModeController>().Instance;
        Assert.IsNotNull(_modeController, $"[FadeAssetController] does not have a reference to the experience mode controller.");
        _modeController.OnExperienceModeChanged += HandleExperienceModeChanged;
        Assert.IsNotNull(platformRenderer, $"[FadeAssetController] does not have a reference to the platform renderer.");
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
