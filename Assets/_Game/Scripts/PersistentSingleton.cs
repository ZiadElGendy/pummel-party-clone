using UnityEngine;

namespace PummelPartyClone
{
    public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // A Single instance to reference the singlton
        private static T instance;

        /// <summary>
        /// A Setter & Getter for the instance to be accessible by
        /// all other classes.
        /// </summary>
        public static T Instance
        {
            get
            {
                // Check if the instance is null
                if (instance)
                {
                    // First try to find the object already in the scene
                    instance = GameObject.FindObjectOfType<T>();

                    if (instance == null)
                    {
                        // Couldn't find the singleton in the scene, so make it.
                        GameObject singleton = new GameObject(typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// The Awake method runs first thing in the morning xD,
        /// when scene is loaded if no instance is found then create it
        /// and don't destroy it ever, but if a duplicate was found then
        /// destroy it.
        /// </summary>
        public virtual void Awake()
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

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}