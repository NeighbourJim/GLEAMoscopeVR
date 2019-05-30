﻿using System;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

// Todo: paired nodes, material change-out, OnPOIDeactivated.

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Base class for Point of Interest nodes.
    /// <see cref="POIMapNode"/> <see cref="POISkyNode"/> <see cref="POIAntennaNode"/>
    /// </summary>
    public abstract class POINode : MonoBehaviour, IActivatable
    {
        /// <summary> Point of Interest ScriptableObject asset file. </summary>
        [Header("Data"), SerializeField]
        protected POIData data;

        [Header("IActivatable")]
        [SerializeField]
        protected float activationTime = 2f;

        /// <summary> Specifies whether the node is currently activated. </summary>
        [SerializeField]
        protected bool isActivated = false;

        #region Events
        public abstract event Action<POINode> OnPOINodeActivated;
        #endregion

        #region Public Accessors
        public abstract POIData Data { get; }
        public abstract ExperienceMode ActivatableMode { get; }
        public abstract float ActivationTime { get; }
        public abstract bool IsActivated { get; }
        #endregion
        
        #region References
        protected ExperienceModeController _modeController;
        #endregion

        #region Unity Methods
        protected virtual void Start()
        {
            SetAndCheckReferences();
        }
        #endregion

        #region IActivatable
        public abstract bool CanActivate();
        public abstract void Activate();
        public abstract void Deactivate();
        #endregion

        protected virtual void SetAndCheckReferences()
        {
            _modeController = FindObjectOfType<ExperienceModeController>().Instance;
            Assert.IsNotNull(_modeController, $"{gameObject.name} cannot find ExperienceModeController.");
            Assert.IsNotNull(data, $"{gameObject.name} has no POIData assigned.");
        }
    }
}