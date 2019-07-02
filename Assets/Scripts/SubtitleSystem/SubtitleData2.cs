using System.Collections.Generic;
using UnityEngine;

namespace GLEAMoscopeVR.SubtitleSystem
{
    /// <summary>
    /// PURPOSE OF SCRIPT GOES HERE 
    /// </summary>
    [CreateAssetMenu(menuName = "GLEAMoscopeVR/SubtitleData2", fileName = "New SubtitleData2")]
    public class SubtitleData2 : ScriptableObject
    {
        public float startDelay = 0;
        [TextArea] public List<string> subtitle = new List<string>();
        public List<float> delayLengthF = new List<float>();
        public List<float> delayLengthM = new List<float>();
    }
}