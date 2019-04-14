using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRayInteractable
{
    //DC 2019/04/08
    //Generic interface method that activates
    //once the camera's coroutine hover countdown completes
    void Activate();
}
