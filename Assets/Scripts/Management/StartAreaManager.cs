using GLEAMoscopeVR.Cameras;
using GLEAMoscopeVR.Events;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.Events;

namespace GLEAMoscopeVR.Menu
{
    public class StartAreaManager : MonoBehaviour
    {
        public CameraBlinkCanvas CameraBlink = null;
        //public CameraBlink CameraBlink = null;
        public GameObject CameraParent = null;
        public Renderer StartAreaCubeRenderer = null;

        public CanvasGroup MainMenuCanvas = null;
        public CanvasGroup SettingsCanvas = null;
        public CanvasGroup CreditsCanvas = null;
        public CanvasGroup AcknowledgementsCanvas = null;

        [Space] public UnityEvent StartFinished;

        #region Unity Methods

        void Awake()
        {
            SetAndCheckReferences();
            CameraBlink.EyeClosed.AddListener(TeleportToScene);
            ShowNoCanvas();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<VideoClipEndedEvent>(_ => ShowMainCanvas());
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<VideoClipEndedEvent>(_ => ShowMainCanvas());
        }

        #endregion

        #region Canvas Hide/Show
        public void ShowNoCanvas()
        {
            ShowMenuCanvasGroup(MainMenuCanvas, false);
            ShowMenuCanvasGroup(SettingsCanvas, false);
            ShowMenuCanvasGroup(CreditsCanvas, false);
            AcknowledgementsCanvas.alpha = 0;
        }

        public void ShowMainCanvas()
        {
            ShowMenuCanvasGroup(MainMenuCanvas, true);
            ShowMenuCanvasGroup(SettingsCanvas, false);
            ShowMenuCanvasGroup(CreditsCanvas, false);
            AcknowledgementsCanvas.alpha = 0;
        }

        public void ShowSettingsCanvas()
        {
            ShowMenuCanvasGroup(MainMenuCanvas, false);
            ShowMenuCanvasGroup(SettingsCanvas, true);
            ShowMenuCanvasGroup(CreditsCanvas, false);
            AcknowledgementsCanvas.alpha = 0;
        }

        public void ShowCreditsCanvas()
        {
            ShowMenuCanvasGroup(MainMenuCanvas, false);
            ShowMenuCanvasGroup(SettingsCanvas, false);
            ShowMenuCanvasGroup(CreditsCanvas, true);
            AcknowledgementsCanvas.alpha = 0;
        }

        public void ShowAcknowledgeCanvas()
        {
            ShowMenuCanvasGroup(MainMenuCanvas, false);
            ShowMenuCanvasGroup(SettingsCanvas, false);
            ShowMenuCanvasGroup(CreditsCanvas, false);
            AcknowledgementsCanvas.alpha = 1;
        }

        private void ShowMenuCanvasGroup(CanvasGroup cg, bool show)
        {
            foreach (Transform child in cg.gameObject.transform)
            {
                child.gameObject.SetActive(show);
            }
        }

        #endregion

        #region Teleportation
        public void StartTeleport()
        {
            CameraBlink.Blink();
        }

        public void TeleportToScene()
        {
            CameraParent.transform.position = new Vector3(0, CameraParent.transform.position.y, CameraParent.transform.position.z);

            if (StartAreaCubeRenderer != null)
            {
                StartAreaCubeRenderer.enabled = false;
            }

            StartFinished.Invoke();
            CameraBlink.EyeClosed.RemoveListener(TeleportToScene);

            EventManager.Instance.Raise(new TeleportToSceneEvent("Teleported to scene."));
        }

        #endregion

        private void SetAndCheckReferences()
        {
            if (CameraBlink == null)
            {
                CameraBlink = Camera.main.GetComponent<CameraBlinkCanvas>();
                //CameraBlink = Camera.main.GetComponent<CameraBlink>();
            }
            Assert.IsNotNull(CameraBlink, $"<b>[{GetType().Name}]</b> Camera blink is not assigned and cannot be found in camera children.");

            Assert.IsNotNull(CameraParent, $"<b>[{GetType().Name}]</b> Main camera parent is not assigned.");
            Assert.IsNotNull(StartAreaCubeRenderer, $"<b>[{GetType().Name}]</b> Start area cube renderer is not assigned.");

            Assert.IsNotNull(MainMenuCanvas, $"<b>[{GetType().Name}]</b> Main menu canvas is not assigned.");
            Assert.IsNotNull(SettingsCanvas, $"<b>[{GetType().Name}]</b> Settings canvas is not assigned.");
            Assert.IsNotNull(CreditsCanvas, $"<b>[{GetType().Name}]</b> Credits canvas is not assigned.");
            Assert.IsNotNull(AcknowledgementsCanvas, $"<b>[{GetType().Name}]</b> Acknowledgements canvas is not assigned.");
        }
    }
}