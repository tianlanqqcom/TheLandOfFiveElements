/*
 * File: Logger.cs
 * Description: 用于打日志
 * Author: tianlan
 * Last update at 24/1/31 17:21
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using System.IO;
using UnityEngine;

namespace Script.GameFramework.Log
{
    public static class Logger
    {
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message">需要输出的信息</param>
        /// <param name="fileName">文件名，如果为空，则不输出到文件</param>
        public static void Log(string message, string fileName = "")
        {
            string logMsg = message + " " + System.DateTime.Now.ToString() + "\n";
            if (!fileName.Equals(""))
            {
                WriteMessageToFile(logMsg, fileName);
            }

            Debug.Log(logMsg);
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="message">需要输出的信息</param>
        /// <param name="fileName">文件名，如果为空，则不输出到文件</param>
        public static void LogError(string message, string fileName = "")
        {
            string logMsg = "Error: " + message + " " + System.DateTime.Now.ToString() + "\n";
            if (!fileName.Equals(""))
            {
                WriteMessageToFile(logMsg, fileName);
            }

            Debug.LogError(logMsg);
        }

        /// <summary>
        /// 把信息写入到对应文件
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="fileName"></param>
        private static void WriteMessageToFile(string logMsg, string fileName)
        {
            string filePath = "";
#if UNITY_EDITOR_WIN
            filePath = Application.dataPath + "/Logs";
#else
        filePath = Application.temporaryCachePath;
#endif
            filePath += "/";
            filePath += fileName;

            File.AppendAllText(filePath, logMsg);
        }
    }
}

