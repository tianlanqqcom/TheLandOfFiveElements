/*
 * File: MyPlayerState.cs
 * Description: MyPlayerState, 存储角色对应的属性，如生命等
 * Author: tianlan
 * Last update at 24/1/31 17:21
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using UnityEngine;
using GameFramework.Core;

namespace GameFramework.GamePlay
{
    public class MyPlayerState : MonoBehaviour
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHP { get; private set; } = 100;

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int HP { get; private set; }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 用户ID，当前未使用
        /// </summary>
        public string UserID { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            HP = MaxHP;
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 设置最大生命值
        /// </summary>
        /// <param name="newMaxHP">新的最大生命值</param>
        public void SetMaxHealthPoint(int newMaxHP)
        {
            MaxHP = newMaxHP;
            HP = MathLibrary.Clamp(HP, 0, MaxHP);
        }

        /// <summary>
        /// 改变生命值（不论是造成伤害还是恢复）
        /// </summary>
        /// <param name="damage">造成的伤害，当恢复生命时该值为负数</param>
        public void ChangeHealthPoint(int damage)
        {
            HP -= damage;
            HP = MathLibrary.Clamp(HP, 0, MaxHP);
        }

        /// <summary>
        /// 重置生命为最大值
        /// </summary>
        public void ResetHealth()
        {
            HP = MaxHP;
        }
    }
}

