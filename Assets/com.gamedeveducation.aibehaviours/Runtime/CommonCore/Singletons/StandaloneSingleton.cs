using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace CommonCore
{
    public abstract class StandaloneSingleton<T> : StandaloneSingleton where T : class, new()
    {
        static T _Instance = null;
        static bool _bInitializing = false;
        static readonly object _InstanceLock = new object();

        public static T Instance
        {
            get
            {
                lock (_InstanceLock)
                {
                    // instance is already found
                    if (_Instance != null)
                        return _Instance;

                    _bInitializing = true;

                    // create a new instance of T
                    _Instance = new T();
                    (_Instance as StandaloneSingleton).Initialize();

                    _bInitializing = false;
                    return _Instance;
                }
            }
        }

        static void ConstructIfNeeded(StandaloneSingleton<T> InInstance)
        {
            lock (_InstanceLock)
            {
                // only construct if the instance is null and is not being initialized
                if (_Instance == null && !_bInitializing)
                {
                    _Instance = InInstance as T;
                }
                else if (_Instance != null && !_bInitializing)
                {
                    Debug.LogError($"Found duplicate {typeof(T)}");
                }
            }
        }

        internal override void Initialize()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif // UNITY_EDITOR

            OnInitialize();
        }

        protected virtual void OnInitialize() { }

#if UNITY_EDITOR
        protected virtual void OnPlayInEditorStopped() { }

        void OnPlayModeChanged(PlayModeStateChange InChange)
        {
            if ((InChange == PlayModeStateChange.ExitingPlayMode) && (_Instance != null))
            {
                EditorApplication.playModeStateChanged -= OnPlayModeChanged;
                OnPlayInEditorStopped();
                _Instance = null;
            }
        }
#endif // UNITY_EDITOR
    }

    public abstract class StandaloneSingleton 
    {
        internal abstract void Initialize();
        public virtual void OnBootstraph() { }
    }
}
