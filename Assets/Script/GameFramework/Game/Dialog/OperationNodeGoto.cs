/*
 * File: OperationNodeGoto.cs
 * Description: OperationNode的子类，GOTO类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */

using Script.GameFramework.Log;

namespace Script.GameFramework.Game.Dialog
{
    public class OperationNodeGoto : OperationNode
    {
        public override void ExecuteOperation()
        {
            Logger.Log("DialogNode:ExecuteOperation() Ready to change dialog to new chapter: " + OperationMsg);
            ParentNode.Parent.NotifyChapterChanged(OperationMsg);
        }
        public OperationNodeGoto(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType, message, operationMsg, parentNode)
        {
            ;
        }
    }
}
