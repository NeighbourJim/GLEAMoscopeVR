using UnityEngine;

namespace GLEAMoscopeVR.Utility
{
    /// <summary>
    /// Generic implementation of the Singleton pattern.
    /// To use this implementation, create your C# script and inherit from this, instead of a MonoBehaviour
    /// Reference: http://www.unitygeek.com/unity_c_singleton/
    /// </summary>
    public class GenericSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance = null;

        /// <summary>
        /// If instance is null, try and find it.
        /// If not found, create it, name it and add it.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    //if (instance == null)
                    //{
                        
                    //    instance = new GameObject("_" + typeof(T).Name).AddComponent<T>();
                    //    DontDestroyOnLoad(instance);
                    //}
                }

                return instance;
            }
        }

        /// <summary>
        /// Override this method in the derived class.
        /// NOTE: You must call base.Awake() when overriding this method in the base class in order
        /// to ensure the implementation functions correctly. 
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}