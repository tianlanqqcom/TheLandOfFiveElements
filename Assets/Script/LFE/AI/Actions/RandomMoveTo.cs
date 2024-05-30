using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("Apply relative movement to the current position. Returns Success.")]
    public class RandomMoveTo : Action
    {
        [UnityEngine.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        [UnityEngine.Tooltip("Unused")]
        // Unused
        public SharedVector3 targetPosition;

        // cache the navmeshagent component
        private NavMeshAgent navMeshAgent;
        private GameObject prevGameObject;
        private bool bIsFirstUpdate;
        private Vector3 _targetPos;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                navMeshAgent = currentGameObject.GetComponent<NavMeshAgent>();
                prevGameObject = currentGameObject;
            }

            var randomCircle = Random.insideUnitCircle * 5;
            _targetPos = new Vector3(currentGameObject.transform.position.x + randomCircle.x,
                                     currentGameObject.transform.position.y,
                                     currentGameObject.transform.position.z + randomCircle.y);

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
                var sceneQuery = navMeshAgent.SetDestination(_targetPos);
                navMeshAgent.isStopped = false;
                bIsFirstUpdate = false;

                return sceneQuery ? TaskStatus.Running : TaskStatus.Failure;
            }

            return Vector3.Distance(_targetPos, navMeshAgent.gameObject.transform.position) > 1
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
            navMeshAgent.isStopped = true;
        }
    }
}