/*
 * File: DialogSkipButton.cs
 * Description: 对话系统的补充功能：跳过对话，直到下一个需要选择的节点
 * Author: tianlan
 * Last update at 24/5/3    21:50
 *
 * Update Records:
 * tianlan  24/5/3
 */

using Script.GameFramework.Game.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Script.LFE.UI
{
    public class DialogSkipButton : MonoBehaviour
    {
        private void Start()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                var dialogSystem = DialogSystem.Instance;
                if (dialogSystem.NowDialog == null)
                {
                    dialogSystem.FinishDialog();
                    return;
                }
                
                while (dialogSystem.NowDialog != null &&
                       dialogSystem.NowDialog.NowDialogNode != null &&
                       dialogSystem.NowDialog.NowDialogNode.Operations.Count == 0)
                {
                    dialogSystem.NowDialog.NotifyDialogStepForward();
                }
            });
        }
    }
}
