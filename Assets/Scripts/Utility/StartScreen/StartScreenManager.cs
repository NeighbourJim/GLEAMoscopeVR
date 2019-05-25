using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartScreenManager : MonoBehaviour
{
    public CameraBlink cameraBlink = null;
    public GameObject main_camera_parent = null;
    public GameObject startArea = null;

    public UnityEvent startFinished;

    public void Awake()
    {
        cameraBlink.EyeClosed.AddListener(TeleportToScene);        
    }

    public void StartTeleport()
    {
        cameraBlink.Blink();
    }

    public void TeleportToScene()
    {
        main_camera_parent.transform.position = new Vector3(0, main_camera_parent.transform.position.y, main_camera_parent.transform.position.z);
        startArea.SetActive(false);
        
        startFinished.Invoke();

        cameraBlink.EyeClosed.RemoveListener(TeleportToScene);
    }


}
