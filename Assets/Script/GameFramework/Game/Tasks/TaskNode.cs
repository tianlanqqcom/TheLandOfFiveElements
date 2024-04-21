/*
 * File: TaskNode.cs
 * Description: 任务节点
 * Author: tianlan
 * Last update at 24/3/8    22:40
 * 
 * Update Records:
 * tianlan  24/3/8  新建文件
 */

using Script.GameFramework.Core;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game.Tasks
{
    public class TaskNode
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        public enum TaskType
        {
            Dialog,
            Fight,
            GotoTargetPosition
        }

        /// <summary>
        /// 与父任务的软链接
        /// </summary>
        public readonly int parentTaskID;

        /// <summary>
        /// 下一个节点
        /// </summary>
        private TaskNode next = null;

        /// <summary>
        /// 任务描述
        /// </summary>
        public FixedString Description;

        /// <summary>
        /// 详细任务描述，对需要做的事情的描述，e.g.击杀10个怪物
        /// </summary>
        public FixedString ConcreteTaskDescription;

        /// <summary>
        /// 该任务节点在当前任务链中的索引
        /// </summary>
        public readonly int IndexInThisTaskChain;

        /// <summary>
        /// 任务类型
        /// </summary>
        public readonly TaskType Type;

        public TaskNode(int taskID, int indexInChain, TaskType type)
        {
            parentTaskID = taskID;
            IndexInThisTaskChain = indexInChain;
            Type = type;
        }

        public TaskNode(FixedString description, FixedString concreteDescription, 
            int taskID, int indexInChain, TaskType type) : this(taskID, indexInChain, type)
        {
            Description = description;
            ConcreteTaskDescription = concreteDescription;
        }

        /// <summary>
        /// 设置下一个任务节点
        /// </summary>
        /// <param name="nextNode">下一个任务节点</param>
        public void SetNext(TaskNode nextNode)
        {
            next = nextNode;
        }

        /// <summary>
        /// 当该任务节点完成，向下移动一次，如果没有下一个则发放奖励
        /// </summary>
        public void MoveNext()
        {
            CleanTaskNodeWorld();

            if (next != null)
            {
                GetParentTaskInList().NowTaskNode = next;
                next.InitTaskNodeWorld();
            }
            else
            {
                Logger.Log("TaskNode:MoveNext() TaskEnd.");
                TaskSystem.Instance.NotifyTaskEnd(parentTaskID);
            }
        }

        /// <summary>
        /// 在任务系统中获取当前节点绑定的父任务，如果未找到则返回null
        /// </summary>
        /// <returns>当前节点绑定的父任务</returns>
        private Task GetParentTaskInList()
        {
            return TaskSystem.Instance.GetTargetTask(parentTaskID);
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="taskEvent">任务事件</param>
        public virtual void ProcessEvent(TaskEvent taskEvent)
        {

        }

        /// <summary>
        /// 获取当前任务的地点
        /// </summary>
        /// <returns>任务地点的坐标</returns>
        public virtual Vector3 GetPosition()
        {
            return Vector3.zero;
        }

        /// <summary>
        /// 初始化任务节点在世界中的表现
        /// </summary>
        public virtual void InitTaskNodeWorld()
        {

        }

        /// <summary>
        /// 清理该任务节点在世界中的表现
        /// </summary>
        public virtual void CleanTaskNodeWorld()
        {

        }
    }
}

