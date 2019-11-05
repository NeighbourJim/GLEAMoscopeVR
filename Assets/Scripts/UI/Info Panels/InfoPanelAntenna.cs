using System;
using GLEAMoscopeVR.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// NOTE: This does NOT derive from InfoPanelBase and is used purely for the Antenna data display in the introduction sequence.
    /// </summary>
    public class InfoPanelAntenna : MonoBehaviour
    { 
        [Header("Text Components")]
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DescriptionText;

        [Header("Image Components")]
        public SpriteRenderer BackgroundSpriteRenderer;
        public Image BorderImage;
        public Image AntennaImage;
        public Sprite AntennaSprite;

        [Header("POI Data")]
        public POIData POIData;

        #region References
        CanvasGroup _canvasGroup;
        #endregion

        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();
            SetTextAndSprites();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<AntennaNodeStateChangedEvent>(HandleAntennaNodeStateChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<AntennaNodeStateChangedEvent>(HandleAntennaNodeStateChanged);
        }
        #endregion

        private void HandleAntennaNodeStateChanged(AntennaNodeStateChangedEvent e)
        {
            switch (e.NodeState)
            {
                case AntennaNodeStateChangedEvent.AntennaNodeState.Unavailable:
                case AntennaNodeStateChangedEvent.AntennaNodeState.Activatable:
                    break;
                case AntennaNodeStateChangedEvent.AntennaNodeState.Activated:
                    SetCanvasGroupAndColliderState(true);
                    break;
                case AntennaNodeStateChangedEvent.AntennaNodeState.Deactivated:
                    SetCanvasGroupAndColliderState(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetTextAndSprites()
        {
            TitleText.text = POIData.Name;
            DescriptionText.text = POIData.Description;
            AntennaImage.sprite = POIData.Sprites[0];
        }

        public void SetCanvasGroupAndColliderState(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            BackgroundSpriteRenderer.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} does not have a canvas group component.");
            
            Assert.IsNotNull(TitleText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Title text is not assigned.");
            Assert.IsNotNull(DescriptionText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Description text is not assigned.");

            Assert.IsNotNull(BackgroundSpriteRenderer, $"<b>[{GetType().Name} - {gameObject.name}]</b> Background sprite renderer not assigned.");
            Assert.IsNotNull(BorderImage, $"<b>[{GetType().Name} - {gameObject.name}]</b> POI image is not assigned.");
            Assert.IsNotNull(AntennaImage, $"<b>[{GetType().Name} - {gameObject.name}]</b> POI image is not assigned.");
            Assert.IsNotNull(AntennaSprite, $"<b>[{GetType().Name} - {gameObject.name}]</b> POI image is not assigned.");
        }
    }
}