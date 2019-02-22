using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Key.Ultility
{
    public abstract class MySingleton <T> : MonoBehaviour where T : MySingleton<T>
    {
        private static bool isShuttingDown;

        private static T instance;
        public static T Instance
        {
            get
            {
                if (isShuttingDown)
                    return null;

                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    // object not found => create new one
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }
                }

                instance.Init();

                return instance;
            }

            set
            {

            }
        }

        // Put all the initializations you need here, as you would do in Awake
        public virtual void Init() { }

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.Init();
            }

            // TESTING
            Debug.Log(string.Format("Awaking: {0}", this.name));
        }

        // Make sure the instance isn't referenced anymore when the user quit, just in case.
        internal virtual void OnApplicationQuit()
        {
            isShuttingDown = true;
            Debug.Log(string.Format("Quitting: {0}", this.name));
            //instance = null;
        }

        //protected virtual void OnDestroy()
        //{
        //    instance = null;
        //}
    }
}
