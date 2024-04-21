/*
 * File: ThirdPersonCameraController.cs
 * Description: 第三人称相机控制，从MyPlayerController拆分出来
 * Bug:1.当出现碰撞时，如果碰撞点和角色十分近，会导致穿模，
 *       而如果往碰撞点方向偏移一段距离，则可能导致过度偏移从而导致镜头永远不会再拍到角色。
 * Author: tianlan
 * Last update at 24/3/14    21：35
 * 
 * Update Records:
 * tianlan  24/3/14 从MyPlayerController中拆分出相机控制代码
 */

using System.Collections;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using UnityEngine;

namespace Script.GameFramework.GamePlay
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        /// <summary>
        /// 当前玩家对应的第三人称相机
        /// </summary>
        [Tooltip("当前玩家对应的第三人称相机")]
        public Camera MyCamera;

        /// <summary>
        /// 相机父物体的Transform，方便控制相机旋转
        /// </summary>
        [Tooltip("相机父物体的Transform")]
        public Transform CameraParentTransform;

        /// <summary>
        /// 角色Mesh位于的物体
        /// </summary>
        [Tooltip("角色Mesh位于的物体")]
        public GameObject CharacterMesh;

        /// <summary>
        /// 镜头偏移
        /// </summary>
        [Tooltip("镜头偏移")]
        public Vector3 CameraOffset;

        [Header("灵敏度和角度设置")]

        /// <summary>
        /// 镜头旋转灵敏度
        /// </summary>
        [Tooltip("镜头旋转灵敏度")]
        public float CameraRotateSensity = 1.0f;

        /// <summary>
        /// 镜头缩放灵敏度
        /// </summary>
        [Tooltip("镜头缩放灵敏度")]
        public float CameraScaleSensity = 10.0f;

        /// <summary>
        /// 镜头最小FOV
        /// </summary>
        [Tooltip("镜头最小FOV")]
        public float CameraMinFov = 25.0f;

        /// <summary>
        /// 镜头最大FOV
        /// </summary>
        [Tooltip("镜头最大FOV")]
        public float CameraMaxFov = 75.0f;

        /// <summary>
        /// 镜头最小俯仰角,注意是从上往下看
        /// </summary>
        [Tooltip("镜头最小俯仰角,注意是从上往下看")]
        public float CameraMinRotateX = -80.0f;

        /// <summary>
        /// 镜头最大俯仰角,注意是从下往上看
        /// </summary>
        [Tooltip("镜头最大俯仰角,注意是从下往上看")]
        public float CameraMaxRotateX = 20.0f;

        /// <summary>
        /// 镜头距离
        /// </summary>
        [Tooltip("镜头距离")]
        public float CameraDistance = 10.0f;

        /// <summary>
        /// 镜头恢复时间
        /// </summary>
        [Tooltip("镜头恢复时间")]
        public float CameraRefreshTime = 1.0f;

        [Header("移动参数")]
        public float MoveSpeed = 1.0f;

        /// <summary>
        /// 当前X旋转角度
        /// </summary>
        private float nowRotateX = .0f;

        /// <summary>
        /// 当前Y旋转角度
        /// </summary>
        private float nowRotateY = .0f;

        /// <summary>
        /// 相机是否正在从近处逐渐恢复到默认距离
        /// </summary>
        private bool isRefreshingCamera = false;

        /// <summary>
        /// 鼠标左键是否按下，该项仅在GameMode为对话模式下有效
        /// </summary>
        private bool isMouseLeftButtonDown = false;

        // Start is called before the first frame update
        void Start()
        {
            // if camera not set, set to main camera.
            if (MyCamera == null)
            {
                MyCamera = Camera.main;
            }

            InputSystem.BindMouseMove(MouseMove_NormalGame);
            InputSystem.BindMouseWheel(MouseWheel_NormalGame);

            InputSystem.BindMouse(0, InputSystem.InputEventType.Pressed, () =>
            {
                isMouseLeftButtonDown = true;
            });

            InputSystem.BindMouse(0, InputSystem.InputEventType.Released, () =>
            {
                isMouseLeftButtonDown = false;
            });

            // Force Camera at a distance of CameraDistance.
            StartCoroutine(GraduallyMoveCameraBack(CameraDistance));
        }

        // Update is called once per frame
        void Update()
        {
            CameraCollisionUpdate();
        }

        private void LateUpdate()
        {
            // Always keep cameraParentTransform the same position with the CharacterMesh
            CameraParentTransform.position = CharacterMesh.transform.position + CameraOffset;
        }

        private void MouseMove_NormalGame(float deltaX, float daltaY)
        {
            // 普通游戏模式且鼠标未显示
            if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
            {
                CameraRotateUpdate(deltaX, daltaY);
            }
            // 对话模式且鼠标显示且鼠标左键按下
            else if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Dialog && 
                MyGameMode.Instance.IsMouseShown && isMouseLeftButtonDown)
            {
                CameraRotateUpdate(deltaX, daltaY);
            }
        }

        private void MouseWheel_NormalGame(float delta)
        {
            // 普通游戏模式且鼠标未显示
            if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
            {
                MyCamera.fieldOfView = MathLibrary.Clamp(
                    MyCamera.fieldOfView - delta * CameraScaleSensity,
                    CameraMinFov,
                    CameraMaxFov);
            }
        }

        /// <summary>
        /// 处理镜头旋转
        /// </summary>
        /// <param name="deltaX">在X方向上的旋转值</param>
        /// <param name="deltaY">在Y方向上的旋转值</param>
        private void CameraRotateUpdate(float deltaX, float deltaY)
        {
            nowRotateY -= deltaX * CameraRotateSensity;
            nowRotateX += deltaY * CameraRotateSensity;
            nowRotateX = MathLibrary.Clamp(nowRotateX, CameraMinRotateX, CameraMaxRotateX);

            Quaternion quaternion = Quaternion.Euler(-nowRotateX, -nowRotateY, 0);
            CameraParentTransform.rotation = quaternion;
        }

        /// <summary>
        /// 处理镜头碰撞后的位置
        /// </summary>
        private void CameraCollisionUpdate()
        {
            Vector3 forward = (MyCamera.transform.position - CameraParentTransform.position).normalized;
            if (Physics.Raycast(CameraParentTransform.position, forward, out RaycastHit hitinfo, CameraDistance))
            {
                isRefreshingCamera = false;
                // MyCamera.transform.position = hitinfo.point - forward * 0.5f;
                MyCamera.transform.position = hitinfo.point;
                Debug.DrawLine(CameraParentTransform.position, hitinfo.point);
            }
            else
            {
                float nowDistance = Vector3.Distance(CameraParentTransform.transform.position, MyCamera.transform.position);
                if (nowDistance < CameraDistance && !isRefreshingCamera)
                {
                    StartCoroutine(GraduallyMoveCameraBack(nowDistance));
                }

            }
        }

        private IEnumerator GraduallyMoveCameraBack(float nowDistance)
        {
            float deltaDistance = (CameraDistance - nowDistance) / CameraRefreshTime;
            isRefreshingCamera = true;
            while (Vector3.Distance(CameraParentTransform.transform.position, MyCamera.transform.position) < CameraDistance)
            {
                if (!isRefreshingCamera)
                {
                    yield break;
                }

                Vector3 forward = (MyCamera.transform.position - CameraParentTransform.position).normalized;
                MyCamera.transform.position += deltaDistance * Time.deltaTime * forward;
                yield return 1;
            }

            Vector3 raycaseForward = (MyCamera.transform.position - CameraParentTransform.position).normalized;
            MyCamera.transform.position = CameraParentTransform.position + raycaseForward * CameraDistance;
            yield break;
        }
    }
}

