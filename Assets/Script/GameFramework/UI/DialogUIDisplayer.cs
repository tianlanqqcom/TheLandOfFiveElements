/*
 * File: DialogUIDisplayer.cs
 * Description: DialogUIDisplayer，负责显示/隐藏对话框UI并设置当前对话框内容
 * Author: tianlan
 * Last update at 24/2/5    21:59
 * 
 * Update Records:
 * tianlan  24/2/5  编写代码主体
 */

using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Game.Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class DialogUIDisplayer : SimpleSingleton<DialogUIDisplayer>
    {
        /// <summary>
        /// 对话框UI的物体
        /// </summary>
        public GameObject DialogUI;

        /// <summary>
        /// 显示角色名
        /// </summary>
        public TMP_Text CharacterName;

        /// <summary>
        /// 显示对话内容
        /// </summary>
        public TMP_Text DialogMessage;

        /// <summary>
        /// 通知对话进行下一步
        /// </summary>
        public Button DialogStepForward;

        /// <summary>
        /// 对话选项列表
        /// </summary>
        public List<DialogSelector> Selectors;

        /// <summary>
        /// 显示对话UI
        /// </summary>
        public void EnableDialogUI()
        {
            if (DialogUI == null)
            {
                Logger.LogError("DialogUIDisplayer:EnableDialogUI: DialogUI is null, did you draged it on the list?");
                return;
            }

            DialogUI.SetActive(true);
        }

        /// <summary>
        /// 隐藏对话UI
        /// </summary>
        public void DisableDialogUI()
        {
            if (DialogUI == null)
            {
                Logger.LogError("DialogUIDisplayer:EnableDialogUI: DialogUI is null, did you draged it on the list?");
                return;
            }

            DialogUI.SetActive(false);
        }

        /// <summary>
        /// 设置对话框信息
        /// </summary>
        public void SetDialogInfo(DialogNode dialogNode)
        {
            if(dialogNode == null)
            {
                Logger.LogError("DialogUIDiaplayer:SetDialogInfo() The target dialogNode is null, maybe you forget to set begin chapter? System will fallback to finish dialog.");
                DialogSystem.Instance.FinishDialog();
                return;
            }
            CharacterName.text = dialogNode.CharacterName;
            DialogMessage.text = dialogNode.Message;

            int nowIndex = 0;

            // Disable all selectors
            foreach (DialogSelector selector in Selectors)
            {
                selector.gameObject.SetActive(false);
            }

            foreach(OperationNode operation in dialogNode.Operations)
            {
                if(nowIndex >= 5)
                {
                    break;
                }

                Selectors[nowIndex].gameObject.SetActive(true);
                Selectors[nowIndex].SetSelector(operation);
                nowIndex++;
            }
        }

        private void Start()
        {
            // 起始时绑定点击操作，令对话往下进行
            DialogStepForward.onClick.AddListener(() =>
            {
                Logger.Log("StepForward has been pressed.");
                if(DialogSystem.Instance.NowDialog.NowDialogNode.Operations.Count == 0)
                {
                    DialogSystem.Instance.NowDialog.NotifyDialogStepForward();
                }
            });
        }
    }
}

