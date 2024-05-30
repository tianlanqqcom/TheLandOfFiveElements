using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class ForceFreeCursor : MonoBehaviour
    {
        // Update is called once per frame
        private void Update()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
