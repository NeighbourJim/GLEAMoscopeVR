using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

//2019/04/10
//TODO:Rework animation logic to correctly handle when current obj that has a interactable interface 
//changes to another object with a interactable interface. ie. interactable INTO interactable.

public class Script_CameraRayCaster : MonoBehaviour
{
    public Camera activeCamera;
    [Header("Mouse Pos Info")]
    public Vector3 mouseCurrentWorldPos = new Vector3(0, 0, 0);
    public Vector3 mouseCurrentScreenPos = new Vector3(0, 0, 0);

    [Header("Raycast Options")]
    public bool isFloatingReticle = false;
    [Tooltip("Set to <=0 for infinite raycast distance.")]
    public float maxRaycastDistance = 0f;
    Ray currentRay;
    RaycastHit currentHit;
    [SerializeField]
    float currentDistance;
    [SerializeField]
    GameObject currentHitObject;
    [SerializeField]
    GameObject lastHitObject;

    [Header("Proximity (Spherecast) Scaling Options")]
    public bool doesProximityScaling = false;
    public float maxProxCastDistance = 0f;
    public float proxCastRadius = 0.5f;

    Ray currentProxRay;
    RaycastHit currentProxHit;
    [SerializeField]
    float currentProxRayDistance;
    [SerializeField]
    GameObject currentProxHitObject;
    [SerializeField]
    GameObject lastProxHitObject;

    Ray currentCentreRay;
    RaycastHit currentCentreHit;
    [SerializeField]
    float currentCentreRayDistance;
    [SerializeField]
    GameObject currentCentreHitObject;
    [SerializeField]
    GameObject lastCentreHitObject;

    [Header("Reticle Options")]
    public bool doesReticleHideOnNoHit = false;
    public bool doesReticleFloatOnNoHit = true;
    public bool doesReticleRotateOffNormal = true;
    public float floatingReticleDistance = 1f;
    public GameObject reticleObject;
    [Range(0f, 5f)]
    public float reticleScale = 1f;
    [Range(0.001f, 1f)]
    public float reticleStartScale = 0.005f;
    
    [Header("Reticle Alpha/Shader Options.")]
    [Range(0, 255)]
    public int reticleAlpha = 255;

    [Header("Animator Controller States/Timer coroutine")]
    public bool isDefault = true;
    public bool isLoading = false;
    public bool isActivated = false;
    public bool hasActivated = false;
    public float secondsToActivate = 2f;
    public float activateTimer = 2f;

    void Start()
    {
        //Camera not set? Use main camera.
        if (activeCamera == null)
        {
            activeCamera = Camera.main;
        }
        activateTimer = secondsToActivate;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateMousePositions();

        ProxRaycastFromCamToMouseWorld();
        UpdateProxReticleObjPosition();
        UpdateProxReticleObjRotation();
        UpdateProxReticleObjScale();
        UpdateProxReticleObjAlpha();

        //Mouse is currently clicked
        if (Input.GetMouseButton(0))
        {
            CheckCurrentClickableInterface();
        }

        
        CheckCurrentInteractableInterface();
        SetCurrentReticleAnimationState();
    }

    //DC 2019/04/10
    //Checks if current object IRay interface
    //if founnd, sets triggers for reticle object animation state machine
    //AND manages timer
    //AND changes current script state boolean logic
    void CheckCurrentInteractableInterface()
    {
        if(currentCentreHitObject != null)
        {
            if(currentCentreHitObject.GetComponent<IRayInteractable>() != null)
            {
                //Is default, set to loading
                if (isDefault && !isLoading 
                    && !isActivated && !hasActivated)
                {
                    isDefault = false;
                    isLoading = true;
                    isActivated = false;
                    hasActivated = false;

                }
                //Is loading, reduce timer
                //if timer is <= , set to activated
                else if(!isDefault && isLoading
                    && !isActivated && !hasActivated)
                {
                    //Activate
                    if(activateTimer <= 0)
                    {
                        isDefault = false;
                        isLoading = false;
                        isActivated = true;
                        hasActivated = true;
                        activateTimer = secondsToActivate;
                        currentCentreHitObject.GetComponent<IRayInteractable>().Activate();
                    }
                    //Still loading, subtract timer.
                    else
                    {
                        activateTimer -= Time.deltaTime;
                    }
                    
                }
                else if (!isDefault && !isLoading
                    && isActivated && hasActivated)
                {
                    isDefault = true;
                    isLoading = false;
                    isActivated = false;
                    hasActivated = true;
                }
            }
            //Not an interactable obj
            else
            {
                isDefault = true;
                isLoading = false;
                isActivated = false;
                hasActivated = false;
                activateTimer = secondsToActivate;
            }
        }
        //No current objs in centre ray
        else
        {
            isDefault = true;
            isLoading = false;
            isActivated = false;
            hasActivated = false;
            activateTimer = secondsToActivate;
        }
    }

