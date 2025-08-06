using System;
using System.Diagnostics.Eventing.Reader;
using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using static TootTallyKeyOverlay.KeyOverlayUIHolder;

namespace TootTallyKeyOverlay
{
    public class SingleKey
    {
        private GameObject _fullGO, _innerGO;
        private Image _outerImage, _innerImage;
        private RawImage _innerBeam;
        private RectTransform _innerBeamRectTransform, _fullGORect, _innerGoRect;
        private Color[] _innerBeamPixelArray;
        private Color _innerBeamPressColor, _innerBeamReleaseColor;
        private Texture2D _innerBeamTexture;
        private TMP_Text _text, _pressCountText;
        private int _pressCount, _beamLength;
        private float _elapsedTime;
        private float _refreshRate;

        public bool isPressed, lastIsPressed;

        public SingleKey(GameObject gameObject, KeyCode key)
        {
            _fullGO = gameObject;
            _fullGORect = _fullGO.GetComponent<RectTransform>();
            _outerImage = _fullGO.GetComponent<Image>();
            _innerGO = _fullGO.transform.GetChild(0).gameObject;
            _innerGoRect = _innerGO.GetComponent<RectTransform>();
            _innerImage = _innerGO.GetComponent<Image>();
            _innerBeam = _fullGO.transform.GetChild(1).GetComponent<RawImage>();
            _innerBeamRectTransform = _innerBeam.GetComponent<RectTransform>();
            _innerBeamRectTransform.pivot = Vector2.one;
            _innerBeamReleaseColor = new Color(0, 0, 0, 0);
            _text = _innerGO.transform.GetChild(0).GetComponent<TMP_Text>();
            _text.text = key.ToString();
            _pressCountText = _innerGO.transform.GetChild(1).GetComponent<TMP_Text>();
            UpdateGraphics();
        }

        public void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < 1f / _refreshRate) return;
            _elapsedTime = 0;

            for (int i = 0; i < _innerBeamPixelArray.Length - 1; i++)
            {
                if (i <= _beamLength / 10f && _innerBeamPixelArray[i + 1] != _innerBeamReleaseColor)
                    _innerBeamPixelArray[i] = Color.Lerp(_innerBeamReleaseColor, _innerBeamPressColor, i / (_beamLength / 10f));
                else
                    _innerBeamPixelArray[i] = _innerBeamPixelArray[i + 1];
            }

            _innerBeamTexture.SetPixels(0, 0, _beamLength, 1, _innerBeamPixelArray);
            _innerBeamTexture.Apply();
            _innerBeam.texture = _innerBeamTexture;
        }

        public void OnKeyPress()
        {
            _innerImage.color = Plugin.Instance.KeyPressedInnerColor.Value;
            _outerImage.color = Plugin.Instance.KeyPressedOuterColor.Value;
            _innerBeamPixelArray[_innerBeamPixelArray.Length - 1] = _innerBeamPressColor;
            _pressCount++;
            _pressCountText.color = Plugin.Instance.KeyPressedTextColor.Value;
            _pressCountText.text = _pressCount.ToString();
            isPressed = true;
        }
        public void OnKeyRelease()
        {
            _innerImage.color = Plugin.Instance.KeyInnerColor.Value;
            _outerImage.color = Plugin.Instance.KeyOuterColor.Value;
            _innerBeamPixelArray[_innerBeamPixelArray.Length - 1] = _innerBeamReleaseColor;
            _pressCountText.color = Plugin.Instance.KeyTextColor.Value;
            isPressed = false;
        }

        public void Dispose()
        {
            GameObject.DestroyImmediate(_fullGO);
        }

        public void UpdateGraphics()
        {
            _refreshRate = Plugin.Instance.BeamSpeed.Value;
            _beamLength = (int)Plugin.Instance.BeamLength.Value;
            _fullGORect.sizeDelta = Vector2.one * (Plugin.Instance.KeyElementSize.Value + Plugin.Instance.KeyOutlineThiccness.Value);
            _innerGoRect.sizeDelta = Vector2.one * Plugin.Instance.KeyElementSize.Value;
            _innerBeamTexture = new Texture2D(_beamLength, 1, TextureFormat.RGBAFloat, false);
            _innerBeamPixelArray = new Color[_beamLength];
            Array.Fill(_innerBeamPixelArray, _innerBeamReleaseColor);
            _innerBeamTexture.SetPixels(0, 0, _beamLength, 1, _innerBeamPixelArray);
            _innerBeamTexture.Apply();
            _innerBeam.texture = _innerBeamTexture;
            _innerBeam.color = Plugin.Instance.BeamColor.Value;
            _innerBeamRectTransform.sizeDelta = new Vector2(Plugin.Instance.BeamLength.Value, Plugin.Instance.KeyElementSize.Value + Plugin.Instance.KeyOutlineThiccness.Value);

            switch (Plugin.Instance.BeamDirection.Value)
            {
                case BeamDirection.Up:
                    _innerBeamRectTransform.anchorMax = _innerBeamRectTransform.anchorMin = Vector2.one;
                    _innerBeamRectTransform.eulerAngles = new Vector3(0, 0, -90);
                    break;
                case BeamDirection.Down:
                    _innerBeamRectTransform.anchorMax = _innerBeamRectTransform.anchorMin = Vector2.zero;
                    _innerBeamRectTransform.eulerAngles = new Vector3(0, 0, 90);
                    break;
                case BeamDirection.Right:
                    _innerBeamRectTransform.anchorMax = _innerBeamRectTransform.anchorMin = new Vector2(1, 0);
                    _innerBeamRectTransform.eulerAngles = new Vector3(0, 0, 180);
                    break;
                default:
                    _innerBeamRectTransform.anchorMax = _innerBeamRectTransform.anchorMin = new Vector2(0, 1);
                    _innerBeamRectTransform.eulerAngles = new Vector3(0, 0, 0);
                    break;
            }

            _innerImage.color = Plugin.Instance.KeyInnerColor.Value;
            _outerImage.color = Plugin.Instance.KeyOuterColor.Value;
            _innerBeamPressColor = new Color(Plugin.Instance.BeamColor.Value.r, Plugin.Instance.BeamColor.Value.g, Plugin.Instance.BeamColor.Value.b, .7f);
            var textColor = Plugin.Instance.KeyTextColor.Value;
            _text.color = new Color(textColor.r, textColor.g, textColor.b, .3f);
            _pressCountText.color = textColor;
            _pressCountText.rectTransform.sizeDelta = _text.rectTransform.sizeDelta = Vector2.one * Plugin.Instance.KeyElementSize.Value;
            _text.margin = Vector4.one * Plugin.Instance.KeyOutlineThiccness.Value;

        }
    }
}
