using System;
using System.Reflection;
using Script.GameFramework.Attributes;
using Script.GameFramework.Core;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game
{
    public class QualityManager : GlobalSingleton<QualityManager>
    {
        /// <summary>
        /// 可选的语言
        /// </summary>
        [Serializable]
        public enum QualitySetting
        {
            Low = 1,
            Medium = 2,
            High = 3,
            Ex = 4
        }

        /// <summary>
        /// 当前语言类型，初始值从配置文件中获取
        /// </summary>
        [FormerlySerializedAs("NowLanguage")] [Config("app.ini", "quality", "qu")]
        public QualitySetting nowQuality;
        public void SetQuality(QualitySetting index)
        {
            // index = Mathf.Clamp(index, 0, 5); 
            QualitySettings.SetQualityLevel((int)index);
            nowQuality = index;
            Logger.Log("QualityManager::SetQuality Change to " + QualitySettings.GetQualityLevel());
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
                    else if (field.FieldType == typeof(QualitySetting))
                    {
                        foreach (QualitySetting qualitySetting in Enum.GetValues(typeof(QualitySetting)))
                        {
                            if (!value.Equals(qualitySetting.ToString())) continue;
                            // field.SetValue(this, qualitySetting);
                            SetQuality(qualitySetting);
                            Logger.Log("QualityManager:[ConfigInit] Set now quality to " + value);
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogError("QualityManager:[ConfigInit] Not supported type: " + field.FieldType.FullName);
                    }
                }
            }
        }
        
    }
}
