using UnityEngine;

namespace Player {
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T instance;
        public static T Instance {
            get {
                instance = (T) FindObjectOfType(typeof(T));
                if (instance is null) {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = typeof(T).ToString();
#if UNITY_EDITOR
                    if (Application.isPlaying) { // avoid test errors
#endif
                        DontDestroyOnLoad(singleton);
#if UNITY_EDITOR
                    }
#endif
                }

                return instance;
            }
        }
    }
}