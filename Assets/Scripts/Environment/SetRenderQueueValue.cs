using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderQueueValue : MonoBehaviour
{
    public int RenderQueueValue = 2000;
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.renderQueue = RenderQueueValue;
    }
}
