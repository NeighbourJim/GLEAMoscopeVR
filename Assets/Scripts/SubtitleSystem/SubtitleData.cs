using UnityEngine;

namespace GLEAMoscopeVR.SubtitleSystem
{
    [CreateAssetMenu(menuName = "GLEAMoscopeVR/Subtitle Data", fileName = "New Subtitle Data")]
    public class SubtitleData : ScriptableObject
    {
        public float startDelay = 0;
        [TextArea]
        public string[] subtitle;
        public float[] delayLengthF;
        public float[] delayLengthM;
    }
}