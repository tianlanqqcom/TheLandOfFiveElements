/*
 * File: TaskFactory.cs
 * Description: 任务工厂
 * Author: tianlan
 * Last update at 24/3/10   18:44
 * 
 * Update Records:
 * tianlan  24/3/10 新建文件
 */

using GameFramework.Game.Tasks.ConcreteTasks;

namespace GameFramework.Game.Tasks
{
    public class TaskFactory
    {
        public static Task CreateTask(int taskID)
        {
            Task task;

            if(taskID == 1000)
            {
                task = new TestGotoTargetPositionTask();
            }
            else
            {
                task = new Task();
            }

            return task;
        }
    }
}

