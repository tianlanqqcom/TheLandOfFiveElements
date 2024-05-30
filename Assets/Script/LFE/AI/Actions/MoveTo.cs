using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("Apply relative movement to the current position. Returns Success.")]
    public class MoveTo : Action
    {
        [UnityEngine.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        [UnityEngine.Tooltip("The relative movement vector")]
        public SharedVector3 targetPosition;

        // cache the navmeshagent component
        private NavMeshAgent navMeshAgent;
        private GameObject prevGameObject;
        private bool bIsFirstUpdate;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                navMeshAgent = currentGameObject.GetComponent<NavMeshAgent>();
                prevGameObject = currentGameObject;
            }

            bIsFirstUpdate = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (navMeshAgent == null)
            {
                Debug.LogWarning("NavMeshAgent is null");
                return TaskStatus.Failure;
            }

            if (bIsFirstUpdate)
            {
                var sceneQuery = navMeshAgent.SetDestination(targetPosition.Value);
                bIsFirstUpdate = false;

                return sceneQuery ? TaskStatus.Running : TaskStatus.Failure;
            }

            return Vector3.Distance(targetPosition.Value, navMeshAgent.gameObject.transform.position) > 1
                ? TaskStatus.Running
                : TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            targetPosition = Vector3.zero;
        }

        public override void OnConditionalAbort()
        {
            navMeshAgent.SetDestination(prevGameObject.transform.position);
        }
    }
}