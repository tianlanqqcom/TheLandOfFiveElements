/*
 * File: IniTest.cs
 * Description: 测试MyAppConfig是否可用
 * Author: tianlan
 * Last update at 24/1/31 17:41
 * 
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 */

using UnityEngine;
using GameFramework.Core;
using GameFramework.Game;

namespace GameFramework.Test
{
    public class IniTest : MonoBehaviour
    {
        MyAppConfig ini;

        void Start()
        {
            //获取ini文件
            ini = new MyAppConfig(Application.streamingAssetsPath + @"/app.ini");

            string speed = ini.ReadIniContent("language", "now");

            Logger.Log(speed);

            ini.WriteIniContent("Count", "count", "1");//写入
                                                       // ini.WriteIniContent("Count", "count", "1");//写入
            ini.WriteIniContent("language", "now", LanguageManager.LanguageSettings.EN_US.ToString());//写入

            //ini.DeleteIniContentAll("Time");//删除一组
            //ini.DeleteIniContentOne("Speed", "speed");//删除一行

        }
    }
}

