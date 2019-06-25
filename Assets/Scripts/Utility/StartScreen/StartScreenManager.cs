using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartScreenManager : MonoBehaviour
{
    public CameraBlink cameraBlink = null;
    public GameObject main_camera_parent = null;
    public GameObject startArea = null;

    public CanvasGroup StartCanvas = null;
    public CanvasGroup SettingsCanvas = null;
    public CanvasGroup CreditsCanvas = null;

    public UnityEvent startFinished;

    #region Unity Methods
    public void Awake()
    {
        cameraBlink.EyeClosed.AddListener(TeleportToScene);
        ShowNoCanvas();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            ShowMainCanvas();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowNoCanvas();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShowSettingsCanvas();
        }
    }
    #endregion

    #region Canvas Hide/Show
    public void ShowNoCanvas()
    {
        Debug.Log("Show no canvas.");
        ShowMenuCanvasGroup(StartCanvas, false);
        ShowMenuCanvasGroup(SettingsCanvas, false);
        ShowMenuCanvasGroup(CreditsCanvas, false);
    }

    public void ShowMainCanvas()
    {
        Debug.Log("Show main canvas.");
        ShowMenuCanvasGroup(StartCanvas, true);
        ShowMenuCanvasGroup(SettingsCanvas, false);
        ShowMenuCanvasGroup(CreditsCanvas, false);
    }

    public void ShowSettingsCanvas()
    {
        Debug.Log("Show settings canvas.");
        ShowMenuCanvasGroup(StartCanvas, false);
        ShowMenuCanvasGroup(SettingsCanvas, true);
        ShowMenuCanvasGroup(CreditsCanvas, false);
    }

    public void ShowCreditsCanvas()
    {
        ShowMenuCanvasGroup(StartCanvas, false);
        ShowMenuCanvasGroup(SettingsCanvas, false);
        ShowMenuCanvasGroup(CreditsCanvas, true);
    }

    private void ShowMenuCanvasGroup(CanvasGroup cg, bool show)
    {
        foreach(Transform child in cg.gameObject.transform)
        {
            child.gameObject.SetActive(show);
        }
        //cg.alpha = show ? 1 : 0;
        //cg.interactable = show;
        //cg.blocksRaycasts = show;
    }
    #endregion

    #region Teleportation
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
    #endregion

    #region Settings Loading

    //TODO: Wire Up settings loading

    #endregion
}
