/*
 * File: MyAppConfig.cs
 * Description: 用于读取和写入配置文件
 * Author: 欸呀呀呀 https://juejin.cn/post/7145844632595742727
 * Last update at 24/1/31 15:53
 * 
 * Update Records:
 * 欸呀呀呀 24/1/30 代码主体
 * tianlan  24/1/31 添加注释
 */


using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Script.GameFramework.Core
{
    // Copied from https://juejin.cn/post/7145844632595742727
    [HelpURL("https://juejin.cn/post/7145844632595742727")]
    public class MyAppConfig
    {

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string value, string path);
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string deval, StringBuilder stringBuilder, int size, string path);


        private readonly string _path;//ini文件的路径

        public MyAppConfig(string path)
        {
            if (IsIniPathExists(Path.Combine(Application.streamingAssetsPath, path)))
            {
                this._path = Path.Combine(Application.streamingAssetsPath, path);
            }
            else
            {
                Debug.LogError("配置文件路径错误");
            }
        }

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section">参数段名称</param>
        /// <param name="key">参数key</param>
        /// <param name="value">参数value</param>
        public void WriteIniContent(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this._path);
        }

        /// <summary>
        /// 读取Ini文件
        /// </summary>
        /// <param name="section">参数段名称</param>
        /// <param name="key">参数的key</param>
        /// <returns></returns>
        public string ReadIniContent(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, this._path);
            return temp.ToString();
        }

        /// <summary>
        /// 删除文件，整个section
        /// </summary>
        /// <param name="section">参数段名称</param>
        /// <returns></returns>
        public void DeleteIniContentAll(string section)
        {
            WritePrivateProfileString(section, null, null, this._path);
        }

        /// <summary>
        /// 删除文件，单独某个参数
        /// </summary>
        /// <param name="section">参数段名称</param>
        /// <param name="key">参数的key</param>
        /// <returns></returns>
        public void DeleteIniContentOne(string section, string key)
        {
            WritePrivateProfileString(section, key, null, this._path);
        }


        /// <summary>
        /// 判断路径是否存在
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static bool IsIniPathExists(string path)
        {
            return File.Exists(path);
        }
    }
}
