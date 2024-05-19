using Script.GameFramework.Game.Dialog;
using UnityEngine;

namespace Script.GameFramework.Test
{
    public class DialogNoInterruptTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DialogSystem.Instance.BeginDialogNoInterrupt("dia1");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
