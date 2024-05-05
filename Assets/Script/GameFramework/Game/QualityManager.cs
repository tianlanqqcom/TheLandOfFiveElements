using Script.GameFramework.Core;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.GameFramework.Game
{
    public class QualityManager : GlobalSingleton<QualityManager>
    {
        public void SetQuality(int index)
        {
            index = Mathf.Clamp(index, 0, 5); 
            QualitySettings.SetQualityLevel(index);
            Logger.Log("QualityManager::SetQuality Change to " + QualitySettings.GetQualityLevel());
        }
        
    }
}
