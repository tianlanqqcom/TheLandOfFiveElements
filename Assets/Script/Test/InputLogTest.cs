/*
 * File: InputLogTest.cs
 * Description: 测试InputHandler和IInputReceiver结构是否可用
 * Author: tianlan
 * Last update at 24/3/6    19:16
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/3/6  使用新输入系统
 */

using UnityEngine;
using GameFramework.Game;

namespace GameFramework.Test
{
    public class InputLogTest : MonoBehaviour
    {
        public void NotifyKeyDown(KeyCode PressedKey)
        {
            Logger.Log("InputLogTest:" + PressedKey.ToString() + " has been pressed.");
        }

        public void NotifyKeyUp(KeyCode ReleasedKey)
        {
            Logger.Log("InputLogTest:" + ReleasedKey.ToString() + " has been released.", "LogTest.log");
        }

        public void NotifyMouseDown(int MouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseMove(float DeltaX, float DeltaY)
        {
            Logger.Log("InputLogTest:MouseMove " + DeltaX + " " + DeltaY + " has been pressed.");
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
            //InputHandler inputHandler = InputHandler.Instance;
            //inputHandler.BindKeyInput(this, KeyCode.A);
            //inputHandler.BindKeyInput(this, KeyCode.W);
            //inputHandler.BindKeyInput(this, KeyCode.Alpha0);
            //// inputHandler.BindMouseInput(this, 0);
            //inputHandler.BindMouseMoveInput(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

