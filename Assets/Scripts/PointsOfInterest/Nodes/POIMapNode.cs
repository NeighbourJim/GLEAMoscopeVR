using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace MM.GLEAMoscopeVR.POIs
{
    /// <summary>
    /// War-table map point of interest nodes.
    /// </summary>
    public class POIMapNode : POINode
    {
        /// <summary>
        /// Transform used to bring the Point of Interest to
        /// the user's original,  forward-facing position.
        /// </summary>
        public Transform POITransform;

        #region References
        TransformRotator _rotator;
        #endregion

        /// <summary>
        /// Invoked when a node is activated.
        /// </summary>
        public override event Action<PointOfInterest> OnNodeActivated;
        
        #region IRayInteractable Implementation

        public override void Activate()
        {
            if (_rotator.CanRotate())
            {
                _rotator.SetTargetTransformAndRotate(POITransform);
                OnNodeActivated?.Invoke(new PointOfInterest(poiData));
            }
        }
        
        #endregion

        protected override void GetComponentReferences()
        {
            base.GetComponentReferences();
            _rotator = FindObjectOfType<TransformRotator>();
            Assert.IsNotNull(_rotator, $"{gameObject.name} cannot find TransformRotator in scene.");
        }
    }
}