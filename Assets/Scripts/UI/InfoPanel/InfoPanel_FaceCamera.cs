using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLEAMoscopeVR.RaycastingSystem;

public class InfoPanel_FaceCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The gameobject to look at. If not assigned, will default to the Main Camera in the scene.")]
    GameObject ToLookAt;
    [SerializeField]
    [Tooltip("Whether the panel should face the camera. Toggle this programmatically with SetLookAtCamera()")]
    bool LookAtCamera = true;

    public bool SlerpRotate = false;
    public float SlerpRotateSpeed = 0.5f;

    public float tetherRange = 0.5f;

    Vector3 previousPos = new Vector3();

    /// <summary>
    /// On Awake, check to see if ToLookAt has been assigned in the editor. 
    /// If it hasn't default to the scene's main camera.
    /// </summary>
    private void Awake()
    {
        if (ToLookAt == null)
        {
            ToLookAt = Camera.main.gameObject;
        }
    }

    /// <summary>
    /// Every frame, face the panel toward the ToLookAt gameobject.
    /// This method by default faces an object AWAY from ToLookAt because of oddities with Canvas transforms.
    /// </summary>
    void LateUpdate()
    {
        if (LookAtCamera)
        {
            if(transform.position != previousPos)
            {
                previousPos = transform.position;
                transform.LookAt(2 * transform.position - ToLookAt.transform.position, ToLookAt.transform.up);
            }
            if (SlerpRotate)
            {
                if (!CheckIsReticleHitting())
                {
                    Quaternion q = Quaternion.LookRotation(2 * transform.position - ToLookAt.transform.position, ToLookAt.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, SlerpRotateSpeed * Time.deltaTime);
                }
            }
            else
            {
                transform.LookAt(2 * transform.position - ToLookAt.transform.position, ToLookAt.transform.up);
            }
        }
    }

    /// <summary>
    /// Check if the reticle is currently hitting the panel or any of its child objects.
    /// </summary>
    /// <returns></returns>
    public bool CheckIsReticleHitting()
    {
        if(ToLookAt.GetComponent<Script_CameraRayCaster>() != null)
        {
            if(ToLookAt.GetComponent<Script_CameraRayCaster>().GetCurrentCentreHit() == gameObject)
            {
                return true;
            }
            foreach (Transform child in transform)
            {
                if (ToLookAt.GetComponent<Script_CameraRayCaster>().GetCurrentCentreHit() == child.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool GetLookAtCamera()
    {
        return LookAtCamera;
    }

    public void SetLookAtCamera(bool value)
    {
        LookAtCamera = value;
    }
}
