using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TootTallyKeyOverlay
{
    public class KeyOverlayController : MonoBehaviour
    {

        private KeyOverlayUIHolder _uiHolder;
        private Dictionary<KeyCode, SingleKey> _keyPressedDict;
        private List<KeyCode> _tootKeys;
        public bool isActive;


        public void Init()
        {
            _uiHolder = new KeyOverlayUIHolder(gameObject);
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
        }

        public void Update()
        {
            if (!isActive) return;

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

        public void ManualKeyPress(KeyCode key)
        {
            if (_keyPressedDict.ContainsKey(key) && !_keyPressedDict[key].isPressed) _keyPressedDict[key].OnKeyPress();
        }

        public void ManualKeyRelease(KeyCode key)
        {
            if (_keyPressedDict.ContainsKey(key) && _keyPressedDict[key].isPressed) _keyPressedDict[key].OnKeyRelease();
        }

        public void Disable() => isActive = false;
        public void Enable() => isActive = true;
    }
}
