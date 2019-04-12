using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace MM.GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Base class for Point of Interest nodes.
    /// <see cref="POIMapNode"/> <see cref="POISkyNode"/>
    /// </summary>
    public abstract class POINode : MonoBehaviour, IRayInteractable
    {
        [SerializeField]
        protected POIData poiData;
        
        public abstract event Action<PointOfInterest> OnNodeActivated;
        
        protected virtual void Start()
        {
            GetComponentReferences();
        }

        /// <summary>
        /// IRayInteractable implementation.
        /// </summary>
        public abstract void Activate();

        protected virtual void GetComponentReferences()
        {
            Assert.IsNotNull(poiData, $"{gameObject.name} has no POIData assigned.");
        }
    }
}