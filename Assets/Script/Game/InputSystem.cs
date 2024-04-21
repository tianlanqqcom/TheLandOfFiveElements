/*
 * File: InputSystem.cs
 * Description: 输入系统于24/3/6重构
 * Author: tianlan
 * Last update at 24/3/6 19:00
 * 
 * Update Records:
 * tianlan  24/3/6  输入系统重构
 */

using GameFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Game
{
    /// <summary>
    /// 键盘事件委托，不论是按下还是抬起
    /// </summary>
    public delegate void KeyInputDelegate();

    /// <summary>
    /// 按键长按委托
    /// </summary>
    /// <param name="time">该按键已按下的时间</param>
    public delegate void KeyLongPressedDelegate(float time);

    /// <summary>
    /// 鼠标事件委托，，不论是按下还是抬起
    /// </summary>
    public delegate void MouseInputDelegate();

    /// <summary>
    /// 鼠标长按事件委托
    /// </summary>
    /// <param name="button">按钮索引</param>
    /// <param name="time">按下的时间</param>
    public delegate void MouseLongPressDelegate(float time);

    /// <summary>
    /// 鼠标移动事件委托，，不论是按下还是抬起
    /// </summary>
    /// <param name="deltaX">鼠标在X方向上的移动值</param>
    /// <param name="deltaY">鼠标在Y方向上的移动值</param>
    public delegate void MouseMoveDelegate(float deltaX, float deltaY);

    /// <summary>
    /// 鼠标滚轮转动
    /// </summary>
    /// <param name="delta">转动的量</param>
    public delegate void MouseWheelDelegate(float delta);

    public class InputSystem : SimpleSingleton<InputSystem>
    {
        private class KeyInputInfo
        {
            public KeyInputDelegate pressAction;

            public KeyInputDelegate releaseAction;

            public KeyLongPressedDelegate longPressAction;

            public bool bIsPressed = false;

            public float pressedTime = .0f;
        }

        private class MouseInputInfo
        {
            public MouseInputDelegate pressAction;
            public MouseInputDelegate releaseAction;
            public MouseLongPressDelegate longPressAction;
            public bool bIsPressed = false;

            public float pressedTime = .0f;
        }

        public enum InputEventType
        {
            IE_Pressed,
            IE_Released,
        }

        private static readonly Dictionary<KeyCode, KeyInputInfo> keyInputAction = new();

        private static readonly Dictionary<int, MouseInputInfo> mouseInputAction = new();

        private static MouseMoveDelegate mouseMoveAction;

        private static MouseWheelDelegate mouseWheelAction;

        public static void BindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
        {
            if (keyInputDelegate == null)
            {
                return;
            }

            if (!keyInputAction.ContainsKey(key))
            {
                KeyInputInfo temp = new KeyInputInfo();

                switch (keyInputEventType)
                {
                    case InputEventType.IE_Pressed:
                        temp.pressAction += keyInputDelegate;
                        break;
                    case InputEventType.IE_Released:
                        temp.releaseAction += keyInputDelegate;
                        break;
                    default:
                        break;
                }

                keyInputAction.Add(key, temp);
            }
            else
            {
                switch (keyInputEventType)
                {
                    case InputEventType.IE_Pressed:

                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                            keyInputAction[key].pressAction,
                            keyInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            keyInputAction[key].pressAction += keyInputDelegate;
                        }

                        break;
                    case InputEventType.IE_Released:
                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                            keyInputAction[key].releaseAction,
                            keyInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            keyInputAction[key].releaseAction += keyInputDelegate;
                        }


                        break;
                    default:
                        break;
                }
            }
        }

        public static void UnbindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
        {
            if (keyInputDelegate == null)
            {
                return;
            }

            if (keyInputAction.ContainsKey(key))
            {
                if (keyInputEventType == InputEventType.IE_Pressed)
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        keyInputAction[key].pressAction,
                        keyInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        keyInputAction[key].pressAction -= keyInputDelegate;
                    }
                }
                else if (keyInputEventType == InputEventType.IE_Released)
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        keyInputAction[key].releaseAction,
                        keyInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        keyInputAction[key].releaseAction -= keyInputDelegate;
                    }
                }

                if ((keyInputAction[key].pressAction.GetInvocationList().Length +
                    keyInputAction[key].releaseAction.GetInvocationList().Length +
                    keyInputAction[key].longPressAction.GetInvocationList().Length) == 0)
                {
                    keyInputAction.Remove(key);
                }

            }
        }

        public static void BindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
        {
            if (keyLongPressedDelegate == null)
            {
                return;
            }

            if (!keyInputAction.ContainsKey(key))
            {
                KeyInputInfo temp = new();

                temp.longPressAction = keyLongPressedDelegate;

                keyInputAction.Add(key, temp);
            }
            else
            {
                if (!DelegateChecker.IsDelegateContainsTargetFunction(
                    keyInputAction[key].longPressAction,
                    keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
                {
                    keyInputAction[key].longPressAction += keyLongPressedDelegate;
                }
            }
        }

        public static void UnbindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
        {
            if (keyLongPressedDelegate == null)
            {
                return;
            }

            if (keyInputAction.ContainsKey(key))
            {
                if (DelegateChecker.IsDelegateContainsTargetFunction(
                    keyInputAction[key].longPressAction,
                    keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
                {
                    keyInputAction[key].longPressAction -= keyLongPressedDelegate;
                }

                if ((keyInputAction[key].pressAction.GetInvocationList().Length +
                    keyInputAction[key].releaseAction.GetInvocationList().Length +
                    keyInputAction[key].longPressAction.GetInvocationList().Length) == 0)
                {
                    keyInputAction.Remove(key);
                }

            }
        }

        public static void BindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
        {
            if (mouseInputDelegate == null)
            {
                return;
            }

            if (!mouseInputAction.ContainsKey(button))
            {
                MouseInputInfo temp = new MouseInputInfo();
                if (eventType == InputEventType.IE_Pressed)
                {
                    temp.pressAction = mouseInputDelegate;
                }
                else if (eventType == InputEventType.IE_Released)
                {
                    temp.releaseAction = mouseInputDelegate;
                }

                mouseInputAction.Add(button, temp);
            }
            else
            {
                if (eventType == InputEventType.IE_Pressed)
                {
                    if (!DelegateChecker.IsDelegateContainsTargetFunction(
                        mouseInputAction[button].pressAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        mouseInputAction[button].pressAction += mouseInputDelegate;
                    }
                }
                else if (eventType == InputEventType.IE_Released)
                {
                    if (!DelegateChecker.IsDelegateContainsTargetFunction(
                        mouseInputAction[button].releaseAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        mouseInputAction[button].releaseAction += mouseInputDelegate;
                    }
                }

            }

        }

        public static void UnbindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
        {
            if (mouseInputDelegate == null)
            {
                return;
            }

            if (eventType == InputEventType.IE_Pressed)
            {
                if (mouseInputAction.ContainsKey(button))
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        mouseInputAction[button].pressAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        mouseInputAction[button].pressAction -= mouseInputDelegate;
                    }

                    if ((mouseInputAction[button].pressAction.GetInvocationList().Length +
                        mouseInputAction[button].releaseAction.GetInvocationList().Length +
                        mouseInputAction[button].longPressAction.GetInvocationList().Length) == 0)
                    {
                        mouseInputAction.Remove(button);
                    }
                }
            }
            else if (eventType == InputEventType.IE_Released)
            {
                if (mouseInputAction.ContainsKey(button))
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        mouseInputAction[button].releaseAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        mouseInputAction[button].releaseAction -= mouseInputDelegate;
                    }

                    if ((mouseInputAction[button].pressAction.GetInvocationList().Length +
                        mouseInputAction[button].releaseAction.GetInvocationList().Length +
                        mouseInputAction[button].longPressAction.GetInvocationList().Length) == 0)
                    {
                        mouseInputAction.Remove(button);
                    }
                }
            }
        }

        public static void BindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
        {
            if (mouseLongPressDelegate == null)
            {
                return;
            }

            if (!mouseInputAction.ContainsKey(button))
            {
                MouseInputInfo temp = new MouseInputInfo();
                temp.longPressAction = mouseLongPressDelegate;
                mouseInputAction.Add(button, temp);
            }
            else
            {
                if (!DelegateChecker.IsDelegateContainsTargetFunction(
                    mouseInputAction[button].longPressAction,
                    mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
                {
                    mouseInputAction[button].longPressAction += mouseLongPressDelegate;
                }
            }
        }

        public static void UnbindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
        {
            if (mouseLongPressDelegate == null)
            {
                return;
            }

            if (mouseInputAction.ContainsKey(button))
            {
                if (DelegateChecker.IsDelegateContainsTargetFunction(
                    mouseInputAction[button].longPressAction,
                    mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
                {
                    mouseInputAction[button].longPressAction -= mouseLongPressDelegate;
                }

                if ((mouseInputAction[button].pressAction.GetInvocationList().Length +
                    mouseInputAction[button].releaseAction.GetInvocationList().Length +
                    mouseInputAction[button].longPressAction.GetInvocationList().Length) == 0)
                {
                    mouseInputAction.Remove(button);
                }
            }
        }

        public static void BindMouseMove(MouseMoveDelegate mouseMoveDelegate) 
        {
            if (mouseMoveDelegate == null)
            {
                return;
            }

            if (!DelegateChecker.IsDelegateContainsTargetFunction(
                mouseMoveAction,
                mouseMoveDelegate.GetInvocationList()[0].Method.Name))
            {
                mouseMoveAction += mouseMoveDelegate;
            }
        }

        public static void UnbindMouseMove(MouseMoveDelegate mouseMoveDelegate)
        {
            if(mouseMoveDelegate == null)
            {
                return;
            }

            if (DelegateChecker.IsDelegateContainsTargetFunction(
                mouseMoveAction,
                mouseMoveDelegate.GetInvocationList()[0].Method.Name))
            {
                mouseMoveAction -= mouseMoveDelegate;
            }
        }

        public static void BindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
        {
            if (mouseWheelDelegate == null)
            {
                return;
            }

            if (!DelegateChecker.IsDelegateContainsTargetFunction(
                mouseWheelAction,
                mouseWheelDelegate.GetInvocationList()[0].Method.Name))
            {
                mouseWheelAction += mouseWheelDelegate;
            }
        }

        public static void UnbindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
        {
            if (mouseWheelDelegate == null)
            {
                return;
            }

            if (DelegateChecker.IsDelegateContainsTargetFunction(
                mouseWheelAction,
                mouseWheelDelegate.GetInvocationList()[0].Method.Name))
            {
                mouseWheelAction -= mouseWheelDelegate;
            }
        }

        private void Update()
        {
            CheckKeyDown();
            CheckKeyUp();
            CheckKeyLongPressed();

            CheckMouseDown();
            CheckMouseUp();
            CheckMouseLongPress();

            CheckMouseMove();
            CheckMouseWheel();
        }

        private void CheckKeyDown()
        {
            foreach (var key in keyInputAction.Keys)
            {
                if (Input.GetKeyDown(key))
                {
                    keyInputAction[key].pressAction?.Invoke();
                    keyInputAction[key].bIsPressed = true;
                    keyInputAction[key].pressedTime = .0f;
                }
            }
        }

        private static void CheckKeyUp()
        {
            foreach (var key in keyInputAction.Keys)
            {
                if (Input.GetKeyUp(key))
                {
                    keyInputAction[key].releaseAction?.Invoke();
                    keyInputAction[key].bIsPressed = false;
                    keyInputAction[key].pressedTime = .0f;
                }
            }
        }

        private static void CheckKeyLongPressed()
        {
            foreach (var key in keyInputAction.Keys)
            {
                if (keyInputAction[key].bIsPressed)
                {
                    keyInputAction[key].pressedTime += Time.deltaTime;

                    if (keyInputAction[key].pressedTime > 0.25f)
                    {
                        keyInputAction[key].longPressAction?.Invoke(keyInputAction[key].pressedTime);
                    }
                }
            }
        }

        private static void CheckMouseDown()
        {
            foreach(var button in mouseInputAction.Keys)
            {
                if (Input.GetMouseButtonDown(button))
                {
                    mouseInputAction[button].pressAction?.Invoke();
                    mouseInputAction[button].bIsPressed = true;
                    mouseInputAction[button].pressedTime = .0f;
                }
            }
        }

        private static void CheckMouseUp()
        {
            foreach (var button in mouseInputAction.Keys)
            {
                if (Input.GetMouseButtonUp(button))
                {
                    mouseInputAction[button].releaseAction?.Invoke();
                    mouseInputAction[button].bIsPressed = false;
                    mouseInputAction[button].pressedTime = .0f;
                }
            }
        }

        private static void CheckMouseLongPress()
        {
            foreach (var button in mouseInputAction.Keys)
            {
                if (mouseInputAction[button].bIsPressed)
                {
                    mouseInputAction[button].pressedTime += Time.deltaTime;

                    if (mouseInputAction[button].pressedTime > 0.25f) 
                    {
                        mouseInputAction[button].longPressAction?.Invoke(mouseInputAction[button].pressedTime);
                    }
                }
            }
        }

        private static void CheckMouseMove()
        {
            float DeltaX = Input.GetAxis("Mouse X");
            float DeltaY = Input.GetAxis("Mouse Y");
            if (DeltaX != .0f || DeltaY != .0f)
            {
                mouseMoveAction?.Invoke(DeltaX, DeltaY);
            }            
        }

        private static void CheckMouseWheel()
        {
            float MouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (MouseWheel != .0f)
            {
                mouseWheelAction?.Invoke(MouseWheel);
            }
        }

    }
}

