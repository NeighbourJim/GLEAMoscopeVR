using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    public CameraBlink cameraBlink = null;
    public GameObject main_camera_parent = null;
    public GameObject introArea = null;

    public UnityEvent introFinished;

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
        introArea.SetActive(false);
        
        introFinished.Invoke();

        cameraBlink.EyeClosed.RemoveListener(TeleportToScene);
    }


}
