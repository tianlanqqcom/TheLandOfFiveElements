/*
 * File: DialogSystemFullStreamTest.cs
 * Description: 对话系统全流程测试
 * Author: tianlan
 * Last update at 24/3/2 ）：06
 * 
 * Update Records:
 * tianlan  24/2/7  添加静态方法CreateDialog，在Start调用时开始对话test  
 * pili     24/3/2  将DialogSystemFullStreamTest部分注释掉
 */

using UnityEngine;
using GameFramework.Game.Dialog;

namespace GameFramework.Test
{
    public class DialogSystemFullStreamTest : MonoBehaviour
    {
        public static Dialog CreateDialog()
        {
            Dialog dialog = new Dialog();

            DialogNode node1 = new DialogNode();
            node1.CharacterName = "李";
            node1.Message = "你来了";

            DialogNode node2 = new DialogNode();
            node2.CharacterName = "张";
            node2.Message = "我来了";

            DialogNode node3 = new DialogNode();
            node3.CharacterName = "李";
            node3.Message = "你不该来的";

            node1.SetParentDialog(dialog);
            node1.SetNextNode(node2);
            node2.SetNextNode(node3);

            dialog.AddChapter("章节1", node1);
            dialog.SetBeginChapterNode("章节1");

            OperationNode operationNode1 = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.FINISH_DIALOG, "扔出飞刀", "", node3);
            node3.AddOperation(operationNode1);

            OperationNode operationNode2 = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.GOTO, "展开折扇", "章节2", node3);
            node3.AddOperation(operationNode2);

            OperationNode operationNode3 = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CONTINUE, "...", "", node3);
            node3.AddOperation(operationNode3);

            DialogNode node4 = new DialogNode();
            node4.CharacterName = "张";
            node4.Message = "所以，该结束了";

            node3.SetNextNode(node4);

            DialogNode node5 = new DialogNode();
            node5.CharacterName = "张";
            node5.Message = "野火烧不尽，春风吹又生。\n可叹，可叹啊！";

            DialogNode node6 = new DialogNode();
            node6.CharacterName = "张";
            node6.Message = "你十八年前做过的事，已经忘得一干二净了？";

            DialogNode node7 = new DialogNode();
            node7.CharacterName = "李";
            node7.Message = "你。。。你是";

            node5.SetParentDialog(dialog);
            node5.SetNextNode(node6);
            node6.SetNextNode(node7);

            dialog.AddChapter("章节2", node5);

            return dialog;
            // return new Dialog();
        }

        private void Start()
        {
            DialogSystem.Instance.BeginDialog("dia1");
        }
    }
}

