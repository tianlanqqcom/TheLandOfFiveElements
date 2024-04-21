/*
 * File: OperationNode.cs
 * Description: 用于对话系统中，存储对话节点的数据信息
 * Author: tianlan
 * Last update at 24/3/2 9：49
 * 
 * Update Records:
 * tianlan  24/2/4  添加主类DialogNode
 * tianlan  24/2/5  添加字段parent，完善ExecuteOperation方法
 * pili     24/3/2  将EexcuteOperation注释更改,将parent变量修改为公开
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game.Dialog
{
    public class DialogNode
    {
        /// <summary>
        /// 对话节点的父容器
        /// </summary>
        public  Script.GameFramework.Game.Dialog.Dialog Parent;

        /// <summary>
        /// 设置父容器
        /// </summary>
        /// <param name="parentDialog"></param>
        public void SetParentDialog(Script.GameFramework.Game.Dialog.Dialog parentDialog)
        {
            Parent = parentDialog;
        }

        /// <summary>
        /// 下一句对话节点
        /// </summary>
        private DialogNode _nextNode;

        /// <summary>
        /// 说话人的名字
        /// </summary>
        public string CharacterName {  get; set; }

        /// <summary>
        /// 对话内容
        /// </summary>
        public string Message {  get; set; }

        /// <summary>
        /// 选项操作
        /// </summary>
        public List<OperationNode> Operations = new();

        /// <summary>
        /// 设置下一个节点
        /// </summary>
        /// <param name="nextNode">下一个节点</param>
        public void SetNextNode(DialogNode nextNode)
        {
            this._nextNode = nextNode;
            nextNode.SetParentDialog(Parent);
        }

        /// <summary>
        /// 获取下一个节点
        /// </summary>
        /// <returns>下一个节点</returns>
        public DialogNode GetNextNode() { return _nextNode; }

        /// <summary>
        /// 添加操作节点
        /// </summary>
        /// <param name="operation">新的操作节点</param>
        public void AddOperation(OperationNode operation)
        {
            Operations.Add(operation);
        }

        /// <summary>
        /// 执行对应索引的操作
        /// </summary>
        /// <param name="index">操作的索引值</param>
        [Obsolete("方法已过时，请使用OperationNode中的ExecuteOperation")]
        public void ExecuteOperation(int index)
        {
            // Check index out of range.
            if(index > Operations.Count)
            {
                Logger.LogError("DialogNode:ExecuteOperation: Trying to execute a out ranged operation. Now OperationsLength = "
                    + Operations.Count + ", but index = " + index, "error_DialogNode.log");
                return;
            }

            // Now check whether the operation is valid. 
            OperationNode operation = Operations[index];
            if(operation == null)
            {
                Logger.LogError("DialogNode:ExecuteOperation: Null operation, how did this been added?",
                    "error_DialogNode.log");
                return;
            }

            // Now begin execute.
            ExecuteOperation(operation);
            
        }

        /// <summary>
        /// 执行节点操作
        /// </summary>
        /// <param name="operation">操作节点</param>
        [Obsolete("方法已过时，请使用OperationNode中的ExecuteOperation")]
        public void ExecuteOperation(OperationNode operation)
        {
            switch (operation.MyOperationType)
            {
                case OperationNode.OperationType.NONE:
                    Logger.LogError("DialogNode:ExecuteOperation() Now operation is NONE, this operation should never happen in correct dialog, only used for debug, and will be seen as FINISH_DIALOG.");
                    goto case OperationNode.OperationType.FINISH_DIALOG;
                case OperationNode.OperationType.FINISH_DIALOG:
                    Logger.Log("DialogNode:ExecuteOperation() Ready to finish the dialog");
                    Parent.NotifyDialogFinished();
                    break;
                case OperationNode.OperationType.GOTO:
                    Logger.Log("DialogNode:ExecuteOperation() Ready to change dialog to new chapter: " + operation.OperationMsg);
                    Parent.NotifyChapterChanged(operation.OperationMsg);
                    break;
                case OperationNode.OperationType.CONTINUE:
                    Parent.NotifyDialogStepForward();
                    break;
                case OperationNode.OperationType.CALL:
                    string[] operationMsgAfterSplit1 = operation.OperationMsg.Split(":");
                    if (operationMsgAfterSplit1.Length < 2)
                    {
                        Logger.LogError("DialogNode:ExecuteOperation() Invalid Operation Message.");
                    }
                    else
                    {
                        try
                        {
                            string className = operationMsgAfterSplit1[0];
                            string methodName = operationMsgAfterSplit1[1];

                            Type type = Type.GetType(className);
                            if(type == null)
                            {
                                Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, failed to get type. ClassName = " + className);
                                Parent.NotifyDialogFinished();
                                break;
                            }

                            object obj = Activator.CreateInstance(type);
                            obj ??= type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

                            MethodInfo methodInfo = type.GetMethod(methodName);
                            methodInfo.Invoke(obj, null);
                            Logger.Log("DialogNode:ExecuteOperation() Call method success. Method = " + operation.OperationMsg);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, there are some thing wrong. OperationMsg = " + operation.OperationMsg + ", Exception Message = " + e.Message);
                        }
                    }

                    Parent.NotifyDialogFinished();
                    break;
                case OperationNode.OperationType.CALL_STATIC:
                    string[] operationMsgAfterSplit2 = operation.OperationMsg.Split(":");
                    if (operationMsgAfterSplit2.Length < 2)
                    {
                        Logger.LogError("DialogNode:ExecuteOperation() Invalid Operation Message.");
                    }
                    else
                    {
                        try
                        {
                            string className = operationMsgAfterSplit2[0];
                            string methodName = operationMsgAfterSplit2[1];

                            Type type = Type.GetType(className);
                            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static);
                            methodInfo.Invoke(null, null);
                            Logger.Log("DialogNode:ExecuteOperation() Call static method success. Method = " + operation.OperationMsg);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("DialogNode:ExecuteOperation() When Calling static method, there are some thing wrong. OperationMsg = " + operation.OperationMsg + ", Exception Message = " + e.Message);
                        }
                    }

                    Parent.NotifyDialogFinished();
                    break;
                case OperationNode.OperationType.CALL_NOT_FINISH:
                    string[] operationMsgAfterSplit3 = operation.OperationMsg.Split(":");
                    if (operationMsgAfterSplit3.Length < 2)
                    {
                        Logger.LogError("DialogNode:ExecuteOperation() Invalid Operation Message.");
                    }
                    else
                    {
                        try
                        {
                            string className = operationMsgAfterSplit3[0];
                            string methodName = operationMsgAfterSplit3[1];

                            Type type = Type.GetType(className);
                            if (type == null)
                            {
                                Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, failed to get type. ClassName = " + className);
                                Parent.NotifyDialogFinished();
                                break;
                            }

                            object obj = Activator.CreateInstance(type);
                            obj ??= type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

                            MethodInfo methodInfo = type.GetMethod(methodName);
                            methodInfo.Invoke(obj, null);
                            Logger.Log("DialogNode:ExecuteOperation() Call method success. Method = " + operation.OperationMsg);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("DialogNode:ExecuteOperation() When Calling Method, there are some thing wrong. OperationMsg = " + operation.OperationMsg + ", Exception Message = " + e.Message);
                        }
                    }

                    break;
                case OperationNode.OperationType.CALL_STATIC_NOT_FINISH:
                    string[] operationMsgAfterSplit4 = operation.OperationMsg.Split(":");
                    if (operationMsgAfterSplit4.Length < 2)
                    {
                        Logger.LogError("DialogNode:ExecuteOperation() Invalid Operation Message.");
                    }
                    else
                    {
                        try
                        {
                            string className = operationMsgAfterSplit4[0];
                            string methodName = operationMsgAfterSplit4[1];

                            Type type = Type.GetType(className);
                            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static);
                            methodInfo.Invoke(null, null);
                            Logger.Log("DialogNode:ExecuteOperation() Call static method success. Method = " + operation.OperationMsg);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("DialogNode:ExecuteOperation() When Calling static method, there are some thing wrong. OperationMsg = " + operation.OperationMsg + ", Exception Message = " + e.Message);
                        }
                    }

                    break;
                default:
                    Logger.LogError("DialogNode:ExecuteOperation() The operation doesn't exist, will finish dialog. Now operation = " + operation.MyOperationType.ToString());
                    Parent.NotifyDialogFinished();
                    break;
            }
        }
    }
}

