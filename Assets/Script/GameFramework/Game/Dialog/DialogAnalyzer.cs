/*
 * File: DialogAnalyzer.cs
 * Description: DialogAnalyzer，用于生成某个文本文件对应的Dialog
 * Author: tianlan，pili
 * Last update at 24/3/2    0：02
 *
 * Update Records:
 * tianlan  24/2/5  规定代码接口
 * tianlan  24/2/8  完成函数AnalyzeText
 * pili     24/3/1  修改AnalyzeText，使用OperationNodeFactory创建实例
 */

using System;
using Script.GameFramework.Log;

namespace Script.GameFramework.Game.Dialog
{
    public static class DialogAnalyzer
    {
        /// <summary>
        /// 在对话中可能会出现的控制命令
        /// </summary>
        private enum Command
        {
            Begin_Chapter,              // 00
            Begin_Selector,             // 01
            End_Selector,               // 02
            Op_Continue,                // 03
            Op_FinishDialog,            // 04
            Op_Goto,                    // 05
            Op_Call,                    // 06
            End_Chapter,                // 07
            Op_CallStatic,              // 08
            Op_CallButNotFinish,        // 09
            Op_CallStaticButNotFinish,  // 0a
            Set_Begin_Chapter           // 10
        }

        private struct CommandResult
        {
            /// <summary>
            /// 当前命令
            /// </summary>
            public Command command;

            /// <summary>
            /// 如果当前为选项，该项有效
            /// </summary>
            public string SelectorContent;

            /// <summary>
            /// 控制命令参数
            /// </summary>
            public string CmdMessage;

        }
        /// <summary>
        /// 分析文本并生成Dialog
        /// </summary>
        /// <param name="text">从文件中读取到的对话文本</param>
        /// <returns>分析得到的对话</returns>
        public static Dialog AnalyzeText(string text)
        {
            Dialog result = new();

            DialogNode nowNode = null;
            DialogNode lastNode = null;

            string beginChapter = "";
            string nowChapter = "";

            bool isInSelector = false;

            string[] lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in lines)
            {
                int indexOfDelimeter = line.IndexOf('\\');
                if (indexOfDelimeter != -1)
                {
                    // 如果存在\符号并具有正常的命令
                    if(indexOfDelimeter + 2 < line.Length)
                    {
                        CommandResult cmdResult = ExecuteOperation(line, line[indexOfDelimeter + 1], line[indexOfDelimeter + 2], isInSelector);

                        switch(cmdResult.command) 
                        {
                            case Command.Begin_Chapter:
                                nowChapter = cmdResult.CmdMessage; 
                                break;
                            case Command.End_Chapter:
                                nowNode = null;
                                lastNode = null;
                                break; 
                            case Command.Set_Begin_Chapter: 
                                beginChapter = cmdResult.CmdMessage; 
                                break;
                            case Command.Begin_Selector:
                                isInSelector = true;
                                nowNode = new DialogNode();
                                break;
                            case Command.End_Selector:
                                isInSelector = false;
                                lastNode = nowNode;
                                break;
                            case Command.Op_Continue:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CONTINUE, cmdResult.SelectorContent, "", nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_FinishDialog:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.FINISH_DIALOG, cmdResult.SelectorContent, "", nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_Goto:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.GOTO, cmdResult.SelectorContent, cmdResult.CmdMessage, nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_Call:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CALL, cmdResult.SelectorContent,cmdResult.CmdMessage, nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_CallStatic:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CALL_STATIC, cmdResult.SelectorContent, cmdResult.CmdMessage, nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_CallButNotFinish:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CALL_NOT_FINISH, cmdResult.SelectorContent,cmdResult.CmdMessage, nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                            case Command.Op_CallStaticButNotFinish:
                                if (isInSelector)
                                {
                                    OperationNode operation = OperationNodeFactory.CreateOperationNode(OperationNode.OperationType.CALL_STATIC_NOT_FINISH, cmdResult.SelectorContent, "", nowNode);
                                    nowNode.AddOperation(operation);
                                }
                                break;
                        }
                    }
                    else
                    {
                        Logger.LogError("DialogAnalyzer:AnalyzeText() The line has \\ but dosen't has command, will ignore this line. Line = " + line);
                    }
                }
                else
                {
                    int indexOfMaohao = line.IndexOf(':');
                    if(indexOfMaohao != -1)
                    {
                        string characterName = line[..indexOfMaohao];
                        string dialogContent = line[++indexOfMaohao..];

                        if (isInSelector)
                        {
                            nowNode.CharacterName = characterName;
                            nowNode.Message = dialogContent;
                        }
                        else
                        {
                            nowNode = new DialogNode
                            {
                                CharacterName = characterName,
                                Message = dialogContent
                            };
                        }


                        nowNode.SetParentDialog(result);
                        
                        if(lastNode != null)
                        {
                            lastNode.SetNextNode(nowNode);
                        }
                        else
                        {
                            result.AddChapter(nowChapter, nowNode);
                        }

                        lastNode = nowNode;
                    }
                    else
                    {
                        Logger.LogError("DialogAnalyzer:AnalyzeText() The line dosen't appear to a common dialog. Line = " + line);
                    }

                }
            }

