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

using GameFramework.Game;
using UnityEngine;

namespace GameFramework.Core
{
    [System.Serializable]
    public class FixedString
    {
        /// <summary>
        /// 文本来源
        /// </summary>
        [Tooltip("文本来源")]
        public MessageLanguageSourceType Source;

        /// <summary>
        /// 原生字符串
        /// </summary>
        [SerializeField]
        [Tooltip("原生字符串")]
        private string RawString;

        /// <summary>
        /// 字符串字典名
        /// </summary>
        [SerializeField]
        [Tooltip("字符串字典名")]
        private string MessageDictionary;

        /// <summary>
        /// 字符串键名
        /// </summary>
        [SerializeField]
        [Tooltip("字符串键名")]
        private string MessageKey;

        /// <summary>
        /// 获取消息字符串
        /// </summary>
        public string Message { get { return GetMessage(); } }

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
            Source = MessageLanguageSourceType.UseNativeMessage;
            RawString = rawString;
        }

        /// <summary>
        /// 双参数构造，使用语言管理器
        /// </summary>
        /// <param name="dict">字典名</param>
        /// <param name="key">键名</param>
        public FixedString(string dict, string key)
        {
            Source = MessageLanguageSourceType.UseLanguageManager;
            MessageDictionary = dict;
            MessageKey = key;
        }

        /// <summary>
        /// 重写ToString方法用于Debug
        /// </summary>
        /// <returns>该对象转化成的字符串</returns>
        public override string ToString()
        {
            string result = "[FixedString]: Source:";
            result += Source.ToString();
            if(Source == MessageLanguageSourceType.UseNativeMessage)
            {
                result += " RawString:";
                result += RawString;
            }
            else if(Source == MessageLanguageSourceType.UseLanguageManager)
            {
                result += " Dict:";
                result += MessageDictionary;
                result += " Key:";
                result += MessageKey;
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
            if(Source == MessageLanguageSourceType.UseLanguageManager)
            {
                return JsonPool.AnalyzeFile(
                    LanguageManager.Instance.GetNowLanguageAssestsPath(MessageDictionary),
                    MessageKey);
            }

            return RawString;
        }

        /// <summary>
        /// 设置原生字符串
        /// </summary>
        /// <param name="rawString"></param>
        public void SetRawString(string rawString)
        {
            RawString = rawString;
        }

        /// <summary>
        /// 设置字典文件
        /// </summary>
        /// <param name="messageDictionary">字典</param>
        public void SetMessageDictionary(string messageDictionary)
        {
            MessageDictionary = messageDictionary;
        }

        /// <summary>
        /// 设置字典键
        /// </summary>
        /// <param name="key">键</param>
        public void SetMessageKey(string key)
        {
            MessageKey = key;
        }

        /// <summary>
        /// 设置文本来源
        /// </summary>
        /// <param name="source">文本源</param>
        public void SetSourceMode(MessageLanguageSourceType source)
        {
            Source = source;
        }
    }
}

