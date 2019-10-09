using System.Linq;
using GLEAMoscopeVR.Cameras;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Utility.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Environment
{
    public class EnvironmentAssetController : MonoBehaviour
    {
        [Header("Exploration Mode")]
        public GameObject ExplorationAssetsParent;

        [Header("Passive Mode")]
        public GameObject PassiveAssetsParent;
        
        [Space] public CameraBlinkCanvas CameraBlink;

        [Header("Debugging")]
        [SerializeField] private Renderer[] explorationAssetRenderers;
        [SerializeField] private Renderer[] passiveAssetRenderers;

        void Start()
        {
            SetAndCheckReferences();
            PrepareAssets();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<SunsetFadeInEvent>(_ => SetEnvironmentAssetRenderState(SettingsManager.Instance.CurrentExperienceMode));
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(e => SetEnvironmentAssetRenderState(e.ExperienceMode));
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<SunsetFadeInEvent>(_ => SetEnvironmentAssetRenderState(SettingsManager.Instance.CurrentExperienceMode));
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(e => SetEnvironmentAssetRenderState(e.ExperienceMode));
        }

        private void PrepareAssets()
        {
            GetExplorationAssetRenderers();
            GetPassiveAssetRenderers();
            SetInitialEnvironmentAssetRenderState();
        }

        
        private void GetExplorationAssetRenderers()
        {
            explorationAssetRenderers = ExplorationAssetsParent.GetComponentsInChildren<Renderer>();
            Assert.IsFalse(explorationAssetRenderers.IsNullOrEmpty(), $"<b>[{GetType().Name}]</b> Exploration asset renderers array is null or empty.");
        }

        private void GetPassiveAssetRenderers()
        {
            passiveAssetRenderers = PassiveAssetsParent.GetComponentsInChildren<Renderer>();
            Assert.IsFalse(passiveAssetRenderers.IsNullOrEmpty(), $"<b>[{GetType().Name}]</b> Passive asset renderers array is null or empty.");
        }

        private void SetInitialEnvironmentAssetRenderState()
        {
            SetExplorationAssetRenderState(true);
            SetPassiveModeAssetRenderState(false);
        }
        
        public void SetEnvironmentAssetRenderState(ExperienceMode mode)
        {
            switch (mode)
            {
                case ExperienceMode.Introduction:
                case ExperienceMode.Exploration:
                    SetExplorationAssetRenderState(true);
                    SetPassiveModeAssetRenderState(false);
                    break;
                case ExperienceMode.Passive:
                    SetExplorationAssetRenderState(false);
                    SetPassiveModeAssetRenderState(true);
                    break;
                default:
                    break;
            }
        }

        private void SetExplorationAssetRenderState(bool render) 
            => explorationAssetRenderers.ToList().ForEach(r => r.enabled = render);


        private void SetPassiveModeAssetRenderState(bool render) 
            => passiveAssetRenderers.ToList().ForEach(r => r.enabled = render);
        
        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(ExplorationAssetsParent, $"<b>[{GetType().Name}]</b> Exploration assets parent not assigned.");
            Assert.IsNotNull(PassiveAssetsParent, $"<b>[{GetType().Name}]</b> Passive assets parent not assigned.");

            if (CameraBlink == null)
            {
                CameraBlink = Camera.main.GetComponentInChildren<CameraBlinkCanvas>();
            }
            Assert.IsNotNull(CameraBlink, $"<b>[{GetType().Name}]</b> Camera blink is not assigned and not found in main camera children.");
        }
    }
}