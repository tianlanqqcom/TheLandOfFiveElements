/*
 * File: BagSystemUI.cs
 * Description: 背包UI主体控制组件
 * Author: tianlan
 * Last update at 24/3/6    20:42
 * 
 * Update Records:
 * tianlan  24/2/28 新建文件
 * tianlan  24/3/6  背包物品类型由Inventory切换成MyInventory
 */

using GameFramework.Core;
using GameFramework.Game.Bag;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public class BagSystemUI : SimpleSingleton<BagSystemUI>
    {
        /// <summary>
        /// 背包UI根物体
        /// </summary>
        [Tooltip("背包UI根物体")]
        public GameObject BagFullUI;

        /// <summary>
        /// 物品预制件
        /// </summary>
        [Tooltip("物品预制件")]
        public GameObject BagItem;

        /// <summary>
        /// 侧栏预制件
        /// </summary>
        [Tooltip("侧栏预制件")]
        public GameObject BagSideBar;

        /// <summary>
        /// 顶栏预制件
        /// </summary>
        [Tooltip("顶栏预制件")]
        public GameObject BagTopBar;

        /// <summary>
        /// 使用按钮
        /// </summary>
        [Tooltip("使用按钮")]
        public GameObject UseBtn;

        /// <summary>
        /// 内容根目录
        /// </summary>
        [Tooltip("内容根目录")]
        public Transform Content;

        /// <summary>
        /// 背景图高斯模糊大小
        /// </summary>
        [Tooltip("背景图高斯模糊大小")]
        [Range(0, 64)]
        public int BackgroundBlurSize;

        /// <summary>
        /// 对背景图的迭代次数
        /// </summary>
        [Tooltip("对背景图的迭代次数")]
        [Range(0, 4)]
        public int IterationCount;

        /// <summary>
        /// 对背景图的缩放系数
        /// </summary>
        [Tooltip("对背景图的缩放系数")]
        [Range(1, 16)]
        public int ScaleDownSize = 4;

        /// <summary>
        /// 背包界面是否显示
        /// </summary>
        public bool IsBagUIShown
        {
            get
            {
                return BagFullUI.activeSelf;
            }
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        /// <param name="inventories">背包中的物品列表</param>
        /// <param name="type">物品类型</param>
        public void BeginLoadingBag(List<MyInventory> inventories, Inventory.InventoryType type)
        {
            // 初始化UI
            UseBtn.SetActive(false);
            BagSideBar.GetComponent<BagSideBarUI>().SetInventory(null);
            BagTopBar.GetComponent<BagTopBarUI>().SwitchType(type);
            ClearContent();

            // 创建物品UI
            foreach(MyInventory inventory in inventories)
            {
                if (inventory != null)
                {
                    if(inventory.BaseInventory.Type != type)
                    {
                        continue;
                    }

                    GameObject newBagItem = Instantiate(BagItem);
                    newBagItem.transform.SetParent(Content);
                    newBagItem.GetComponent<BagItemUI>().SetInventory(inventory);
                }
            }

            BagTopBar.GetComponent<BagTopBarUI>().FlushBar(type);
        }

        /// <summary>
        /// 清空子项
        /// </summary>
        private void ClearContent()
        {
            for (int i = 0; i < Content.childCount; i++)
            {
                Destroy(Content.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 显示侧栏
        /// </summary>
        /// <param name="inventory">目标物品</param>
        public void ShowSideBar(MyInventory inventory)
        {
            if(inventory == null)
            {
                Logger.LogError("BagSystemUI: ShowSideBar() Target inventory is null.");
                return;
            }

            if (inventory.BaseInventory.CanBeUsedInBag)
            {
                UseBtn.SetActive(true);
            }

            BagSideBar.GetComponent<BagSideBarUI>().SetInventory(inventory);
        }

        /// <summary>
        /// 获取当前侧栏展示的物品（当前选中的物品）
        /// </summary>
        /// <returns>选中的物品</returns>
        public MyInventory GetNowSideBarInventory()
        {
            return BagSideBar.GetComponent<BagSideBarUI>().GetInventory();
        }

        /// <summary>
        /// 设置背包背景图片为当前摄像机经高斯模糊后的图片
        /// </summary>
        public void SetBackgroundSprite()
        {
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = renderTexture;
            Camera.main.targetDisplay = 0;
            Camera.main.Render();

            if (Camera.main.targetTexture == null)
            {
                Logger.LogError("BagSystemUI:SetBackgroundSprite() Camera.main.targetTexture is null");
                return;
            }

            Texture2D source = ScaleTexture(RenderTextureToTexture2D(Camera.main.targetTexture), 
                Screen.width / ScaleDownSize, Screen.height / ScaleDownSize);

            Texture2D backgroundTextureAfterGaussianBlur = source;

            // 对背包背景图进行高斯模糊，采用相机获取的图片作为背景
            for (int i = 0; i < IterationCount; ++i)
            {
                ImageProcessing.SmoothProcess(ImageProcessing.ProcessType.GaussianBlur,
                    source,
                    out backgroundTextureAfterGaussianBlur,
                    BackgroundBlurSize);

                source = backgroundTextureAfterGaussianBlur;
            }

            Sprite background = Sprite.Create(backgroundTextureAfterGaussianBlur,
                new Rect(0, 0, backgroundTextureAfterGaussianBlur.width, backgroundTextureAfterGaussianBlur.height),
                Vector2.one * 0.5f);

            Camera.main.targetTexture = null;

            if (background == null)
            {
                Logger.LogError("BagSystemUI:SetBackgroundSprite() background is null");
                return;
            }

            BagFullUI.GetComponent<Image>().sprite = background;
        }

        /// <summary>
        /// 把RenderTexture转换为Texture2D
        /// </summary>
        /// <param name="renderTexture">目标RenderTexture</param>
        /// <returns>转换后的Texture2D</returns>
        private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            return texture;
        }

        /// <summary>
        /// 对Texture2D进行缩放
        /// </summary>
        /// <param name="source">原图像</param>
        /// <param name="targetWidth">目标宽度</param>
        /// <param name="targetHeight">目标高度</param>
        /// <returns>缩放后的图片</returns>
        private Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
        {
            Texture2D result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);

            float incX = (1.0f / targetWidth);
            float incY = (1.0f / targetHeight);

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }


        /// <summary>
        /// 显示侧栏
        /// </summary>
        public void EnableSideBarUI()
        {
            BagSideBar.SetActive(true);
        }

        /// <summary>
        /// 隐藏侧栏
        /// </summary>
        public void DisableSideBarUI()
        {
            BagSideBar.SetActive(false);
            UseBtn.SetActive(false);
        }

        /// <summary>
        /// 显示背包界面UI
        /// </summary>
        public void EnableUI()
        {
            BagFullUI.SetActive(true);
        }

        /// <summary>
        /// 隐藏背包界面UI
        /// </summary>
        public void DisableUI()
        {
            BagFullUI.SetActive(false);
        }
    }
}

