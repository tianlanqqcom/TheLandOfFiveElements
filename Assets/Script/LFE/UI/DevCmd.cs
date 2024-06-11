using System;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using Script.GameFramework.GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace Script.LFE.UI
{
    public class DevCmd : SimpleSingleton<DevCmd>
    {
        public InputField inputField;

        // Start is called before the first frame update
        private void Start()
        {
            InputSystem.Instance.BindKey(KeyCode.LeftAlt, InputSystem.InputEventType.Pressed, () =>
            {
                inputField.gameObject.SetActive(true);
                inputField.text = "";
            });

            InputSystem.Instance.BindKey(KeyCode.Return, InputSystem.InputEventType.Pressed, () =>
            {
                string cmd = inputField.text.Trim();
                if (!string.IsNullOrEmpty(cmd))
                {
                    if (cmd.StartsWith("re"))
                    {
                        var cmds = cmd.Split(" ");
                        if (int.TryParse(cmds[1], out int restartPointIndex))
                        {
                            RestartPlayerAtRestartPoint(restartPointIndex);
                        }
                    }
                    else if (cmd.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    }
                }

                inputField.text = "";
                inputField.gameObject.SetActive(false);
            });
        }

        private void RestartPlayerAtRestartPoint(int index = 0)
        {
            string targetName = $"Restart{index}";
            GameObject[] restartPoints = GameObject.FindGameObjectsWithTag("Restart");
            foreach (var point in restartPoints)
            {
                if (point.name == targetName)
                {
                    MyGameMode.Instance?.RestartPlayerAt(point.transform.position, 5.0f);
                }
            }
        }
    }
}