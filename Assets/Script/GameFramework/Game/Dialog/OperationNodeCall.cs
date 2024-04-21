/*
 * File: OperationNodeCall.cs
 * Description: OperationNode的子类，CALL类型
 * Author: pili
 * Last update at 24/3/2 10:04
 * 
 * Update Records:
 * pili 24/3/2 复写ExecuteOperation函数
 */

using System;
using System.Reflection;
using Script.GameFramework.Log;

namespace Script.GameFramework.Game.Dialog
{
    public class OperationNodeCall : OperationNode
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
                    if (type == null)
                    {
                        Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, failed to get type. ClassName = " + className);
                        ParentNode.Parent.NotifyDialogFinished();
                        
                    }

                    object obj = Activator.CreateInstance(type);
                    obj ??= type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

                    MethodInfo methodInfo = type.GetMethod(methodName);
                    methodInfo.Invoke(obj, null);
                    Logger.Log("DialogNode:ExecuteOperation() Call method success. Method = " + OperationMsg);
                }
                catch (Exception e)
                {
                    Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, there are some thing wrong. OperationMsg = " + OperationMsg + ", Exception Message = " + e.Message);
                }
            }

            ParentNode.Parent.NotifyDialogFinished();
        }
        public OperationNodeCall(OperationType operationType, string message, string operationMsg, DialogNode parentNode) : base(operationType, message, operationMsg, parentNode)
        {
            ;
        }
    }
}
