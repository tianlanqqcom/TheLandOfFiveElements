using System;
using Script.GameFramework.Core;
using Script.GameFramework.Game;
using TMPro;
using UnityEngine;

namespace Script.LFE.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class FloatingText : MonoBehaviour
    {
        public FixedString fixedString;

        [Header("Sin Arguments")] public float a;
        public float w;

        private LanguageManager.LanguageSettings _cachedLanguageType;
        private Vector3 _originPos;
        private float _yOffset;

        // Start is called before the first frame update
        private void Start()
        {
            _cachedLanguageType = LanguageManager.Instance.nowLanguage;
            gameObject.GetComponent<TMP_Text>().text = fixedString.Message;
            _originPos = gameObject.transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            _yOffset = Mathf.Sin(w * Time.time) * a;
            gameObject.transform.position = new Vector3(_originPos.x, _originPos.y + _yOffset, _originPos.z);

            if (LanguageManager.Instance.nowLanguage == _cachedLanguageType) return;

            _cachedLanguageType = LanguageManager.Instance.nowLanguage;
            gameObject.GetComponent<TMP_Text>().text = fixedString.Message;
        }
    }
}