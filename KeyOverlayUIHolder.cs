using System;
using System.Collections.Generic;
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

        public KeyOverlayUIHolder(GameObject uiHolder)
        {
            elementSize = Plugin.Instance.KeyElementSize.Value;
            marginSize = Plugin.Instance.KeyOutlineThiccness.Value;
            fullSize = elementSize + marginSize;
            _uiHolder = uiHolder;
            _rectTransform = _uiHolder.GetComponent<RectTransform>();
            _rectTransform.anchoredPosition3D = Vector3.zero;
            _rectTransform.offsetMin = _rectTransform.offsetMax = Vector2.zero;
            _rectTransform.anchorMin = _rectTransform.anchorMax = _rectTransform.pivot = new Vector2(1,0);
            _rectTransform.localScale = Vector3.one;
            _rectTransform.sizeDelta = Vector2.one * 55f;
            _gridLayout = _uiHolder.GetComponent<GridLayoutGroup>();
            _gridLayout.constraint = Plugin.Instance.HorizontalAlignement.Value ? GridLayoutGroup.Constraint.FixedRowCount : GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.startAxis = Plugin.Instance.HorizontalAlignement.Value ? GridLayoutGroup.Axis.Horizontal : GridLayoutGroup.Axis.Vertical;
            _gridLayout.constraintCount = 1;
            _gridLayout.childAlignment = TextAnchor.LowerCenter;
            _gridLayout.spacing = Vector2.one * 2f;
            _gridLayout.childAlignment = TextAnchor.MiddleCenter;
            _gridLayout.cellSize = Vector2.one * fullSize;
            SetSingleKeyPrefab();
        }

        private void SetSingleKeyPrefab()
        {
            _singleKeyPrefab = new GameObject("SingleKeyPrefab", typeof(Image));
            var rectTransform = _singleKeyPrefab.GetComponent<RectTransform>();
            rectTransform.sizeDelta = Vector2.one * fullSize;
            var outerImage = _singleKeyPrefab.GetComponent<Image>();
            outerImage.color = Plugin.Instance.KeyOuterColor.Value;

            var inner = new GameObject("InnerImage", typeof(Image));
            inner.transform.SetParent(_singleKeyPrefab.transform);
            var innerRectTransform = inner.GetComponent<RectTransform>();
            innerRectTransform.sizeDelta = Vector2.one * elementSize;
            var innerImage = inner.GetComponent<Image>();
            innerImage.color = Plugin.Instance.KeyInnerColor.Value;

            var innerBeam = new GameObject("InnerImage", typeof(RawImage));
            innerBeam.transform.SetParent(_singleKeyPrefab.transform);
            var innerBeamRectTransform = innerBeam.GetComponent<RectTransform>();
            innerBeamRectTransform.sizeDelta = new Vector2(Plugin.Instance.BeamLength.Value, fullSize);
            innerBeamRectTransform.anchorMax = innerBeamRectTransform.anchorMin = Vector2.zero;
            innerBeamRectTransform.pivot = new Vector2(1, 0);
            if (Plugin.Instance.HorizontalAlignement.Value)
                innerBeamRectTransform.eulerAngles = new Vector3(0, 0, -90);

            var innerBeamImage = innerBeam.GetComponent<RawImage>();
            innerBeamImage.color = Plugin.Instance.KeyInnerColor.Value;

            var textKeyHolder = GameObjectFactory.CreateSingleText(inner.transform, "TextKey", "X");
            var text = textKeyHolder.GetComponent<TMP_Text>();
            text.rectTransform.sizeDelta = new Vector2(elementSize, marginSize);
            var textColor = Plugin.Instance.KeyTextColor.Value;
            text.color = new Color(textColor.r, textColor.g, textColor.b, .3f);
            text.margin = Vector4.one * marginSize;
            text.enableAutoSizing = true;
            text.fontSizeMin = 5;

            var textKeyPressHolder = GameObjectFactory.CreateSingleText(inner.transform, "TextKeyPress", "0");
            var textKeypress = textKeyPressHolder.GetComponent<TMP_Text>();
            textKeypress.rectTransform.sizeDelta = new Vector2(elementSize, 0);
            textKeypress.text = "0";
            textKeypress.color = textColor;
            textKeypress.fontSize = 6;
            textKeypress.fontSizeMin = 5;
        }

        public SingleKey CreateNewKey(KeyCode key) => new SingleKey(GameObject.Instantiate(_singleKeyPrefab, _uiHolder.transform), key);
    }
}
