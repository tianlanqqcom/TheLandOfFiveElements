/*
 * File: TaskPanelUI.cs
 * Description: 任务系统右侧面板UI
 * Author: tianlan
 * Last update at 24/3/16   17：14
 * 
 * Update Records:
 * tianlan  24/3/17 新建文件
 */

using Script.GameFramework.Core;
using Script.GameFramework.Game.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class TaskPanelUI : MonoBehaviour
    {
        /// <summary>
        /// 未选中任务的时候的提示，这里是根物体
        /// </summary>
        [Tooltip("未选中任务的时候的提示，这里是根物体")]
        public GameObject UnSelectedTip;

        /// <summary>
        /// 选中了任务的时候的提示，这里是根物体
        /// </summary>
        [Tooltip("选中了任务的时候的提示，这里是根物体")]
        public GameObject SelectedTip;

        /// <summary>
        /// 任务名
        /// </summary>
        [Tooltip("任务名")]
        public TMP_Text TaskName;

        /// <summary>
        /// 任务指引
        /// </summary>
        [Tooltip("任务指引")]
        public TMP_Text TaskConcreteDescription;

        /// <summary>
        /// 任务描述
        /// </summary>
        [Tooltip("任务描述")]
        public TMP_Text TaskDescription;

        /// <summary>
        /// 完成提示
        /// </summary>
        [Tooltip("完成提示")]
        public TMP_Text GetTip;

        /// <summary>
        /// 追踪按钮
        /// </summary>
        [Tooltip("追踪按钮")]
        public Button TrackButton;

        /// <summary>
        /// 奖励列表根物体
        /// </summary>
        [Tooltip("奖励列表根物体")]
        public Transform AwardContentRoot;

        /// <summary>
        /// 奖励物体预制体
        /// </summary>
        [Tooltip("奖励物体预制体")]
        public GameObject TaskAwardPrefab;

        /// <summary>
        /// 提示信息
        /// </summary>
        [Tooltip("提示信息")]
        public FixedString TipMessage;

        /// <summary>
        /// 当前任务ID
        /// </summary>
        public int NowTaskID { get; private set; }

        private void Start()
        {
            TrackButton.onClick.AddListener(TrackTask);
        }

        /// <summary>
        /// 设置任务
        /// </summary>
        /// <param name="taskID">任务ID</param>
        public void SetTask(int taskID)
        {
            NowTaskID = taskID;
            Task task = TaskSystem.Instance.GetTargetTask(taskID);

            if(task == null)
            {
                UnSelectedTip.SetActive(true);
                SelectedTip.SetActive(false);
            }
            else
            {
                UnSelectedTip.SetActive(false);
                SelectedTip.SetActive(true);

                TaskName.text = task.Name.Message;
                TaskConcreteDescription.text = task.NowTaskNode.ConcreteTaskDescription.Message;
                TaskDescription.text = task.NowTaskNode.Description.Message;

                for (int i = 0; i < AwardContentRoot.childCount; i++)
                {
                    Destroy(AwardContentRoot.GetChild(i).gameObject);
                }

                // Create prefabs in scroll view
                foreach (TaskAward taskAward in task.Awards)
                {
                    if (task != null)
                    {
                        GameObject newTaskAwardItem = Instantiate(TaskAwardPrefab);
                        newTaskAwardItem.transform.SetParent(AwardContentRoot);
                        newTaskAwardItem.GetComponent<TaskAwardItemUI>().SetAward(taskAward);
                    }
                }
            }

            GetTip.text = TipMessage.Message;
        }

        /// <summary>
        /// 追踪任务
        /// </summary>
        private void TrackTask()
        {
            Logger.Log("TaskPanelUI:TrackTask() Track Button has been pressed.");
            TaskPositionTrackViewUI.Instance.SetTraceTaskID(NowTaskID);
        }
    }
}

