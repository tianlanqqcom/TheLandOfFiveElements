/*
 * File: InputReceiver.cs
 * Description: 输入接受器，用于接收并处理对应的输入事件
 * Author: tianlan
 * Last update at 24/3/6    16:00
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/3/6  InputHandler已弃用，输入系统重构
 */

using System;
using UnityEngine;

namespace Script.GameFramework.Game
{
    [Obsolete("Use InputSystem instead of InputHandler.")]
    public interface IInputReceiver
    {
        /// <summary>
        /// 通知有按键按下
        /// </summary>
        /// <param name="pressedKey">被按下的按键代码</param>
        public void NotifyKeyDown(KeyCode pressedKey);

        /// <summary>
        /// 通知有按键松开
        /// </summary>
        /// <param name="releasedKey">被松开的按键代码</param>
        public void NotifyKeyUp(KeyCode releasedKey);

        /*
         * 鼠标按钮类型。
        0   左键
        1   右键
        2   中键
         */
        /// <summary>
        /// 通知有鼠标按键按下
        /// </summary>
        /// <param name="mouseButtonType"> 被按下的按键代码
        /// 鼠标按钮类型。
        /// 0   左键
        /// 1   右键
        /// 2   中键
        /// </param>
        public void NotifyMouseDown(int mouseButtonType);

        /// <summary>
        /// 通知有鼠标按键松开
        /// </summary>
        /// <param name="mouseButtonType"> 被松开的按键代码
        /// 鼠标按钮类型。
        /// 0   左键
        /// 1   右键
        /// 2   中键
        /// </param>
        public void NotifyMouseUp(int mouseButtonType);

        /// <summary>
        /// 通知鼠标移动
        /// </summary>
        /// <param name="DeltaX">鼠标在X方向上移动的距离</param>
        /// <param name="DeltaY">鼠标在Y方向上移动的距离</param>
        public void NotifyMouseMove(float DeltaX, float DeltaY);

        /// <summary>
        /// 通知鼠标滚轮
        /// </summary>
        /// <param name="Delta">鼠标滚轮滚动的距离</param>
        public void NotifyMouseWheel(float Delta);

        /// <summary>
        /// 注册输入事件
        /// </summary>
        public void RegisterInput();

        /// <summary>
        /// 清除自己的输入事件
        /// </summary>
        public void UnregisterInput();
    }
}


