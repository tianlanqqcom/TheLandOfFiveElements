/*
 * File: SwitchMouseStateTest.cs
 * Description: 测试MyGameMode中鼠标控制是否可用，按LeftAlt显示鼠标，松开隐藏
 * Author: tianlan
 * Last update at 24/3/6    19:17
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/2/7  修复：使用GameMode.NowWorkingMode来判断输入处理方式，而非原先的永远处理。
 * tianlan  24/3/6  使用新输入系统
 */

using UnityEngine;
using GameFramework.Game;
using GameFramework.GamePlay;

namespace GameFramework.Test
{
    public class SwitchMouseStateTest : MonoBehaviour
    {
        public void NotifyKeyDown(KeyCode PressedKey)
        {
            Logger.Log("SwitchMouseStateTest: Test");
            if(MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                return;
            }

            if (PressedKey == KeyCode.LeftAlt)
            {
                MyGameMode.Instance.ShowMouse();
                Logger.Log("SwitchMouseStateTest: ShowMouse()");
            }
            else
            {
                Logger.Log("SwitchMouseStateTest: Recv KeyDown but not Alt, Key = " + PressedKey.ToString());
            }

        }

        public void NotifyKeyUp(KeyCode ReleasedKey)
        {
            if (MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                Logger.Log("NotifyKeyUp: return");
                return;
            }

            if (ReleasedKey == KeyCode.LeftAlt)
            {
                MyGameMode.Instance.HideMouse();
                Logger.Log("SwitchMouseStateTest: HideMouse()");
            }
            else
            {
                Logger.Log("SwitchMouseStateTest: Recv KeyDown but not Alt, Key = " + ReleasedKey.ToString());
            }
        }

        public void NotifyMouseDown(int MouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseMove(float DeltaX, float DeltaY)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseUp(int MouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseWheel(float Delta)
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
            // InputHandler.Instance.BindKeyInput(this, KeyCode.LeftAlt);
            InputSystem.BindKey(KeyCode.LeftAlt, InputSystem.InputEventType.IE_Pressed, () =>
            { 
                if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
                {
                    MyGameMode.Instance.ShowMouse();
                }
            });

            InputSystem.BindKey(KeyCode.LeftAlt, InputSystem.InputEventType.IE_Released, () =>
            {
                if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && MyGameMode.Instance.IsMouseShown)
                {
                    MyGameMode.Instance.HideMouse();
                }
            });
        }
    }
}

