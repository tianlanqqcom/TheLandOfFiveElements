/*
 * File: GotoTargetPositionTaskNode.cs
 * Description: 前往指定地点的任务节点
 * Author: tianlan
 * Last update at 24/5/18   21:10
 * 
 * Update Records:
 * tianlan  24/3/14 新建文件
 * tianlan  24/5/18 修改硬编码参数触发器大小并添加到构造函数中
 */

using Script.GameFramework.Core;
using Script.GameFramework.GamePlay.InteractiveSystem;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game.Tasks.TaskNodes
{
    public class GotoTargetPositionTaskNode : TaskNode
    {
        /// <summary>
        /// 目标地点
        /// </summary>
        Vector3 targetPosition;

        /// <summary>
        /// 触发器大小
        /// </summary>
        private Vector3 triggerSize;

        /// <summary>
        /// 目标地点触发器
        /// </summary>
        GameObject targetPosTrigger = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">任务描述</param>
        /// <param name="concreteDescription">任务详细描述</param>
        /// <param name="taskID">任务ID</param>
        /// <param name="indexInChain">在任务链中的序号</param>
        /// <param name="targetPosition">目标地点</param>
        /// <param name="triggerSize">触发器大小</param>
        public GotoTargetPositionTaskNode(FixedString description, FixedString concreteDescription, 
            int taskID, int indexInChain, Vector3 targetPosition, Vector3 triggerSize) :
            base(description, concreteDescription, taskID, indexInChain, TaskType.GotoTargetPosition)
        {
            this.targetPosition = targetPosition;
            this.triggerSize = triggerSize;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">任务描述</param>
        /// <param name="concreteDescription">任务详细描述</param>
        /// <param name="taskID">任务ID</param>
        /// <param name="indexInChain">在任务链中的序号</param>
        /// <param name="targetPosition">目标地点</param>
        public GotoTargetPositionTaskNode(FixedString description, FixedString concreteDescription, 
            int taskID, int indexInChain, Vector3 targetPosition) :
            base(description, concreteDescription, taskID, indexInChain, TaskType.GotoTargetPosition)
        {
            this.targetPosition = targetPosition;
            triggerSize = new Vector3(2,2,2);
        }

        /// <summary>
        /// 获取当前任务的地点
        /// </summary>
        /// <returns>任务地点的坐标</returns>
        public override Vector3 GetPosition()
        {
            return targetPosition;
        }

        /// <summary>
        /// 初始化任务节点在世界中的表现
        /// </summary>
        public override void InitTaskNodeWorld()
        {
            base.InitTaskNodeWorld();

            targetPosTrigger = new GameObject(
                $"TaskNode({parentTaskID},{IndexInThisTaskChain},{Type}):");
            targetPosTrigger.transform.position = targetPosition;
            BoxCollider collider = targetPosTrigger.AddComponent<BoxCollider>();
            collider.center = Vector3.zero;
            collider.size = triggerSize;
            collider.isTrigger = true;
            targetPosTrigger.AddComponent<InteractiveItem>();
            InteractiveItem interactiveItem = targetPosTrigger.GetComponent<InteractiveItem>();
            if (interactiveItem != null)
            {
                interactiveItem.FMessage = new FixedString("目标地点");
                interactiveItem.IsAutoPlay = true;
                interactiveItem.InteractiveAction.AddListener(OnPlayerArrivedAtTargetPosition);
            }
        }

        /// <summary>
        /// 清理该任务节点在世界中的表现
        /// </summary>
        public override void CleanTaskNodeWorld()
        {
            if (targetPosTrigger)
            {
                Object.Destroy(targetPosTrigger);
            }
        }

        private void OnPlayerArrivedAtTargetPosition()
        {
            Logger.Log("GotoTargetPositionTaskNode:OnPlayerArrivedAtTargetPosition() Player Arrived at " + targetPosition);
            MoveNext();
        }
    }
}

