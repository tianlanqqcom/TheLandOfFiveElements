/*
 * File: DialogSystem.cs
 * Description: DialogSystem，负责控制对话系统全流程
 * Author: tianlan
 * Last update at 24/5/15   21:30
 * 
 * Update Records:
 * tianlan  24/2/5  编写代码主体
 * tianlan  24/2/7  在开始和结束对话的时候添加对GameMode模式的切换
 * tianlan  24/5/15 添加不打断游戏流程的非阻塞式对话
 */

using Script.GameFramework.Core;
using Script.GameFramework.Game.Tasks;
using Script.GameFramework.GamePlay;
using Script.GameFramework.Test;
using Script.GameFramework.UI;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game.Dialog
{
    public class DialogSystem : SimpleSingleton<DialogSystem>
    {
        /// <summary>
        /// 当前的对话
        /// </summary>
        public Dialog NowDialog = null;

        /// <summary>
        /// 当前对话名称,该项用于当普通对话结束时广播任务事件。
        /// </summary>
        public string NowDialogName = "";

        #region NormalDialog

        /// <summary>
        /// 开始一段对话
        /// </summary>
        /// <param name="dialogName">对话名</param>
        public void BeginDialog(string dialogName)
        {
            NowDialog = LoadDialog(dialogName);

            if(NowDialog == null)
            {
                Logger.LogError("DialogSystem:BeginDialog: Null Dialog, check if the resource exists. dialogName = " +  dialogName); 
                return;
            }

            NowDialogName = dialogName;
            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Dialog);
            DialogUIDisplayer.Instance.EnableDialogUI();
            DialogUIDisplayer.Instance.SetDialogInfo(NowDialog.NowDialogNode);

        }

        /// <summary>
        /// 用于加载一段对话
        /// </summary>
        /// <param name="dialogName">对话名</param>
        /// <returns></returns>
        private static Dialog LoadDialog(string dialogName)
        {
            // Test
            if(dialogName == "test")
            {
                return DialogSystemFullStreamTest.CreateDialog();
            }

            string fullPathOfDialogResource =  LanguageManager.Instance.GetNowLanguageAssestsPath("Dialogs/" + dialogName);
            TextAsset dialogAsset = Resources.Load<TextAsset>(fullPathOfDialogResource);
            return DialogAnalyzer.AnalyzeText(dialogAsset.text);
        }

        /// <summary>
        /// 对话节点改变
        /// </summary>
        /// <param name="dialogNode">新的对话节点</param>
        public void ChangeDialogNode(DialogNode dialogNode)
        {
            DialogUIDisplayer.Instance.SetDialogInfo(NowDialog.NowDialogNode);
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        public void FinishDialog()
        {
            DialogUIDisplayer.Instance.DisableDialogUI();
            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Normal_Game);

            TaskSystem taskSystem = TaskSystem.Instance;
            if (taskSystem)
            {
                TaskEvent taskEvent = new TaskEvent(-1, $"DialogFinished:{NowDialogName}");
                taskSystem.DispatchEvent(taskEvent);
            }
            else
            {
                Logger.Log("DialogSystem::FinishDialog TaskSystem Not Found.");
            }

            NowDialogName = "";
        }

        #endregion

        #region DialogNoInterrupt

        /// <summary>
        /// 开始一段非阻塞对话
        /// </summary>
        /// <param name="dialogName">对话名</param>
        public void BeginDialogNoInterrupt(string dialogName)
        {
            NowDialog = LoadDialog(dialogName);

            if(NowDialog == null)
            {
                Logger.LogError("DialogSystem:BeginDialog: Null Dialog, check if the resource exists. dialogName = " +  dialogName); 
                return;
            }

            // MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Dialog);
            DialogUIDisplayer.Instance.EnableDialogUINoInterrupt();
            DialogUIDisplayer.Instance.SetDialogInfoNoInterrupt(NowDialog.NowDialogNode);
        }
        
        /// <summary>
        /// 对话节点改变
        /// </summary>
        /// <param name="dialogNode">新的对话节点</param>
        public void ChangeDialogNodeNoInterrupt(DialogNode dialogNode)
        {
            DialogUIDisplayer.Instance.SetDialogInfoNoInterrupt(NowDialog.NowDialogNode);
        }
        
        /// <summary>
        /// 结束对话
        /// </summary>
        public void FinishDialogNoInterrupt()
        {
            DialogUIDisplayer.Instance.DisableDialogUINoInterrupt();
            // MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Normal_Game);
        }
        

        #endregion
        
    }
}

