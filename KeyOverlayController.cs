using HarmonyLib;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TootTallyKeyOverlay
{
    public class KeyOverlayController : MonoBehaviour
    {
        private static readonly KeyCode[] _PREVIEW_KEYCODES = { KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.D,
                                                                KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G};
        private KeyOverlayUIHolder _uiHolder;
        private Dictionary<KeyCode, SingleKey> _keyPressedDict;
        private List<KeyCode> _tootKeys;
        public bool isActive;
        private bool _isPreview;


        public void Init(bool isPreview = false)
        {
            _uiHolder = new KeyOverlayUIHolder(gameObject, isPreview);
            _tootKeys = new List<KeyCode>();
            for (int i = 0; i < GlobalVariables.localsettings.keyboard_tooting_keys.Length; i++)
            {
                if (GlobalVariables.localsettings.keyboard_tooting_keys[i])
                {
                    _tootKeys.Add(GlobalVariables.keyboard_key_keycodes[i]);
                }
            }
            _keyPressedDict = new Dictionary<KeyCode, SingleKey>();
            enabled = true;
            isActive = true;
            _isPreview = isPreview;
        }

        public void Update()
        {
            if (!isActive) return;

            if (_isPreview)
            {
                if (Random.Range(0f, 1f) >= .96f)
                {
                    var index = (int)Random.Range(0, Mathf.Min(Plugin.Instance.KeyCountLimit.Value, _PREVIEW_KEYCODES.Length));
                    var keycode = _PREVIEW_KEYCODES[index];
                    if (_keyPressedDict.ContainsKey(keycode))
                        if (_keyPressedDict[keycode].isPressed)
                            ManualKeyRelease(keycode);
                        else
                            ManualKeyPress(keycode);
                    else
                        ManualKeyPress(keycode);
                }
                _keyPressedDict.Values.Do(k => k.Update());
                return;
            }
            
            _tootKeys.ForEach(key =>
            {
                if (Input.GetKey(key))
                {
                    if (!_keyPressedDict.ContainsKey(key))
                    {
                        if (_keyPressedDict.Count >= Plugin.Instance.KeyCountLimit.Value) return;

                        _keyPressedDict.Add(key, _uiHolder.CreateNewKey(key));
                        _keyPressedDict[key].OnKeyPress();
                        Plugin.LogInfo($"New key pressed, adding {key} to overlay.");
                    }
                    else if (!_keyPressedDict[key].isPressed)
                        _keyPressedDict[key].OnKeyPress();
                }
                else if (_keyPressedDict.ContainsKey(key) && _keyPressedDict[key].isPressed)
                    _keyPressedDict[key].OnKeyRelease();
            });

            _keyPressedDict.Values.Do(k => k.Update());
        }

        public void OnDestroy()
        {
            _uiHolder?.Dispose();
            _tootKeys?.Clear();
            _keyPressedDict?.Clear();
        }

        public void ManualKeyPress(KeyCode key)
        {
            if (!_keyPressedDict.ContainsKey(key) && _keyPressedDict.Count < Plugin.Instance.KeyCountLimit.Value)
            {
                _keyPressedDict.Add(key, _uiHolder.CreateNewKey(key));
                _keyPressedDict[key].OnKeyPress();
                return;
            }
            else if (_keyPressedDict.ContainsKey(key) && !_keyPressedDict[key].isPressed)
                _keyPressedDict[key].OnKeyPress();
        }

        public void ManualKeyRelease(KeyCode key)
        {
            if (_keyPressedDict.ContainsKey(key) && _keyPressedDict[key].isPressed) _keyPressedDict[key].OnKeyRelease();
        }

        public void UpdateGraphics()
        {
            _uiHolder.UpdateGraphics();

            if (_keyPressedDict.Count > Plugin.Instance.KeyCountLimit.Value)
            {
                for (int i = (int)Plugin.Instance.KeyCountLimit.Value; i < _PREVIEW_KEYCODES.Length; i++)
                {
                    var key = _PREVIEW_KEYCODES[i];
                    if (_keyPressedDict.ContainsKey(key))
                    {
                        _keyPressedDict[key].Dispose();
                        _keyPressedDict.Remove(key);
                    }
                }
                
            }

            _keyPressedDict.Values.Do(x => x.UpdateGraphics());
        }

        public void Disable() => isActive = false;
        public void Enable() => isActive = true;
    }
}
