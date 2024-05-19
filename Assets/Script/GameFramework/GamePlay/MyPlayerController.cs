/*
 * File: MyPlayerController.cs
 * Description: MyPlayerController，局部单例类，第三人称控制器，
 * 整体角色预制体结构如下：
 * CharacterRoot
 * -CameraParent
 * --Camera
 * -Character(With MeshRenderer)
 * Author: tianlan
 * Last update at 24/3/6    19:06
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/2/1  添加防镜头碰撞功能（弹簧臂），在CameraCollisionUpdate
 * tianlan  24/2/7  修复：使用GameMode.NowWorkingMode来判断输入处理方式，而非原先的使用布尔值判断。
 * tianlan  24/3/6  使用新InputSystem
 */

using System.Collections;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using UnityEngine;

namespace Script.GameFramework.GamePlay
{
    // Note: This is a third person controller.
    [RequireComponent(typeof(MyPlayerState), typeof(CharacterController))]
    public class MyPlayerController : MonoBehaviour
    {
        /// <summary>
        /// 当前玩家对应的第三人称相机
        /// </summary>
        [Header("物体绑定列表")]

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
        /// 角色控制器
        /// </summary>
        [Tooltip("角色控制器")]
        public CharacterController MyCharacterController;

        /// <summary>
        /// 是否启用动画
        /// </summary>
        [Tooltip("是否启用动画")]
        public bool EnableAnim;

        /// <summary>
        /// 动画播放器
        /// </summary>
        [Tooltip("动画播放器")]
        public Animator CharactorAnimator;

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

        /// <summary>
        /// 当前行走方向（X,Z）
        /// </summary>
        private Vector2 nowWalkingDirection = Vector2.zero;


        // Start is called before the first frame update
        void Start()
        {
            // if camera not set, set to main camera.
            if (MyCamera == null)
            {
                MyCamera = Camera.main;
            }

            if (MyCharacterController == null)
            {
                MyCharacterController = GetComponent<CharacterController>();
            }

            // Bind input
            //InputHandler.Instance.BindMouseMoveInput(this);
            //InputHandler.Instance.BindMouseWheelInput(this);
            //InputHandler.Instance.BindMouseInput(this, 0);
            InputSystem.Instance?.BindMouseMove(MouseMove_NormalGame);
            InputSystem.Instance?.BindMouseWheel(MouseWheel_NormalGame);

            RegisterAnimatorControlStates();
        }

        // Update is called once per frame
        void Update()
        {
            CharacterMove();
            CameraCollisionUpdate();
        }

        private void LateUpdate()
        {
            // Always keep cameraParentTransform the same position with the CharacterMesh
            CameraParentTransform.position = CharacterMesh.transform.position;
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
                MyCamera.transform.position = hitinfo.point - forward * 0.5f;
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
            while(Vector3.Distance(CameraParentTransform.transform.position, MyCamera.transform.position) < CameraDistance)
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

        /// <summary>
        /// 当鼠标移动时，更新镜头旋转角度
        /// </summary>
        /// <param name="deltaX">鼠标在X方向上的移动值</param>
        /// <param name="deltaY">鼠标在Y方向上的移动值</param>
        public void NotifyMouseMove(float deltaX, float deltaY)
        {
            if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
            {
                CameraRotateUpdate(deltaX, deltaY);
            }
            else if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Dialog && isMouseLeftButtonDown)
            {
                CameraRotateUpdate(deltaX, deltaY);
            }

        }

        public void NotifyMouseUp(int mouseButtonType)
        {
            if (mouseButtonType == 0)
            {
                isMouseLeftButtonDown = false;
            }

            // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 当鼠标滚轮转动时，更新镜头FOV以达到缩放的效果
        /// </summary>
        /// <param name="delta">鼠标滚轮的移动值</param>
        public void NotifyMouseWheel(float delta)
        {
            if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Pure_UI)
            {
                return;
            }

            // Logger.Log("PlayerController:NotifyMouseWheel: Delta = " + Delta);
            MyCamera.fieldOfView =
                MathLibrary.Clamp(
                    MyCamera.fieldOfView - delta * CameraScaleSensity,
                    CameraMinFov,
                    CameraMaxFov);
        }

        private void MouseMove_NormalGame(float deltaX, float daltaY)
        {
            // 普通游戏模式且鼠标未显示
            if(MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
            {
                CameraRotateUpdate(deltaX, daltaY);
            }
        }

        private void MouseWheel_NormalGame(float delta)
        {
            // 普通游戏模式且鼠标未显示
            if (MyGameMode.Instance.NowWorkingMode == MyGameMode.WorkingMode.Normal_Game && !MyGameMode.Instance.IsMouseShown)
            {
                MyCamera.fieldOfView =
                MathLibrary.Clamp(
                    MyCamera.fieldOfView - delta * CameraScaleSensity,
                    CameraMinFov,
                    CameraMaxFov);
            }
        }

        /// <summary>
        /// 绑定动画控制条件
        /// </summary>
        private void RegisterAnimatorControlStates()
        {
            InputSystem.Instance?.BindKey(KeyCode.W, InputSystem.InputEventType.Pressed, () =>
            {
                if (EnableAnim)
                {
                    CharactorAnimator.SetBool("bIsWalkingForward", true);
                }

                nowWalkingDirection.y += 1.0f;
            });

            InputSystem.Instance?.BindKey(KeyCode.W, InputSystem.InputEventType.Released, () =>
            {
                if (EnableAnim)
                {
                    CharactorAnimator.SetBool("bIsWalkingForward", false);
                }

                nowWalkingDirection.y -= 1.0f;
            });
        }

        /// <summary>
        /// 根据标记控制角色移动，每帧调用
        /// </summary>
        private void CharacterMove()
        {
            // Position translate
            MyCharacterController.SimpleMove(new Vector3(nowWalkingDirection.x, 0, nowWalkingDirection.y) * MoveSpeed);
            //MyCharacterController.SimpleMove(Vector3.zero);
        }
    }
}

