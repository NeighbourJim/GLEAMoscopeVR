using UnityEngine;
using UnityEngine.UI;

namespace GLEAMoscopeVR.RaycastingSystem
{
    public class Script_RayReceiverButton : MonoBehaviour, IRayInteractable, IRayClickable
    {
        void IRayInteractable.Activate()
        {
            print("Tested");
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