using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_RayReceiverButton : MonoBehaviour, IRayClickable, IRayInteractable
{
    void IRayClickable.Click()
    {
        //Object script is attached to contains a Button component.
        //Invoke click event.
        //This is to be REWORKED and TALKED ABOUT on how we want to handle firing events.
        //Recommend using unity events or ?c# delegates?
        if(gameObject.GetComponent<Button>() != null)
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
            print("IRayClickable.Click() was invoke correctly on - " +gameObject.name);
        }
    }

    // MM 12/04/19 - added temporarily for interaction with iteration 1 UI
    void IRayInteractable.Activate()
    {
        if (gameObject.GetComponent<Button>() != null)
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
            print("IRayClickable.Click() was invoke correctly on - " + gameObject.name);
        }
        else
        {
            print("Tested");
        }
    }
}
