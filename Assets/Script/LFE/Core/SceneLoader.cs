/*
 * File: SceneLoader.cs
 * Description: 异步加载场景并显示加载进度条
 * Author: tianlan
 * Last update at 24/5/14   22:09
 *
 * Update Records:
 * tianlan  24/5/14
 */

using System.Collections;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.LFE.Core
{
    public class SceneLoader : SimpleSingleton<SceneLoader>
    {
        public GameObject loadingUI; // 加载UI的父对象
        public Slider progressBar;
        public TMP_Text progressText;

        public void NormalLoadScene(string sceneName)
        {
            InputSystem.Instance?.ClearAllInput();
            SceneManager.LoadScene(sceneName);
        }

        // 调用此方法开始加载新场景
        public void LoadScene(string sceneName)
        {
            InputSystem.Instance?.ClearAllInput();
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (!loadingUI || !progressBar || !progressText)
            {
                Logger.LogError("SceneLoader::LoadSceneAsync Loading UI is null, LoadSceneAsync Failed.");
                yield break;
            }

            // 激活加载UI
            loadingUI.SetActive(true);

            // 异步加载场景
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                Logger.LogError("SceneLoader::LoadSceneAsync AsyncOperation is null, LoadSceneAsync Failed.");
                yield break;
            }

            operation.allowSceneActivation = false; // 暂时不自动切换场景

            // 更新进度条和进度文本
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                progressBar.value = progress;
                progressText.text = (progress * 100).ToString("F2") + "%";

                // 当加载进度达到90%时，等待用户确认或条件触发
                if (operation.progress >= 0.9f)
                {
                    // 设置进度为100%
                    progressBar.value = 1f;
                    progressText.text = "100%";

                    // 可以添加用户确认或者延时等
                    yield return new WaitForSeconds(.5f); // 例如延时0.5秒

                    // 激活场景切换
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            // 隐藏加载UI（如果需要在新场景中继续使用此UI，则可以移除这行）
            loadingUI.SetActive(false);
        }
    }
}