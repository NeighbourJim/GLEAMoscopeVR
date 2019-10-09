using System.Collections.Generic;
using UnityEngine;

namespace GLEAMoscopeVR.SubtitleSystem
{
    [CreateAssetMenu(menuName = "GLEAMoscopeVR/Subtitle Data", fileName = "New Subtitle Data")]
    public class SubtitleData : ScriptableObject
    {
        public float startDelay = 0;

        [TextArea]
        public List<string> subtitle = new List<string>();

        public List<float> delayLengthF = new List<float>();
        public List<float> delayLengthM = new List<float>();
    }
}