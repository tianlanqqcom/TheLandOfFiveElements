using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("计算与玩家的距离，如果距离大于指定值，返回Success.")]
    public class DiableAgent : Action
    {
        public SharedGameObject targetGameObject;
        
        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            NavMeshAgent nav = currentGameObject.GetComponent<NavMeshAgent>();
            nav.SetDestination(currentGameObject.transform.position);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}