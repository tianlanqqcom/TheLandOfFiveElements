/*
 * File: InteractiveManager.cs
 * Description: 交互管理器，负责处理交互选项的加入与删除。
 * Author: tianlan
 * Last update at 24/2/18 20:25
 * 
 * Update Records:
 * tianlan  24/2/18 编写代码主体
 */

using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Log;
using Script.GameFramework.UI;

namespace Script.GameFramework.GamePlay.InteractiveSystem
{
    public class InteractiveManager : SimpleSingleton<InteractiveManager>
    {
        /// <summary>
        /// 交互项
        /// </summary>
        List<InteractiveItem> interactiveItems = new();

        /// <summary>
        /// 加入交互项
        /// </summary>
        /// <param name="item">交互项</param>
        public void AddInteractiveItem(InteractiveItem item)
        {
            Logger.Log("InteractiveManager:AddInteractiveItem");
            interactiveItems.Add(item);

            NotifyUIInteractiveInfoChanged();
        }

        /// <summary>
        /// 移除交互项
        /// </summary>
        /// <param name="item"></param>
        public void RemoveInteractiveItem(InteractiveItem item)
        {
            Logger.Log("InteractiveManager:RemoveInteractiveItem");
            interactiveItems.Remove(item);

            NotifyUIInteractiveInfoChanged();
        }

        /// <summary>
        /// 通知UI交互项信息改变
        /// </summary>
        public void NotifyUIInteractiveInfoChanged()
        {
            List<string> messageList = new ();
            foreach (InteractiveItem item in interactiveItems)
            {
                messageList.Add(item.FMessage.Message);
            }

            InteractiveUIManager.Instance.UpdateUIInfo(messageList);
        }

        /// <summary>
        /// 触发列表中对应物体的交互事件
        /// </summary>
        /// <param name="index">目标交互项的索引</param>
        public void InvokeInteractiveEvent(int index)
        {
            if(index >= 0 && index < interactiveItems.Count)
            {
                interactiveItems[index].ExecuteInteractiveAction();
            }
            else
            {
                Logger.LogError("InteractiveManager:InvokeInteractiveEvent() Index out of range, index = " + index +
                    " maxIndex = " + (interactiveItems.Count - 1));
                return;
            }

            RemoveInteractiveItem(interactiveItems[index]);
            NotifyUIInteractiveInfoChanged();
        }
    }
}


