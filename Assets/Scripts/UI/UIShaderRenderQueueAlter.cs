using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShaderRenderQueueAlter : MonoBehaviour
{
    //DC 2019/04/15
    //Script to allow UI shaders to be overriden.
    //Does not allow shader values/parameters to be changed from the script. Supplied shader's default values have to be changed.

    public Shader shaderMat;
    // Start is called before the first frame update
    private void Start()
    {
        if(shaderMat != null)
        {
            Canvas.GetDefaultCanvasMaterial().shader = shaderMat;
        }
    }
}
