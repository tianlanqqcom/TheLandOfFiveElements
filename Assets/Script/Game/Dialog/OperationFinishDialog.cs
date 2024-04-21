/*
 * File: OperationFinishDialog.cs
 * Description: OperationNode的子类，FONISH_DIALOG类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */
using GameFramework.Game.Dialog;
namespace GameFramework.Game.Dialog
{
    public class OperationFinishDialog : OperationNode
    {
        public override void ExecuteOperation()
        {
            Logger.Log("DialogNode:ExecuteOperation() Ready to finish the dialog");
            ParentNode.Parent.NotifyDialogFinished();
        }
        public OperationFinishDialog(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType,message,operationMsg,parentNode)
        {
            ;
        }
    }
}

