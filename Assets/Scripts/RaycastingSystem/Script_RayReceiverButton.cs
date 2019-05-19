using UnityEngine;
using UnityEngine.UI;

namespace GLEAMoscopeVR.RaycastingSystem
{
    public class Script_RayReceiverButton : MonoBehaviour, IRayClickable
    {
        [SerializeField]
        protected float activationTime = 1f;

        float IRayClickable.GetActivationTime()
        {
            return activationTime;
        }

        void IRayClickable.Click()
        {
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.Invoke();
            }
        }
    }
}