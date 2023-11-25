using BaboonAPI.Hooks.Initializer;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using TootTallyCore.Graphics;
using TootTallyCore.Utils.TootTallyModules;
using TootTallySettings;
using UnityEngine;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TootTallySettings", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin, ITootTallyModule
    {
        public static Plugin Instance;

        private Harmony _harmony;
        public ConfigEntry<bool> ModuleConfigEnabled { get; set; }
        public bool IsConfigInitialized { get; set; }

        //Change this name to whatever you want
        public string Name { get => "KeyOverlay"; set => Name = value; }

        public static TootTallySettingPage settingPage;

        public static void LogInfo(string msg) => Instance.Logger.LogInfo(msg);
        public static void LogError(string msg) => Instance.Logger.LogError(msg);

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _harmony = new Harmony(Info.Metadata.GUID);

            GameInitializationEvent.Register(Info, TryInitialize);
        }

        private void TryInitialize()
        {
            // Bind to the TTModules Config for TootTally
            ModuleConfigEnabled = TootTallyCore.Plugin.Instance.Config.Bind("Modules", "KeyOverlay", true, "Enabled visual display for key strokes during a song.");
            TootTallyModuleManager.AddModule(this);
            TootTallySettings.Plugin.Instance.AddModuleToSettingPage(this);
        }

        public void LoadModule()
        {
            _harmony.PatchAll(typeof(KeyOverlayPatches));
            LogInfo($"Module loaded!");
        }

        public void UnloadModule()
        {
            _harmony.UnpatchSelf();
            settingPage.Remove();
            LogInfo($"Module unloaded!");
        }

        public static class KeyOverlayPatches
        {
            private static Dictionary<KeyCode, SingleKey> _keyPressedDict;
            private static CustomButton _keyPrefab;
            private static GameObject _keyOverlayUI;

            [HarmonyPatch(typeof(LevelSelectController), nameof(LevelSelectController.Start))]
            [HarmonyPostfix]
            public static void SetKeyOverlayPrefab(LevelSelectController __instance)
            {
                if (_keyPrefab != null) return;
                var tempObj = GameObjectFactory.CreateCustomButton(__instance.bgshape.transform, Vector2.zero, new Vector2(50, 50), "test", "tempObj"); //idk where to put the tempObj just put it somewhere random lmfao
                _keyPrefab = GameObject.Instantiate(tempObj);
                _keyPrefab.gameObject.name = "KeyOverlayPrefab";
                _keyPrefab.GetComponent<RectTransform>().sizeDelta = Vector2.one * 30;
                _keyPrefab.textHolder.fontSize = 10;
                var keyText = GameObject.Instantiate(_keyPrefab.transform.Find("Text"), _keyPrefab.transform);
                keyText.name = "KeyText";
                keyText.GetComponent<Text>().color = new Color(_keyPrefab.textHolder.color.r, _keyPrefab.textHolder.color.g, _keyPrefab.textHolder.color.b, .45f);
                keyText.GetComponent<Text>().fontSize = 30;
                GameObject.DestroyImmediate(tempObj.gameObject);
                GameObject.DontDestroyOnLoad(_keyPrefab);
            }


            [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
            [HarmonyPostfix]
            public static void OnGameControllerStartSetupOverlay(GameController __instance)
            {
                if (__instance.freeplay) return;

                var uiCanvas = GameObject.Find("GameplayCanvas/UIHolder");

                _keyOverlayUI = GameObject.Instantiate(_keyPrefab, uiCanvas.transform).gameObject;
                GameObject.DestroyImmediate(_keyOverlayUI.transform.Find("Text"));
                GameObject.DestroyImmediate(_keyOverlayUI.GetComponent<UnityEngine.UI.Image>());
                var rectTransform = _keyOverlayUI.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(30, 180);
                rectTransform.anchoredPosition = new Vector2(17, -90);
                var verticalLayout = _keyOverlayUI.AddComponent<VerticalLayoutGroup>();
                verticalLayout.childAlignment = TextAnchor.MiddleCenter;
                verticalLayout.childScaleWidth = verticalLayout.childScaleHeight = false;
                verticalLayout.childForceExpandWidth = verticalLayout.childForceExpandHeight = false;
                verticalLayout.childControlWidth = verticalLayout.childControlHeight = false;

                _keyPressedDict = new Dictionary<KeyCode, SingleKey>();
            }

            [HarmonyPatch(typeof(GameController), nameof(GameController.Update))]
            [HarmonyPostfix]
            public static void OnUpdateDetectKeyPressed(GameController __instance, List<KeyCode> ___toot_keys)
            {
                if (__instance.freeplay || _keyPressedDict == null) return;
                ___toot_keys.ForEach(key =>
                {
                    if (Input.GetKey(key))
                    {
                        if (!_keyPressedDict.ContainsKey(key))
                        {
                            //put it inside to not trigger the else statement when already at 6 keys
                            if (_keyPressedDict.Count < 6)
                            {
                                var newKeyGameObject = GameObject.Instantiate(_keyPrefab, _keyOverlayUI.transform);
                                newKeyGameObject.transform.Find("KeyText").GetComponent<Text>().text = key.ToString();
                                newKeyGameObject.name = $"KeyOverlay{key}";
                                _keyPressedDict.Add(key, new SingleKey(newKeyGameObject));
                                _keyPressedDict[key].OnKeyPress();
                                Plugin.LogInfo($"New key pressed, adding {key} to overlay.");
                            }
                        }
                        else if (!_keyPressedDict[key].isPressed)
                            _keyPressedDict[key].OnKeyPress();
                    }
                    else if (_keyPressedDict.ContainsKey(key) && _keyPressedDict[key].isPressed)
                        _keyPressedDict[key].OnKeyRelease();
                });
            }

        }
    }
}