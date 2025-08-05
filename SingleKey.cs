using System;
using System.Diagnostics.Eventing.Reader;
using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    public class SingleKey
    {
        private GameObject _fullGO, _innerGO;
        private Image _outerImage, _innerImage;
        private RawImage _innerBeam;
        private Color[] _innerBeamPixelArray;
        private Color _innerBeamPressColor, _innerBeamReleaseColor;
        private Texture2D _innerBeamTexture;
        private TMP_Text _text, _pressCountText;
        private int _pressCount, _beamLength;
        private float _elapsedTime;
        private readonly float _REFRESH_RATE;

        public bool isPressed, lastIsPressed;

        public SingleKey(GameObject gameObject, KeyCode key)
        {
            _REFRESH_RATE = Plugin.Instance.BeamSpeed.Value;
            _beamLength = (int)Plugin.Instance.BeamLength.Value;
            _fullGO = gameObject;
            _outerImage = _fullGO.GetComponent<Image>();
            _outerImage.color = Plugin.Instance.KeyOuterColor.Value;

            _innerGO = _fullGO.transform.GetChild(0).gameObject;
            _innerImage = _innerGO.GetComponent<Image>();
            _innerImage.color = Plugin.Instance.KeyInnerColor.Value;

            _innerBeam = _fullGO.transform.GetChild(1).GetComponent<RawImage>();
            _innerBeamTexture = new Texture2D(_beamLength, 1, TextureFormat.RGBAFloat, false);
            _innerBeamPixelArray = new Color[_beamLength];
            _innerBeamPressColor = new Color(Plugin.Instance.BeamColor.Value.r, Plugin.Instance.BeamColor.Value.g, Plugin.Instance.BeamColor.Value.b, .7f);
            _innerBeamReleaseColor = new Color(0, 0, 0, 0);
            Array.Fill(_innerBeamPixelArray, _innerBeamPressColor);
            _innerBeamTexture.SetPixels(0, 0, _beamLength, 1, _innerBeamPixelArray);
            _innerBeamTexture.Apply();
            _innerBeam.texture = _innerBeamTexture;
            _innerBeam.color = Plugin.Instance.BeamColor.Value;

            _text = _innerGO.transform.GetChild(0).GetComponent<TMP_Text>();
            _text.text = key.ToString();
            _pressCountText = _innerGO.transform.GetChild(1).GetComponent<TMP_Text>();
        }

        public void Update()
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime < 1f / _REFRESH_RATE) return;
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
    }
}
