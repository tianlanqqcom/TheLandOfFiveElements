/*
 * File: MyEnemyController.cs
 * Description: EnemyController类，负责启动或关闭行为树组件
 * Author: tianlan
 * Last update at 24/1/31 16:46
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using BehaviorDesigner.Runtime;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.GamePlay
{
    [RequireComponent(typeof(BehaviorTree), typeof(MyPlayerState))]
    public class MyEnemyController : MonoBehaviour
    {
        /// <summary>
        /// 需要的行为树组件
        /// </summary>
        BehaviorTree behaviorTree;

        /// <summary>
        /// 玩家状态，也称角色状态，虽然AI控制的敌人并不是真人玩家，但显然他们控制的角色有和真人玩家相同的属性
        /// </summary>
        MyPlayerState playerState;
        // Start is called before the first frame update
        void Start()
        {
            // 尝试获取行为树组件
            if (!TryGetComponent(out behaviorTree))
            {
                Logger.LogError("EnemyController: " + gameObject.name + " dosen't have BehaviorTree Component.", "Errors.log");
            }
            else
            {
                behaviorTree.enabled = true;
                behaviorTree.EnableBehavior();
            }

            // 尝试获取PlayerState组件
            if (!TryGetComponent(out playerState))
            {
                Logger.LogError("EnemyController: " + gameObject.name + " dosen't have BehaviorTree Component.", "Errors.log");
            }
        }

        // Update is called once per frame
        void Update()
        {
            // 如果生命小于0，暂停行动
            if (playerState.HP <= 0)
            {
                behaviorTree.enabled = false;
            }
        }

        /// <summary>
        /// 重启敌人
        /// </summary>
        public void RestartEnemy()
        {
            behaviorTree.enabled = true;
            playerState.ResetHealth();
        }
    }
}

