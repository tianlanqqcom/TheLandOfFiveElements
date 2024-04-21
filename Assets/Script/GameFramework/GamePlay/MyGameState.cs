/*
 * File: MyGameState.cs
 * Description: MyGameState，局部单例类，处理游戏状态数据
 * Author: tianlan
 * Last update at 24/1/31 16:53
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using Script.GameFramework.Core;
using UnityEngine;

namespace Script.GameFramework.GamePlay
{
    public class MyGameState : SimpleSingleton<MyGameState>
    {
        /// <summary>
        /// 游戏开始的时间
        /// </summary>
        public System.DateTime BeginTime { get; private set; }

        /// <summary>
        /// 游戏世界持续运行的时间（受timeScale影响）
        /// </summary>
        public float WorldRunningTime { get; private set; } = .0f;

        /// <summary>
        /// 当前关卡
        /// </summary>
        public string NowLevel { get; set; }

        protected override void Awake()
        {
            base.Awake();
            BeginTime = System.DateTime.Now;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            WorldRunningTime += Time.deltaTime;
        }
    }
}

