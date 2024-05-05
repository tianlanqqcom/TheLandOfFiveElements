/*
 * File: DialogSystem.cs
 * Description: DialogSystem，负责控制对话系统全流程
 * Author: tianlan
 * Last update at 24/2/7    23:09
 * 
 * Update Records:
 * tianlan  24/2/5  编写代码主体
 * tianlan  24/2/7  在开始和结束对话的时候添加对GameMode模式的切换
 */

using Script.GameFramework.Core;
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

            MyGameMode.Instance.SetMode(MyGameMode.WorkingMode.Dialog);
            DialogUIDisplayer.Instance.EnableDialogUI();
            DialogUIDisplayer.Instance.SetDialogInfo(NowDialog.NowDialogNode);

        }

        /// <summary>
        /// 用于加载一段对话
        /// </summary>
        /// <param name="dialogName">对话名</param>
        /// <returns></returns>
        private Script.GameFramework.Game.Dialog.Dialog LoadDialog(string dialogName)
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
        }
    }
}

