using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("计算与玩家的距离，如果距离大于指定值，返回Success.")]
    public class CalculatePlayerDistance : Action
    {
        public SharedGameObject targetGameObject;
        public SharedFloat targetDistance;

        private float _distance;
        private GameObject _player;
        private GameObject _currentGameObject;

        public override void OnStart()
        {
            _currentGameObject = GetDefaultGameObject(targetGameObject.Value);

            var player = GameObject.FindWithTag("Player");
            if (player != _player)
            {
                _player = player;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_player == null)
            {
                Debug.LogWarning("BTA::CalculatePlayerDistance Failed to find Player.");
                return TaskStatus.Failure;
            }

            _distance = Vector3.Distance(_currentGameObject.transform.position, _player.transform.position);

            return _distance < targetDistance.Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            targetDistance = 0;
        }
    }
}