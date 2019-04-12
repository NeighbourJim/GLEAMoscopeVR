using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MM.GLEAMoscopeVR.POIs;

/// <summary>
/// Script that manages the data displayed on the InfoPanel as well as its location in worldspace.
/// </summary>
public class InfoPanel_Manager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Title text object.")]
    TextMeshProUGUI TitleText;
    [SerializeField]
    [Tooltip("Distance text object.")]
    TextMeshProUGUI DistanceText;
    [SerializeField]
    [Tooltip("Description text object.")]
    TextMeshProUGUI DescriptionText;
    [SerializeField]
    [Tooltip("Point of Interest Image object.")]
    Image PointImage;

    /// <summary>
    /// Update the panel's display of information with a new point of interest.
    /// Takes a PointOfInterest object, and utilises the Name, Distance, Description and Sprite parameters.
    /// </summary>
    /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
    void UpdateDisplay(PointOfInterest point)
    {
        TitleText.text = point.Name;
        DistanceText.text = string.Format("Distance: {0}", point.Distance);
        DescriptionText.text = point.Description;
        PointImage.sprite = point.Sprite;
    }

    /// <summary>
    /// Sets the location of the tooltip in worldspace, offset from a transform.
    /// </summary>
    /// <param name="targetTransform">The transform of the object that the info panel will be offset from. This should generally be a Point Of Interest object.</param>
    /// <param name="xOffset">The offset value on the X-axis.</param>
    /// <param name="yOffset">The offset value on the Y-axis.</param>
    /// <param name="zOffset">The offset value on the Z-axis.</param>
    void SetLocation(Transform targetTransform, float xOffset, float yOffset, float zOffset)
    {
        Vector3 targetPos = targetTransform.position;
        gameObject.transform.position = new Vector3(targetPos.x + xOffset, targetPos.y + yOffset, targetPos.z + zOffset);
    }

    /// <summary>
    /// Creates a tooltip. Calls the UpdateDisplay() and SetLocation() methods.
    /// </summary>
    /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
    /// <param name="targetTransform">The transform of the object that the info panel will be offset from. This should generally be a Point Of Interest object.</param>
    /// <param name="xOffset">The offset value on the X-axis.</param>
    /// <param name="yOffset">The offset value on the Y-axis.</param>
    /// <param name="zOffset">The offset value on the Z-axis.</param>
    public void CreateToolTip(PointOfInterest point, Transform targetTransform, float xOffset, float yOffset, float zOffset)
    {
        UpdateDisplay(point);
        SetLocation(targetTransform, xOffset, yOffset, zOffset);
    }
}
