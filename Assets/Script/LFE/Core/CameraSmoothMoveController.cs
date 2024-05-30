/*
 * File: CameraSmoothMoveController.cs
 * Description: 控制相机进行平滑移动。
 * Author: tianlan
 * Last update at 24/5/25   17:50
 *
 * Update Records:
 * tianlan  24/5/25 编写代码主体
 *
 * Sample Usage:
 * CameraSmoothMoveController.Instance.SmoothMove(begin, end, targetCam, moveTime);
 */

using System.Collections;
using UnityEngine;

namespace Script.LFE.Core
{
    public class CameraSmoothMoveController : MonoBehaviour
    {
        private static CameraSmoothMoveController _instance;

        public static CameraSmoothMoveController Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 令指定相机从起始地点平滑移动到目标地点(不进行旋转)
        /// </summary>
        /// <param name="beginPos">起始地点</param>
        /// <param name="endPos">目标地点</param>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="time">移动时间</param>
        public void SmoothMove(Vector3 beginPos, Vector3 endPos, Camera targetCamera, float time)
        {
            if (!targetCamera)
            {
                Debug.Log("CameraSmoothMoveController::SmoothMove Target Camera is null.");
                return;
            }
            
            Debug.Log("Begin");
            StartCoroutine(Move(beginPos, endPos, targetCamera, time));
        }

        /// <summary>
        /// 令指定相机从起始地点平滑移动到目标地点(进行旋转)
        /// </summary>
        /// <param name="beginPos">起始地点</param>
        /// <param name="endPos">目标地点</param>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="time">移动时间</param>
        public void SmoothMove(Transform beginPos, Transform endPos, Camera targetCamera, float time)
        {
            if (!targetCamera)
            {
                Debug.Log("CameraSmoothMoveController::SmoothMove Target Camera is null.");
                return;
            }
            Debug.Log("BeginT");
            StartCoroutine(Move(beginPos, endPos, targetCamera, time));
        }

        /// <summary>
        /// 令指定相机从起始地点平滑移动到目标地点(不进行旋转)
        /// </summary>
        /// <param name="beginPos">起始地点</param>
        /// <param name="endPos">目标地点</param>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="time">移动时间</param>
        private static IEnumerator Move(Vector3 beginPos, Vector3 endPos, Camera targetCamera, float time)
        {
            Debug.Log("BeginM");
            var timeRecorder = .0f;
            while (timeRecorder < time)
            {
                targetCamera.transform.position = Vector3.Lerp(beginPos, endPos, timeRecorder / time);
                timeRecorder += Time.deltaTime;
                yield return 1;
            }

            targetCamera.transform.position = endPos;
        }

        /// <summary>
        /// 令指定相机从起始地点平滑移动到目标地点(进行旋转)
        /// </summary>
        /// <param name="beginPos">起始地点</param>
        /// <param name="endPos">目标地点</param>
        /// <param name="targetCamera">目标相机</param>
        /// <param name="time">移动时间</param>
        private static IEnumerator Move(Transform beginPos, Transform endPos, Camera targetCamera, float time)
        {
            Debug.Log("BeginMT");
            var timeRecorder = .0f;
            while (timeRecorder < time)
            {
                var t = Mathf.Clamp01(timeRecorder / time);
                targetCamera.transform.position = Vector3.Lerp(beginPos.position, endPos.position, t);
                targetCamera.transform.rotation = Quaternion.Slerp(beginPos.rotation, endPos.rotation, t);
                timeRecorder += Time.deltaTime;
                yield return 1;
            }

            targetCamera.transform.position = endPos.position;
            targetCamera.transform.rotation = endPos.rotation;
        }
    }
}