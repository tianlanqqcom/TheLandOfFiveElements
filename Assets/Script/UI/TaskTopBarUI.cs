/*
 * File: TaskTopBarUI.cs
 * Description: 任务控制组件
 * Author: tianlan
 * Last update at 24/3/15   21:04
 * 
 * Update Records:
 * tianlan  24/3/15 新建文件
 */

using GameFramework.Core;
using TMPro;
using UnityEngine;

namespace GameFramework.UI
{
    public class TaskTopBarUI : MonoBehaviour
    {
        /// <summary>
        /// 顶栏文字
        /// </summary>
        [Tooltip("顶栏文字")]
        public TMP_Text TaskTitle;

        /// <summary>
        /// 字典文件,默认为TaskType，Key默认为Header
        /// </summary>
        [Tooltip("字典文件")]
        public string MessageDictionary = "TaskType";

        /// <summary>
        /// 当打开任务面板的时候，初始化顶栏上的文字
        /// </summary>
        public void OnOpen()
        {
            FixedString fixedString = new();
            fixedString.SetSourceMode(MessageLanguageSourceType.UseLanguageManager);
            fixedString.SetMessageDictionary(MessageDictionary);
            fixedString.SetMessageKey("Header");

            TaskTitle.text = fixedString.Message;
        }
    }
}

