using System.Collections.Generic;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using Script.LFE.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.LFE.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public List<Toggle> quality;

        public List<Toggle> lang;

        public GameObject exitGameButton;

        // private QualityManager _qm;
        //
        // private LanguageManager _lm;
        // Start is called before the first frame update
        private void Start()
        {
            // _qm = QualityManager.Instance;
            // _lm = LanguageManager.Instance;
        }

        public void BackToBeginScene()
        {
            SceneLoader.Instance?.LoadScene("BeginScene");
        }

        public void InitUI()
        {
            int idx = (int)QualityManager.Instance.nowQuality - 1;
            quality[idx].isOn = true;

            if (LanguageManager.Instance.nowLanguage == LanguageManager.LanguageSettings.ZH_CN)
            {
                lang[0].isOn = true;
            }
            else
            {
                lang[1].isOn = true;
            }

            if (SceneManager.GetActiveScene().name == "BeginScene")
            {
                exitGameButton.SetActive(false);
            }
            else
            {
                exitGameButton.SetActive(true);
            }
        }

        public void Save()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!quality[i].isOn) continue;
            
                QualityManager.Instance.SetQuality((QualityManager.QualitySetting)(i + 1));
                break;
            }

            if (lang[0].isOn)
            {
                LanguageManager.Instance.SetLanguage(LanguageManager.LanguageSettings.ZH_CN);
            }
            else
            {
                LanguageManager.Instance.SetLanguage(LanguageManager.LanguageSettings.EN_US);
            }

            var ini = new MyAppConfig(Application.streamingAssetsPath + "/app.ini");
            ini.WriteIniContent("language", "now", LanguageManager.Instance.nowLanguage.ToString());
            ini.WriteIniContent("quality", "qu", QualityManager.Instance.nowQuality.ToString());
        
        }
    }
}
