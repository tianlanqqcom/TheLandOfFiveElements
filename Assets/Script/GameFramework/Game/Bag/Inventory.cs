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

using System;
using System.IO;
using Script.GameFramework.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Script.GameFramework.Game.Bag
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
    public class Inventory : ScriptableObject
    {
        /// <summary>
        /// 物品唯一标识索引
        /// </summary>
        [FormerlySerializedAs("UID")] [Tooltip("物品唯一标识索引")]
        public int uid;

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
        [FormerlySerializedAs("Level")] [Tooltip("物品等级")]
        public InventoryLevel level;

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
        [FormerlySerializedAs("Type")] [Tooltip("物品类型")]
        public InventoryType type;

        /// <summary>
        /// 图片路径
        /// </summary>
        [FormerlySerializedAs("ImagePath")]
        [SerializeField]
        [Tooltip("图片路径")]
        private string imagePath;

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThumbnailPath => Path.Combine("Images/Thubnails", imagePath);

        public string RawImagePath => Path.Combine("Images/RawImages", imagePath);

        /// <summary>
        /// 物品名
        /// </summary>
        [FormerlySerializedAs("Name")] [Tooltip("物品名")]
        public FixedString objectName;

        /// <summary>
        /// 效果描述
        /// </summary>
        [FormerlySerializedAs("EffectDescription")] [Tooltip("效果描述")]
        public FixedString effectDescription;

        /// <summary>
        /// 描述
        /// </summary>
        [FormerlySerializedAs("Description")] [Tooltip("描述")]
        public FixedString description;

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        [FormerlySerializedAs("CanBeStacked")] [Tooltip("是否可堆叠")]
        public bool canBeStacked;

        /// <summary>
        /// 是否可在背包中直接使用
        /// </summary>
        [FormerlySerializedAs("CanBeUsedInBag")] [Tooltip("是否可在背包中直接使用")]
        public bool canBeUsedInBag;

        /// <summary>
        /// 使用后是否销毁
        /// </summary>
        [FormerlySerializedAs("DestoryAfterUse")] [Tooltip("使用后是否销毁")]
        public bool destoryAfterUse;

        /// <summary>
        /// 物品数量
        /// </summary>
        //[Tooltip("物品数量")]
        //public int Count = 1;

        /// <summary>
        /// 当物品被使用时调用
        /// </summary>
        [FormerlySerializedAs("useFuncs")] [FormerlySerializedAs("UseFuncs")] [Tooltip("当物品被使用时调用")]
        public UnityEvent<int, bool> useFunctions;

        /// <summary>
        /// 调用使用方法
        /// </summary>
        public void InvokeUseFunctions()
        {
            useFunctions.Invoke(1, destoryAfterUse);
        }

        /// <summary>
        /// 调用使用方法
        /// </summary>
        /// <param name="count">使用的数量</param>
        public void InvokeUseFunctions(int count)
        {
            useFunctions.Invoke(count, destoryAfterUse);
        }
    }
}

