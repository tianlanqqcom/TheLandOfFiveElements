using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Script.LFE.GamePlay;
using UnityEngine;
using UnityEngine.AI;

namespace Script.LFE.AI.Actions
{
    [TaskCategory("LFE")]
    [TaskDescription("Apply relative movement to the current position. Returns Success.")]
    public class MonsterAttack : Action
    {
        [UnityEngine.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        public SharedGameObject attackPrefab;

        private GameObject _player;
        private GameObject _currentGameObject;

        public override void OnStart()
        {
            _currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            var attack = Object.Instantiate(attackPrefab.Value);
            attack.transform.position = _currentGameObject.transform.position;
            attack.transform.rotation = _currentGameObject.transform.rotation;
            var fireBall = attack.GetComponent<MonsterFireBall>();

            if (fireBall)
            {
                fireBall.speed = _currentGameObject.transform.forward * 5;
                // fireBall.speed = Vector3.zero;
                fireBall.flyDistance = 100;
                fireBall.damage = Random.Range(1, 20);
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            attackPrefab = null;
        }
    }
}