    void SetCurrentReticleAnimationState()
    {
        if (reticleObject.GetComponent<Animator>().runtimeAnimatorController != null)
        {
            Animator reticleRefAnimator = reticleObject.GetComponent<Animator>();
            reticleRefAnimator.SetBool("IsDefault", isDefault);
            reticleRefAnimator.SetBool("IsLoading", isLoading);
            reticleRefAnimator.SetBool("IsActivated", isActivated);
            reticleRefAnimator.SetBool("HasActivated", hasActivated);
        }
    }

    void CheckCurrentClickableInterface()
    {
        if (currentHitObject != null)
        {
            if (currentHitObject.GetComponent<IRayClickable>() != null)
            {
                currentHitObject.GetComponent<IRayClickable>().Click();
            }
        }
    }

    //DC 2019/03/28
    void UpdateProxReticleObjPosition()
    {
        if (reticleObject != null)
        {
            if (isFloatingReticle)
            {
                reticleObject.transform.localPosition = new Vector3(0,
                                                                    0,
                                                                    floatingReticleDistance);
            }
            //IF proxObj != null AND centreObj != null, scale based on centre distance
            else if (currentProxHitObject != null && currentCentreHitObject != null)
            {

                //Centre of camera is on an object.
                if (currentProxHitObject == currentCentreHitObject)
                {
                    reticleObject.transform.localPosition = new Vector3(0,
                                                                        0,
                                                                        currentCentreRayDistance);
                }
                //Prox is not on the same obj as centre obj.
                else
                {
                    reticleObject.transform.localPosition = new Vector3(0,
                                                                        0,
                                                                        currentProxRayDistance+proxCastRadius);
                }
            }
            //ELSE IF currentProxObj != null
            else if(currentProxHitObject != null)
            {
                reticleObject.transform.localPosition = new Vector3(0,
                                                                    0,
                                                                    currentProxRayDistance + proxCastRadius);
            }
            else
            {
                reticleObject.transform.localPosition = new Vector3(0,
                                                                    0,
                                                                    floatingReticleDistance);
            }
        }
    }

    /*reticleObject.transform.rotation = Quaternion.LookRotation(activeCamera.transform.up,
                                                                           activeCamera.transform.forward* -1);*/

