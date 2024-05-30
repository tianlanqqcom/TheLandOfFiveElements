using Script.GameFramework.Game.Dialog;
using Script.LFE.GamePlay;
using TMPro;
using UnityEngine;
using Logger = Script.GameFramework.Log.Logger;

namespace Script.LFE.UI
{
    public class StarCountUI : MonoBehaviour
    {
        public int maxStarCount;
        public TMP_Text starLabel;

        public GameObject endTrans;
        public string endDialogName;
        
        private LPlayerState _playerState;
        private int _nowStarCount;
        
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player)
            {
                _playerState = player.GetComponent<LPlayerState>();
            }

            if (!_playerState)
            {
                Logger.LogError("StarCountUI::Start Failed to find player state.");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_playerState) return;
            if (_playerState.NowStarCount == _nowStarCount) return;
            
            _nowStarCount = _playerState.NowStarCount;
            starLabel.text = $"{_nowStarCount}/{maxStarCount}";

            if (_nowStarCount != maxStarCount) return;
            endTrans.SetActive(true);
            DialogSystem.Instance?.BeginDialogNoInterrupt(endDialogName);
        }
    }
}
