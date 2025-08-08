using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TootTallyCore.Graphics;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    public class KeyOverlayUIHolder
    {
        private float fullSize, elementSize, marginSize;
        private GameObject _uiHolder;
        private GridLayoutGroup _gridLayout;
        private RectTransform _rectTransform;
        private GameObject _singleKeyPrefab;

        public KeyOverlayUIHolder(GameObject uiHolder, bool isPreview = false)
        {
            elementSize = Plugin.Instance.KeyElementSize.Value;
            marginSize = Plugin.Instance.KeyOutlineThiccness.Value;
            fullSize = elementSize + marginSize;
            _uiHolder = uiHolder;
            _rectTransform = _uiHolder.GetComponent<RectTransform>();
            _gridLayout = _uiHolder.GetComponent<GridLayoutGroup>();
            _rectTransform.offsetMin = _rectTransform.offsetMax = Vector2.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.sizeDelta = Vector2.one * 55f;
            if (!isPreview)
            {
                switch (Plugin.Instance.PositionAlignment.Value)
                {
                    case UIAlignment.TopLeft:
                        _gridLayout.childAlignment = TextAnchor.UpperLeft;
                        _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = new Vector2(0, 1);
                        break;
                    case UIAlignment.TopRight:
                        _gridLayout.childAlignment = TextAnchor.UpperRight;
                        _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = Vector2.one;
                        break;
                    case UIAlignment.BottomLeft:
                        _gridLayout.childAlignment = TextAnchor.LowerLeft;
                        _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = Vector2.zero;
                        break;
                    default:
                        _gridLayout.childAlignment = TextAnchor.LowerRight;
                        _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = new Vector2(1, 0);
                        break;
                }
                _rectTransform.anchoredPosition3D = new Vector3(Plugin.Instance.PosXOffset.Value, Plugin.Instance.PosYOffset.Value,0);
            }
            else
            {
                _gridLayout.childAlignment = TextAnchor.LowerRight;
                _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = new Vector2(1, 0);
            }

            _gridLayout.constraintCount = 1;
            _gridLayout.spacing = Vector2.one * 2f;
            _gridLayout.cellSize = Vector2.one * fullSize;
            SetSingleKeyPrefab();
            UpdateGraphics();
        }

        private void SetSingleKeyPrefab()
        {
            _singleKeyPrefab = new GameObject("SingleKeyPrefab", typeof(Image));

            var inner = new GameObject("InnerImage", typeof(Image));
            inner.transform.SetParent(_singleKeyPrefab.transform);

            var innerBeam = new GameObject("InnerImage", typeof(RawImage));
            innerBeam.transform.SetParent(_singleKeyPrefab.transform);

            var textKeyHolder = GameObjectFactory.CreateSingleText(inner.transform, "TextKey", "X");
            var text = textKeyHolder.GetComponent<TMP_Text>();
            text.rectTransform.anchorMin = text.rectTransform.anchorMax = text.rectTransform.pivot = Vector2.zero;
            text.enableAutoSizing = true;
            text.fontSize = 18;
            text.fontSizeMin = 5;

            var textKeyPressHolder = GameObjectFactory.CreateSingleText(inner.transform, "TextKeyPress", "0");
            var textKeypress = textKeyPressHolder.GetComponent<TMP_Text>();
            textKeypress.rectTransform.anchorMin = textKeypress.rectTransform.anchorMax = textKeypress.rectTransform.pivot = Vector2.zero;
            textKeypress.text = "0";
            textKeypress.fontSize = 6;
            textKeypress.fontSizeMin = 5;
        }

        public void UpdateGraphics()
        {
            elementSize = Plugin.Instance.KeyElementSize.Value;
            marginSize = Plugin.Instance.KeyOutlineThiccness.Value;
            fullSize = elementSize + marginSize;
            if (Plugin.Instance.HorizontalAlignment.Value)
            {
                _gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                _gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            }
            else
            {
                _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                _gridLayout.startAxis = GridLayoutGroup.Axis.Vertical;
            }
            _gridLayout.cellSize = Vector2.one * fullSize;
        }

        public SingleKey CreateNewKey(KeyCode key) => new SingleKey(GameObject.Instantiate(_singleKeyPrefab, _uiHolder.transform), key);


        public enum UIAlignment
        {
            BottomRight,
            BottomLeft,
            TopRight,
            TopLeft,
        }

        public enum BeamDirection
        {
            Left,
            Up,
            Right,
            Down,
        }
    }
}
