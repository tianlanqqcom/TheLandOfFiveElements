/*
 * File: LanguageManager.cs
 * Description: 语言管理器
 * Author: tianlan
 * Last update at 24/1/31 16:27
 *
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using System;
using System.Reflection;
using Script.GameFramework.Attributes;
using Script.GameFramework.Core;
using Script.GameFramework.Log;
using UnityEngine.Serialization;

namespace Script.GameFramework.Game
{
    public class LanguageManager : GlobalSingleton<LanguageManager>
    {
        /// <summary>
        /// 可选的语言
        /// </summary>
        [Serializable]
        public enum LanguageSettings
        {
            EN_US,  // 英文（美国）
            ZH_CN   // 中文（中国）
        }

        /// <summary>
        /// 当前语言类型，初始值从配置文件中获取
        /// </summary>
        [FormerlySerializedAs("NowLanguage")] [Config("app.ini", "language", "now")]
        public LanguageSettings nowLanguage;

        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="newLanguage">新语言</param>
        public void SetLanguage(LanguageSettings newLanguage)
        {
            nowLanguage = newLanguage;
        }

        /// <summary>
        /// 获取对应文件在当前语言下的路径
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>当前语言下的资产路径</returns>
        public string GetNowLanguageAssestsPath(string filename)
        {
            if (nowLanguage == LanguageSettings.EN_US)
            {
                return "EN_US/" + filename;
            }

            if (nowLanguage == LanguageSettings.ZH_CN)
            {
                return "ZH_CN/" + filename;
            }

            return filename;

        }

        protected override void Awake()
        {
            base.Awake();

            // 查找具有ConfigAttribute特性的字段并初始化
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                // 检查字段是否带有 ConfigAttribute 特性
                ConfigAttribute attribute = (ConfigAttribute)Attribute.GetCustomAttribute(field, typeof(ConfigAttribute));
                if (attribute != null)
                {
                    // 从INI文件中读取对应的数值并赋给字段
                    string value = attribute.Value;
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(this, int.Parse(value));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(this, float.Parse(value));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(this, value);
                    }
                    else if (field.FieldType == typeof(LanguageSettings))
                    {
                        foreach (LanguageSettings languageSettings in Enum.GetValues(typeof(LanguageSettings)))
                        {
                            if (!value.Equals(languageSettings.ToString())) continue;
                            field.SetValue(this, languageSettings);
                            Logger.Log("LanguageManager:[ConfigInit] Set now language to " + value);
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogError("LanguageManager:[ConfigInit] Not supported type: " + field.FieldType.FullName);
                    }
                }
            }
        }
    }
}

