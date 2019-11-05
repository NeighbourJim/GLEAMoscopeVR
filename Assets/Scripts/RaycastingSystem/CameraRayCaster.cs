using UnityEngine;

namespace GLEAMoscopeVR.Interaction
{
    //2019/05/17
    //TODO: Seperate Animation from Activate. Read through IActivatable script for logic. Use triggers from any state to possibly remove issues/clutter with boolean logic.
    //TODO: CheckingInteractable() should NOT handle activated. Extract method and refactor.
    //TODO: Move time to activate from current script to IActivatable so it do can contextual to the object.
    //TODO: Correctly implement proximity reticle transform in update methods.
    public class CameraRayCaster : MonoBehaviour
    {
        public Camera activeCamera;

        [Header("Mouse Pos Info")]
        [SerializeField] Vector3 mouseCurrentWorldPos = new Vector3(0, 0, 0);
        [SerializeField] Vector3 mouseCurrentScreenPos = new Vector3(0, 0, 0);

        [Header("Reticle Options")]
        public bool isFloatingReticle = false;
        public bool doesReticleHideOnNoHit = false;
        public bool doesReticleFloatOnNoHit = true;
        public bool doesReticleRotateOffNormal = true;
        public float floatingReticleDistance = 1f;
        public GameObject reticleObject;
        [Range(0f, 1f)] public float reticleScale = 0.01f;
        [Range(0.001f, 1f)] public float reticleStartScale = 1f;

        [Header("Reticle Alpha/Shader Options.")]
        [Range(0, 255)]
        public int reticleAlpha = 255;
        public int reticleStartingAlpha;

        [Header("Proximity (Spherecast) Scaling Options/Info")]
        [Tooltip("Set to <=0 for infinite raycast distance.")]
        public float maxProxCastDistance = 0f;
        public float proxCastRadius = 0.5f;

        //Proximity Sphere Raycast Info/Vars
        Ray currentProxRay;
        RaycastHit currentProxHit;
        [SerializeField] float currentProxRayDistance;
        [SerializeField] GameObject currentProxHitObject;
        [SerializeField] GameObject lastProxHitObject;

        //Proximity Centre Raycast Info/Vars
        Ray currentCentreRay;
        RaycastHit currentCentreHit;
        [SerializeField] float currentCentreRayDistance;
        [SerializeField] GameObject currentCentreHitObject;
        [SerializeField] GameObject lastCentreHitObject;

        [Header("Animator Controller States/Timer Info")]
        [SerializeField] GameObject lastActivatedObject;
        [SerializeField] float currentLoadingTimer = 2f;
        [SerializeField] int currentAnimationState = 0;
        enum AnimationStates
        {
            Default,
            Loading,
            Activated
        }

        void Start()
        {
            SetStartingFields();
            //Camera not set? Use main camera.
            if (activeCamera == null)
            {
                activeCamera = Camera.main;
            }
        }

        void SetStartingFields()
        {
            reticleStartingAlpha = reticleAlpha;
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

            CheckCurrentCentreHitObjectInterfaces();
        }

        //DC 2019/05/17
        //Checks for interfaces and passes component to appropriate method.
        //Use's component's timeToActivate field and passes to scale reticle activation state
        private void CheckCurrentCentreHitObjectInterfaces()
        {
            if (currentCentreHitObject != null)
            {
                IActivatable activatable = currentCentreHitObject.GetComponent<IActivatable>();

                if (activatable != null && activatable.CanActivate()
                    && currentAnimationState != (int)AnimationStates.Activated
                    && lastActivatedObject == null)
                {
                    HandleActivatableObjectTimer(activatable);
                }
                else if (currentAnimationState == (int)AnimationStates.Activated && lastActivatedObject != currentCentreHitObject)
                {
                    SetDefaultReticleState();
                }
                else if (currentAnimationState == (int)AnimationStates.Activated)
                {
                    //Meant to be empty
                }
                else
                {
                    SetDefaultReticleState();
                }
            }
            else
            {
                SetDefaultReticleState();
            }
        }

        private void SetDefaultReticleState()
        {
            lastActivatedObject = null;
            currentAnimationState = (int)AnimationStates.Default;
            UpdateCurrentReticleAnimationState(2f);
        }

