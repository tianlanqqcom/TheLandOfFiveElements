/*
 * File: GlobalSingleton.cs
 * Description: 全局单例模板类，添加后切换场景不会销毁，但需要自己手动创建初始实例。
 * Author: tianlan
 * Last update at 24/1/31 15:39
 * 
 * Update Records:
 * tianlan  24/1/30 编写模板主体
 * tianlan  24/1/31 添加注释
 */

using UnityEngine;

namespace GameFramework.Core
{
    public class GlobalSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}

