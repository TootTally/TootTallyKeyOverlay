using System;
using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    public class SingleKey
    {
        private GameObject _fullGO, _innerGO;
        private Image _outerImage, _innerImage;
        private TMP_Text _text, _pressCountText;
        private int _pressCount;

        public bool isPressed;

        public SingleKey(GameObject gameObject, KeyCode key)
        {
            _fullGO = gameObject;
            _outerImage = _fullGO.GetComponent<Image>();
            _outerImage.color = Plugin.Instance.KeyOuterColor.Value;
            _innerGO = _fullGO.transform.GetChild(0).gameObject;
            _innerImage = _innerGO.GetComponent<Image>();
            _innerImage.color = Plugin.Instance.KeyInnerColor.Value;
            _text = _innerGO.transform.GetChild(0).GetComponent<TMP_Text>();
            _text.text = key.ToString();
            _pressCountText = _innerGO.transform.GetChild(1).GetComponent<TMP_Text>();
        }

        public void Update()
        {

        }

        public void OnKeyPress()
        {
            _innerImage.color = Plugin.Instance.KeyPressedInnerColor.Value;
            _outerImage.color = Plugin.Instance.KeyPressedOuterColor.Value;

            _pressCount++;
            _pressCountText.color = Plugin.Instance.KeyPressedTextColor.Value;
            _pressCountText.text = _pressCount.ToString();
            isPressed = true;
        }
        public void OnKeyRelease()
        {
            _innerImage.color = Plugin.Instance.KeyInnerColor.Value;
            _outerImage.color = Plugin.Instance.KeyOuterColor.Value;

            _pressCountText.color = Plugin.Instance.KeyTextColor.Value;
            isPressed = false;
        }
    }
}
