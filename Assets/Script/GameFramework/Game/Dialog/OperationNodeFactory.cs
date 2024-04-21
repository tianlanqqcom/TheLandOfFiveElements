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

namespace Script.GameFramework.Game.Dialog
{
    public static class OperationNodeFactory
    {
        public static OperationNode CreateOperationNode(OperationNode.OperationType operationType, string message, string operationMsg, DialogNode parentNode)
        {
            return operationType switch
            {
                OperationNode.OperationType.NONE => new OperationNodeNone(operationType, message, operationMsg,
                    parentNode),
                OperationNode.OperationType.FINISH_DIALOG => new OperationFinishDialog(operationType, message,
                    operationMsg, parentNode),
                OperationNode.OperationType.GOTO => new OperationNodeGoto(operationType, message, operationMsg,
                    parentNode),
                OperationNode.OperationType.CONTINUE => new OperationNodeContinue(operationType, message, operationMsg,
                    parentNode),
                OperationNode.OperationType.CALL => new OperationNodeCall(operationType, message, operationMsg,
                    parentNode),
                OperationNode.OperationType.CALL_STATIC => new OperationNodeCallStatic(operationType, message,
                    operationMsg, parentNode),
                OperationNode.OperationType.CALL_NOT_FINISH => new OperationNodeCallNotFinish(operationType, message,
                    operationMsg, parentNode),
                OperationNode.OperationType.CALL_STATIC_NOT_FINISH => new OperationNodeCallStaticNotFinish(
                    operationType, message, operationMsg, parentNode),
                _ => throw new ArgumentException("Invalid operation type.")
            };
        }
    }
}
