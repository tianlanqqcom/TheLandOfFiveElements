/*
 * File: MessageLanguageSourceType.cs
 * Description: 游戏中字符串的来源（原生字符串或从指定Json中查找）
 * Author: tianlan
 * Last update at 24/2/18 20:25
 * 
 * Update Records:
 * tianlan  24/2/26 字符串来源
 */

using System;
using UnityEngine;

namespace GameFramework.Core
{
    /// <summary>
    /// 字符串信息来源
    /// </summary>
    [Serializable]
    public enum MessageLanguageSourceType
    {
        [InspectorName("使用原生文本")]
        UseNativeMessage,       // 直接使用填入的文本

        [InspectorName("使用语言管理器")]
        UseLanguageManager      // 使用语言管理器查找对应的文本字典并获取对应的文本
    }
}

