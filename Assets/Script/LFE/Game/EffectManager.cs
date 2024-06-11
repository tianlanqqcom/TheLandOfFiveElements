using System.Collections;
using Script.GameFramework.Core;
using UnityEngine;

namespace Script.LFE.Game
{
    public class EffectManager : SimpleSingleton<EffectManager>
    {
        public void PlayEffectOnPlayer(GameObject effect, float time = 3.0f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                PlayEffect(player, effect, time);
            }
        }

        public void PlayEffect(GameObject parent, GameObject effect, float time = 3.0f)
        {
            GameObject obj = Instantiate(effect, parent.transform);
            StartCoroutine(RemoveEffectAfter(obj, time));
        }

        private static IEnumerator RemoveEffectAfter(GameObject o, float time)
        {
            yield return new WaitForSeconds(time);
            if (o)
            {
                Destroy(o);
            }
        }
    }
}