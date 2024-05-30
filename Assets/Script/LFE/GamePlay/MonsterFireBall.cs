using System;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class MonsterFireBall : MonoBehaviour
    {
        public Vector3 speed;
        public float flyDistance;
        public int damage = 10;

        private float _alreadyFly = .0f;

        private void Update()
        {
            gameObject.transform.position += speed * Time.deltaTime;
            _alreadyFly += speed.magnitude * Time.deltaTime;

            if (_alreadyFly > flyDistance)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("MonsterFireBall::OnCollisionEnter OnTriggerEnter.");
            if (other.gameObject.CompareTag("Monster"))
            {
                return;
            }
            
            if (other.gameObject.CompareTag("Player"))
            {
                var playerState = other.gameObject.GetComponent<LPlayerState>();
                if (playerState)
                {
                    playerState.ChangeHealthPoint(damage);
                    Debug.Log($"MonsterFireBall::OnCollisionEnter Make damage = {damage}");
                }
            }

            Destroy(gameObject);
        }
    }
}