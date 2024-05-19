/*
 * File: MyGameMode.cs
 * Description: MyGameMode，局部单例类，负责判定胜负等与游戏直接相关的设置
 * Author: tianlan
 * Last update at 24/2/7    23:09
 *
 * Update Records:
 * tianlan  24/1/30 编写代码主体
 * tianlan  24/1/31 添加注释
 * tianlan  24/2/7  引入WorkingMode来替换原先的布尔值判断工作状态
 * tianlan  24/3/1  添加SetMode对PureUI状态的切换逻辑
 * tianlan  24/3/6  添加切换模式的保护，避免在一瞬间多次切换
 * tianlan  24/5/18 添加方法：RestartPlayer和相关方法
 */

using System;
using System.Collections;
using Script.GameFramework.Core;
using UnityEngine;
using UnityEngine.UI;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.GamePlay
{
    public class MyGameMode : SimpleSingleton<MyGameMode>
    {
        [System.Serializable]
        public enum WorkingMode
        {
            Normal_Game, // 正常游戏，接受玩家输入
            Dialog, // 对话状态，玩家输入仅保留鼠标拖拽事件
            Pure_UI // 纯UI模式，完全禁止玩家输入
        }

        /// <summary>
        /// 当前工作状态
        /// </summary>
        public WorkingMode NowWorkingMode = WorkingMode.Normal_Game;

        /// <summary>
        /// 鼠标是否显示
        /// </summary>
        public bool IsMouseShown
        {
            get => Cursor.visible;
        }

        /// <summary>
        /// 已经获胜？
        /// </summary>
        public bool IsWin
        {
            get { return false; }
        }

        /// <summary>
        /// 已经失败？
        /// </summary>
        public bool IsLose
        {
            get { return false; }
        }

        /// <summary>
        /// 是否允许玩家控制器接收输入
        /// </summary>
        [Obsolete("Use NowWorkingMode to judge how to process input.")]
        public bool IsReceivingCharacterInput { get; private set; }

        /// <summary>
        /// 是否刚刚切换模式？与上次切换时间间隔小于0.016s就认为是
        /// </summary>
        public bool IsJustChangeMode
        {
            get { return timeAgainstLastChangeMode < 0.016f; }
        }

        /// <summary>
        /// 距离上次切换模式的时间，注意，这个值最多为10
        /// </summary>
        [Range(0f, 10.0f)] private float timeAgainstLastChangeMode = .0f;

        /// <summary>
        /// 游戏实例数据
        /// </summary>
        MyGameInstance myGameInstance;

        /// <summary>
        /// 游戏状态
        /// </summary>
        MyGameState myGameState;
        
        /// <summary>
        /// 死亡UI显隐协程
        /// </summary>
        private Coroutine _coroutine;

        protected override void Awake()
        {
            base.Awake();

            // IsReceivingCharacterInput = true;

            // Init singletons
            myGameInstance = MyGameInstance.Instance;
            myGameState = MyGameState.Instance;

            HideMouse();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            timeAgainstLastChangeMode += Time.deltaTime;

            if (IsWin || IsLose)
            {
                if (IsWin)
                {
                    WinOptions();
                }
                else
                {
                    LoseOptions();
                }
            }
        }

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        /// <returns>如果游戏结束返回<c>true</c>,否则返回<c>false</c></returns>
        public bool IsGameFinished()
        {
            return IsWin || IsLose;
        }

        /// <summary>
        /// 显示鼠标
        /// </summary>
        public void ShowMouse()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            // IsReceivingCharacterInput = false;
        }

        /// <summary>
        /// 隐藏鼠标
        /// </summary>
        public void HideMouse()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // IsReceivingCharacterInput = true;
        }

        /// <summary>
        /// 获胜后的操作
        /// </summary>
        private void WinOptions()
        {
            Logger.Log("Win");
        }

        /// <summary>
        /// 失败后的操作
        /// </summary>
        private void LoseOptions()
        {
            Logger.Log("Lose");
        }

        /// <summary>
        /// 禁用玩家输入
        /// </summary>
        [Obsolete("Switch NowWorkingMode to change input mode.")]
        public void DisablePlayerInput()
        {
            IsReceivingCharacterInput = false;
            ShowMouse();
        }

        /// <summary>
        /// 恢复玩家输入
        /// </summary>
        [Obsolete("Switch NowWorkingMode to change input mode.")]
        public void EnablePlayerInput()
        {
            IsReceivingCharacterInput = true;
            HideMouse();
        }

        public void SetMode(WorkingMode mode)
        {
            NowWorkingMode = mode;

            if (mode == WorkingMode.Normal_Game)
            {
                HideMouse();
            }
            else
            {
                ShowMouse();
            }

            if (mode == WorkingMode.Pure_UI)
            {
                Time.timeScale = .0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            if (mode == WorkingMode.Dialog)
            {
                // InteractiveUIManager.Instance.DisableUI();
                // BagSystemUI.Instance.DisableUI();
            }

            timeAgainstLastChangeMode = .0f;
        }

        /// <summary>
        /// 在地图原点重生玩家
        /// </summary>
        /// <param name="uiTime">是否启用死亡UI，如果是，该参数为死亡UI显示的时间，否则为.0f</param>
        public void RestartPlayer(float uiTime = .0f)
        {
            RestartPlayerAt(Vector3.zero, uiTime);
        }

        /// <summary>
        /// 在指定地点重生玩家，但不改变玩家死亡时的朝向
        /// </summary>
        /// <param name="pos">指定地点的坐标</param>
        /// <param name="uiTime">是否启用死亡UI，如果是，该参数为死亡UI显示的时间，否则为.0f</param>
        public void RestartPlayerAt(Vector3 pos, float uiTime = .0f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
            {
                Logger.LogError("MyGameMode::RestartPlayerAt Failed to find player.");
                return;
            }

            if (uiTime > .0f)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
                // Show Death UI
                _coroutine = StartCoroutine(ShowDeathUI(uiTime));
            }

            player.transform.position = pos;
        }

        /// <summary>
        /// 在指定地点重生玩家，并设置朝向
        /// </summary>
        /// <param name="pos">指定地点的坐标</param>
        /// <param name="rotation">玩家的新朝向</param>
        /// <param name="uiTime">是否启用死亡UI，如果是，该参数为死亡UI显示的时间，否则为.0f</param>
        public void RestartPlayerAtWithRotation(Vector3 pos, Quaternion rotation, float uiTime = .0f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
            {
                Logger.LogError("MyGameMode::RestartPlayerAt Failed to find player.");
                return;
            }

            if (uiTime > .0f)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                    _coroutine = null;
                }
                // Show Death UI
                _coroutine = StartCoroutine(ShowDeathUI(uiTime));
            }

            player.transform.position = pos;
            player.transform.rotation = rotation;
        }

        /// <summary>
        /// 显示死亡UI
        /// </summary>
        /// <param name="time">死亡UI的显示时长</param>
        /// <returns></returns>
        private IEnumerator ShowDeathUI(float time)
        {
            Transform tdeathUI = GameObject.Find("/Canvas").transform.Find("DeathUI");
            GameObject deathUI = tdeathUI ? tdeathUI.gameObject : null;
            if (!deathUI)
            {
                Logger.LogError("MygameMode::ShowDeathUI Failed to find death ui.");
                yield break;
            }

            Image deathUIImage = deathUI.GetComponent<Image>();
            deathUIImage.color = new Color(0, 0, 0, 0);
            deathUI.SetActive(true);
            float fadeTime = time / 4;
            float deltaAlpha = 1.0f / fadeTime;
            while (deathUIImage.color.a < 1)
            {
                float alpha = deathUIImage.color.a;
                alpha += Time.deltaTime * deltaAlpha;
                alpha = Mathf.Clamp01(alpha);

                deathUIImage.color = new Color(0, 0, 0, alpha);
                yield return 1;
            }

            deathUIImage.color = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(time / 2);
            
            while (deathUIImage.color.a > 0)
            {
                float alpha = deathUIImage.color.a;
                alpha -= Time.deltaTime * deltaAlpha;
                alpha = Mathf.Clamp01(alpha);

                deathUIImage.color = new Color(0, 0, 0, alpha);
                yield return 1;
            }
            deathUIImage.color = new Color(0, 0, 0, 0);
            deathUI.SetActive(false);
        }
    }
}