using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanel_FaceCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The gameobject to look at. If not assigned, will default to the Main Camera in the scene.")]
    GameObject ToLookAt;
    [SerializeField]
    [Tooltip("Whether the panel should face the camera. Toggle this programmatically with SetLookAtCamera()")]
    bool LookAtCamera = false;

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
            transform.LookAt(2 * transform.position - ToLookAt.transform.position);
        }
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
