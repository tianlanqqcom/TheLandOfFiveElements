/*
 * File: Inventory.cs
 * Description: 背包中物品的基类，可在编辑器中创建和编辑
 * Author: tianlan
 * Last update at 24/3/6    20:41
 * 
 * Update Records:
 * tianlan  24/2/26 确定主要成员
 * tianlan  24/2/28 添加成员:效果描述
 * tianlan  24/3/6  删除数量字段，移入MyInventory中
 */

using GameFramework.Core;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Game.Bag
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
    public class Inventory : ScriptableObject
    {
        /// <summary>
        /// 物品唯一标识索引
        /// </summary>
        [Tooltip("物品唯一标识索引")]
        public int UID;

        /// <summary>
        /// 物品等级
        /// </summary>
        [Serializable]
        public enum InventoryLevel
        {
            White,
            Green,
            Blue,
            Purple,
            Gold
        }

        /// <summary>
        /// 物品等级
        /// </summary>
        [Tooltip("物品等级")]
        public InventoryLevel Level;

        /// <summary>
        /// 物品类型
        /// </summary>
        [Serializable]
        public enum InventoryType
        {
            Material,
            Prop,
            Weapon,
            Quests,
            Food,
            All
        }

        /// <summary>
        /// 物品类型
        /// </summary>
        [Tooltip("物品类型")]
        public InventoryType Type;

        /// <summary>
        /// 图片路径
        /// </summary>
        [SerializeField]
        [Tooltip("图片路径")]
        private string ImagePath;

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThubnailPath
        {
            get
            {
                return Path.Combine("Images/Thubnails", ImagePath);
            }
        }

        public string RawImagePath
        {
            get
            {
                return Path.Combine("Images/RawImages", ImagePath);
            }
        }

        /// <summary>
        /// 物品名
        /// </summary>
        [Tooltip("物品名")]
        public FixedString Name;

        /// <summary>
        /// 效果描述
        /// </summary>
        [Tooltip("效果描述")]
        public FixedString EffectDescription;

        /// <summary>
        /// 描述
        /// </summary>
        [Tooltip("描述")]
        public FixedString Description;

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        [Tooltip("是否可堆叠")]
        public bool CanBeStacked;

        /// <summary>
        /// 是否可在背包中直接使用
        /// </summary>
        [Tooltip("是否可在背包中直接使用")]
        public bool CanBeUsedInBag;

        /// <summary>
        /// 使用后是否销毁
        /// </summary>
        [Tooltip("使用后是否销毁")]
        public bool DestoryAfterUse;

        /// <summary>
        /// 物品数量
        /// </summary>
        //[Tooltip("物品数量")]
        //public int Count = 1;

        /// <summary>
        /// 当物品被使用时调用
        /// </summary>
        [Tooltip("当物品被使用时调用")]
        public UnityEvent<int, bool> UseFuncs;

        /// <summary>
        /// 调用使用方法
        /// </summary>
        public void InvokeUseFuncs()
        {
            UseFuncs.Invoke(1, DestoryAfterUse);
        }

        /// <summary>
        /// 调用使用方法
        /// </summary>
        /// <param name="count">使用的数量</param>
        public void InvokeUseFuncs(int count)
        {
            UseFuncs.Invoke(count, DestoryAfterUse);
        }
    }
}

