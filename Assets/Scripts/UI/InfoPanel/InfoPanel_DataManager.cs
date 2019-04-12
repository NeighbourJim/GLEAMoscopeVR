using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MM.GLEAMoscopeVR.POIs;

/// <summary>
/// Script that manages the data displayed on the InfoPanel.
/// </summary>
public class InfoPanel_DataManager : MonoBehaviour
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
    public void UpdateDisplay(PointOfInterest point)
    {
        TitleText.text = point.Name;
        DistanceText.text = string.Format("Distance: {0}", point.Distance);
        DescriptionText.text = point.Description;
        PointImage.sprite = point.Sprite;
    }
}
