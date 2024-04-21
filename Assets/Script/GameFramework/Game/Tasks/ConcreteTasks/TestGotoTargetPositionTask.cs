/*
 * File: TestGotoTargetPositionTask.cs
 * Description: 测试任务，由若干个前往指定地点组成，taskID=1000
 * Author: tianlan
 * Last update at 24/3/14   22:25
 * 
 * Update Records:
 * tianlan  24/3/14 新建文件
 */

using Script.GameFramework.Core;
using Script.GameFramework.Game.Bag;
using Script.GameFramework.Game.Tasks.TaskNodes;
using UnityEngine;

namespace Script.GameFramework.Game.Tasks.ConcreteTasks
{
    public class TestGotoTargetPositionTask : Task
    {
        public TestGotoTargetPositionTask()
        {
            TaskID = 1000;

            Name = new FixedString("test task: goto target position");

            GotoTargetPositionTaskNode lastNode = null;
            for (int i = 0; i < 5; ++i)
            {
                GotoTargetPositionTaskNode nowNode = new(
                    new FixedString("前往指定地点"),
                    new FixedString("前往指定地点：具体描述"),
                    1000, i, Vector3.zero);

                if(lastNode == null)
                {
                    SetBeginNode(nowNode);
                }
                else
                {
                    lastNode.SetNext(nowNode);
                }


                lastNode = nowNode;
            }

            Awards.Add(new TaskAward(BagSystem.Instance.availableInventoriesTemplates[0], 5));
        }
    }
}

