/*
 * File: OperationNodeContinue.cs
 * Description: OperationNode的子类，CONTINUE类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */


namespace GameFramework.Game.Dialog
{
    public class OperationNodeContinue : OperationNode
    {
        public override void ExecuteOperation()
        {
            ParentNode.Parent.NotifyDialogStepForward();
        }
        public OperationNodeContinue(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType, message, operationMsg, parentNode)
        {
            
        }
    }
}
