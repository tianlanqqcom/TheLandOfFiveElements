/*
 * File: BillboardUIElement.cs
 * Description: 用于指定世界UI并使之朝向主摄像机
 * Author: tianlan
 * Last update at 24/5/2    20:20
 *
 * Update Records:
 * tianlan  24/5/2  Copied from https://blog.csdn.net/cgchunzi/article/details/135550798
 */

using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    [HelpURL("https://blog.csdn.net/cgchunzi/article/details/135550798")]
    public class BillboardUIElement : MonoBehaviour
    {
        public Transform target; // 根物体的位置
        public Vector3 offset; // 与根物体之间的偏移量
        public Camera cam; // 主相机
        public float desiredScreenHeight = 200f; // 目标屏幕大小

        private void Start()
        {
            if (!target)
            {
                target = transform;
                offset = Vector3.zero;
                Logger.Log("BillboardUIElement::Start Target is null, reset target as self and force offset zero.");
            }
            
            if (cam) return;
            cam = Camera.main;
            
            if (cam)
            {
                Logger.Log("BillboardUIElement::LateUpdate cam is null, set to Camera.main.");
            }
            else
            {
                Logger.Log("BillboardUIElement::LateUpdate cam & Camera.main is null, force to disable.");
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            // 将血条位置设置为角色位置加上偏移量
            transform.position = target.position + offset;
            
            Vector3 screenPosition = cam.WorldToScreenPoint(transform.position);
            
            // 计算模型在屏幕上的高度
            float modelScreenHeight = Mathf.Abs(screenPosition.y - cam.WorldToScreenPoint(transform.position + transform.up).y);
            
            // 计算缩放比例
            float scaleRatio = desiredScreenHeight / modelScreenHeight;
            
            // 设置缩放
            transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);

            // 使血条始终朝向相机
            transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                             cam.transform.rotation * Vector3.up);
        }
    }
}
