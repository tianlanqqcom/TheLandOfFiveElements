/*
 * File: TaskPositionTrackViewUI.cs
 * Description: 任务点追踪视图
 * Author: tianlan
 * Last update at 24/3/15   19:45
 * 
 * Update Records:
 * tianlan  24/3/15 新建文件
 */

using GameFramework.Core;
using GameFramework.Game;
using GameFramework.Game.Tasks;
using GameFramework.GamePlay;
using UnityEngine;

namespace GameFramework.UI
{
    public class TaskPositionTrackViewUI : SimpleSingleton<TaskPositionTrackViewUI>
    {
        /// <summary>
        /// 目标摄像机
        /// </summary>
        [Tooltip("目标摄像机")]
        public Camera TagetCamera;

        /// <summary>
        /// 椭圆长轴
        /// </summary>
        [Tooltip("椭圆长轴")]
        public float SemiMajorAxis = .4f;

        /// <summary>
        /// 椭圆短轴
        /// </summary>
        [Tooltip("椭圆短轴")]
        public float SemiMinorAxis = .3f;

        /// <summary>
        /// 当前正在追踪的任务
        /// </summary>
        [Tooltip("当前正在追踪的任务")]
        public int TraceTaskID { get; private set; }

        /// <summary>
        /// 跟踪UI根物体
        /// </summary>
        [Tooltip("跟踪UI根物体")]
        public GameObject TraceUI;

        /// <summary>
        /// UI根物体RectTransform缓存
        /// </summary>
        private RectTransform rectTransformForTraceUI;

        /// <summary>
        /// 跟踪UI是否启动
        /// </summary>
        private bool isTraceUIEnable = false;

        /// <summary>
        /// 设置跟踪任务ID
        /// </summary>
        /// <param name="taskID">任务ID</param>
        public void SetTraceTaskID(int taskID)
        {
            TraceTaskID = taskID;
            isTraceUIEnable = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Bind Camera
            if (TagetCamera == null)
            {
                TagetCamera = Camera.main;
            }

            // Cache rectTransform
            rectTransformForTraceUI = TraceUI.GetComponent<RectTransform>();

            // Bind input, switch enable type of TraceUI 
            InputSystem.BindKey(KeyCode.V, InputSystem.InputEventType.IE_Pressed, () =>
            {
                if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game)
                {
                    isTraceUIEnable = !isTraceUIEnable;
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            // If not in Normai_Game
            if(MyGameMode.Instance.NowWorkingMode != MyGameMode.WorkingMode.Normal_Game)
            {
                // If UI is active, disable
                if (TraceUI.activeSelf)
                {
                    TraceUI.SetActive(false);
                }

                // Leave loop to reduce performance consumption
                return;
            }

            // If UI isn't enable
            if(!isTraceUIEnable)
            {
                if (TraceUI.activeSelf)
                {
                    TraceUI.SetActive(false);
                }
                return;
            }

            // If enable

            // Get where is the target position
            // First, get the task
            Task TraceTask = TaskSystem.Instance.GetTargetTask(TraceTaskID);

            // If success
            if(TraceTask != null)
            {
                if (!TraceUI.activeSelf)
                {
                    TraceUI.SetActive(true);
                }

                Vector3 taskPosition = TraceTask.NowTaskNode.GetPosition();
                
                // If in ellipse, set UI to the point in screen
                if(IsPointInsideEllipse(taskPosition, SemiMinorAxis, SemiMinorAxis, out Vector3 pointInViewport))
                {
                    // 设置anchorMin和anchorMax属性
                    rectTransformForTraceUI.anchorMin = new Vector2(pointInViewport.x, pointInViewport.y);
                    rectTransformForTraceUI.anchorMax = new Vector2(pointInViewport.x, pointInViewport.y);

                    rectTransformForTraceUI.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                // set UI to the edge of ellipse
                else
                {
                    float angle = Mathf.Atan((pointInViewport.y -.5f) / (pointInViewport.x - .5f));

                    if(pointInViewport.x < .5f)
                    {
                        angle += Mathf.PI;
                    }

                    float x = .5f + SemiMajorAxis * Mathf.Cos(angle);
                    float y = .5f + SemiMinorAxis * Mathf.Sin(angle);

                    rectTransformForTraceUI.anchorMin = new Vector2(x, y);
                    rectTransformForTraceUI.anchorMax = new Vector2(x, y);

                    rectTransformForTraceUI.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));
                }
            }
            // If task is invalid, disable UI
            else
            {
                if (TraceUI.activeSelf)
                {
                    TraceUI.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 判断世界中一个点是否在屏幕中指定的一个椭圆(中心在屏幕中心)内
        /// </summary>
        /// <param name="point">点的世界坐标</param>
        /// <param name="a">椭圆长轴</param>
        /// <param name="b">椭圆短轴</param>
        /// <param name="pointInViewport">该点在视口坐标系中的位置</param>
        /// <returns>true if the point inside ellipse.</returns>
        private bool IsPointInsideEllipse(Vector3 point, float a, float b, out Vector3 pointInViewport)
        {
            pointInViewport = TagetCamera.WorldToViewportPoint(point);

            if (pointInViewport.z < 0)
            {
                return false; // Point is behind the camera
            }

            Vector3 center = new Vector3(0.5f, 0.5f, pointInViewport.z); // Screen center

            float x = (pointInViewport.x - center.x) / a;
            float y = (pointInViewport.y - center.y) / b;

            return x * x + y * y <= 1;
        }

        /// <summary>
        /// 测试用函数，在屏幕上绘制一个椭圆
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(0.5f, 0.5f, 0); // Screen center
            for (int i = 0; i < 360; i++)
            {
                float angle = i * Mathf.Deg2Rad;
                float x = center.x + SemiMajorAxis * Mathf.Cos(angle);
                float y = center.y + SemiMinorAxis * Mathf.Sin(angle);
                Vector3 point = Camera.main.ViewportToWorldPoint(new Vector3(x, y, 25)); // 10 is arbitrary depth
                Gizmos.DrawLine(point, point + Vector3.forward);
            }
        }
    }
}