        //DC 2019/05/19
        //Takes a object's interface component and handles relevent interface methods.
        //Use's component's timeToActivate field and passes to scale reticle activation state
        private void HandleActivatableObjectTimer(IActivatable activatableComponent)
        {
            float localActivationTime = activatableComponent.ActivationTime;

            if (currentAnimationState == (int)AnimationStates.Default)
            {
                UpdateCurrentReticleAnimationState(2f);
                currentAnimationState = (int)AnimationStates.Loading;
                currentLoadingTimer = activatableComponent.ActivationTime;
            }
            else if (currentAnimationState == (int)AnimationStates.Loading)
            {
                if (currentLoadingTimer <= 0f)
                {
                    activatableComponent.Activate();
                    lastActivatedObject = currentCentreHitObject;
                    currentAnimationState = (int)AnimationStates.Activated;
                }
                else
                {
                    currentLoadingTimer -= Time.deltaTime;
                }
                UpdateCurrentReticleAnimationState(localActivationTime);
            }
            else if (currentAnimationState == (int)AnimationStates.Activated)
            {
                currentAnimationState = (int)AnimationStates.Activated;
                currentLoadingTimer = activatableComponent.ActivationTime;
                UpdateCurrentReticleAnimationState(localActivationTime);
            }
        }



        //DC 2019/05/19
        //Handles animatorController state after logic has been handled on update.
        private void UpdateCurrentReticleAnimationState(float componentActivationTime)
        {
            if (reticleObject.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                Animator reticleAnimator = reticleObject.GetComponent<Animator>();

                if (currentAnimationState == (int)AnimationStates.Default)
                {
                    reticleAnimator.SetBool("IsDefault", true);
                    reticleAnimator.SetBool("IsLoading", false);
                    reticleAnimator.SetBool("HasActivated", false);
                }
                else if (currentAnimationState == (int)AnimationStates.Loading)
                {
                    SetLoadingAnimationMult(componentActivationTime);
                    reticleAnimator.SetBool("IsDefault", false);
                    reticleAnimator.SetBool("IsLoading", true);
                    reticleAnimator.SetBool("HasActivated", false);
                }
                else if (currentAnimationState == (int)AnimationStates.Activated)
                {
                    reticleAnimator.SetBool("IsDefault", true);
                    reticleAnimator.SetBool("IsLoading", false);
                    reticleAnimator.SetBool("HasActivated", true);
                }
            }
        }

        //DC 2019/05/19
        //Calculates the speed mult by dividing the time (seconds) it takes to execute a full loading animation.
        //(240frames/60fps)(2seconds) 
        private void SetLoadingAnimationMult(float activationTime)
        {
            if (reticleObject.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                Animator reticleRefAnimator = reticleObject.GetComponent<Animator>();
                float activationMult = 2 / activationTime;
                reticleRefAnimator.SetFloat("ActivationTime", activationMult);
            }
        }

