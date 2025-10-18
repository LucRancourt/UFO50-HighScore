using UnityEngine;

namespace _Project.Code.Core.General
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isApplicationQuitting;

        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject singletonObject = new();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T) + " (Singleton)";
                    }
                }

                return _instance;
            }
        }


        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }
    }
}