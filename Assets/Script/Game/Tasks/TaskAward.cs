/*
 * File: TaskAward.cs
 * Description: 任务奖励
 * Author: tianlan
 * Last update at 24/3/10   18:35
 * 
 * Update Records:
 * tianlan  24/3/10 新建文件
 */

using GameFramework.Game.Bag;

namespace GameFramework.Game.Tasks
{
    public class TaskAward : MyInventory
    {
        public TaskAward(Inventory inventory) : base(inventory)
        {
        }

        public TaskAward(Inventory inventory, int count) : base(inventory, count)
        {
        }
    }
}

