/*
 * File: BagItemUI.cs
 * Description: 背包项UI控制组件
 * Author: tianlan
 * Last update at 24/3/6    20:42
 * 
 * Update Records:
 * tianlan  24/2/27 编写SetInventory方法
 * tianlan  24/3/6  背包物品类型由Inventory切换成MyInventory
 */

using GameFramework.Core;
using GameFramework.Game.Bag;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public class BagItemUI : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 物品缩略图
        /// </summary>
        Image itemThubnail;

        /// <summary>
        /// 物品数量
        /// </summary>
        TMP_Text itemCount;

        /// <summary>
        /// 当前物品
        /// </summary>
        MyInventory myInventory;

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <returns>初始化是否成功</returns>
        private bool Init()
        {
            Transform thubnail = transform.Find("ItemThubnail");
            if (thubnail != null)
            {
                itemThubnail = thubnail.GetComponent<Image>();
            }
            else
            {
                itemThubnail = null;
                Logger.LogError("BagItem:Start() thumbnail is null");
                return false;
            }

            Transform countTMPText = transform.Find("ItemCount");
            if (countTMPText != null)
            {
                itemCount = countTMPText.GetComponent<TMP_Text>();
            }
            else
            {
                itemCount = null;
                Logger.LogError("BagItem:Start() countTMPText is null");
                return false;
            }

            return true;
            // Logger.Log("BagItemUI:Init Success.");
        }

        /// <summary>
        /// 为背包项设置对应的物品，并在设置完成后通知背包系统
        /// </summary>
        /// <param name="inventory">目标物品</param>
        public void SetInventory(MyInventory inventory)
        {
            myInventory = inventory;
            if (Init())
            {
                itemCount.text = inventory.Count.ToString();

                FileLoaderAsync.Instance.LoadFileAsync(inventory.BaseInventory.ThubnailPath, SetSprite);
            }
            else
            {
                Logger.LogError("BagItemUI: Init failed.");
            }
        }

        public void SetInventoryWithoutAsync(MyInventory inventory)
        {
            myInventory = inventory;
            if (Init())
            {
                itemCount.text = inventory.Count.ToString();

                // FileLoaderAsync.Instance.LoadFileAsync(inventory.ThubnailPath, SetSprite);
            }
            else
            {
                Logger.LogError("BagItemUI: Init failed.");
            }
        }

        /// <summary>
        /// 设置精灵贴图
        /// </summary>
        /// <param name="data">图片数据</param>
        private void SetSprite(byte[] data)
        {
            BagSystem.Instance.NotifyItemLoaded();
            if (itemThubnail == null)
            {
                return;
            }

            Texture2D texture = new(256, 256);
            texture.LoadImage(data);

            itemThubnail.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 当被点击时，显示侧栏
        /// </summary>
        /// <param name="eventData">无用</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            BagSystemUI.Instance.ShowSideBar(myInventory);
        }
    }
}

