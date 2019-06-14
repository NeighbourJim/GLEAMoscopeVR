using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLEAMoscopeVR.SubtitleSystem
{
    public class Subtitle : MonoBehaviour
    {
        [SerializeField]
        SubtitleDisplayer _subtitleDisplayer;
        [SerializeField]
        public float startDelay = 0;
        [SerializeField]
        [TextArea]
        public List<string> subtitle;
        [SerializeField]
        public List<float> delayLengthF;
        [SerializeField]
        public List<float> delayLengthM;
        
        

        public Subtitle(List<string> sub, List<float> delayF, List<float> delayM, float sDelay)
        {
            subtitle = sub;
            delayLengthF = delayF;
            delayLengthM = delayM;
            startDelay = sDelay;
        }

        public void SendSubtitle()
        {
            _subtitleDisplayer.SetSubQueue(this);
        }
    }
}

