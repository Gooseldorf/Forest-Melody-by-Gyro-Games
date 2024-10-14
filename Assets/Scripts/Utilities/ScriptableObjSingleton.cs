using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjSingleton<T> : ScriptableObject where T: ScriptableObject 
{
        private static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                    _instance = Resources.Load<T>(typeof(T).Name);
                return _instance;
            }
        }
}
