/*
 * File: InputHandler.cs
 * Description: 输入处理器，用于监听各种输入并转发给已经绑定好的输入接受类，为场景单例类
 * Author: tianlan && pili
 * Last update at 24/3/6    16:00
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * pili     24/2/16 重写有关键盘的部分，将42键扩为任意键
 * tianlan  24/3/6  InputHandler已弃用，输入系统重构
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game
{
    [Obsolete("Use InputSystem instead of InputHandler.")]
    public class InputHandler : MonoBehaviour
    {
        /// <summary>
        /// 键盘输入总数
        /// </summary>
        // private static readonly int TotalKeyInputAmount = 100;

        /// <summary>
        /// 单例句柄
        /// </summary>
        protected static InputHandler instance;

        /// <summary>
        /// 单例句柄
        /// </summary>
        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {

                    GameObject obj = new()
                    {
                        name = typeof(InputHandler).Name
                    };
                    instance = obj.AddComponent<InputHandler>();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 键盘按键按下时的输入字典
        /// </summary>
        private Dictionary<KeyCode, Stack<IInputReceiver>> KeyPressedInput = new();

        /// <summary>
        /// 键盘按键松开时的输入字典
        /// </summary>
        private Dictionary<KeyCode, Stack<IInputReceiver>> KeyReleasedInput = new();


        /// <summary>
        /// 鼠标按键按下时的输入列表
        /// <para>
        /// 0   左键
        /// 1   右键
        /// 2   中键
        /// 3   无用
        /// </para>
        /// </summary>
        private List<IInputReceiver> MousePressedInput = Enumerable.Repeat<IInputReceiver>(null, 4).ToList();

        /// <summary>
        /// 鼠标按键松开时的输入列表
        /// <para>
        /// 0   左键
        /// 1   右键
        /// 2   中键
        /// 3   无用
        /// </para>
        /// </summary>
        private List<IInputReceiver> MouseReleasedInput = Enumerable.Repeat<IInputReceiver>(null, 4).ToList();
        
        /// <summary>
        /// 鼠标移动时的输入接受器
        /// </summary>
        private IInputReceiver MouseMoveInput = null;

        /// <summary>
        /// 鼠标滚轮的输入接收器
        /// </summary>
        private IInputReceiver MouseWheelInput = null;

        /// <summary>
        /// 绑定键盘输入
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        /// <param name="keyType">按键代码</param>
        /// <param name="isPressing">如果为<c>true</c>,绑定按下事件，<c>false</c>则绑定松开事件</param>
        public void BindKeyInputAction(IInputReceiver receiver, KeyCode keyType, bool isPressing = true)
        {
            if (isPressing)
            {
                if (!KeyPressedInput.ContainsKey(keyType))
                {
                    KeyPressedInput[keyType] = new Stack<IInputReceiver>();
                }

                if(receiver != null)
                {
                    KeyPressedInput[keyType].Push(receiver);
                }
                else
                {
                    KeyPressedInput[keyType].Pop();
                }

            }
            else
            {
                if (!KeyReleasedInput.ContainsKey(keyType))
                {
                    KeyReleasedInput[keyType] = new Stack<IInputReceiver>();
                }

                if (receiver != null)
                {
                    KeyReleasedInput[keyType].Push(receiver);
                }
                else
                {
                    KeyReleasedInput[keyType].Pop();
                }
            }
        }

        /// <summary>
        /// 公开给外部的绑定键盘方法，无需指定按下和松开类型，
        /// 因为按键有按下必有抬起，如果只需要按下事件则收到抬起事件时不处理即可
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        /// <param name="keyType">按键代码</param>
        public void BindKeyInput(IInputReceiver receiver, KeyCode keyType)
        {
            //if (receiver == null)
            //{
            //    Logger.LogError("InputHandler: Trying to bind " + keyType.ToString() + " to null object.");
            //    return;
            //}

            BindKeyInputAction(receiver, keyType);
            BindKeyInputAction(receiver, keyType, false);
        }

        /// <summary>
        /// 绑定鼠标输入
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        /// <param name="mouseType">按键代码</param>
        /// <param name="isPressing">如果为<c>true</c>,绑定按下事件，<c>false</c>则绑定松开事件</param>
        private void BindMouseInputAction(IInputReceiver receiver, int mouseType, bool isPressing = true)
        {
            if (mouseType > 2)
            {
                Logger.LogError("InputHandler: Trying to bind an invalid mouse button: " + mouseType.ToString());
                return;
            }

            if (isPressing)
            {
                MousePressedInput[mouseType] = receiver;
            }
            else
            {
                MouseReleasedInput[mouseType] = receiver;
            }
        }

        /// <summary>
        /// 公开给外部的绑定鼠标方法，无需指定按下和松开类型，
        /// 因为按键有按下必有抬起，如果只需要按下事件则收到抬起事件时不处理即可
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        /// <param name="mouseType">按键代码</param>
        public void BindMouseInput(IInputReceiver receiver, int mouseType)
        {
            BindMouseInputAction(receiver, mouseType);
            BindMouseInputAction(receiver, mouseType, false);
        }

        /// <summary>
        /// 绑定鼠标移动输入
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        public void BindMouseMoveInput(IInputReceiver receiver)
        {
            MouseMoveInput = receiver;
        }

        /// <summary>
        /// 绑定鼠标滚轮输入
        /// </summary>
        /// <param name="receiver">输入接收器</param>
        public void BindMouseWheelInput(IInputReceiver receiver)
        {
            MouseWheelInput = receiver;
        }

        // Update is called once per frame
        void Update()
        {
            CheckKeyDown();
            CheckKeyUp();
            CheckMouseDown();
            CheckMouseUp();

            // If mouse move, notify.
            float DeltaX = Input.GetAxis("Mouse X");
            float DeltaY = Input.GetAxis("Mouse Y");
            if (DeltaX != .0f || DeltaY != .0f)
            {
                MouseMoveInput?.NotifyMouseMove(DeltaX, DeltaY);
            }

            float MouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (MouseWheel != .0f)
            {
                MouseWheelInput?.NotifyMouseWheel(MouseWheel);
            }
        }

        /// <summary>
        /// 检查是否有键盘按下，如果有则通知对应接收器
        /// </summary>
        private void CheckKeyDown()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) &&                // If key down
                    KeyPressedInput.ContainsKey(key) &&     // Ignore unregistered keys
                    !key.ToString().Contains("Mouse"))      // Ignore mouse buttons
                {
                    if (KeyPressedInput[key].Any())
                    {
                        KeyPressedInput[key].Peek()?.NotifyKeyDown(key);
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否有键盘松开，如果有则通知对应接收器
        /// </summary>
        private void CheckKeyUp()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyUp(key) &&                  // If key up
                    KeyReleasedInput.ContainsKey(key) &&     // Ignore unregistered keys
                    !key.ToString().Contains("Mouse"))      // Ignore mouse buttons
                {
                    if (KeyReleasedInput[key].Any())
                    {
                        KeyReleasedInput[key].Peek()?.NotifyKeyUp(key);
                    }
                }
            }
        }

        /// <summary>
        /// 检查是否有鼠标按键按下，如果有则通知对应接收器
        /// </summary>
        private void CheckMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MousePressedInput[0]?.NotifyMouseDown(0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                MousePressedInput[1]?.NotifyMouseDown(1);
            }

            if (Input.GetMouseButtonDown(2))
            {
                MousePressedInput[2]?.NotifyMouseDown(2);
            }
        }

        /// <summary>
        /// 检查是否有鼠标按键松开，如果有则通知对应接收器
        /// </summary>
        private void CheckMouseUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                MouseReleasedInput[0]?.NotifyMouseUp(0);
            }

            if (Input.GetMouseButtonUp(1))
            {
                MouseReleasedInput[1]?.NotifyMouseUp(1);
            }

            if (Input.GetMouseButtonUp(2))
            {
                MouseReleasedInput[2]?.NotifyMouseUp(2);
            }
        }
    }
}
