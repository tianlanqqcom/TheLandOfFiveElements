/*
 * File: TaskSystem.cs
 * Description: 任务系统
 * Author: tianlan
 * Last update at 24/3/10   18:10
 * 
 * Update Records:
 * tianlan  24/3/10 新建文件
 * tianlan  24/3/17 添加两个静态变量代表特殊状态值
 */

using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Log;

namespace Script.GameFramework.Game.Tasks
{
    public class TaskSystem : SimpleSingleton<TaskSystem>
    {
        /// <summary>
        /// 任意任务ID
        /// </summary>
        public static int AnyTaskID = -1;

        /// <summary>
        /// 无效任务ID
        /// </summary>
        public static int InvalidTaskID = -2;

        /// <summary>
        /// 当前活动中的任务
        /// </summary>
        public readonly List<Task> NowActiveTasks = new();

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        public void AddTask(int taskID)
        {
            if(GetTargetTask(taskID) != null)
            {
                Logger.LogError("TaskSystem:AddTask() Already has this task. TaskID = " + taskID);
                return;
            }

            Task task = TaskFactory.CreateTask(taskID);
            NowActiveTasks.Add(task);
        }

        /// <summary>
        /// 获取指定ID的任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <returns>指定的任务，如果无则返回null</returns>
        public Task GetTargetTask(int taskID)
        {
            foreach (var task in NowActiveTasks)
            {
                if(taskID == task.TaskID)
                {
                    return task;
                }
            }
            return null;
        }

        /// <summary>
        /// 通知有任务结束
        /// </summary>
        /// <param name="taskID">任务ID</param>
        public void NotifyTaskEnd(int taskID)
        {
            Task task = GetTargetTask(taskID);
            if (task != null)
            {
                task.SendAward();
                NowActiveTasks.Remove(task);
            }
            else
            {
                Logger.LogError("TaskSystem:NotifyTaskEnd() Target task doesn't exist. TaskID = " + taskID);
            }
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="taskEvent">任务事件</param>
        public void DispatchEvent(TaskEvent taskEvent)
        {
            if(taskEvent == null)
            {
                Logger.LogError("TaskSystem:DispatchEvent() taskEvent is null");
                return;
            }

            if(taskEvent.TargetTaskID == -1)
            {
                foreach(var task in NowActiveTasks)
                {
                    task.ProcessEvent(taskEvent);
                }
            }
            else
            {
                foreach (var task in NowActiveTasks)
                {
                    if(task.TaskID == taskEvent.TargetTaskID)
                    {
                        task.ProcessEvent(taskEvent);
                        break;
                    }
                }
            }
        }
    }
}

