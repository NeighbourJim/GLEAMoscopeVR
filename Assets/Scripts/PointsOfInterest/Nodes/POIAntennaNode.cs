﻿using System;
using System.Collections;
using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Specifies behaviour for Antenna Point of Interest made available in the introduction sequence (<see cref="ExperienceMode.Introduction"/>).
    /// </summary>
    public class POIAntennaNode : POINode
    {
        #region Constants
        private const ExperienceMode activatableMode = ExperienceMode.Introduction;
        private string rotateAnimation = "Rotate";
        private string idleAnimation = "SetIdle";
        #endregion

        [Header("Debugging"), SerializeField]
        private bool activatable = false;

        #region Public Accessors
        public override ExperienceMode ActivatableMode => activatableMode;
        public override bool IsActivated => isActivated;
        public override POIData Data => data;
        public override float ActivationTime => activationTime;
        #endregion

        #region Events
        public override event Action<POINode> OnPOINodeActivated;
        #endregion

        #region References
        Animator _animator;
        Renderer _nodeRenderer;
        SunsetController _sunsetController;
        VoiceOverController _voiceController;
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();
            Deactivate();
        }

        #endregion

        public void SetActivatable()
        {
            activatable = true;
            _nodeRenderer.enabled = true;
            _animator.enabled = true;
            _animator.SetTrigger(rotateAnimation);
        }

        #region IActivatable Implentation

        /// <summary> Specifies whether this GameObject can be activated. </summary>
        public override bool CanActivate()
        {
            return activatableMode == _modeController.CurrentMode && !isActivated && activatable;
        }

        /// <summary>
        /// Invoked by the <see cref="Script_CameraRayCaster"/> when the reticle loading process is complete.
        /// Notifies the <see cref="POIManager"/> that it has been activated.
        /// </summary>
        public override void Activate()
        {
            if (!CanActivate())
            {
                Debug.LogWarning($"[POIAntennaNode] {gameObject.name} is trying to activate when it shouldn't be able to.");
                Debug.Break();
                return;
            }

            isActivated = true;
            _animator.SetTrigger(idleAnimation);
            OnPOINodeActivated?.Invoke(this);
            StartCoroutine(WaitUntilSunsetComplete());
        }

        public override void Deactivate()
        {
            activatable = false;
            isActivated = false;
            _nodeRenderer.enabled = false;
            //_animator.SetTrigger(idleAnimation);
        }

        #endregion

        IEnumerator WaitUntilSunsetComplete()
        {
            yield return new WaitUntil(() => _sunsetController.SunsetCompleted);
            Deactivate();
        }

        #region Debugging

        protected override void SetAndCheckReferences()
        {
            base.SetAndCheckReferences();
            _animator = GetComponentInChildren<Animator>();
            _nodeRenderer = GetComponentInChildren<Renderer>();
            _sunsetController = FindObjectOfType<SunsetController>();

            Assert.IsNotNull(_animator, $"[POIAntennaNode] {gameObject.name} can't find Animator component not found in children.");
            Assert.IsNotNull(_nodeRenderer, $"[POIAntennaNode] {gameObject.name} can't find Mesh Renderer component not found in children.");
            Assert.IsNotNull(_sunsetController, $"[POIAntennaNode] {gameObject.name} can't find SunsetController in scene.");

            VoiceOverController.Instance.OnGreetingComplete += SetActivatable;
        }
        #endregion
    }
}