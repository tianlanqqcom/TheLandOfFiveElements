/*
 * File: CameraDeviceController.cs
 * Description: 控制相机进行在场景中若干个物体之间聚焦。
 * Author: tianlan
 * Last update at 24/5/27   19:05
 *
 * Update Records:
 * tianlan  24/5/27 编写代码主体
 *
 * How to use:
 * 1. Make sure there is a CameraSmoothMoveController in your scene.
 * 2. Fill in the necessary fields in Editor.
 * 3. Make sure your object(s) has collider and tag.
 * 3. Run your game, and use cursor to focus object, use Key U to exit focus mode.
 */

using System;
using System.Collections.Generic;
using Script.LFE.Core;
using UnityEngine;

namespace Script.LFE.Game.Temp
{
    public class CameraDeviceController : MonoBehaviour
    {
        /// <summary>
        /// 相机位置和对应物体的键值对
        /// </summary>
        [Serializable]
        public struct CameraPositionPair
        {
            public GameObject obj;
            public Transform position;
        }

        /// <summary>
        /// 目标相机，一般为主摄像机
        /// </summary>
        [Tooltip("目标相机")] public Camera targetCamera;

        /// <summary>
        /// 需要被选择的物体的TAG
        /// </summary>
        [Tooltip("需要选择的物体具有的TAG")] public string tagName;

        /// <summary>
        /// 取消聚焦模式的时候相机会向后退回一段距离，根据需要自行设定
        /// </summary>
        [Tooltip("相机回退的距离，根据实际需要设置")] [Range(1, 100)]
        public float backDistance = 2.0f;

        /// <summary>
        /// 相机移动耗时
        /// </summary>
        [Tooltip("相机移动耗时")] [Range(.1f, 100f)] public float cameraMoveTime = .5f;

        /// <summary>
        /// 相机位置和对应物体的映射表
        /// </summary>
        [Tooltip("相机位置和对应物体的映射表")] public List<CameraPositionPair> cameraPositionPairs;

        /// <summary>
        /// 实际工作的映射表
        /// </summary>
        private readonly Dictionary<GameObject, Transform> _cameraPositionMap = new();

        /// <summary>
        /// 是否处于聚焦模式
        /// </summary>
        private bool _isInFocusMode;

        private void Start()
        {
            // Init map.
            foreach (var pair in cameraPositionPairs)
            {
                _cameraPositionMap.Add(pair.obj, pair.position);
            }
        }

        private void Update()
        {
            // If mouse left button down.
            if (Input.GetMouseButtonDown(0))
            {
                // Find a object with cursor.
                var ray = targetCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var obj = hit.collider.gameObject;

                    // If the object has correct tag and mapped camera position.
                    if (obj.CompareTag(tagName) && _cameraPositionMap.TryGetValue(obj, out var value))
                    {
                        // Move camera to target position.
                        CameraSmoothMoveController.Instance.SmoothMove(
                            targetCamera.transform, value, targetCamera, cameraMoveTime);

                        // Update focus mode.
                        _isInFocusMode = true;
                    }
                }
            }

            // If in focus mode and Key U pressed.
            if (_isInFocusMode && Input.GetKeyDown(KeyCode.U))
            {
                // Move back camera.
                CameraSmoothMoveController.Instance.SmoothMove(
                    targetCamera.transform.position,
                    targetCamera.transform.position - targetCamera.transform.forward * backDistance,
                    targetCamera, cameraMoveTime);

                // Exit focus mode.
                _isInFocusMode = false;
            }
        }
    }
}