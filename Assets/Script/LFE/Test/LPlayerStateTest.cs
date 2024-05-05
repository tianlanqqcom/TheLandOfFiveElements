using Script.GameFramework.Game;
using Script.LFE.GamePlay;
using UnityEngine;

namespace Script.LFE.Test
{
    [RequireComponent(typeof(LPlayerState))]
    public class LPlayerStateTest : MonoBehaviour
    {
        private int _damage = 5;
        // Start is called before the first frame update
        private void Start()
        {
            InputSystem.BindKey(KeyCode.K, InputSystem.InputEventType.Pressed, () =>
            {
                gameObject.GetComponent<LPlayerState>().MoveToNextElement();
                Debug.Log(gameObject.GetComponent<LPlayerState>().NowHp);
                gameObject.GetComponent<LPlayerState>().ChangeHealthPoint(_damage);
                Debug.Log("Convert to " + gameObject.GetComponent<LPlayerState>().nowElement);
            });
        }
    }
}