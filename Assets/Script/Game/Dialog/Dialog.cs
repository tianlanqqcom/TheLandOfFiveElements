/*
 * File: Dialog.cs
 * Description: 用于对话系统中一个完整的对话
 * Author: tianlan
 * Last update at 24/2/5 22:30
 * 
 * Update Records:
 * tianlan  24/2/4  添加主类Dialog
 * tianlan  24/2/4  添加方法NotifyDialogNodeChanged，NotifyDialogFinished，
 *                          NotifyChapterChanged，NotifyDialogStepForward
 *                          和属性NowDialogNode
 * tianlan  24/2/7  添加起始章节设置
 */

using System.Collections.Generic;

namespace GameFramework.Game.Dialog
{
    public class Dialog
    {
        /// <summary>
        /// 各章节的起始节点
        /// T1:string       章节名
        /// T2:DialogNode   起始节点
        /// </summary>
        readonly Dictionary<string, DialogNode> chapter = new();

        /// <summary>
        /// 起始章节
        /// </summary>
        public string BeginChapter;

        /// <summary>
        /// 当前对话节点
        /// </summary>
        public DialogNode NowDialogNode { get; private set; }

        /// <summary>
        /// 添加新章节
        /// </summary>
        /// <param name="chapterName">章节名</param>
        /// <param name="node">新章节的起始节点</param>
        public void AddChapter(string chapterName, DialogNode node)
        {
            chapter.Add(chapterName, node);
        }

        /// <summary>
        /// 通知对话系统对话节点改变
        /// </summary>
        public void NotifyDialogNodeChanged()
        {
            DialogSystem.Instance.ChangeDialogNode(NowDialogNode);
        }

        /// <summary>
        /// 通知对话系统对话结束
        /// </summary>
        public void NotifyDialogFinished()
        {
            DialogSystem.Instance.FinishDialog();
        }

        /// <summary>
        /// 通知对话系统章节改变
        /// </summary>
        /// <param name="chapterName">章节名</param>
        public void NotifyChapterChanged(string chapterName)
        {
            NowDialogNode = chapter[chapterName];
            if (NowDialogNode == null)
            {
                Logger.LogError("Dialog:NotifyChapterChanged() The target chapter doesn't exist. Finish dialog. Target chapterName = " + chapterName);
                NotifyDialogFinished();
                return;
            }

            NotifyDialogNodeChanged();
        }

        /// <summary>
        /// 通知对话系统前进一步
        /// </summary>
        public void NotifyDialogStepForward()
        {
            NowDialogNode = NowDialogNode.GetNextNode();
            if (NowDialogNode == null)
            {
                Logger.Log("Dialog:NotifyDialogStepForward() Next node is null, dialog finished.");
                NotifyDialogFinished();
                return;
            }

            NotifyDialogNodeChanged();
        }

        public void SetBeginChapterNode(string chapterName)
        {
            BeginChapter = chapterName;
            NowDialogNode = chapter[chapterName];

            if(NowDialogNode == null)
            {
                Logger.LogError("Dialog:SetBeginChapterNode() Begin is null, there might be some errors. Finish dialog.");
                NotifyDialogFinished();
            }
        }
    }
}

