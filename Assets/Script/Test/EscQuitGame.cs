/*
 * File: EscQuitGame.cs
 * Description: 测试键盘输入是否可用，以及实用功能：按ESC键退出游戏
 * Author: tianlan
 * Last update at 24/1/31 17:37
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/3/6  改用新输入系统
 */

using UnityEngine;
using GameFramework.Game;
using GameFramework.GamePlay;

namespace GameFramework.Test
{
    public class EscQuitGame : MonoBehaviour
    {
        public void NotifyKeyDown(KeyCode pressedKey)
        {
            if (pressedKey == KeyCode.Escape)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif

            }
        }

        public void NotifyKeyUp(KeyCode releasedKey)
        {
//            if (releasedKey == KeyCode.Escape)
//            {
//#if UNITY_EDITOR
//                UnityEditor.EditorApplication.isPlaying = false;
//#else
//                Application.Quit();
//#endif

//            }
        }

        public void NotifyMouseDown(int mouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseMove(float deltaX, float deltaY)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseUp(int mouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseWheel(float delta)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterInput()
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterInput()
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {
            // 绑定ESC键
            // InputHandler.Instance.BindKeyInputAction(this, KeyCode.Escape);
            InputSystem.BindKey(KeyCode.Escape, InputSystem.InputEventType.IE_Pressed, ExitGame);
        }

        private void ExitGame()
        {
            if(MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game || MyGameMode.Instance.IsJustChangeMode)
            {
                return;
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

