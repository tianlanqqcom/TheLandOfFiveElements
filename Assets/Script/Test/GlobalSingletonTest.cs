/*
 * File: GlobalSingletonTest.cs
 * Description: 测试全局实例是否可用
 * Author: tianlan
 * Last update at 24/1/31 17:39
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using UnityEngine;
using GameFramework.GamePlay;

namespace GameFramework.Test
{
    public class GlobalSingletonTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MyGameInstance instance = MyGameInstance.Instance;
            if (instance != null)
            {
                Logger.Log(instance.Money.ToString());
            }
            else
            {
                Logger.Log("instance is null");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

