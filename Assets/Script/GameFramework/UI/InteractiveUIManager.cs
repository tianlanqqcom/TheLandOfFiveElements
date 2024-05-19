/*
 * File: InteractiveUIManager.cs
 * Description: 交互系统UI管理器，负责处理交互选项的UI显示。
 * Author: tianlan
 * Last update at 24/2/19 19:34
 * 
 * Update Records:
 * tianlan  24/2/18 编写代码主体
 * tianlan  24/2/19 完善UpdateUIInfo
 * tianlan  24/3/6  使用新输入系统
 */

using System;
using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using Script.GameFramework.GamePlay;
using Script.GameFramework.GamePlay.InteractiveSystem;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class InteractiveUIManager : SimpleSingleton<InteractiveUIManager>
    {
        [Header("运行时必要逻辑项")]
        /// <summary>
        /// 当前最上方选项的索引值
        /// </summary>
        [SerializeField]
        [Tooltip("当前最上方选项的索引值")]
        private int nowTopItemIndex;

        /// <summary>
        /// 当前指针位置
        /// </summary>
        [SerializeField]
        [Tooltip("当前指针位置")]
        [Range(0, 3)]
        private int nowPointerIndex;

        /// <summary>
        /// 交互UI根物体
        /// </summary>
        [Tooltip("交互UI根物体")]
        public GameObject InteractiveUIRoot;

        /// <summary>
        /// 指针对应的物体
        /// </summary>
        [Tooltip("指针对应的物体")]
        public GameObject PointerObject;

        /// <summary>
        /// 向上的箭头，表示上方还有交互项
        /// </summary>
        [Tooltip("向上的箭头，表示上方还有交互项")]
        public GameObject InteractiveUIUpperTip;

        /// <summary>
        /// 向下的箭头，表示下方还有交互项
        /// </summary>
        [Tooltip("向下的箭头，表示下方还有交互项")]
        public GameObject InteractiveUILowerTip;

        /// <summary>
        /// 显示的选项列表，应在编辑器中预配置好，注意顺序不要乱
        /// </summary>
        [Tooltip("显示的选项列表，应在编辑器中预配置好,注意顺序不要乱")]
        public List<GameObject> InteractiveUIItemList;

        /// <summary>
        /// 指针的位置坐标，长度应与InteractiveUIItemList一致
        /// </summary>
        [Tooltip("指针的位置坐标，长度应与InteractiveUIItemList一致")]
        public List<RectTransform> PointerPositions;

        [Header("美术表现")]

        /// <summary>
        /// 背景图
        /// </summary>
        [Tooltip("背景图")]
        public Sprite BachgroundImage;

        /// <summary>
        /// 背景颜色
        /// </summary>
        [Tooltip("背景颜色")]
        public Color BackgroundColor;

        /// <summary>
        /// 按钮图片
        /// </summary>
        [Tooltip("按钮图片")]
        public Sprite ButtonImage;

        /// <summary>
        /// 按钮颜色
        /// </summary>
        [Tooltip("按钮颜色")]
        public Color ButtonColor;

        /// <summary>
        /// 指针图片
        /// </summary>
        [Tooltip("指针图片")]
        public Sprite PointerImage;

        /// <summary>
        /// 指针颜色
        /// </summary>
        [Tooltip("指针颜色")]
        public Color PointerColor;

        /// <summary>
        /// 上箭头图片
        /// </summary>
        [Tooltip("上箭头图片")]
        public Sprite UpArrowImage;

        /// <summary>
        /// 下箭头图片
        /// </summary>
        [Tooltip("下箭头图片")]
        public Sprite DownArrowImage;

        /// <summary>
        /// 缓存交互信息
        /// </summary>
        private List<string> interactiveMessagesCache = new();

        protected override void Awake()
        {
            base.Awake();
            if (InteractiveUIItemList.Count != PointerPositions.Count)
            {
                Logger.LogError("InteractiveUIManager:Awake() Length of InteractiveUIItemList and PointerPositions are not equal, check in editor to ensure these correct.");
                enabled = false;
            }
        }

        private void Start()
        {
            // Bind input
            InputSystem.Instance?.BindKey(KeyCode.F, InputSystem.InputEventType.Pressed, InteractiveWithTargetItem);
            InputSystem.Instance?.BindKey(KeyCode.UpArrow, InputSystem.InputEventType.Pressed, PointerUp);
            InputSystem.Instance?.BindKey(KeyCode.DownArrow, InputSystem.InputEventType.Pressed, PointerDown);

            if (BachgroundImage != null)
            {
                InteractiveUIRoot.GetComponent<Image>().sprite = BachgroundImage;
            }

            if (ButtonImage != null)
            {
                foreach (GameObject item in InteractiveUIItemList)
                {
                    item.GetComponent<Image>().sprite = ButtonImage;
                }
            }

            if (PointerImage != null)
            {
                PointerObject.GetComponent<Image>().sprite = PointerImage;
            }

            if (UpArrowImage != null)
            {
                InteractiveUIUpperTip.GetComponent<Image>().sprite = UpArrowImage;
            }

            if (DownArrowImage != null)
            {
                InteractiveUILowerTip.GetComponent<Image>().sprite = DownArrowImage;
            }

            if (BackgroundColor != null)
            {
                InteractiveUIRoot.GetComponent<Image>().color = BackgroundColor;
            }

            if (ButtonColor != null)
            {
                foreach (GameObject item in InteractiveUIItemList)
                {
                    item.GetComponent<Image>().color = ButtonColor;
                }
            }

            if (PointerColor != null)
            {
                PointerObject.GetComponent<Image>().color = PointerColor;
            }
        }

        /// <summary>
        /// 更新交互项信息
        /// </summary>
        /// <param name="messages">信息列表</param>
        public void UpdateUIInfo(List<string> messages)
        {
            interactiveMessagesCache = messages;

            int len = messages.Count;

            // If list is empty, disable all
            if (len == 0)
            {
                // Disable all ui objects
                DisableUI();

                // Reset pointer index and top index
                nowPointerIndex = 0;
                nowTopItemIndex = 0;
                return;
            }

            // If length is less than the max UI objects count
            if (len >= 1 && len <= InteractiveUIItemList.Count)
            {
                // Enable InteractiveSystem UI
                EnableUI();

                // Clear top index
                nowTopItemIndex = 0;

                // Set pointer index, to avoid pointer out of range
                nowPointerIndex = MathLibrary.Clamp(nowPointerIndex, 0, len - 1);
                PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;

                // Disable UpperTip and LowerTip
                InteractiveUIUpperTip.SetActive(false);
                InteractiveUILowerTip.SetActive(false);

                // Disable all selectors
                foreach (GameObject item in InteractiveUIItemList)
                {
                    item.SetActive(false);
                }

                // Enable selectors according to the length of messages, and set message meanwhile
                for (int i = 0; i < len; i++)
                {
                    InteractiveUIItemList[i].SetActive(true);
                    InteractiveUIItemList[i].GetComponent<InteractiveUISelector>().SetMessage(messages[i]);
                }

                return;
            }

            // If length is greater than InteractiveUIItemList
            if (len > InteractiveUIItemList.Count)
            {
                // Enable InteractiveSystem UI
                EnableUI();

                // Restrict nowTopItemIndex between 0 and len - InteractiveUIItemList.Count
                // e.g. if len = 5, InteractiveUIItemList.Count = 4, nowTopItemIndex should be 0 or 1
                nowTopItemIndex = MathLibrary.Clamp(nowTopItemIndex, 0, len - InteractiveUIItemList.Count);

                // If there are items above, enable InteractiveUIUpperTip
                if (nowTopItemIndex > 0)
                {
                    InteractiveUIUpperTip.SetActive(true);
                }
                else
                {
                    InteractiveUIUpperTip.SetActive(false);
                }

                // If there are items below, enable nowTopItemIndex
                if (nowTopItemIndex < len - InteractiveUIItemList.Count)
                {
                    InteractiveUILowerTip.SetActive(true);
                }
                else
                {
                    InteractiveUILowerTip.SetActive(false);
                }

                // Set pointer index, to avoid pointer out of range
                nowPointerIndex = MathLibrary.Clamp(nowPointerIndex, 0, InteractiveUIItemList.Count);
                PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;

                // Enable selectors according to the length of messages, and set message meanwhile
                for (int i = 0; i < InteractiveUIItemList.Count; i++)
                {
                    InteractiveUIItemList[i].SetActive(true);
                    InteractiveUIItemList[i].GetComponent<InteractiveUISelector>().SetMessage(messages[nowTopItemIndex + i]);
                }

                return;
            }

            return;
        }

        public void InvokeInteractiveEvent(int index)
        {
            InteractiveManager.Instance.InvokeInteractiveEvent(nowTopItemIndex + index);
        }

        public void NotifyKeyDown(KeyCode pressedKey)
        {
            if(MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                return;
            }

            if (pressedKey == KeyCode.F)
            {
                Logger.Log("InteractiveUIManager:NotifyKeyDown() F Pressed.");

                if (InteractiveUIRoot.activeSelf == true)
                {
                    InvokeInteractiveEvent(nowPointerIndex);
                }
            }
            else if (pressedKey == KeyCode.UpArrow)
            {
                Logger.Log("InteractiveUIManager:NotifyKeyDown() UpArrow Pressed.");

                if (InteractiveUIRoot.activeSelf == false)
                {
                    return;
                }

                if (nowPointerIndex > 0)
                {
                    nowPointerIndex--;
                    PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
                }
                else if (nowPointerIndex == 0)
                {
                    if (nowTopItemIndex > 0)
                    {
                        nowTopItemIndex--;
                        UpdateUIInfo(interactiveMessagesCache);
                    }
                    else
                    {
                        ;   // Ignore
                    }
                }
            }
            else if (pressedKey == KeyCode.DownArrow)
            {
                Logger.Log("InteractiveUIManager:NotifyKeyDown() DownArrow Pressed.");

                if (InteractiveUIRoot.activeSelf == false)
                {
                    return;
                }

                if (nowPointerIndex < InteractiveUIItemList.Count - 1 && nowTopItemIndex != 0)
                {
                    nowPointerIndex++;
                    PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
                }
                else if (nowPointerIndex < InteractiveUIItemList.Count - 1 && nowTopItemIndex == 0)
                {
                    if (nowPointerIndex < interactiveMessagesCache.Count - 1)
                    {
                        nowPointerIndex++;
                        PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
                    }
                    else
                    {
                        ;   // Ignore
                    }
                }
                else if (nowPointerIndex == InteractiveUIItemList.Count - 1)
                {
                    if (nowPointerIndex == interactiveMessagesCache.Count - 1)
                    {
                        ;   // Ignore
                    }
                    else
                    {
                        nowTopItemIndex++;
                        UpdateUIInfo(interactiveMessagesCache);
                    }
                }
            }
        }

        public void EnableUI()
        {
            InteractiveUIRoot.SetActive(true);
            // BindUIInput();
        }

        public void DisableUI()
        {
            InteractiveUIRoot.SetActive(false);
            // BindUIInput(false);
        }

        [Obsolete("Bind UI Input is unused now.")]
        private void BindUIInput(bool enableInput = true)
        {
            if (enableInput)
            {
                //InputHandler.Instance.BindKeyInput(this, KeyCode.F);
                //InputHandler.Instance.BindKeyInput(this, KeyCode.UpArrow);
                //InputHandler.Instance.BindKeyInput(this, KeyCode.DownArrow);
            }
            else
            {
                //InputHandler.Instance.BindKeyInput(null, KeyCode.F);
                //InputHandler.Instance.BindKeyInput(null, KeyCode.UpArrow);
                //InputHandler.Instance.BindKeyInput(null, KeyCode.DownArrow);
            }
        }

        public void NotifyKeyUp(KeyCode releasedKey)
        {
            ;
        }

        public void NotifyMouseDown(int mouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseUp(int mouseButtonType)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseMove(float DeltaX, float DeltaY)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyMouseWheel(float Delta)
        {
            throw new System.NotImplementedException();
        }

        public void EmptyEventTest()
        {
            Logger.Log("Event invoked");
        }

        public void RegisterInput()
        {
            throw new System.NotImplementedException();
        }

        public void UnregisterInput()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 与物品交互，按F键
        /// </summary>
        private void InteractiveWithTargetItem()
        {
            if (MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                return;
            }


            if (InteractiveUIRoot.activeSelf == true)
            {
                InvokeInteractiveEvent(nowPointerIndex);
            }
        }

        /// <summary>
        /// 指针上移，上箭头
        /// </summary>
        private void PointerUp()
        {
            if (MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                return;
            }

            if (InteractiveUIRoot.activeSelf == false)
            {
                return;
            }

            if (nowPointerIndex > 0)
            {
                nowPointerIndex--;
                PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
            }
            else if (nowPointerIndex == 0)
            {
                if (nowTopItemIndex > 0)
                {
                    nowTopItemIndex--;
                    UpdateUIInfo(interactiveMessagesCache);
                }
                else
                {
                    ;   // Ignore
                }
            }
        }

        /// <summary>
        /// 指针下移，下箭头
        /// </summary>
        private void PointerDown()
        {
            if (MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                return;
            }

            if (InteractiveUIRoot.activeSelf == false)
            {
                return;
            }

            if (nowPointerIndex < InteractiveUIItemList.Count - 1 && nowTopItemIndex != 0)
            {
                nowPointerIndex++;
                PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
            }
            else if (nowPointerIndex < InteractiveUIItemList.Count - 1 && nowTopItemIndex == 0)
            {
                if (nowPointerIndex < interactiveMessagesCache.Count - 1)
                {
                    nowPointerIndex++;
                    PointerObject.GetComponent<RectTransform>().position = PointerPositions[nowPointerIndex].position;
                }
                else
                {
                    ;   // Ignore
                }
            }
            else if (nowPointerIndex == InteractiveUIItemList.Count - 1)
            {
                if (nowPointerIndex == interactiveMessagesCache.Count - 1)
                {
                    ;   // Ignore
                }
                else
                {
                    nowTopItemIndex++;
                    UpdateUIInfo(interactiveMessagesCache);
                }
            }
        }
    }
}

