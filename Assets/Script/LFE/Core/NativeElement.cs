/*
 * File: NativeElement.cs
 * Description: 玩家或怪物所有可能的属性
 * Author: tianlan
 * Last update at 24/4/22   23:00
 *
 * Update Records:
 * tianlan  24/4/22
 */

using System;

namespace Script.LFE.Core
{
    /// <summary>
    /// 玩家或怪物所有可能的属性
    /// </summary>
    [Flags]
    public enum NativeElement
    {
        None = 0,
        Jin = 1, // 金
        Mu = 2, // 木
        Shui = 4, // 水
        Huo = 8, // 火
        Tu = 16 // 土
    }
}