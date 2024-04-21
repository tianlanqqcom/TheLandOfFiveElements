/*
 * File: Task.cs
 * Description: 单个任务
 * Author: tianlan
 * Last update at 24/3/10   18:00
 * 
 * Update Records:
 * tianlan  24/3/10 新建文件
 */

using System.Collections.Generic;
using Script.GameFramework.Core;

namespace Script.GameFramework.Game.Tasks
{
    public class Task
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskID { get; protected set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public FixedString Name { get; protected set; }

        /// <summary>
        /// 当前任务节点
        /// </summary>
        public TaskNode NowTaskNode;

        /// <summary>
        /// 奖励列表
        /// </summary>
        public List<TaskAward> Awards { get; protected set; } = new();

        /// <summary>
        /// 设置当前任务的开始节点
        /// </summary>
        /// <param name="node">开始节点</param>
        public void SetBeginNode(TaskNode node)
        {
            NowTaskNode = node;
            node.InitTaskNodeWorld();
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="taskEvent">任务事件</param>
        public virtual void ProcessEvent(TaskEvent taskEvent)
        {
            NowTaskNode?.ProcessEvent(taskEvent);
        }

        /// <summary>
        /// 发送奖励
        /// </summary>
        public virtual void SendAward()
        {

        }
    }
}

