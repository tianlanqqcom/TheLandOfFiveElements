/*
 * File: BagSideBarUI.cs
 * Description: 背包侧栏控制组件，用于显示物品详细信息
 * Author: tianlan
 * Last update at 24/3/6    20:42
 * 
 * Update Records:
 * tianlan  24/2/28 编写SetInventory, SetBackground, SetSprite方法
 * tianlan  24/3/6  背包物品类型由Inventory切换成MyInventory
 */

using Script.GameFramework.Core;
using Script.GameFramework.Game.Bag;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Script.GameFramework.Game.Bag.Inventory;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class BagSideBarUI : MonoBehaviour
    {
        [Header("各种等级物品的颜色，具体值见Inspector面板")]
        [SerializeField]
        private Color White = new Color(251, 252, 252);

        [SerializeField]
        private Color Green = new Color(88, 214, 141);

        [SerializeField]
        private Color Blue = new Color(52, 152, 219);

        [SerializeField]
        private Color Purple = new Color(136, 78, 160);

        [SerializeField]
        private Color Orange = new Color(241, 196, 15);

        /// <summary>
        /// 侧栏是否已初始化
        /// </summary>
        bool hasInited = false;

        /// <summary>
        /// 原图背景
        /// </summary>
        Image rawImageBackground;

        /// <summary>
        /// 原图像
        /// </summary>
        Image rawImage;

        /// <summary>
        /// 物品名
        /// </summary>
        TMP_Text inventoryName;

        /// <summary>
        /// 物品描述
        /// </summary>
        TMP_Text inventoryDescription;

        /// <summary>
        /// 当前物品
        /// </summary>
        MyInventory nowInventory;

        /// <summary>
        /// 初始化侧边栏
        /// </summary>
        private void Init()
        {
            rawImage = transform.Find("InventoryRawImage").GetComponent<Image>();
            rawImageBackground = transform.Find("InventoryRawImageBackground").GetComponent<Image>();
            inventoryName = transform.Find("InventoryName").GetComponent<TMP_Text>();
            inventoryDescription = transform.Find("InventoryDescription").GetComponent <TMP_Text>();

            hasInited = true;

            Logger.Log("BagSideBarUI: Init");
        }

        /// <summary>
        /// 获取当前侧栏的物品
        /// </summary>
        /// <returns>当前侧栏的物品</returns>
        public MyInventory GetInventory()
        {
            return nowInventory;
        }

        /// <summary>
        /// 设置物品，当目标物品为null时隐藏侧栏
        /// </summary>
        /// <param name="inventory">目标物品</param>
        public void SetInventory(MyInventory inventory)
        {
            //if (nowInventory == inventory || inventory == null)
            //{
            //    nowInventory = null;
            //    BagSystemUI.Instance.DisableSideBarUI();
            //    return;
            //}

            // 如果为空，隐藏侧栏
            if(inventory == null)
            {
                nowInventory = null;
                BagSystemUI.Instance.DisableSideBarUI();
                return;
            }

            // 如果未初始化，初始化
            if (!hasInited)
            {
                Init();
            }

            // 设置侧栏属性
            nowInventory = inventory;
            inventoryName.text = inventory.BaseInventory.objectName.Message;
            inventoryDescription.text = inventory.BaseInventory.description.Message;
            SetBackgroundColor(inventory.BaseInventory.level);
            FileLoaderAsync.Instance.LoadFileAsync(inventory.BaseInventory.RawImagePath, SetSprite);
        }

        /// <summary>
        /// 设置背景色
        /// </summary>
        /// <param name="level">物品的等级</param>
        private void SetBackgroundColor(InventoryLevel level)
        {
            if(rawImageBackground == null)
            {
                Logger.LogError("BagSideBarUI:Set bgColor() bg is null");
                return;
            }

            switch (level)
            {
                case InventoryLevel.White: rawImageBackground.color = White; break;
                case InventoryLevel.Green: rawImageBackground.color = Green; break;
                case InventoryLevel.Blue: rawImageBackground.color = Blue; break;
                case InventoryLevel.Purple: rawImageBackground.color = Purple; break;
                case InventoryLevel.Gold: rawImageBackground.color = Orange; break;
                default: break;
            }
        }

        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="data">文件字节数据</param>
        private void SetSprite(byte[] data)
        {
            if (rawImage == null)
            {
                return;
            }

            Texture2D texture = new(256, 256);
            texture.LoadImage(data);

            rawImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // 加载完成，显示侧栏
            BagSystemUI.Instance.EnableSideBarUI();
        }
    }
}

