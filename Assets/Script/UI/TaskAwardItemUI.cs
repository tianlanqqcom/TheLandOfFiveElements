/*
 * File: TaskAwardItemUI.cs
 * Description: 奖励项UI
 * Author: tianlan
 * Last update at 24/3/16   22：33
 * 
 * Update Records:
 * tianlan  24/3/16 新建文件
 */

using GameFramework.Core;
using GameFramework.Game.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameFramework.Game.Bag.Inventory;

namespace GameFramework.UI
{
    public class TaskAwardItemUI : MonoBehaviour
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

        [Header("物体绑定列表")]

        /// <summary>
        /// 背景色
        /// </summary>
        [Tooltip("背景色")]
        public Image Background;

        /// <summary>
        /// 缩略图
        /// </summary>
        [Tooltip("缩略图")]
        public Image AwardItemThubnail;

        /// <summary>
        /// 数量
        /// </summary>
        [Tooltip("数量")]
        public TMP_Text Count;

        /// <summary>
        /// 当前的奖励
        /// </summary>
        public TaskAward NowAward { get; private set; }

        /// <summary>
        /// 设置奖励
        /// </summary>
        /// <param name="taskAward">奖励</param>
        public void SetAward(TaskAward taskAward)
        {
            NowAward = taskAward;
            SetBackgroundColor(taskAward.BaseInventory.Level);
            Count.text = taskAward.Count.ToString();
            FileLoaderAsync.Instance.LoadFileAsync(taskAward.BaseInventory.RawImagePath, SetSprite);
        }

        /// <summary>
        /// 设置背景色
        /// </summary>
        /// <param name="level">物品的等级</param>
        private void SetBackgroundColor(InventoryLevel level)
        {
            if (Background == null)
            {
                Logger.LogError("TaskAwardItemUI:Set bgColor() bg is null");
                return;
            }

            switch (level)
            {
                case InventoryLevel.White: Background.color = White; break;
                case InventoryLevel.Green: Background.color = Green; break;
                case InventoryLevel.Blue: Background.color = Blue; break;
                case InventoryLevel.Purple: Background.color = Purple; break;
                case InventoryLevel.Gold: Background.color = Orange; break;
                default: break;
            }
        }

        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="data">文件字节数据</param>
        private void SetSprite(byte[] data)
        {
            if (AwardItemThubnail == null)
            {
                return;
            }

            Texture2D texture = new(100, 100);
            texture.LoadImage(data);

            AwardItemThubnail.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

    }
}

