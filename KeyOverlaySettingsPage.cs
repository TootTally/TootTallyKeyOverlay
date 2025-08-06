using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TootTallySettings;
using TootTallySettings.TootTallySettingsObjects;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    public class KeyOverlaySettingsPage : TootTallySettingPage
    {
        private static readonly ColorBlock _pageBtnColors = new ColorBlock()
        {
            colorMultiplier = 1f,
            fadeDuration = .2f,
            disabledColor = Color.gray,
            normalColor = new Color(1, 0, 0),
            pressedColor = new Color(1f, .2f, .2f),
            highlightedColor = new Color(.8f, 0, 0),
            selectedColor = new Color(1, 0, 0)
        };

        private KeyOverlayController _keyOverlayPreview;
        private TootTallySettingDropdown _positionDropdown, _beamDirectionDropdown;
        private TootTallySettingToggle _horizontalToggle;
        private TootTallySettingSlider _posXSlider, _posYSlider,
            _keyCountLimitSlider, _keyElementSizeSlider, _keyOutlineSizeSlider,
            _beamSpeedSlider, _beamLengthSlider;
        private TootTallySettingColorSliders _beamColorSliders,
            _keyOuterSliders, _keyPressedOuterSliders,
            _keyInnerSliders, _keyPressedInnerSliders,
            _keyTextSliders, _keyPressedTextSliders;

        public KeyOverlaySettingsPage() : base("Key Overlay", "Key Overlay", 40f, new Color(0, 0, 0, 0), _pageBtnColors)
        {
            AddLabel("Position Alignment");
            _positionDropdown = AddDropdown("Position Alignment", Plugin.Instance.PositionAlignment);
            _posXSlider = AddSlider("PosX Offset", -800f, 800f, 500f, "PosX Offset", Plugin.Instance.PosXOffset, true);
            _posYSlider = AddSlider("PosY Offset", -800f, 800f, 500f, "PosY Offset", Plugin.Instance.PosYOffset, true);

            _horizontalToggle = AddToggle("Horizontal Alignment", Plugin.Instance.HorizontalAlignment);

            _keyCountLimitSlider = AddSlider("Key Count Limit", 1f, 10f, Plugin.Instance.KeyCountLimit, true);

            _keyElementSizeSlider = AddSlider("Key Element Size", 4f, 32f, Plugin.Instance.KeyElementSize, true);

            _keyOutlineSizeSlider = AddSlider("Key Outline Size", 0f, 8f, Plugin.Instance.KeyOutlineThiccness, true);

            AddLabel("Beam Direction");
            _beamDirectionDropdown = AddDropdown("Beam Direction", Plugin.Instance.BeamDirection);

            _beamSpeedSlider = AddSlider("Beam Speed", 60f, 600f, Plugin.Instance.BeamSpeed, true);

            _beamLengthSlider = AddSlider("Beam Length", 30f, 600f, Plugin.Instance.BeamLength, true);

            AddLabel("Beam Color");
            _beamColorSliders = AddColorSliders("Beam Color", "Beam Color", Plugin.Instance.BeamColor);

            AddLabel("KeyOuterColor");
            _keyOuterSliders = AddColorSliders("KeyOuterColor", "KeyOuterColor", Plugin.Instance.KeyOuterColor);

            AddLabel("KeyPressedOuterColor");
            _keyPressedOuterSliders = AddColorSliders("KeyPressedOuterColor", "KeyPressedOuterColor", Plugin.Instance.KeyPressedOuterColor);

            AddLabel("KeyInnerColor");
            _keyInnerSliders = AddColorSliders("KeyInnerColor", "KeyInnerColor", Plugin.Instance.KeyInnerColor);

            AddLabel("KeyPressedInnerColor");
            _keyPressedInnerSliders = AddColorSliders("KeyPressedInnerColor", "KeyPressedInnerColor", Plugin.Instance.KeyPressedInnerColor);

            AddLabel("KeyTextColor");
            _keyTextSliders = AddColorSliders("KeyTextColor", "KeyTextColor", Plugin.Instance.KeyTextColor);

            AddLabel("KeyPressedTextColor");
            _keyPressedTextSliders = AddColorSliders("KeyPressedTextColor", "KeyPressedTextColor", Plugin.Instance.KeyPressedTextColor);
        }

        public override void Initialize()
        {
            base.Initialize();
            _horizontalToggle.toggle.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyCountLimitSlider.slider.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyElementSizeSlider.slider.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyOutlineSizeSlider.slider.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamDirectionDropdown.dropdown.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamSpeedSlider.slider.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamLengthSlider.slider.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamColorSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamColorSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _beamColorSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyOuterSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyOuterSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyOuterSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedOuterSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedOuterSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedOuterSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyInnerSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyInnerSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyInnerSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedInnerSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedInnerSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedInnerSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyTextSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyTextSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyTextSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedTextSliders.sliderR.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedTextSliders.sliderG.onValueChanged.AddListener(UpdatePreviewGraphics);
            _keyPressedTextSliders.sliderB.onValueChanged.AddListener(UpdatePreviewGraphics);
        }

        public override void OnShow()
        {
            InitPreview();
            base.OnShow();
        }

        public override void OnHide()
        {
            DestroyPreview();
            base.OnHide();
        }

        private void InitPreview()
        {
            if (_keyOverlayPreview != null)
                DestroyPreview();

            var uiHolder = new GameObject("KeyOverlayUIHolder", typeof(RectTransform), typeof(GridLayoutGroup), typeof(KeyOverlayController));
            uiHolder.transform.SetParent(gridPanel.transform);
            uiHolder.name = "KeyOverlayUIHolder";
            var layout = uiHolder.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
            _keyOverlayPreview = uiHolder.GetComponent<KeyOverlayController>();
            _keyOverlayPreview.Init(true);
            var rect = uiHolder.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(-750, 1800);
            rect.localScale = Vector3.one * 2.5f;
            
        }

        private void DestroyPreview()
        {
            GameObject.DestroyImmediate(_keyOverlayPreview);
            _keyOverlayPreview = null;
        }

        private void UpdatePreviewGraphics(bool b) => _keyOverlayPreview.UpdateGraphics();
        private void UpdatePreviewGraphics(int b) => _keyOverlayPreview.UpdateGraphics();
        private void UpdatePreviewGraphics(float f) => _keyOverlayPreview.UpdateGraphics();
    }
}
