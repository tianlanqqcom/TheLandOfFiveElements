using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("Apply relative movement to the current position. Returns Success.")]
    public class LookAtPlayer : Action
    {
        [UnityEngine.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        private GameObject _player;
        private GameObject _currentGameObject;

        public override void OnStart()
        {
            _currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            _player = GameObject.FindWithTag("Player");
        }

        public override TaskStatus OnUpdate()
        {
            if (_player == null)
            {
                Debug.LogWarning("BTA::LookAtPlayer Failed to find Player.");
                return TaskStatus.Failure;
            }

            _currentGameObject.transform.LookAt(_player.transform);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}