    //DC 2019/03/28
    void UpdateProxReticleObjRotation()
    {
        if (reticleObject != null)
        {
            if (isFloatingReticle)
            {
                reticleObject.transform.rotation = Quaternion.LookRotation(-activeCamera.transform.forward,
                                                                           activeCamera.transform.forward * -1);
            }
            //IF proxObj != null AND centreObj != null, scale based on centre distance
            else if (currentProxHitObject != null && currentCentreHitObject != null)
            {
                //Centre of camera is on an object.
                if (currentProxHitObject == currentCentreHitObject)
                {
                    if(doesReticleRotateOffNormal)
                    {
                        reticleObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,
                                                                         currentCentreHit.normal);
                    }
                    else
                    {
                        reticleObject.transform.rotation = Quaternion.LookRotation(-activeCamera.transform.forward,
                                                                           activeCamera.transform.forward * -1);
                    }
                }
                //Prox is not on the same obj as centre obj.
                else
                {
                    if (doesReticleRotateOffNormal)
                    {
                        reticleObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,
                                                                         currentProxHit.normal);
                    }
                    else
                    {
                        reticleObject.transform.rotation = Quaternion.LookRotation(-activeCamera.transform.forward,
                                                                           activeCamera.transform.forward * -1);
                    }
                }
            }
            //ELSE IF currentProxObj != null
            else if (currentProxHitObject != null)
            {
                if (doesReticleRotateOffNormal)
                {
                    reticleObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward,
                                                                         currentProxHit.normal);
                }
                else
                {
                    reticleObject.transform.rotation = Quaternion.LookRotation(-activeCamera.transform.forward,
                                                                           activeCamera.transform.forward * -1);
                }
            }
            else
            {
                reticleObject.transform.rotation = Quaternion.LookRotation(-activeCamera.transform.up,
                                                                           activeCamera.transform.forward * -1);
            }
        }
    }

    //DC 2019/04/08
    void UpdateProxReticleObjScale()
    {
        if (reticleObject != null)
        {
            if (isFloatingReticle)
            {
                //TODO: DOESN'T CORRECTLY WORK. POS and ROT work as intended but could change when scale is reworked.
                reticleObject.transform.localScale = new Vector3((reticleStartScale * reticleScale) * floatingReticleDistance,
                                                                 (reticleStartScale * reticleScale) * floatingReticleDistance,
                                                                 (reticleStartScale * reticleScale) * floatingReticleDistance);
            }
            //IF proxObj != null AND centreObj != null, scale based on centre distance
            else if (currentProxHitObject != null && currentCentreHitObject != null)
            {
                

                //Centre of camera is on an object.
                if (currentProxHitObject == currentCentreHitObject)
                {
                    reticleObject.transform.localScale = new Vector3((reticleStartScale * reticleScale)* currentCentreRayDistance,
                                                                 (reticleStartScale * reticleScale) * currentCentreRayDistance,
                                                                 (reticleStartScale * reticleScale) * currentCentreRayDistance);
                }
                //Prox is not on the same obj as centre obj.
                else
                {
                    float twoPointDistance = Vector3.Distance(currentProxHit.point, currentCentreRay.GetPoint(currentProxRayDistance + proxCastRadius));
                    if (twoPointDistance >= proxCastRadius)
                    {
                        twoPointDistance = proxCastRadius;
                    }

                    float proxScale = 1 - (twoPointDistance / proxCastRadius);

                    reticleObject.transform.localScale = new Vector3(((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                                                                 ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                                                                 ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale);
                }
            }
            //ELSE IF currentProxObj != null
            else if (currentProxHitObject != null)
            {
                float twoPointDistance = Vector3.Distance(currentProxHit.point, currentCentreRay.GetPoint(currentProxRayDistance + proxCastRadius));
                if (twoPointDistance >= proxCastRadius)
                {
                    twoPointDistance = proxCastRadius;
                }

                float proxScale = 1 - (twoPointDistance / proxCastRadius);

                reticleObject.transform.localScale = new Vector3(((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                                                                 ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                                                                 ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale);
            }
            else
            {
                reticleObject.transform.localScale = new Vector3((reticleStartScale * reticleScale) * floatingReticleDistance,
                                                                 (reticleStartScale * reticleScale) * floatingReticleDistance,
                                                                 (reticleStartScale * reticleScale) * floatingReticleDistance);
            }
        }
    }

    //DC 2019/03/28
    void UpdateProxReticleObjAlpha()
    {
        if(reticleObject != null)
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

    //DC 2019/03/25
    void ProxRaycastFromCamToMouseWorld()
    {
        RaycastHit hit;
        Ray ray;
        RaycastHit sphereHit;
        Ray sphereRay;

        ray = new Ray(activeCamera.transform.position, activeCamera.transform.forward);
        //Debug.DrawRay(activeCamera.transform.position, activeCamera.transform.forward, Color.red, 0.1f);
        sphereRay = new Ray(activeCamera.transform.position, activeCamera.transform.forward);


        float localRaycastDistance;
        if(maxProxCastDistance <= 0f)
        {
            localRaycastDistance = Mathf.Infinity;
        }
        else
        {
            localRaycastDistance = maxProxCastDistance;
        }

        //Sphere cast for proximity
        if (Physics.SphereCast(sphereRay, proxCastRadius, out sphereHit, localRaycastDistance))
        {
            currentProxRay = sphereRay;
            currentProxHit = sphereHit;
            currentProxRayDistance = sphereHit.distance;

            Debug.DrawRay(activeCamera.transform.position, -activeCamera.transform.position + sphereHit.point, Color.cyan, 0.5f);

            currentProxHitObject = sphereHit.transform.gameObject;

            GameObject tempProxObject = currentProxHitObject;

            if(lastProxHitObject == null)
            {
                lastProxHitObject = currentProxHitObject;
            }
            else if (currentProxHitObject != tempProxObject)
            {
                lastProxHitObject = tempProxObject;
            }
            
            //Centre cast of proximity cast.
            if (Physics.Raycast(ray, out hit, localRaycastDistance+proxCastRadius))
            {
                currentCentreRay = ray;
                currentCentreHit = hit;
                currentCentreRayDistance = hit.distance;

                Debug.DrawRay(activeCamera.transform.position, -activeCamera.transform.position + hit.point, Color.magenta, 0.5f);

                currentCentreHitObject = hit.transform.gameObject;

                GameObject tempCentreObject = currentCentreHitObject;

                if (lastCentreHitObject == null)
                {
                    lastCentreHitObject = currentCentreHitObject;
                }
                else if (currentCentreHitObject != tempCentreObject)
                {
                    lastCentreHitObject = tempCentreObject;
                }
            }
            else
            {
                if (currentCentreHitObject != null)
                {
                    lastCentreHitObject = currentCentreHitObject;
                }

                currentCentreHitObject = null;

                //Disable/Hides reticle mesh/material.
                if (doesReticleHideOnNoHit)
                {
                    reticleObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        else
        {
            if (currentProxHitObject != null)
            {
                lastProxHitObject = currentProxHitObject;
            }

            currentProxHitObject = null;

            //Disable/Hides reticle mesh/material.
            if (doesReticleHideOnNoHit)
            {
                reticleObject.GetComponent<MeshRenderer>().enabled = false;
            }
            
        }
    }

    void UpdateMousePositions()
    {
        mouseCurrentScreenPos = Input.mousePosition;
    }
}
