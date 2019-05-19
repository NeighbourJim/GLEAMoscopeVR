using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IRayClickable
{
    float GetActivationTime();
    void Click();
}
