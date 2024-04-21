/*
 * File: TaskItemUI.cs
 * Description: 任务项控制组件
 * Author: tianlan
 * Last update at 24/3/15   21:35
 * 
 * Update Records:
 * tianlan  24/3/15 新建文件
 */

using GameFramework.Game.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI
{
    public class TaskItemUI : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 任务名
        /// </summary>
        public TMP_Text TaskName;

        /// <summary>
        /// 任务距离
        /// </summary>
        public TMP_Text TaskDistance;

        /// <summary>
        /// 标识是否选中的图片
        /// </summary>
        public GameObject SelectedImage;

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected { get; private set; } = false;

        /// <summary>
        /// 当前任务项对应的任务
        /// </summary>
        public Task TaskForThisItem { get; set; }

        public void SetTask(Task task)
        {
            if (task != null)
            {
                TaskForThisItem = task;
                TaskName.text = task.Name.Message;

                Vector3 taskPosition = task.NowTaskNode.GetPosition();
                Vector3 nowCharacterPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

                float distance = Vector3.Distance(nowCharacterPosition, taskPosition);
                TaskDistance.text = string.Format("{0:f2}m", distance);
            }
        }

        [Obsolete("Use SetSelected")]
        public void SetTracked(bool tracked)
        {
            if (tracked)
            {
                SelectedImage.SetActive(true);
            }
            else
            {
                SelectedImage.SetActive(false);
            }
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        /// <param name="selected">是否被选中</param>
        public void SetSelected(bool selected)
        {
            if (selected)
            {
                TaskName.color = new Color32(54,63,79,255);
                gameObject.GetComponent<Image>().color = new Color32(237, 229, 218, 255);
            }
            else
            {
                gameObject.GetComponent<Image>().color  = new Color32(54, 63, 79, 255);
                TaskName.color = new Color32(237, 229, 218, 255);
            }

            IsSelected = selected;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TaskSystemUI.Instance.SetSelectedTaskItem(this);
        }
    }
}

