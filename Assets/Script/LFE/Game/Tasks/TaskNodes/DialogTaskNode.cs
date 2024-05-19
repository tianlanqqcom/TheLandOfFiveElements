using System;
using UnityEngine;
using Script.GameFramework.Core;
using Script.GameFramework.Game.Tasks;

namespace Script.LFE.Game.Tasks.TaskNodes
{
    public class DialogTaskNode : TaskNode
    {
        private Vector3 _position;
        private string _dialogName;

        public DialogTaskNode(FixedString description, FixedString concreteDescription,
            int taskID, int indexInChain, Vector3 pos, string dialogName) :
            base(description, concreteDescription, taskID, indexInChain, TaskType.Dialog)
        {
            _position = pos;
            _dialogName = dialogName;
        }

        public override void ProcessEvent(TaskEvent taskEvent)
        {
            base.ProcessEvent(taskEvent);
            if (!taskEvent.EventMessage.StartsWith("DialogFinished:")) return;
            if (taskEvent.EventMessage[(taskEvent.EventMessage.IndexOf(":") + 1)..] ==
                _dialogName)
            {
                MoveNext();
            }
        }

        public override Vector3 GetPosition()
        {
            return _position;
        }
    }
}