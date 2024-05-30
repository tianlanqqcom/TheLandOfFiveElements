using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.LFE.GamePlay
{
    public class Magma : MonoBehaviour
    {
        public int damage = 5;

        public bool enableRandomAttack;

        [Range(1, 100)] public int randomRange = 1;

        public float attackTime = .2f;

        private bool _playerInMagma;

        private IEnumerator OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;

            _playerInMagma = true;
            var bNeedBreak = false;
            while (_playerInMagma)
            {
                var attack = damage;
                if (enableRandomAttack)
                {
                    attack += Random.Range(0, randomRange) * (Random.Range(0, 2) > 0 ? 1 : -1);
                }

                if (other.gameObject.GetComponent<LPlayerState>().NowHp <= attack)
                {
                    bNeedBreak = true;
                }

                other.gameObject.GetComponent<LPlayerState>().ChangeHealthPoint(attack);

                if (bNeedBreak)
                {
                    yield break;
                }

                yield return new WaitForSeconds(attackTime);
            }
        }

        private IEnumerator OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) yield break;

            _playerInMagma = false;
        }
    }
}