/*
 * File: TaskSystemUI.cs
 * Description: 任务系统UI控制组件
 * Author: tianlan
 * Last update at 24/3/16   16:57
 * 
 * Update Records:
 * tianlan  24/3/16 新建文件
 */

using Script.GameFramework.Core;
using Script.GameFramework.Game;
using Script.GameFramework.Game.Tasks;
using Script.GameFramework.GamePlay;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class TaskSystemUI : SimpleSingleton<TaskSystemUI>
    {
        [Header("物体绑定列表")]
        /// <summary>
        /// UI根物体
        /// </summary>
        [Tooltip("UI根物体")]
        public GameObject UIRoot;

        /// <summary>
        /// 内容根目录
        /// </summary>
        [Tooltip("内容根目录")]
        public Transform ContentRoot;

        /// <summary>
        /// 顶栏
        /// </summary>
        [Tooltip("顶栏")]
        public TaskTopBarUI TopBar;

        /// <summary>
        /// 右侧面板
        /// </summary>
        [Tooltip("右侧面板")]
        public TaskPanelUI RightPanel;

        /// <summary>
        /// 任务目标项预制体
        /// </summary>
        [Tooltip("任务目标项预制体")]
        public GameObject TaskItemPrefab;

        /// <summary>
        /// 当前已经选中的任务
        /// </summary>
        [Tooltip("当前已经选中的任务")]
        public Task NowSelectedTask;

        [Header("图形模糊选项")]
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
        /// 任务UI是否已显示
        /// </summary>
        public bool IsTaskUIShown
        {
            get
            {
                return UIRoot.activeSelf;
            }
        }

        private void Start()
        {
            // 绑定输入
            InputSystem.BindKey(KeyCode.T, InputSystem.InputEventType.Pressed, () =>
            {
                if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game)
                {
                    OpenTaskSystemUI();
                }
            });

            InputSystem.BindKey(KeyCode.Escape, InputSystem.InputEventType.Pressed, () =>
            {
                if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Pure_UI && IsTaskUIShown)
                {
                    CloseTaskSystemUI();
                }
            });

            // Test
            TaskSystem.Instance.AddTask(1000);
        }

        /// <summary>
        /// 打开任务面板
        /// </summary>
        public void OpenTaskSystemUI()
        {
            if (UIRoot == null)
            {
                Logger.LogError("TaskSystemUI:OpenTaskSystemUI() UI Root is not assigned or has been destoryed.");
                return;
            }

            UIRoot.SetActive(true);
            SetBackgroundSprite();

            // Init top bar
            TopBar.OnOpen();

            // Clear Tasks
            for (int i = 0; i < ContentRoot.childCount; i++)
            {
                Destroy(ContentRoot.GetChild(i).gameObject);
            }

            int taskID = NowSelectedTask != null ? NowSelectedTask.TaskID : -2;

            bool isTaskInList = false;
            // Create prefabs in scroll view
            foreach (Task task in TaskSystem.Instance.NowActiveTasks)
            {
                if (task != null)
                {
                    GameObject newTaskItem = Instantiate(TaskItemPrefab);
                    newTaskItem.transform.SetParent(ContentRoot);
                    TaskItemUI taskItemUI = newTaskItem.GetComponent<TaskItemUI>();
                    taskItemUI.SetTask(task);
                    if (task.TaskID == taskID)
                    {
                        taskItemUI.SetSelected(true);
                        isTaskInList = true;
                    }
                }
            }

            if (isTaskInList)
            {
                RightPanel.SetTask(NowSelectedTask.TaskID);
            }
            else
            {
                RightPanel.SetTask(TaskSystem.InvalidTaskID);
            }


            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Pure_UI);
        }

        /// <summary>
        /// 关闭任务面板
        /// </summary>
        public void CloseTaskSystemUI()
        {
            UIRoot.SetActive(false);
            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Normal_Game);
        }

        /// <summary>
        /// 设置选中的任务项
        /// </summary>
        /// <param name="item"></param>
        public void SetSelectedTaskItem(TaskItemUI item)
        {
            NowSelectedTask = item.TaskForThisItem;
            RightPanel.SetTask(NowSelectedTask.TaskID);

            // 刷新列表中任务项的状态
            GameObject nowProcessObject;
            for (int i = 0; i < ContentRoot.childCount; i++)
            {
                nowProcessObject = ContentRoot.GetChild(i).gameObject;
                if (nowProcessObject == item.gameObject)
                {
                    nowProcessObject.GetComponent<TaskItemUI>().SetSelected(true);
                }
                else
                {
                    nowProcessObject.GetComponent<TaskItemUI>().SetSelected(false);
                }
            }
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

            UIRoot.GetComponent<Image>().sprite = background;
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

            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear(j / (float)result.width, i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }

            result.Apply();
            return result;
        }
    }
}

