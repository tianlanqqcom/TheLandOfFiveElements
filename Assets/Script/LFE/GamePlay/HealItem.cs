using Script.GameFramework.GamePlay.InteractiveSystem;
using Script.LFE.Game;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    [RequireComponent(typeof(InteractiveItem))]
    public class HealItem : MonoBehaviour
    {
        public GameObject effect;
        public float effectTime;

        private void Start()
        {
            EffectManager effectManager = EffectManager.Instance;
            if (effectManager && effect)
            {
                gameObject.GetComponent<InteractiveItem>().InteractiveAction
                    .AddListener(() => { effectManager.PlayEffectOnPlayer(effect, effectTime); });
            }
        }
    }
}