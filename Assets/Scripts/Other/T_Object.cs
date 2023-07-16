﻿using System;
using UnityEngine;


    public class MonoSingleton<T>: MonoBehaviour  where T : MonoSingleton<T>
    {
        private static T _instance;
 
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Debug.LogError(typeof(T).ToString() + " is missing.");
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                //Debug.LogWarning("Second instance of " + typeof(T) + " created. Automatic self-destruct triggered.");
                Destroy(this.gameObject);
                return;
            }
            _instance = this as T;
            Init();
        }
        
        protected virtual void OnDestroy()
        {
            // if (_instance == this)
            // {
            //     _instance = null;
            // }
        }


        public virtual void Init() { Debug.Log($"<color=#00FF00>INSTANCED-->> </color>" + this.GetType()); }
    }
