/*
 * File: InteractiveItem.cs
 * Description: 可交互物体，具有至少一个触发器，当玩家进入触发器时显示交互选项。
 * Author: tianlan
 * Last update at 24/3/14   21:30
 * 
 * Update Records:
 * tianlan  24/2/18 编写代码主体
 * tianlan  24/2/20 添加自动播放选项
 * tianlan  24/2/21 添加语言切换选项，支持多语言或指定字符串
 * tianlan  24/2/26 将MessageSettings移动至GameFramework.Core并改名为MessageLanguageSourceType
 * tianlan  24/3/14 修改触发条件为CompareTag而不是判断是否拥有MyPlayerController组件
 * 
 */

using Script.GameFramework.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Script.GameFramework.GamePlay.InteractiveSystem
{
    [RequireComponent(typeof(Collider))]
    public class InteractiveItem : MonoBehaviour
    {
        ///// <summary>
        ///// 语言来源设置
        ///// </summary>
        //[Tooltip("语言来源设置")]
        //public MessageLanguageSourceType LanguageSettings;

        /// <summary>
        /// 交互选项显示的信息
        /// </summary>
        [Tooltip("交互选项显示的信息")]
        public FixedString FMessage;

        ///// <summary>
        ///// 文本字典名称
        ///// </summary>
        //[Tooltip("文本字典")]
        //public string MessageDictionary;

        ///// <summary>
        ///// 文本字典中对应的键
        ///// </summary>
        //[Tooltip("文本字典中对应的键")]
        //public string MessageKey;

        /// <summary>
        /// 是否启用
        /// </summary>
        [Tooltip("是否启用")]
        public bool IsEnable = true;

        /// <summary>
        /// 是否自动播放
        /// </summary>
        [Tooltip("是否自动播放")]
        public bool IsAutoPlay = false;

        /// <summary>
        /// 是否只触发一次
        /// </summary>
        [Tooltip("是否只触发一次")]
        public bool PlayOnce = false;

        /// <summary>
        /// 交互时触发的动作
        /// </summary>
        [Tooltip("交互时触发的动作")]
        public UnityEvent InteractiveAction = new UnityEvent();

        /// <summary>
        /// 是否已经触发
        /// </summary>
        private bool hasBeenPlayed = false;

        /// <summary>
        /// 执行交互动作
        /// </summary>
        public void ExecuteInteractiveAction()
        {
            InteractiveAction?.Invoke();
            hasBeenPlayed = true;
        }

        /// <summary>
        /// 当角色进入触发器范围时，将交互选项加入列表
        /// </summary>
        /// <param name="other">对方的触发器碰撞体</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!IsEnable || (PlayOnce && hasBeenPlayed))
            {
                return;
            }

            // Logger.Log("InteractiveItem:OnTriggerEnter");
            if (other.CompareTag("Player"))
            {
                if (IsAutoPlay)
                {
                    ExecuteInteractiveAction();
                    enabled = false;
                }
                else
                {
                    //if (LanguageSettings == MessageLanguageSourceType.UseLanguageManager)
                    //{
                    //    TextAsset messageDict = Resources.Load<TextAsset>(
                    //        LanguageManager.Instance.GetNowLanguageAssestsPath(MessageDictionary));

                    //    string jsonContent;
                    //    if (messageDict != null)
                    //    {
                    //        jsonContent = messageDict.text;
                    //    }
                    //    else
                    //    {
                    //        Logger.LogError("InteractiveItem: OnTriggerEnter() Target dictionary file doesn't exist.Path = " +
                    //            LanguageManager.Instance.GetNowLanguageAssestsPath(MessageDictionary));
                    //        return;
                    //    }

                    //    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                    //    if (dict.ContainsKey(MessageKey))
                    //    {
                    //        Message = dict[MessageKey];
                    //    }
                    //}
                    InteractiveManager.Instance.AddInteractiveItem(this);
                }
            }
            else
            {
                // Logger.Log("InteractiveItem:OnTriggerEnter Other.MyPlayerController is null");
            }
        }

        /// <summary>
        /// 当角色离开触发器范围时，将交互选项移除列表
        /// </summary>
        /// <param name="other">对方的触发器碰撞体</param>
        private void OnTriggerExit(Collider other)
        {
            if (!IsEnable || (PlayOnce && hasBeenPlayed))
            {
                return;
            }

            // Logger.Log("InteractiveItem:OnTriggerExit");
            if (other.CompareTag("Player"))
            {
                if (!IsAutoPlay)
                {
                    InteractiveManager.Instance.RemoveInteractiveItem(this);
                }
            }
            else
            {
                // Logger.Log("InteractiveItem:OnTriggerExit Other.MyPlayerController is null");
            }
        }
    }
}

