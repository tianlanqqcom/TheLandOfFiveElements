/*
 * File: DialogSelector.cs
 * Description: 用于对话系统中，控制对话选项的显示和操作
 * Author: tianlan
 * Last update at 24/3/2 0:19
 * 
 * Update Records:
 * tianlan  24/2/4  添加主类DialogNode
 * tianlan  24/2/5  添加字段parent，完善ExecuteOperation方法
 * pili     24/3/2  修改Line40,operationNode.ParentNode.ExecuteOperation(operationNode) ->operationNode.ExecuteOperation()
 */

using Script.GameFramework.Game.Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.GameFramework.UI
{
    public class DialogSelector : MonoBehaviour
    {
        /// <summary>
        /// 索引号（现在好像没用了）
        /// </summary>
        public int Index;

        /// <summary>
        /// 该选项对应的操作节点
        /// </summary>
        OperationNode operationNode;

        /// <summary>
        /// 选项文本
        /// </summary>
        public TMP_Text selectorMessage;

        private void Start()
        {
            // 开始时绑定点击操作
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                operationNode?.ExecuteOperation();
            });
        }

        /// <summary>
        /// 设置选项内容
        /// </summary>
        /// <param name="operationNode"></param>
        public void SetSelector(OperationNode operationNode)
        {
            this.operationNode = operationNode;
            selectorMessage.text = operationNode.Message;
        }


    }
}

