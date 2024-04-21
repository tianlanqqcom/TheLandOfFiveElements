/*
 * File: LanguageManagerTest.cs
 * Description: 测试LanguageManager是否可用
 * Author: tianlan
 * Last update at 24/1/31 17:41
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using Script.GameFramework.Game;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Test
{
    public class LanguageManagerTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // LanguageManager.Instance.SetLanguage(LanguageManager.LanguageSettings.ZH_CN);
            TextAsset txt = Resources.Load<TextAsset>(LanguageManager.Instance.GetNowLanguageAssestsPath("zhtest"));
            if (txt != null)
            {
                Logger.Log(txt.text);
            }
            else
            {
                Logger.LogError("Failed to load file");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

