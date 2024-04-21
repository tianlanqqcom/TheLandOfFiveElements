/*
 * File: JsonPool.cs
 * Description: Json解析缓存池
 * Author: tianlan
 * Last update at 24/2/26   21:37
 * 
 * Update Records:
 * tianlan  24/2/26 Json解析缓存池
 */

using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Core
{
    public class JsonPool
    {
        /// <summary>
        /// Json解析缓存
        /// </summary>
        private readonly static Dictionary<string, Dictionary<string, string>> pool = new();

        /// <summary>
        /// 解析文件，仅支持Dict<string,string>类型的json
        /// </summary>
        /// <param name="shortPath">文件的短路径，不含后缀，e.g. ZH_CN/test</param>
        /// <returns>返回该文件解析后的字典</returns>
        public static Dictionary<string, string> AnalyzeFile(string shortPath)
        {
            if(pool.ContainsKey(shortPath))
            {
                return pool[shortPath];
            }

            TextAsset messageDict = Resources.Load<TextAsset>(shortPath);

            string jsonContent;
            if (messageDict != null)
            {
                jsonContent = messageDict.text;
            }
            else
            {
                Logger.LogError("JsonPool: AnalyzeFile() Target dictionary file doesn't exist.Path = " +
                    shortPath);
                return null;
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            pool.Add(shortPath, dict);
            return dict;
        }

        /// <summary>
        /// 解析文件并获得对应键的值
        /// </summary>
        /// <param name="shortPath">文件的短路径</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string AnalyzeFile(string shortPath, string key)
        {
            var cache = AnalyzeFile(shortPath);

            if (cache == null || !cache.ContainsKey(key))
            {
                return string.Empty;
            }

            return cache[key];
        }
    }
}

