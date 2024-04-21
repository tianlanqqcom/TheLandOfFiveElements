/*
 * File: BagTopBarUI.cs
 * Description: 背包顶栏控制组件
 * Author: tianlan
 * Last update at 24/2/28   22:15
 * 
 * Update Records:
 * tianlan  24/2/28 新建文件
 */

using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Game.Bag;
using TMPro;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class BagTopBarUI : MonoBehaviour
    {
        [Tooltip("顶栏文字")]
        public TMP_Text BagTitle;

        [Tooltip("字典文件")]
        public string MessageDictionary;

        [Tooltip("按钮下方的状态条")]
        public List<GameObject> ButtonBar = new ();

        public void SwitchType(int type)
        {
            if(type > (int)Inventory.InventoryType.All)
            {
                Logger.LogError("BagTopBarUI:SwitchType(int) type out of range");
                return;
            }
            SwitchType((Inventory.InventoryType)type);
            BagSystem.Instance.ShowBag(false);
        }

        public void SwitchType(Inventory.InventoryType type)
        {
            Inventory.InventoryType nowType = BagSystem.Instance.NowInventoryType;

            //if(nowType == type)
            //{
            //    return;
            //}

            FixedString fixedString = new ();
            fixedString.SetSourceMode(MessageLanguageSourceType.UseLanguageManager);
            fixedString.SetMessageDictionary(MessageDictionary);
            fixedString.SetMessageKey(type.ToString());

            BagTitle.text = fixedString.Message;

            BagSystem.Instance.NowInventoryType = type;

        }

        public void FlushBar(Inventory.InventoryType type)
        {
            int index = (int)type;

            foreach(GameObject item in ButtonBar)
            {
                item.SetActive(false);
            }
            ButtonBar[index].SetActive(true);
        }
    }
}