        //DC 2019/03/28
        private void UpdateProxReticleObjPosition()
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
                            currentProxRayDistance + proxCastRadius);
                    }
                }
                //ELSE IF currentProxObj != null
                else if (currentProxHitObject != null)
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

        //DC 2019/03/28
        private void UpdateProxReticleObjRotation()
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
                        if (doesReticleRotateOffNormal)
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
        private void UpdateProxReticleObjScale()
        {
            if (reticleObject != null)
            {
                if (isFloatingReticle)
                {
                    //TODO: DOESN'T CORRECTLY WORK. POS and ROT work as intended but could change when scale is reworked.
                    reticleObject.transform.localScale = new Vector3(
                        (reticleStartScale * reticleScale) * floatingReticleDistance,
                        (reticleStartScale * reticleScale) * floatingReticleDistance,
                        (reticleStartScale * reticleScale) * floatingReticleDistance);
                }
                //IF proxObj != null AND centreObj != null, scale based on centre distance
                else if (currentProxHitObject != null && currentCentreHitObject != null)
                {


                    //Centre of camera is on an object.
                    if (currentProxHitObject == currentCentreHitObject)
                    {
                        reticleObject.transform.localScale = new Vector3(
                            (reticleStartScale * reticleScale) * currentCentreRayDistance,
                            (reticleStartScale * reticleScale) * currentCentreRayDistance,
                            (reticleStartScale * reticleScale) * currentCentreRayDistance);
                    }
                    //Prox is not on the same obj as centre obj.
                    else
                    {
                        float twoPointDistance = Vector3.Distance(currentProxHit.point,
                            currentCentreRay.GetPoint(currentProxRayDistance + proxCastRadius));
                        if (twoPointDistance >= proxCastRadius)
                        {
                            twoPointDistance = proxCastRadius;
                        }

                        float proxScale = 1 - (twoPointDistance / proxCastRadius);

                        reticleObject.transform.localScale = new Vector3(
                            ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) *
                            proxScale,
                            ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) *
                            proxScale,
                            ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) *
                            proxScale);
                    }
                }
                //ELSE IF currentProxObj != null
                else if (currentProxHitObject != null)
                {
                    float twoPointDistance = Vector3.Distance(currentProxHit.point,
                        currentCentreRay.GetPoint(currentProxRayDistance + proxCastRadius));
                    if (twoPointDistance >= proxCastRadius)
                    {
                        twoPointDistance = proxCastRadius;
                    }

                    float proxScale = 1 - (twoPointDistance / proxCastRadius);

                    reticleObject.transform.localScale = new Vector3(
                        ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                        ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale,
                        ((reticleStartScale * reticleScale) * (currentProxRayDistance + proxCastRadius)) * proxScale);
                }
                else
                {
                    reticleObject.transform.localScale = new Vector3(
                        (reticleStartScale * reticleScale) * floatingReticleDistance,
                        (reticleStartScale * reticleScale) * floatingReticleDistance,
                        (reticleStartScale * reticleScale) * floatingReticleDistance);
                }
            }
        }

        //DC 2019/03/28
        private void UpdateProxReticleObjAlpha()
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

        private void UpdateMousePositions()
        {
            mouseCurrentScreenPos = Input.mousePosition;
        }

        //DC 2019/03/25
        private void ProxRaycastFromCamToMouseWorld()
        {
            RaycastHit hit;
            Ray ray;
            RaycastHit sphereHit;
            Ray sphereRay;

            ray = new Ray(activeCamera.transform.position, activeCamera.transform.forward);
            //Debug.DrawRay(activeCamera.transform.position, activeCamera.transform.forward, Color.red, 0.1f);
            sphereRay = new Ray(activeCamera.transform.position, activeCamera.transform.forward);


            float localRaycastDistance;
            if (maxProxCastDistance <= 0f)
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

                Debug.DrawRay(activeCamera.transform.position, -activeCamera.transform.position + sphereHit.point,
                    Color.cyan, 0.5f);

                currentProxHitObject = sphereHit.transform.gameObject;

                GameObject tempProxObject = currentProxHitObject;

                if (lastProxHitObject == null)
                {
                    lastProxHitObject = currentProxHitObject;
                }
                else if (currentProxHitObject != tempProxObject)
                {
                    lastProxHitObject = tempProxObject;
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

            //Centre cast of proximity cast.
            if (Physics.Raycast(ray, out hit, localRaycastDistance + proxCastRadius))
            {
                currentCentreRay = ray;
                currentCentreHit = hit;
                currentCentreRayDistance = hit.distance;

                Debug.DrawRay(activeCamera.transform.position, -activeCamera.transform.position + hit.point,
                    Color.magenta, 0.5f);

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

        #region Hit Object Getters - CP 11/06/2019
        public GameObject GetCurrentCentreHit()
        {
            return currentCentreHitObject;
        }

        public GameObject GetLastCentreHit()
        {
            return lastCentreHitObject;
        }

        public GameObject GetCurrentProximityHit()
        {
            return currentProxHitObject;
        }

        public GameObject GetLastProximityHit()
        {
            return lastProxHitObject;
        }
        #endregion
    }
}


/*
MM - 20/07/2019 - Removed:

//DC 2019/05/19
//Takes a object's interface component and handles relevent interface methods.
//Use's component's timeToActivate field and passes to scale reticle activation state
private void HandleClickableObjectTimer(IRayClickable clickableComponent)
{
float localActivationTime = clickableComponent.GetActivationTime();

    if (currentAnimationState == (int)AnimationStates.Default)
{
    UpdateCurrentReticleAnimationState(2f);
    currentAnimationState = (int)AnimationStates.Loading;
    currentLoadingTimer = clickableComponent.GetActivationTime();
}
else if (currentAnimationState == (int)AnimationStates.Loading)
{
if (currentLoadingTimer <= 0f)
{
clickableComponent.Click();
lastActivatedObject = currentCentreHitObject;
currentAnimationState = (int)AnimationStates.Activated;
}
else
{
currentLoadingTimer -= Time.deltaTime;
}
UpdateCurrentReticleAnimationState(localActivationTime);
}
else if (currentAnimationState == (int)AnimationStates.Activated)
{
currentAnimationState = (int)AnimationStates.Activated;
currentLoadingTimer = clickableComponent.GetActivationTime();
UpdateCurrentReticleAnimationState(localActivationTime);
}
}
*/
