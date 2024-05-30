using Script.GameFramework.GamePlay;
using Script.GameFramework.GamePlay.InteractiveSystem;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class Portal : MonoBehaviour
    {
        public Transform newPosition;

        public float deathTime;
        
        // Start is called before the first frame update
        private void Start()
        {
            var item = gameObject.GetComponent<InteractiveItem>();
            item?.InteractiveAction.AddListener(ChangeCharacterPosition);
        }

        private void ChangeCharacterPosition()
        {
            if (newPosition)
            {
                MyGameMode.Instance?.RestartPlayerAt(newPosition.position, deathTime);
            }
            else
            {
                MyGameMode.Instance?.RestartPlayer(deathTime);
            }
            
        }
    }
}
