/*
 * File: DebugCamera.cs
 * Description: 类似UE的运行时脱离状态的相机
 * Additional: 当前版本仅供测试使用
 * Author: tianlan
 * Last update at 24/3/13   19:56
 * 
 * Update Records:
 * tianlan  24/3/13
 */

using GameFramework.Game;
using UnityEngine;

namespace GameFramework.Test
{
    public class DebugCamera : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotateSpeed = 3f;

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 moveDirection = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
            //transform.Translate(moveDirection);

            float up = Input.GetKey(KeyCode.Q) ? 0 : 1;
            up += Input.GetKey(KeyCode.E) ? 0 : -1;
            transform.Translate(new Vector3(0, up * moveSpeed * Time.deltaTime, 0));

            if (Input.GetMouseButton(0))
            {
                transform.Rotate(Vector3.up, mouseX * rotateSpeed);
                float desiredXAngle = transform.eulerAngles.x - mouseY * rotateSpeed;
                transform.eulerAngles = new Vector3(desiredXAngle, transform.eulerAngles.y, 0);
            }

        }
    }
}

