/*
 * File: OperationNodeNone.cs
 * Description: OperationNode的子类，NONE类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */


namespace GameFramework.Game.Dialog
{
    public class OperationNodeNone : OperationNode
    {
        private OperationFinishDialog dialog=null;
        public override void ExecuteOperation()
        {
                Logger.LogError("DialogNode:ExecuteOperation() Now operation is NONE, this operation should never happen in correct dialog, only used for debug, and will be seen as FINISH_DIALOG.");
                dialog.ExecuteOperation();
        }
        public OperationNodeNone(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType, message, operationMsg, parentNode)
        {
            ;
        }
    }
}