            if(beginChapter != "")
            {
                result.SetBeginChapterNode(beginChapter);
            }
            else
            {
                Logger.LogError("DialogAnalyzer:AnalyzeText() The beginChapter is empty, did you set it? ");
            }
            return result;
        }

        private static CommandResult ExecuteOperation(string line, char op1, char op2, bool isInSelector)
        {
            CommandResult result = new();
            if(op1 == '1')
            {
                if(op2 == '0')
                {
                    result.command = Command.Set_Begin_Chapter;
                    int index = line.IndexOf("\\10");
                    if (index != -1)
                    {
                        result.CmdMessage = line[(index + 3)..];
                    }
                    else
                    {
                        ;
                    }
                }
            }
            else if(op1 == '0') 
            {
                switch(op2)
                {
                    case '0':
                        result.command = Command.Begin_Chapter;
                        int index = line.IndexOf("\\00");
                        if (index != -1)
                        {
                            result.CmdMessage = line[(index + 3)..];
                        }
                        else
                        {
                            ;
                        }
                        break;
                    case '1':
                        result.command = Command.Begin_Selector;
                        break;
                    case '2':
                        result.command = Command.End_Selector;
                        break;
                    case '3':
                        result.command = Command.Op_Continue;

                        int indexOfSharp1 = line.IndexOf('#');
                        int indexOfDelimeter1 = line.IndexOf('\\');

                        if(indexOfSharp1 < indexOfDelimeter1 && indexOfDelimeter1 != -1 && indexOfSharp1 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp1 + 1)..indexOfDelimeter1];
                        }
                        break;
                    case '4':
                        result.command = Command.Op_FinishDialog;

                        int indexOfSharp2 = line.IndexOf('#');
                        int indexOfDelimeter2 = line.IndexOf('\\');

                        if (indexOfSharp2 < indexOfDelimeter2 && indexOfDelimeter2 != -1 && indexOfSharp2 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp2 + 1)..indexOfDelimeter2];
                        }
                        break;
                    case '5':
                        result.command = Command.Op_Goto;

                        int indexOfSharp3 = line.IndexOf('#');
                        int indexOfDelimeter3 = line.IndexOf('\\');

                        if (indexOfSharp3 < indexOfDelimeter3 && indexOfDelimeter3 != -1 && indexOfSharp3 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp3 + 1)..indexOfDelimeter3];
                            result.CmdMessage = line[(indexOfDelimeter3 + 3)..];
                        }
                        break;
                    case '6':
                        result.command = Command.Op_Call;

                        int indexOfSharp4 = line.IndexOf('#');
                        int indexOfDelimeter4 = line.IndexOf('\\');

                        if (indexOfSharp4 < indexOfDelimeter4 && indexOfDelimeter4 != -1 && indexOfSharp4 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp4 + 1)..indexOfDelimeter4];
                            result.CmdMessage = line[(indexOfDelimeter4 + 3)..];
                        }
                        break;
                    case '7':
                        result.command= Command.End_Chapter;
                        break;
                    case '8':
                        result.command = Command.Op_CallStatic;

                        int indexOfSharp5 = line.IndexOf('#');
                        int indexOfDelimeter5 = line.IndexOf('\\');

                        if (indexOfSharp5 < indexOfDelimeter5 && indexOfDelimeter5 != -1 && indexOfSharp5 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp5 + 1)..indexOfDelimeter5];
                            result.CmdMessage = line[(indexOfDelimeter5 + 3)..];
                        }
                        break;
                    case '9':
                        result.command = Command.Op_CallButNotFinish;

                        int indexOfSharp6 = line.IndexOf('#');
                        int indexOfDelimeter6 = line.IndexOf('\\');

                        if (indexOfSharp6 < indexOfDelimeter6 && indexOfDelimeter6 != -1 && indexOfSharp6 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp6 + 1)..indexOfDelimeter6];
                            result.CmdMessage = line[(indexOfDelimeter6 + 3)..];
                        }
                        break;
                    case 'a':
                    case 'A':
                        result.command = Command.Op_CallButNotFinish;

                        int indexOfSharp7 = line.IndexOf('#');
                        int indexOfDelimeter7 = line.IndexOf('\\');

                        if (indexOfSharp7 < indexOfDelimeter7 && indexOfDelimeter7 != -1 && indexOfSharp7 != -1)
                        {
                            result.SelectorContent = line[(indexOfSharp7 + 1)..indexOfDelimeter7];
                            result.CmdMessage = line[(indexOfDelimeter7 + 3)..];
                        }
                        break;
                }
            }

            return result;
        }
    }
}

