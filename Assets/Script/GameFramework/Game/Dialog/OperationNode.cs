/*
 * File: OperationNode.cs
 * Description: 用于对话系统中，对话节点的操作行为
 * Author: tianlan
 * Last update at 24/2/29 21:48
 * 
 * Update Records:
 * tianlan  24/2/4  添加枚举OperationType，表示操作类型
 * tianlan  24/2/4  添加主类OperationNode
 * pili     24/2/29 添加虚函数ExecuteOperation
 */

namespace Script.GameFramework.Game.Dialog
{

    public abstract class OperationNode
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        [System.Serializable]
        public enum OperationType
         {
            NONE,                   // 无操作
            FINISH_DIALOG,          // 终止对话
            GOTO,                   // 跳转到对应章节
            CONTINUE,               // 继续下一句对话
            CALL,                   // 调用函数，如打开商店面板，调用后视为对话结束,只能为无参数函数，
                                    // OperationMsg格式为[命名空间（如果有）].类名:函数名
            CALL_STATIC,            // 调用静态函数
            CALL_NOT_FINISH,        // 调用但不结束对话
            CALL_STATIC_NOT_FINISH  // 调用但不结束对话
        }

        /// <summary>
        /// 操作类型
        /// [Obsolete("方法已过时")]
        /// </summary>
        public OperationType MyOperationType { get; set; }

        /// <summary>
        /// 操作节点的父对话节点
        /// </summary>
        public DialogNode ParentNode { get; set; }

        /// <summary>
        /// 需要显示的信息，如 选项1:[这是选项1]，[]中的内容即为该字符串的值。
        /// </summary>
        public string Message { get; } = "";

        /// <summary>
        /// 操作需要的信息，如跳转到[章节2]，调用[MyGameMode.ShowMouse]
        /// </summary>
        public string OperationMsg { get; } = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="message">信息</param>
        /// <param name="operationMsg">操作信息</param>
        public OperationNode(OperationType operationType, string message, string operationMsg, DialogNode parentNode)
        {
            MyOperationType = operationType;
            Message = message;
            OperationMsg = operationMsg;
            ParentNode = parentNode;
        }

      
        /// <summary>
        /// 虚函数ExecuteOperation，无参数
        /// </summary>
        public abstract void ExecuteOperation();

    }
}

