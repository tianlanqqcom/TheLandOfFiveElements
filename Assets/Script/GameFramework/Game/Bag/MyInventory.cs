/*
 * File: Inventory.cs
 * Description: 背包中物品
 * Author: tianlan
 * Last update at 24/3/6    20:41
 * 
 * Update Records:
 * tianlan  24/3/6  确定主要成员
 */

namespace Script.GameFramework.Game.Bag
{
    public class MyInventory
    {
        /// <summary>
        /// 物品的通用属性
        /// </summary>
        public Inventory BaseInventory { get; private set; }

        /// <summary>
        /// 物品数量
        /// </summary>
        public int Count;

        public MyInventory(Inventory inventory)
        {
            BaseInventory = inventory;
            Count = 1;
        }

        public MyInventory(Inventory inventory, int count) : this(inventory)
        {
            Count = count;
        }
    }
}

