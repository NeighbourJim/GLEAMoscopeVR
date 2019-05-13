using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroManager : MonoBehaviour
{
    public CameraBlink cameraBlink = null;
    public Camera main_camera = null;
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
        main_camera.transform.position = new Vector3(0, 1.7f, 0);
        introArea.SetActive(false);
        
        introFinished.Invoke();

        cameraBlink.EyeClosed.RemoveListener(TeleportToScene);
    }


}
