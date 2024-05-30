/*
 * File: CameraSmoothMoveControllerTest.cs
 * Description: 用于测试CameraSmoothMoveController。
 * Author: tianlan
 * Last update at 24/5/25   17:51
 *
 * Update Records:
 * tianlan  24/5/25 编写代码主体
 */

using System;
using Script.GameFramework.Game;
using Script.LFE.Core;
using UnityEngine;

namespace Script.LFE.Test
{
    public class CameraSmoothMoveControllerTest : MonoBehaviour
    {
        public Transform begin;
        public Transform end;
        public Camera targetCam;
        public float moveTime;

        private void Start()
        {
            if (!begin || !end)
            {
                return;
            }

            targetCam.transform.position = begin.position;
            targetCam.transform.rotation = begin.rotation;

            InputSystem.Instance.BindKey(KeyCode.L, InputSystem.InputEventType.Pressed, () =>
            {
                Debug.Log("Begin Camera Move Test.");
                CameraSmoothMoveController.Instance.SmoothMove(begin, end, targetCam, moveTime);
            });
        }
    }
}