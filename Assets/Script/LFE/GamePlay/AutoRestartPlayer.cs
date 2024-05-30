using Script.GameFramework.GamePlay;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class AutoRestartPlayer : MonoBehaviour
    {
        public float yLimit = -10;

        public float deathTime = 5.0f;

        // Update is called once per frame
        private void Update()
        {
            if (gameObject.transform.position.y < yLimit && gameObject.CompareTag("Player"))
            {
                MyGameMode.Instance?.RestartPlayer(deathTime);
            }
        }
    }
}
