/*
 * File: DialogUIDisplayer.cs
 * Description: DialogUIDisplayer，负责显示/隐藏对话框UI并设置当前对话框内容
 * Author: tianlan
 * Last update at 24/5/15   21:37
 *
 * Update Records:
 * tianlan  24/2/5  编写代码主体
 * tianlan  24/4/27 添加功能：对话文本逐渐显示
 * tianlan  24/5/3  在显示和关闭UI功能时添加跳过按钮的处理
 * tianlan  24/5/15 添加非阻塞对话相关代码
 */

using System.Collections;
using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Game.Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.UI
{
    public class DialogUIDisplayer : SimpleSingleton<DialogUIDisplayer>
    {
        /// <summary>
        /// 对话框UI的物体
        /// </summary>
        [FormerlySerializedAs("DialogUI")] public GameObject dialogUI;

        /// <summary>
        /// 非阻塞对话框UI
        /// </summary>
        public GameObject dialogUINoInterrupt;

        /// <summary>
        /// 是否启用跳过功能
        /// </summary>
        public bool bEnableSkip;

        /// <summary>
        /// 跳过按钮
        /// </summary>
        public GameObject skipButton;

        /// <summary>
        /// 显示角色名
        /// </summary>
        [FormerlySerializedAs("CharacterName")]
        public TMP_Text characterName;

        /// <summary>
        /// 显示对话内容
        /// </summary>
        [FormerlySerializedAs("DialogMessage")]
        public TMP_Text dialogMessage;

        /// <summary>
        /// 显示角色名(非阻塞)
        /// </summary>
        public TMP_Text characterNameNoInterrupt;

        /// <summary>
        /// 显示对话内容(非阻塞)
        /// </summary>
        public TMP_Text dialogMessageNoInterrupt;

        /// <summary>
        /// 通知对话进行下一步
        /// </summary>
        [FormerlySerializedAs("DialogStepForward")]
        public Button dialogStepForward;

        /// <summary>
        /// 对话选项列表
        /// </summary>
        [FormerlySerializedAs("Selectors")] public List<DialogSelector> selectors;

        /// <summary>
        /// 显示一个字符的时间
        /// </summary>
        public float timeToShowOneCharacter = 0.05f;

        /// <summary>
        /// 非阻塞式对话显示下一段对话之间的间隔
        /// </summary>
        public float timeForDialogNoInterruptToDisplayNextMessage = 2.0f;

        /// <summary>
        /// 文本是否正在显示中
        /// </summary>
        public bool textIsDisplaying = false;

        /// <summary>
        /// 强制文本立即显示完毕
        /// </summary>
        public bool forceShowTextImmediately = false;

        #region NormalDialog

        /// <summary>
        /// 显示对话UI
        /// </summary>
        public void EnableDialogUI()
        {
            if (dialogUI == null)
            {
                Logger.LogError("DialogUIDisplayer:EnableDialogUI: DialogUI is null, did you draged it on the list?");
                return;
            }

            if (bEnableSkip)
            {
                if (skipButton)
                {
                    skipButton.SetActive(true);
                }
                else
                {
                    Logger.LogError(
                        "DialogUIDisplayer:EnableDialogUI Enable skip functions but haven't assigned skip button.");
                }
            }

            dialogUI.SetActive(true);
        }

        /// <summary>
        /// 隐藏对话UI
        /// </summary>
        public void DisableDialogUI()
        {
            if (dialogUI == null)
            {
                Logger.LogError("DialogUIDisplayer:DisableDialogUI: DialogUI is null, did you draged it on the list?");
                return;
            }

            if (!skipButton)
            {
                skipButton.SetActive(false);
            }

            dialogUI.SetActive(false);
        }

        /// <summary>
        /// 设置对话框信息
        /// </summary>
        public void SetDialogInfo(DialogNode dialogNode)
        {
            if (dialogNode == null)
            {
                Logger.LogError(
                    "DialogUIDisplayer:SetDialogInfo() The target dialogNode is null, maybe you forget to set begin chapter? System will fallback to finish dialog.");
                DialogSystem.Instance.FinishDialog();
                return;
            }

            characterName.text = dialogNode.CharacterName;
            // dialogMessage.text = dialogNode.Message;
            StartCoroutine(ShowMessage(dialogNode.Message, dialogNode.Operations));

            // Disable all selectors
            foreach (DialogSelector selector in selectors)
            {
                selector.gameObject.SetActive(false);
            }

            // The part of code is move to execute after ShowMessage.
            // foreach (OperationNode operation in dialogNode.Operations)
            // {
            //     if (nowIndex >= 5)
            //     {
            //         break;
            //     }
            //
            //     selectors[nowIndex].gameObject.SetActive(true);
            //     selectors[nowIndex].SetSelector(operation);
            //     nowIndex++;
            // }
        }

        private IEnumerator ShowMessage(string message, List<OperationNode> operationNodes)
        {
            dialogMessage.text = "";
            textIsDisplaying = true;
            foreach (var c in message)
            {
                if (forceShowTextImmediately)
                {
                    dialogMessage.text = message;
                    break;
                }

                dialogMessage.text += c;
                yield return new WaitForSeconds(timeToShowOneCharacter);
            }

            // Flush text displaying status
            textIsDisplaying = false;
            forceShowTextImmediately = false;

            // If we has operations, show them.
            int nowIndex = 0;
            foreach (OperationNode operation in operationNodes)
            {
                if (nowIndex >= 5)
                {
                    break;
                }

                selectors[nowIndex].gameObject.SetActive(true);
                selectors[nowIndex].SetSelector(operation);
                nowIndex++;
            }
        }

        #endregion

        #region DialogNoInterrupt

        /// <summary>
        /// 显示对话UI
        /// </summary>
        public void EnableDialogUINoInterrupt()
        {
            if (dialogUINoInterrupt == null)
            {
                Logger.LogError(
                    "DialogUIDisplayer:EnableDialogUINoInterrupt: DialogUINoInterrupt is null, did you dragged it on the list?");
                return;
            }

            // Ignore skip button
            // if (bEnableSkip)
            // {
            //     if (skipButton)
            //     {
            //         skipButton.SetActive(true);
            //     }
            //     else
            //     {
            //         Logger.LogError("DialogUIDisplayer:EnableDialogUI Enable skip functions but haven't assigned skip button.");
            //     }
            // }

            dialogUINoInterrupt.SetActive(true);
        }

        /// <summary>
        /// 隐藏对话UI
        /// </summary>
        public void DisableDialogUINoInterrupt()
        {
            if (dialogUINoInterrupt == null)
            {
                Logger.LogError(
                    "DialogUIDisplayer::DisableDialogUINoInterrupt: DialogUINoInterrupt is null, did you dragged it on the list?");
                return;
            }

            // Ignore skip button
            // if (!skipButton)
            // {
            //     skipButton.SetActive(false);
            // }

            dialogUINoInterrupt.SetActive(false);
        }

        /// <summary>
        /// 设置对话框信息
        /// </summary>
        public void SetDialogInfoNoInterrupt(DialogNode dialogNode)
        {
            if (dialogNode == null)
            {
                Logger.LogError(
                    "DialogUIDisplayer:SetDialogInfoNoInterrupt() The target dialogNode is null, maybe you forget to set begin chapter? System will fallback to finish dialog.");
                DialogSystem.Instance.FinishDialogNoInterrupt();
                return;
            }

            characterNameNoInterrupt.text = dialogNode.CharacterName;
            // dialogMessage.text = dialogNode.Message;
            StartCoroutine(ShowMessageNoInterrupt(dialogNode.Message, dialogNode.Operations));

            // Disable all selectors
            // foreach (DialogSelector selector in selectors)
            // {
            //     selector.gameObject.SetActive(false);
            // }

            // The part of code is move to execute after ShowMessage.
            // foreach (OperationNode operation in dialogNode.Operations)
            // {
            //     if (nowIndex >= 5)
            //     {
            //         break;
            //     }
            //
            //     selectors[nowIndex].gameObject.SetActive(true);
            //     selectors[nowIndex].SetSelector(operation);
            //     nowIndex++;
            // }
        }

        private IEnumerator ShowMessageNoInterrupt(string message, List<OperationNode> operationNodes)
        {
            Logger.Log($"DialogUIDisplayer::ShowMessageNoInterrupt Begin set, message = {message}");
            dialogMessageNoInterrupt.text = "";
            textIsDisplaying = true;
            foreach (var c in message)
            {
                if (forceShowTextImmediately)
                {
                    dialogMessageNoInterrupt.text = message;
                    break;
                }

                dialogMessageNoInterrupt.text += c;
                yield return new WaitForSeconds(timeToShowOneCharacter);
            }

            // Flush text displaying status
            textIsDisplaying = false;
            forceShowTextImmediately = false;

            // If we has operations, show them.
            // int nowIndex = 0;
            // foreach (OperationNode operation in operationNodes)
            // {
            //     if (nowIndex >= 5)
            //     {
            //         break;
            //     }
            //
            //     selectors[nowIndex].gameObject.SetActive(true);
            //     selectors[nowIndex].SetSelector(operation);
            //     nowIndex++;
            // }
            yield return new WaitForSeconds(timeForDialogNoInterruptToDisplayNextMessage);
            DialogSystem.Instance.NowDialog.NotifyDialogStepForward(true);
        }

        #endregion


        private void Start()
        {
            // 起始时绑定点击操作，令对话往下进行
            dialogStepForward.onClick.AddListener(() =>
            {
                Logger.Log("StepForward has been pressed.");
                if (textIsDisplaying)
                {
                    forceShowTextImmediately = true;
                }
                else if (DialogSystem.Instance.NowDialog.NowDialogNode.Operations.Count == 0)
                {
                    DialogSystem.Instance.NowDialog.NotifyDialogStepForward();
                }
            });
        }
    }
}