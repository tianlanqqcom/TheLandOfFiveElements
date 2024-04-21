/*
 * File: ConfigAttribute.cs
 * Description: 用于标记字段或属性的自定义特性，标记后将从对应的配置文件中获取对应的值，但需要手动调用函数来初始化字段的值。
 * Author: tianlan
 * Last update at 24/1/31 15:31
 * 
 * Update Records:
 * tianlan  24/1/30 编写特性主体
 * tianlan  24/1/31 添加注释
 */

using System;
using UnityEngine;
using GameFramework.Core;

namespace GameFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// 属性或字段在配置文件中对应的值
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 从配置文件中读取对应节的键的值到Value
        /// </summary>
        /// <param name="configFileName">配置文件名</param>
        /// <param name="section">配置节名</param>
        /// <param name="key">配置键</param>
        public ConfigAttribute(string configFileName, string section, string key)
        {
            MyAppConfig ini;

            // 如果配置文件名为空，从默认配置文件读取
            if (configFileName.Length == 0)
            {
                ini = new MyAppConfig(Application.streamingAssetsPath + @"/app.ini");
            }
            // 否则去指定配置文件读取
            else
            {
                ini = new MyAppConfig(Application.streamingAssetsPath + "/" + configFileName);
            }

            // 读取到Value
            Value = ini.ReadIniContent(section, key);
        }
    }

}
