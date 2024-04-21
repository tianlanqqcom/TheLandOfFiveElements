/*
 * File: FileLoaderAsync.cs
 * Description: 在Unity中异步加载StreamingAssets文件并在加载完成后通知主线程处理
 * Author: tianlan
 * Last update at 24/3/6    20:24
 * 
 * Update Records:
 * tianlan  24/2/25 编写模板主体
 * tianlan  24/3/6  完善LoadFileCoroutine在不同平台的表现
 */

using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework.Core
{
    public class FileLoaderAsync : GlobalSingleton<FileLoaderAsync>
    {
        /// <summary>
        /// 回调委托，在加载完成后调用
        /// </summary>
        /// <param name="fileData">文件数据</param>
        public delegate void FileLoadedCallback(byte[] fileData);

        /// <summary>
        /// 异步加载流式文件
        /// </summary>
        /// <param name="fileName">文件名，如Image/pic.jpg</param>
        /// <param name="callback">回调函数</param>
        public void LoadFileAsync(string fileName, FileLoadedCallback callback)
        {
            StartCoroutine(LoadFileCoroutine(fileName, callback));
        }

        /// <summary>
        /// 使用UnityWebRequest加载流式文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="callback">回调函数</param>
        /// <returns></returns>
        private static IEnumerator LoadFileCoroutine(string fileName, FileLoadedCallback callback)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                byte[] fileData = www.downloadHandler.data;
                callback(fileData);
            }
            else
            {
                Logger.LogError("FileLoaderAsync:Failed to load file, path = " + filePath + " error = " + www.error);
            }
#else
            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                callback(fileData);
            }
            else
            {
                Logger.LogError("FileLoaderAsync:Failed to load file, path = " + filePath + " error = " + "File doesn't exist.");
            }
#endif
            yield break;
        }
    }
}
