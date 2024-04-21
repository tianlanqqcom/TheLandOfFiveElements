/*
 * File: InteractiveUISelector.cs
 * Description: 交互系统UI选择项，主要是一个按钮。
 * Author: tianlan
 * Last update at 24/2/19 23:47
 * 
 * Update Records:
 * tianlan  24/2/19 编写代码主体
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.GameFramework.UI
{
    public class InteractiveUISelector : MonoBehaviour
    {
        /// <summary>
        /// 选项按钮
        /// </summary>
        [Tooltip("选项按钮")]
        public Button SelectorButton;

        /// <summary>
        /// 选项文本
        /// </summary>
        [Tooltip("选项文本")]
        public TMP_Text SelectorText;

        /// <summary>
        /// 设置选项文本
        /// </summary>
        /// <param name="message">目标文本</param>
        public void SetMessage(string message)
        {
            SelectorText.text = message;
        }
    }
}

