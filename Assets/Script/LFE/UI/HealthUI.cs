using System;
using Script.GameFramework.GamePlay;
using Script.LFE.GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.LFE.UI
{
    public class HealthUI : MonoBehaviour
    {
        public TMP_Text hpLabel;
        private LPlayerState _playerState;

        private Slider _slider;

        // Start is called before the first frame update
        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            _playerState = player.GetComponent<LPlayerState>();

            _slider = gameObject.GetComponent<Slider>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_playerState || !_slider)
            {
                Debug.LogError("HealthUI::Update No Player or Slider Found!");
                return;
            }

            var progress = Mathf.Clamp01(_playerState.NowHp * 1.0f / _playerState.MaxHp);
            _slider.value = progress;
            hpLabel.text = $"{_playerState.NowHp}/{_playerState.MaxHp}";
            if (_playerState.NowHp == 0)
            {
                MyGameMode.Instance.RestartPlayer(1.0f);
            }
        }
    }
}