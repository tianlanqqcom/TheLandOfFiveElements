/*
 * File: BagSystem.cs
 * Description: 背包系统
 * Author: tianlan
 * Last update at 24/3/6    20:40
 * 
 * Update Records:
 * tianlan  24/2/28 新建文件
 * tianlan  24/3/1  完成主体结构
 * tianlan  24/3/6  使用新输入系统
 * tianlan  24/3/6  背包物品类型由Inventory切换成MyInventory
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.GameFramework.Core;
using Script.GameFramework.GamePlay;
using Script.GameFramework.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game.Bag
{
    public class BagSystem : SimpleSingleton<BagSystem>
    {
        /// <summary>
        /// 可用物品模板列表，在启动前把所有物品都先拽进来
        /// </summary>
        [FormerlySerializedAs("AvailableInventoriesTemplates")] [Tooltip("可用物品模板列表")]
        public List<Inventory> availableInventoriesTemplates = new();

        /// <summary>
        /// 可堆叠物品列表，以物品UID作区分
        /// </summary>
        private readonly Dictionary<int, MyInventory> _storedInventories = new();

        /// <summary>
        /// 不可堆叠物品列表
        /// </summary>
        private readonly List<MyInventory> _unstackedInventories = new();

        /// <summary>
        /// 记录需要加载图片文件的数量
        /// </summary>
        int _loadingFinishedCount;

        /// <summary>
        /// 当前物品类型
        /// </summary>
        public Inventory.InventoryType NowInventoryType { get; set; } = Inventory.InventoryType.Material;

        private void Start()
        {
            InputSystem.BindKey(KeyCode.B, InputSystem.InputEventType.Pressed, ShowBag_KeyB);
            InputSystem.BindKey(KeyCode.Escape, InputSystem.InputEventType.Pressed, HideBag_KeyESC);

            InitBag();
            // RegisterInput();
        }

        /// <summary>
        /// 初始化背包内容，当前为测试版，后续修改
        /// </summary>
        private void InitBag()
        {
            Logger.Log("BagSystem:Init Bag Test init.");
            _storedInventories.Clear();
            // inventories.Add(AvailableInventoriesTemplates[0]);
            for(int i = 0; i < 100; i++)
            {
                AddInventory(availableInventoriesTemplates[0]);
            }
        }

        /// <summary>
        /// 向背包中添加物品
        /// </summary>
        /// <param name="inventory">目标物品</param>
        public void AddInventory(Inventory inventory)
        {
            if (inventory.canBeStacked)
            {
                if(!_storedInventories.TryGetValue(inventory.uid, out var storedInventory))
                {
                    _storedInventories.Add(inventory.uid, new MyInventory(inventory));
                }
                else
                {
                    storedInventory.Count++;
                }
            }
            else
            {
                _unstackedInventories.Add(new MyInventory(inventory));
            }
        }

        /// <summary>
        /// 显示背包
        /// </summary>
        /// <param name="resetInput">是否需要重设输入</param>
        public void ShowBag(bool resetInput = true)
        {
            _loadingFinishedCount = 0;
            
            StartCoroutine(ShowBagUI(_storedInventories.Count + _unstackedInventories.Count, NowInventoryType, resetInput));
        }

        /// <summary>
        /// 隐藏背包并清除背包输入
        /// </summary>
        public void HideBag()
        {
            BagSystemUI.Instance.DisableUI();
            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Normal_Game);
            //RegisterInput();
            //InputHandler.Instance.BindKeyInputAction(null, KeyCode.Escape);
        }

        /// <summary>
        /// 扔掉一个当前侧栏正在显示的物品
        /// </summary>
        public void ThrowItem()
        {
            MyInventory inventory = BagSystemUI.Instance.GetNowSideBarInventory();

            if (inventory == null)
            {
                Logger.Log("BagSystem:ThrowItem() target item is null, please select an item first.");
                return;
            }

            if (inventory.BaseInventory.canBeStacked)
            {
                _storedInventories[inventory.BaseInventory.uid].Count--;
                if(_storedInventories[inventory.BaseInventory.uid].Count <= 0)
                {
                    _storedInventories.Remove(inventory.BaseInventory.uid);
                }
            }
            else
            {
                _unstackedInventories.Remove(inventory);
            }

            ShowBag(false);
        }

        /// <summary>
        /// 显示背包UI具体处理过程
        /// </summary>
        /// <param name="maxItemCount">需要加载的最大数量</param>
        /// <param name="type">物品类型</param>
        /// <param name="resetInput">是否重设输入,Warning:resetInput is unused</param>
        /// <returns></returns>
        private IEnumerator ShowBagUI(int maxItemCount, Inventory.InventoryType type, bool resetInput = true)
        {
            if(!BagSystemUI.Instance)
            {
                Logger.LogError("BagSystem: BagSystemUI is null.");
                yield break;
            }

            // 对背包物品排序
            //unstackedInventories.Sort(delegate(MyInventory a, MyInventory b)
            //{
            //    if(a == null && b == null)
            //    {
            //        return 0;
            //    }

            //    if(a == null)
            //    {
            //        return -1;
            //    }

            //    if(b == null)
            //    {
            //        return 1;
            //    }


            //    return a.UID.CompareTo(b.UID);
            //});

            // 开始加载图片
            BagSystemUI.Instance.BeginLoadingBag(_storedInventories.Values.ToList().Concat(_unstackedInventories).ToList(), type);

            // 启动计时器
            // float timer = 0f;
            // yield return 1;
            
            // 设置背包背景
            BagSystemUI.Instance.SetBackgroundSprite();

            // 当计时器大于3s或加载完毕时结束
            //while (timer < 3.0f && loadingFinishedCount < maxItemCount)
            //{
            //    timer += Time.deltaTime;
            //    yield return 1;
            //}

            // 显示UI
            BagSystemUI.Instance.EnableUI();

            // 设置工作模式
            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Pure_UI);

            // 重设输入
            //if (resetInput)
            //{
            //    UnregisterInput();
            //    InputHandler.Instance.BindKeyInput(this, KeyCode.Escape);
            //}
        }

        /// <summary>
        /// 当加载完成时通知完成数加1
        /// </summary>
        public void NotifyItemLoaded()
        {
            _loadingFinishedCount++;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        public void UseInventory()
        {
            MyInventory inventory = BagSystemUI.Instance.GetNowSideBarInventory();
            if (inventory == null)
            {
                Logger.LogError("BagSystem:UseInventory() Target inventory is null");
                return;
            }

            inventory.BaseInventory.InvokeUseFunctions();
        }

        public void NotifyKeyDown(KeyCode pressedKey)
        {
            if(pressedKey == KeyCode.B)
            {
                ShowBag();
            }
            else if(pressedKey == KeyCode.Escape)
            {
                HideBag();
            }
        }

        public void NotifyKeyUp(KeyCode releasedKey)
        {
            if(releasedKey == KeyCode.B)
            {
                Logger.Log("BagSystem: B released");
            }
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

        [Obsolete("Register Input is unused.")]
        public void RegisterInput()
        {
            // InputHandler.Instance.BindKeyInputAction(this, KeyCode.B);
        }

        [Obsolete("UnRegister Input is unused.")]
        public void UnregisterInput()
        {
            // InputHandler.Instance.BindKeyInputAction(null, KeyCode.B);
        }

        private void ShowBag_KeyB()
        {
            if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game)
            {
                ShowBag();
            }
        }

        private void HideBag_KeyESC()
        {
            if (BagSystemUI.Instance.IsBagUIShown)
            {
                HideBag();
            }
        }

        public void ShowBagWithoutASync()
        {
            ShowBag();
        }
    }
}

