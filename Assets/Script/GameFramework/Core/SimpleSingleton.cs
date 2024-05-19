/*
 * File: SimpleSingleton.cs
 * Description: 简单单例模板类，添加后切换场景会销毁，需要自己手动创建初始实例。
 * Author: tianlan
 * Last update at 24/1/31 15:55
 *
 * Update Records:
 * tianlan  24/1/30 编写模板主体
 * tianlan  24/1/31 添加注释
 */

using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Core
{
    public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else if (Instance != this)
            {
                // Destroy(gameObject);
                // Logger.Log($"SimpleSingleton::Awake Destroy self{gameObject.name}");
                Instance = this as T;
                Logger.Log("SimpleSingleton::Awake Try to set instance = self.");
            }
        }
    }
}