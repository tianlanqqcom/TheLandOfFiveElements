/*
 * File: OperationNodeFactory.cs
 * Description: OperationNode的工厂类
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 添加CreateOperationNode函数
 */


using System;

namespace GameFramework.Game.Dialog
{
    public class OperationNodeFactory
    {
        public static OperationNode CreateOperationNode(OperationNode.OperationType operationType, string message, string operationMsg, DialogNode parentNode)
        {
            switch (operationType)
            {
                case OperationNode.OperationType.NONE:
                    return new OperationNodeNone(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.FINISH_DIALOG:
                    return new OperationFinishDialog(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.GOTO:
                    return new OperationNodeGoto(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.CONTINUE:
                    return new OperationNodeContinue(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.CALL:
                    return new OperationNodeCall(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.CALL_STATIC:
                    return new OperationNodeCallStatic(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.CALL_NOT_FINISH:
                    return new OperationNodeCallNotFinish(operationType, message, operationMsg, parentNode);
                case OperationNode.OperationType.CALL_STATIC_NOT_FINISH:
                    return new OperationNodeCallStaticNotFinish(operationType, message, operationMsg, parentNode);
                default:
                    throw new ArgumentException("Invalid operation type.");
            }
        }
    }
}
