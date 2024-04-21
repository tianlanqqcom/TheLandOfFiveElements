/*
 * File: TaskEvent.cs
 * Description: 任务事件
 * Author: tianlan
 * Last update at 24/3/10   18:10
 * 
 * Update Records:
 * tianlan  24/3/10 新建文件
 */

namespace Script.GameFramework.Game.Tasks
{
    public class TaskEvent
    {
        /// <summary>
        /// 目标任务ID，如果为-1，则表示所有任务
        /// </summary>
        public int TargetTaskID = -1;

        /// <summary>
        /// 事件信息
        /// </summary>
        public string EventMessage;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventMessage">事件信息</param>
        public TaskEvent(string eventMessage)
        {
            EventMessage = eventMessage;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="targetTaskID">目标任务ID</param>
        /// <param name="eventMessage">事件信息</param>
        public TaskEvent(int targetTaskID, string eventMessage)
        {
            TargetTaskID = targetTaskID;
            EventMessage = eventMessage;
        }
    }
}

