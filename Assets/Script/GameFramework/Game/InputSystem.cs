/*
 * File: InputSystem.cs
 * Description: 输入系统于24/3/6重构
 * Author: tianlan
 * Last update at 24/5/15   22:24
 * 
 * Update Records:
 * tianlan  24/3/6  输入系统重构
 * tianlan  24/5/15 开始定位BUG:输入系统切换场景后失效，先添加个控制名称检查的Flog试试。然后再加个清除所有输入？
 */

using System.Collections.Generic;
using System.Linq;
using Script.GameFramework.Core;
using UnityEditor;
using UnityEngine;

namespace Script.GameFramework.Game
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
    /// 鼠标事件委托,不论是按下还是抬起
    /// </summary>
    public delegate void MouseInputDelegate();

    /// <summary>
    /// 鼠标长按事件委托
    /// </summary>
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
        /// <summary>
        /// 是否启用绑定检查
        /// </summary>
        public bool bEnableFunctionNameCheck = true;
        
        // 长按事件标记阈值
        private static readonly float ShortestLongPressTime = 0.25f;

        private static bool bNeedToClearInput = false;
        
        private class KeyInputInfo
        {
            public KeyInputDelegate PressAction;

            public KeyInputDelegate ReleaseAction;

            public KeyLongPressedDelegate LongPressAction;

            public bool IsPressed = false;

            public float PressedTime = .0f;
        }

        private class MouseInputInfo
        {
            public MouseInputDelegate PressAction;
            public MouseInputDelegate ReleaseAction;
            public MouseLongPressDelegate LongPressAction;
            public bool IsPressed = false;
            public float PressedTime = .0f;
        }

        public enum InputEventType
        {
            Pressed,
            Released,
        }

        private Dictionary<KeyCode, KeyInputInfo> KeyInputAction = new();

        private Dictionary<int, MouseInputInfo> MouseInputAction = new();

        private MouseMoveDelegate _mouseMoveAction;

        private MouseWheelDelegate _mouseWheelAction;

        public void BindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
        {
            if (keyInputDelegate == null)
            {
                return;
            }

            if (!KeyInputAction.ContainsKey(key))
            {
                KeyInputInfo temp = new KeyInputInfo();

                switch (keyInputEventType)
                {
                    case InputEventType.Pressed:
                        temp.PressAction += keyInputDelegate;
                        break;
                    case InputEventType.Released:
                        temp.ReleaseAction += keyInputDelegate;
                        break;
                }

                KeyInputAction.Add(key, temp);
            }
            else
            {
                switch (keyInputEventType)
                {
                    case InputEventType.Pressed:

                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                            KeyInputAction[key].PressAction,
                            keyInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            KeyInputAction[key].PressAction += keyInputDelegate;
                        }

                        break;
                    case InputEventType.Released:
                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                            KeyInputAction[key].ReleaseAction,
                            keyInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            KeyInputAction[key].ReleaseAction += keyInputDelegate;
                        }


                        break;
                }
            }
        }

        public void UnbindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
        {
            if (keyInputDelegate == null)
            {
                return;
            }

            if (KeyInputAction.ContainsKey(key))
            {
                if (keyInputEventType == InputEventType.Pressed)
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        KeyInputAction[key].PressAction,
                        keyInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        KeyInputAction[key].PressAction -= keyInputDelegate;
                    }
                }
                else if (keyInputEventType == InputEventType.Released)
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        KeyInputAction[key].ReleaseAction,
                        keyInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        KeyInputAction[key].ReleaseAction -= keyInputDelegate;
                    }
                }

                if ((KeyInputAction[key].PressAction.GetInvocationList().Length +
                    KeyInputAction[key].ReleaseAction.GetInvocationList().Length +
                    KeyInputAction[key].LongPressAction.GetInvocationList().Length) == 0)
                {
                    KeyInputAction.Remove(key);
                }

            }
        }

        public void BindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
        {
            if (keyLongPressedDelegate == null)
            {
                return;
            }

            if (!KeyInputAction.ContainsKey(key))
            {
                KeyInputInfo temp = new()
                {
                    LongPressAction = keyLongPressedDelegate
                };

                KeyInputAction.Add(key, temp);
            }
            else
            {
                if (!DelegateChecker.IsDelegateContainsTargetFunction(
                    KeyInputAction[key].LongPressAction,
                    keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
                {
                    KeyInputAction[key].LongPressAction += keyLongPressedDelegate;
                }
            }
        }

        public void UnbindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
        {
            if (keyLongPressedDelegate == null)
            {
                return;
            }

            if (KeyInputAction.ContainsKey(key))
            {
                if (DelegateChecker.IsDelegateContainsTargetFunction(
                    KeyInputAction[key].LongPressAction,
                    keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
                {
                    KeyInputAction[key].LongPressAction -= keyLongPressedDelegate;
                }

                if ((KeyInputAction[key].PressAction.GetInvocationList().Length +
                    KeyInputAction[key].ReleaseAction.GetInvocationList().Length +
                    KeyInputAction[key].LongPressAction.GetInvocationList().Length) == 0)
                {
                    KeyInputAction.Remove(key);
                }

            }
        }

        public void BindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
        {
            if (mouseInputDelegate == null)
            {
                return;
            }

            if (!MouseInputAction.ContainsKey(button))
            {
                MouseInputInfo temp = new MouseInputInfo();
                switch (eventType)
                {
                    case InputEventType.Pressed:
                        temp.PressAction = mouseInputDelegate;
                        break;
                    case InputEventType.Released:
                        temp.ReleaseAction = mouseInputDelegate;
                        break;
                }

                MouseInputAction.Add(button, temp);
            }
            else
            {
                switch (eventType)
                {
                    case InputEventType.Pressed:
                    {
                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                                MouseInputAction[button].PressAction,
                                mouseInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            MouseInputAction[button].PressAction += mouseInputDelegate;
                        }

                        break;
                    }
                    case InputEventType.Released:
                    {
                        if (!DelegateChecker.IsDelegateContainsTargetFunction(
                                MouseInputAction[button].ReleaseAction,
                                mouseInputDelegate.GetInvocationList()[0].Method.Name))
                        {
                            MouseInputAction[button].ReleaseAction += mouseInputDelegate;
                        }

                        break;
                    }
                }
            }

        }

        public void UnbindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
        {
            if (mouseInputDelegate == null)
            {
                return;
            }

            if (eventType == InputEventType.Pressed)
            {
                if (MouseInputAction.ContainsKey(button))
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        MouseInputAction[button].PressAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        MouseInputAction[button].PressAction -= mouseInputDelegate;
                    }

                    if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
                        MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
                        MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
                    {
                        MouseInputAction.Remove(button);
                    }
                }
            }
            else if (eventType == InputEventType.Released)
            {
                if (MouseInputAction.ContainsKey(button))
                {
                    if (DelegateChecker.IsDelegateContainsTargetFunction(
                        MouseInputAction[button].ReleaseAction,
                        mouseInputDelegate.GetInvocationList()[0].Method.Name))
                    {
                        MouseInputAction[button].ReleaseAction -= mouseInputDelegate;
                    }

                    if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
                        MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
                        MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
                    {
                        MouseInputAction.Remove(button);
                    }
                }
            }
        }

        public void BindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
        {
            if (mouseLongPressDelegate == null)
            {
                return;
            }

            if (!MouseInputAction.ContainsKey(button))
            {
                MouseInputInfo temp = new MouseInputInfo
                {
                    LongPressAction = mouseLongPressDelegate
                };
                MouseInputAction.Add(button, temp);
            }
            else
            {
                if (!DelegateChecker.IsDelegateContainsTargetFunction(
                    MouseInputAction[button].LongPressAction,
                    mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
                {
                    MouseInputAction[button].LongPressAction += mouseLongPressDelegate;
                }
            }
        }

        public void UnbindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
        {
            if (mouseLongPressDelegate == null)
            {
                return;
            }

            if (MouseInputAction.ContainsKey(button))
            {
                if (DelegateChecker.IsDelegateContainsTargetFunction(
                    MouseInputAction[button].LongPressAction,
                    mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
                {
                    MouseInputAction[button].LongPressAction -= mouseLongPressDelegate;
                }

                if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
                    MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
                    MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
                {
                    MouseInputAction.Remove(button);
                }
            }
        }

        public void BindMouseMove(MouseMoveDelegate mouseMoveDelegate) 
        {
            if (mouseMoveDelegate == null)
            {
                return;
            }

            if (!DelegateChecker.IsDelegateContainsTargetFunction(
                _mouseMoveAction,
                mouseMoveDelegate.GetInvocationList()[0].Method.Name))
            {
                _mouseMoveAction += mouseMoveDelegate;
            }
        }

        public void UnbindMouseMove(MouseMoveDelegate mouseMoveDelegate)
        {
            if(mouseMoveDelegate == null)
            {
                return;
            }

            if (DelegateChecker.IsDelegateContainsTargetFunction(
                _mouseMoveAction,
                mouseMoveDelegate.GetInvocationList()[0].Method.Name))
            {
                _mouseMoveAction -= mouseMoveDelegate;
            }
        }

        public void BindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
        {
            if (mouseWheelDelegate == null)
            {
                return;
            }

            if (!DelegateChecker.IsDelegateContainsTargetFunction(
                _mouseWheelAction,
                mouseWheelDelegate.GetInvocationList()[0].Method.Name))
            {
                _mouseWheelAction += mouseWheelDelegate;
            }
        }

        public void UnbindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
        {
            if (mouseWheelDelegate == null)
            {
                return;
            }

            if (DelegateChecker.IsDelegateContainsTargetFunction(
                _mouseWheelAction,
                mouseWheelDelegate.GetInvocationList()[0].Method.Name))
            {
                _mouseWheelAction -= mouseWheelDelegate;
            }
        }

        /// <summary>
        /// Mark All Input Need to be cleared.
        /// </summary>
        public void ClearAllInput()
        {
            bNeedToClearInput = true;
        }

        private void Update()
        {
            if (bNeedToClearInput)
            {
                KeyInputAction.Clear();
                MouseInputAction.Clear();
                bNeedToClearInput = false;
                return;
            }
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
            foreach (var key in KeyInputAction.Keys.Where(Input.GetKeyDown))
            {
                KeyInputAction[key].PressAction?.Invoke();
                KeyInputAction[key].IsPressed = true;
                KeyInputAction[key].PressedTime = .0f;
            }
        }

        private void CheckKeyUp()
        {
            foreach (var key in KeyInputAction.Keys.Where(Input.GetKeyUp))
            {
                KeyInputAction[key].ReleaseAction?.Invoke();
                KeyInputAction[key].IsPressed = false;
                KeyInputAction[key].PressedTime = .0f;
            }
        }

        private void CheckKeyLongPressed()
        {
            foreach (var key in KeyInputAction.Keys.Where(key => KeyInputAction[key].IsPressed))
            {
                KeyInputAction[key].PressedTime += Time.deltaTime;
                    
                if (KeyInputAction[key].PressedTime > ShortestLongPressTime)
                {
                    KeyInputAction[key].LongPressAction?.Invoke(KeyInputAction[key].PressedTime);
                }
            }
        }

        private void CheckMouseDown()
        {
            foreach (var button in MouseInputAction.Keys.Where(Input.GetMouseButtonDown))
            {
                MouseInputAction[button].PressAction?.Invoke();
                MouseInputAction[button].IsPressed = true;
                MouseInputAction[button].PressedTime = .0f;
            }
        }

        private void CheckMouseUp()
        {
            foreach (var button in MouseInputAction.Keys.Where(Input.GetMouseButtonUp))
            {
                MouseInputAction[button].ReleaseAction?.Invoke();
                MouseInputAction[button].IsPressed = false;
                MouseInputAction[button].PressedTime = .0f;
            }
        }

        private void CheckMouseLongPress()
        {
            foreach (var button in MouseInputAction.Keys.Where(button => MouseInputAction[button].IsPressed))
            {
                MouseInputAction[button].PressedTime += Time.deltaTime;

                if (MouseInputAction[button].PressedTime > ShortestLongPressTime) 
                {
                    MouseInputAction[button].LongPressAction?.Invoke(MouseInputAction[button].PressedTime);
                }
            }
        }

        private void CheckMouseMove()
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");
            if (deltaX != .0f || deltaY != .0f)
            {
                _mouseMoveAction?.Invoke(deltaX, deltaY);
            }            
        }

        private void CheckMouseWheel()
        {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheel != .0f)
            {
                _mouseWheelAction?.Invoke(mouseWheel);
            }
        }

    }
    
    // public class InputSystem : SimpleSingleton<InputSystem>
    // {
    //     /// <summary>
    //     /// 是否启用绑定检查
    //     /// </summary>
    //     public bool bEnableFunctionNameCheck = true;
    //     
    //     // 长按事件标记阈值
    //     private static readonly float ShortestLongPressTime = 0.25f;
    //
    //     private static bool bNeedToClearInput = false;
    //     
    //     private class KeyInputInfo
    //     {
    //         public KeyInputDelegate PressAction;
    //
    //         public KeyInputDelegate ReleaseAction;
    //
    //         public KeyLongPressedDelegate LongPressAction;
    //
    //         public bool IsPressed = false;
    //
    //         public float PressedTime = .0f;
    //     }
    //
    //     private class MouseInputInfo
    //     {
    //         public MouseInputDelegate PressAction;
    //         public MouseInputDelegate ReleaseAction;
    //         public MouseLongPressDelegate LongPressAction;
    //         public bool IsPressed = false;
    //         public float PressedTime = .0f;
    //     }
    //
    //     public enum InputEventType
    //     {
    //         Pressed,
    //         Released,
    //     }
    //
    //     private static readonly Dictionary<KeyCode, KeyInputInfo> KeyInputAction = new();
    //
    //     private static readonly Dictionary<int, MouseInputInfo> MouseInputAction = new();
    //
    //     private static MouseMoveDelegate _mouseMoveAction;
    //
    //     private static MouseWheelDelegate _mouseWheelAction;
    //
    //     public static void BindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
    //     {
    //         if (keyInputDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!KeyInputAction.ContainsKey(key))
    //         {
    //             KeyInputInfo temp = new KeyInputInfo();
    //
    //             switch (keyInputEventType)
    //             {
    //                 case InputEventType.Pressed:
    //                     temp.PressAction += keyInputDelegate;
    //                     break;
    //                 case InputEventType.Released:
    //                     temp.ReleaseAction += keyInputDelegate;
    //                     break;
    //             }
    //
    //             KeyInputAction.Add(key, temp);
    //         }
    //         else
    //         {
    //             switch (keyInputEventType)
    //             {
    //                 case InputEventType.Pressed:
    //
    //                     if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                         KeyInputAction[key].PressAction,
    //                         keyInputDelegate.GetInvocationList()[0].Method.Name))
    //                     {
    //                         KeyInputAction[key].PressAction += keyInputDelegate;
    //                     }
    //
    //                     break;
    //                 case InputEventType.Released:
    //                     if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                         KeyInputAction[key].ReleaseAction,
    //                         keyInputDelegate.GetInvocationList()[0].Method.Name))
    //                     {
    //                         KeyInputAction[key].ReleaseAction += keyInputDelegate;
    //                     }
    //
    //
    //                     break;
    //             }
    //         }
    //     }
    //
    //     public static void UnbindKey(KeyCode key, InputEventType keyInputEventType, KeyInputDelegate keyInputDelegate)
    //     {
    //         if (keyInputDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (KeyInputAction.ContainsKey(key))
    //         {
    //             if (keyInputEventType == InputEventType.Pressed)
    //             {
    //                 if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                     KeyInputAction[key].PressAction,
    //                     keyInputDelegate.GetInvocationList()[0].Method.Name))
    //                 {
    //                     KeyInputAction[key].PressAction -= keyInputDelegate;
    //                 }
    //             }
    //             else if (keyInputEventType == InputEventType.Released)
    //             {
    //                 if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                     KeyInputAction[key].ReleaseAction,
    //                     keyInputDelegate.GetInvocationList()[0].Method.Name))
    //                 {
    //                     KeyInputAction[key].ReleaseAction -= keyInputDelegate;
    //                 }
    //             }
    //
    //             if ((KeyInputAction[key].PressAction.GetInvocationList().Length +
    //                 KeyInputAction[key].ReleaseAction.GetInvocationList().Length +
    //                 KeyInputAction[key].LongPressAction.GetInvocationList().Length) == 0)
    //             {
    //                 KeyInputAction.Remove(key);
    //             }
    //
    //         }
    //     }
    //
    //     public static void BindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
    //     {
    //         if (keyLongPressedDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!KeyInputAction.ContainsKey(key))
    //         {
    //             KeyInputInfo temp = new()
    //             {
    //                 LongPressAction = keyLongPressedDelegate
    //             };
    //
    //             KeyInputAction.Add(key, temp);
    //         }
    //         else
    //         {
    //             if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                 KeyInputAction[key].LongPressAction,
    //                 keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
    //             {
    //                 KeyInputAction[key].LongPressAction += keyLongPressedDelegate;
    //             }
    //         }
    //     }
    //
    //     public static void UnbindKeyLongPress(KeyCode key, KeyLongPressedDelegate keyLongPressedDelegate)
    //     {
    //         if (keyLongPressedDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (KeyInputAction.ContainsKey(key))
    //         {
    //             if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                 KeyInputAction[key].LongPressAction,
    //                 keyLongPressedDelegate.GetInvocationList()[0].Method.Name))
    //             {
    //                 KeyInputAction[key].LongPressAction -= keyLongPressedDelegate;
    //             }
    //
    //             if ((KeyInputAction[key].PressAction.GetInvocationList().Length +
    //                 KeyInputAction[key].ReleaseAction.GetInvocationList().Length +
    //                 KeyInputAction[key].LongPressAction.GetInvocationList().Length) == 0)
    //             {
    //                 KeyInputAction.Remove(key);
    //             }
    //
    //         }
    //     }
    //
    //     public static void BindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
    //     {
    //         if (mouseInputDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!MouseInputAction.ContainsKey(button))
    //         {
    //             MouseInputInfo temp = new MouseInputInfo();
    //             switch (eventType)
    //             {
    //                 case InputEventType.Pressed:
    //                     temp.PressAction = mouseInputDelegate;
    //                     break;
    //                 case InputEventType.Released:
    //                     temp.ReleaseAction = mouseInputDelegate;
    //                     break;
    //             }
    //
    //             MouseInputAction.Add(button, temp);
    //         }
    //         else
    //         {
    //             switch (eventType)
    //             {
    //                 case InputEventType.Pressed:
    //                 {
    //                     if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                             MouseInputAction[button].PressAction,
    //                             mouseInputDelegate.GetInvocationList()[0].Method.Name))
    //                     {
    //                         MouseInputAction[button].PressAction += mouseInputDelegate;
    //                     }
    //
    //                     break;
    //                 }
    //                 case InputEventType.Released:
    //                 {
    //                     if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                             MouseInputAction[button].ReleaseAction,
    //                             mouseInputDelegate.GetInvocationList()[0].Method.Name))
    //                     {
    //                         MouseInputAction[button].ReleaseAction += mouseInputDelegate;
    //                     }
    //
    //                     break;
    //                 }
    //             }
    //         }
    //
    //     }
    //
    //     public static void UnbindMouse(int button, InputEventType eventType, MouseInputDelegate mouseInputDelegate)
    //     {
    //         if (mouseInputDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (eventType == InputEventType.Pressed)
    //         {
    //             if (MouseInputAction.ContainsKey(button))
    //             {
    //                 if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                     MouseInputAction[button].PressAction,
    //                     mouseInputDelegate.GetInvocationList()[0].Method.Name))
    //                 {
    //                     MouseInputAction[button].PressAction -= mouseInputDelegate;
    //                 }
    //
    //                 if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
    //                     MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
    //                     MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
    //                 {
    //                     MouseInputAction.Remove(button);
    //                 }
    //             }
    //         }
    //         else if (eventType == InputEventType.Released)
    //         {
    //             if (MouseInputAction.ContainsKey(button))
    //             {
    //                 if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                     MouseInputAction[button].ReleaseAction,
    //                     mouseInputDelegate.GetInvocationList()[0].Method.Name))
    //                 {
    //                     MouseInputAction[button].ReleaseAction -= mouseInputDelegate;
    //                 }
    //
    //                 if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
    //                     MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
    //                     MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
    //                 {
    //                     MouseInputAction.Remove(button);
    //                 }
    //             }
    //         }
    //     }
    //
    //     public static void BindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
    //     {
    //         if (mouseLongPressDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!MouseInputAction.ContainsKey(button))
    //         {
    //             MouseInputInfo temp = new MouseInputInfo
    //             {
    //                 LongPressAction = mouseLongPressDelegate
    //             };
    //             MouseInputAction.Add(button, temp);
    //         }
    //         else
    //         {
    //             if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //                 MouseInputAction[button].LongPressAction,
    //                 mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
    //             {
    //                 MouseInputAction[button].LongPressAction += mouseLongPressDelegate;
    //             }
    //         }
    //     }
    //
    //     public static void UnbindMouseLongPress(int button, MouseLongPressDelegate mouseLongPressDelegate)
    //     {
    //         if (mouseLongPressDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (MouseInputAction.ContainsKey(button))
    //         {
    //             if (DelegateChecker.IsDelegateContainsTargetFunction(
    //                 MouseInputAction[button].LongPressAction,
    //                 mouseLongPressDelegate.GetInvocationList()[0].Method.Name))
    //             {
    //                 MouseInputAction[button].LongPressAction -= mouseLongPressDelegate;
    //             }
    //
    //             if ((MouseInputAction[button].PressAction.GetInvocationList().Length +
    //                 MouseInputAction[button].ReleaseAction.GetInvocationList().Length +
    //                 MouseInputAction[button].LongPressAction.GetInvocationList().Length) == 0)
    //             {
    //                 MouseInputAction.Remove(button);
    //             }
    //         }
    //     }
    //
    //     public static void BindMouseMove(MouseMoveDelegate mouseMoveDelegate) 
    //     {
    //         if (mouseMoveDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //             _mouseMoveAction,
    //             mouseMoveDelegate.GetInvocationList()[0].Method.Name))
    //         {
    //             _mouseMoveAction += mouseMoveDelegate;
    //         }
    //     }
    //
    //     public static void UnbindMouseMove(MouseMoveDelegate mouseMoveDelegate)
    //     {
    //         if(mouseMoveDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (DelegateChecker.IsDelegateContainsTargetFunction(
    //             _mouseMoveAction,
    //             mouseMoveDelegate.GetInvocationList()[0].Method.Name))
    //         {
    //             _mouseMoveAction -= mouseMoveDelegate;
    //         }
    //     }
    //
    //     public static void BindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
    //     {
    //         if (mouseWheelDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (!DelegateChecker.IsDelegateContainsTargetFunction(
    //             _mouseWheelAction,
    //             mouseWheelDelegate.GetInvocationList()[0].Method.Name))
    //         {
    //             _mouseWheelAction += mouseWheelDelegate;
    //         }
    //     }
    //
    //     public static void UnbindMouseWheel(MouseWheelDelegate mouseWheelDelegate)
    //     {
    //         if (mouseWheelDelegate == null)
    //         {
    //             return;
    //         }
    //
    //         if (DelegateChecker.IsDelegateContainsTargetFunction(
    //             _mouseWheelAction,
    //             mouseWheelDelegate.GetInvocationList()[0].Method.Name))
    //         {
    //             _mouseWheelAction -= mouseWheelDelegate;
    //         }
    //     }
    //
    //     /// <summary>
    //     /// Mark All Input Need to be cleared.
    //     /// </summary>
    //     public static void ClearAllInput()
    //     {
    //         bNeedToClearInput = true;
    //     }
    //
    //     private void Update()
    //     {
    //         if (bNeedToClearInput)
    //         {
    //             KeyInputAction.Clear();
    //             MouseInputAction.Clear();
    //             bNeedToClearInput = false;
    //             return;
    //         }
    //         CheckKeyDown();
    //         CheckKeyUp();
    //         CheckKeyLongPressed();
    //
    //         CheckMouseDown();
    //         CheckMouseUp();
    //         CheckMouseLongPress();
    //
    //         CheckMouseMove();
    //         CheckMouseWheel();
    //     }
    //
    //     private static void CheckKeyDown()
    //     {
    //         foreach (var key in KeyInputAction.Keys.Where(Input.GetKeyDown))
    //         {
    //             KeyInputAction[key].PressAction?.Invoke();
    //             KeyInputAction[key].IsPressed = true;
    //             KeyInputAction[key].PressedTime = .0f;
    //         }
    //     }
    //
    //     private static void CheckKeyUp()
    //     {
    //         foreach (var key in KeyInputAction.Keys.Where(Input.GetKeyUp))
    //         {
    //             KeyInputAction[key].ReleaseAction?.Invoke();
    //             KeyInputAction[key].IsPressed = false;
    //             KeyInputAction[key].PressedTime = .0f;
    //         }
    //     }
    //
    //     private static void CheckKeyLongPressed()
    //     {
    //         foreach (var key in KeyInputAction.Keys.Where(key => KeyInputAction[key].IsPressed))
    //         {
    //             KeyInputAction[key].PressedTime += Time.deltaTime;
    //                 
    //             if (KeyInputAction[key].PressedTime > ShortestLongPressTime)
    //             {
    //                 KeyInputAction[key].LongPressAction?.Invoke(KeyInputAction[key].PressedTime);
    //             }
    //         }
    //     }
    //
    //     private static void CheckMouseDown()
    //     {
    //         foreach (var button in MouseInputAction.Keys.Where(Input.GetMouseButtonDown))
    //         {
    //             MouseInputAction[button].PressAction?.Invoke();
    //             MouseInputAction[button].IsPressed = true;
    //             MouseInputAction[button].PressedTime = .0f;
    //         }
    //     }
    //
    //     private static void CheckMouseUp()
    //     {
    //         foreach (var button in MouseInputAction.Keys.Where(Input.GetMouseButtonUp))
    //         {
    //             MouseInputAction[button].ReleaseAction?.Invoke();
    //             MouseInputAction[button].IsPressed = false;
    //             MouseInputAction[button].PressedTime = .0f;
    //         }
    //     }
    //
    //     private static void CheckMouseLongPress()
    //     {
    //         foreach (var button in MouseInputAction.Keys.Where(button => MouseInputAction[button].IsPressed))
    //         {
    //             MouseInputAction[button].PressedTime += Time.deltaTime;
    //
    //             if (MouseInputAction[button].PressedTime > ShortestLongPressTime) 
    //             {
    //                 MouseInputAction[button].LongPressAction?.Invoke(MouseInputAction[button].PressedTime);
    //             }
    //         }
    //     }
    //
    //     private static void CheckMouseMove()
    //     {
    //         float deltaX = Input.GetAxis("Mouse X");
    //         float deltaY = Input.GetAxis("Mouse Y");
    //         if (deltaX != .0f || deltaY != .0f)
    //         {
    //             _mouseMoveAction?.Invoke(deltaX, deltaY);
    //         }            
    //     }
    //
    //     private static void CheckMouseWheel()
    //     {
    //         float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
    //         if (mouseWheel != .0f)
    //         {
    //             _mouseWheelAction?.Invoke(mouseWheel);
    //         }
    //     }
    //
    // }
}

