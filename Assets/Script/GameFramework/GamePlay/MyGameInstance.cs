/*
 * File: MyGameInstance.cs
 * Description: MyGameInstance，全局单例类，负责存储玩家在全游戏过程中的一些全局数据
 * Additional: 当前版本仅供测试使用
 * Author: tianlan
 * Last update at 24/1/31 16:46
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using Script.GameFramework.Core;

namespace Script.GameFramework.GamePlay
{
    public class MyGameInstance : GlobalSingleton<MyGameInstance>
    {
        /// <summary>
        /// Test
        /// </summary>
        public int Money = 5;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}

