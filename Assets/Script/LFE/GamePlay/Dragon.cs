using System.Collections;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class Dragon : MonoBehaviour
    {
        public GameObject firePrefab;

        public float yOffset = .5f;

        public float attackTime = 5.0f;

        public float fireSpeed = 1.0f;

        public float fireDistance = 20.0f;

        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            while (true)
            {
                var attack = Instantiate(firePrefab, gameObject.transform);
                attack.transform.Translate(0, yOffset, 0);

                var fireBall = attack.GetComponent<MonsterFireBall>();

                if (fireBall)
                {
                    fireBall.speed = new Vector3(-1, 0, 0) * fireSpeed;
                    // fireBall.speed = Vector3.zero;
                    fireBall.flyDistance = fireDistance;
                    fireBall.damage = Random.Range(1, 20);
                }

                yield return new WaitForSeconds(attackTime);
            }
        }
    }
}