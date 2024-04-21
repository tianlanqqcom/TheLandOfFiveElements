/*
 * File: OperationNodeCallStatic.cs
 * Description: OperationNode的子类，CALL_STATIC类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */
using System.Reflection;
using System;

namespace GameFramework.Game.Dialog
{
    public class OperationNodeCallStatic : OperationNode
    {
        public override void ExecuteOperation()
        {
            string[] operationMsgAfterSplit = OperationMsg.Split(":");
            if (operationMsgAfterSplit.Length < 2)
            {
                Logger.LogError("DialogNode:ExecuteOperation() Invalid Operation Message.");
            }
            else
            {
                try
                {
                    string className = operationMsgAfterSplit[0];
                    string methodName = operationMsgAfterSplit[1];

                    Type type = Type.GetType(className);
                    MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static);
                    methodInfo.Invoke(null, null);
                    Logger.Log("DialogNode:ExecuteOperation() Call static method success. Method = " + OperationMsg);
                }
                catch (Exception e)
                {
                    Logger.LogError("DialogNode:ExecuteOperation() When Calling static method, there are some thing wrong. OperationMsg = " + OperationMsg + ", Exception Message = " + e.Message);
                }
            }

            ParentNode.Parent.NotifyDialogFinished();
        }
        public OperationNodeCallStatic(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType, message, operationMsg, parentNode)
        {
            ;
        }
    }
}
