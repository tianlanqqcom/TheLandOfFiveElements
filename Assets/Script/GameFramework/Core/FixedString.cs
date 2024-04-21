/*
 * File: FixedString.cs
 * Description: 修正后的字符串，字符串来源可选
 * Author: tianlan
 * Last update at 24/3/8    23:05
 * 
 * Update Records:
 * tianlan  24/2/26 修正后的字符串
 * tianlan  24/3/8  添加构造函数并重写ToString()
 */

using System;
using Script.GameFramework.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.GameFramework.Core
{
    [Serializable]
    public class FixedString
    {
        /// <summary>
        /// 文本来源
        /// </summary>
        [FormerlySerializedAs("Source")] [Tooltip("文本来源")]
        public MessageLanguageSourceType source;

        /// <summary>
        /// 原生字符串
        /// </summary>
        [FormerlySerializedAs("RawString")]
        [SerializeField]
        [Tooltip("原生字符串")]
        private string rawString;

        /// <summary>
        /// 字符串字典名
        /// </summary>
        [FormerlySerializedAs("MessageDictionary")]
        [SerializeField]
        [Tooltip("字符串字典名")]
        private string messageDictionary;

        /// <summary>
        /// 字符串键名
        /// </summary>
        [FormerlySerializedAs("MessageKey")]
        [SerializeField]
        [Tooltip("字符串键名")]
        private string messageKey;

        /// <summary>
        /// 获取消息字符串
        /// </summary>
        public string Message => GetMessage();

        /// <summary>
        /// 无参构造
        /// </summary>
        public FixedString()
        {

        }

        /// <summary>
        /// 单参数构造，使用原生字符串
        /// </summary>
        /// <param name="rawString">原生字符串</param>
        public FixedString(string rawString)
        {
            source = MessageLanguageSourceType.UseNativeMessage;
            this.rawString = rawString;
        }

        /// <summary>
        /// 双参数构造，使用语言管理器
        /// </summary>
        /// <param name="dict">字典名</param>
        /// <param name="key">键名</param>
        public FixedString(string dict, string key)
        {
            source = MessageLanguageSourceType.UseLanguageManager;
            messageDictionary = dict;
            messageKey = key;
        }

        /// <summary>
        /// 重写ToString方法用于Debug
        /// </summary>
        /// <returns>该对象转化成的字符串</returns>
        public override string ToString()
        {
            var result = "[FixedString]: Source:";
            result += source.ToString();
            switch (source)
            {
                case MessageLanguageSourceType.UseNativeMessage:
                    result += " RawString:";
                    result += rawString;
                    break;
                case MessageLanguageSourceType.UseLanguageManager:
                    result += " Dict:";
                    result += messageDictionary;
                    result += " Key:";
                    result += messageKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            result += " Message:";
            result += GetMessage();
            return result;
        }

        /// <summary>
        /// 获取消息字符串
        /// </summary>
        /// <returns></returns>
        private string GetMessage()
        {
            if(source == MessageLanguageSourceType.UseLanguageManager)
            {
                return JsonPool.AnalyzeFile(
                    LanguageManager.Instance.GetNowLanguageAssestsPath(messageDictionary),
                    messageKey);
            }

            return rawString;
        }

        /// <summary>
        /// 设置原生字符串
        /// </summary>
        /// <param name="newRawString"></param>
        public void SetRawString(string newRawString)
        {
            this.rawString = newRawString;
        }

        /// <summary>
        /// 设置字典文件
        /// </summary>
        /// <param name="newMessageDictionary">字典</param>
        public void SetMessageDictionary(string newMessageDictionary)
        {
            this.messageDictionary = newMessageDictionary;
        }

        /// <summary>
        /// 设置字典键
        /// </summary>
        /// <param name="key">键</param>
        public void SetMessageKey(string key)
        {
            messageKey = key;
        }

        /// <summary>
        /// 设置文本来源
        /// </summary>
        /// <param name="newSource">文本源</param>
        public void SetSourceMode(MessageLanguageSourceType newSource)
        {
            this.source = newSource;
        }
    }
}

