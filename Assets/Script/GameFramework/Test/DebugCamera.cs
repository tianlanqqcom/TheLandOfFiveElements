/*
 * File: DebugCamera.cs
 * Description: 类似UE的运行时脱离状态的相机
 * Additional: 当前版本仅供测试使用
 * Author: tianlan
 * Last update at 24/3/13   19:56
 * 
 * Update Records:
 * tianlan  24/3/13
 * tianlan  24/4/21 添加镜头移动选项，现在相机可以选择依赖于可操控物体或独立操控。
 *                  使用独立操控时请确保场景中没有其他可被玩家操控的物体，否则可能导致物体不正常的移动。
 */

using UnityEngine;

namespace Script.GameFramework.Test
{
    public class DebugCamera : MonoBehaviour
    {
        [Tooltip("移动速度")]
        public float moveSpeed = 5f;

        [Tooltip("旋转速度")]
        public float rotateSpeed = 3f;

        [Tooltip("是否启用相机运动")]
        public bool bEnableMove = false;

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            //Vector3 moveDirection = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
            //transform.Translate(moveDirection);

            

            if (Input.GetMouseButton(0))
            {
                transform.Rotate(Vector3.up, mouseX * rotateSpeed);
                float desiredXAngle = transform.eulerAngles.x - mouseY * rotateSpeed;
                transform.eulerAngles = new Vector3(desiredXAngle, transform.eulerAngles.y, 0);
            }

            if (bEnableMove)
            {
                float up = Input.GetKey(KeyCode.Q) ? 0 : 1;
                up += Input.GetKey(KeyCode.E) ? 0 : -1;
                transform.Translate(new Vector3(0, up * moveSpeed * Time.deltaTime, 0));

                float forward = Input.GetKey(KeyCode.W) ? 0 : -1;
                forward += Input.GetKey(KeyCode.S) ? 0 : 1;
                transform.Translate(new Vector3(0,0, forward * moveSpeed * Time.deltaTime));

                float right = Input.GetKey(KeyCode.D) ? 0 : -1;
                right += Input.GetKey(KeyCode.A) ? 0 : 1;
                transform.Translate(new Vector3(right * moveSpeed * Time.deltaTime, 0, 0));
            }

        }
    }
}

