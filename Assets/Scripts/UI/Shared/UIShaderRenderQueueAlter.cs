using UnityEngine;

namespace GLEAMoscopeVR.UI
{
    /// <summary>
    /// DC 2019/04/15
    /// Script to allow UI shaders to be overriden.
    /// Does not allow shader values/parameters to be changed from the script. Supplied shader's default values have to be changed.
    /// </summary>
    public class UIShaderRenderQueueAlter : MonoBehaviour
    {
        public Shader shaderMat;

        private void Start()
        {
            if (shaderMat != null)
            {
                Canvas.GetDefaultCanvasMaterial().shader = shaderMat;
            }
        }
    }
}