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
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace TootTallyKeyOverlay
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TootTallySettings", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin, ITootTallyModule
    {
        public static Plugin Instance;

        private const string CONFIG_NAME = "TootTallyKeyOverlay.cfg";
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
            string configPath = Path.Combine(Paths.BepInExRootPath, "config/");
            ConfigFile config = new ConfigFile(configPath + CONFIG_NAME, true) { SaveOnConfigSet = true };
            KeyCountLimit = config.Bind("General", nameof(KeyCountLimit), 4f, "Limit of keys displayed at the same time. Limited to 10 because on average, humans have less than 10 fingers.");
            KeyElementSize = config.Bind("General", nameof(KeyElementSize), 18f, "Size in pixels of a single key element.");
            KeyOutlineThiccness = config.Bind("General", nameof(KeyOutlineThiccness), 2f, "Size in pixels of a single key element.");
            HorizontalAlignement = config.Bind("General", nameof(HorizontalAlignement), false, "Switch to horizontal layout.");

            BeamSpeed = config.Bind("General", nameof(BeamSpeed), 200f, "Speed of the beam when pressing a key. Frame dependent and default at 200fps");
            BeamLength = config.Bind("General", nameof(BeamLength), 150f, "Length of the beam when pressing a key. Default 150px");
            BeamColor = config.Bind("General", nameof(BeamColor), Color.gray, "Color of the beam when pressing a key.");
            KeyOuterColor = config.Bind("General", nameof(KeyOuterColor), Color.white, "Color of the outline of a single key element.");
            KeyPressedOuterColor = config.Bind("General", nameof(KeyPressedOuterColor), Color.gray, "Color of the outline of a single key element when key is pressed.");
            KeyInnerColor = config.Bind("General", nameof(KeyInnerColor), Color.black, "Color of the inside of a single key element.");
            KeyPressedInnerColor = config.Bind("General", nameof(KeyPressedInnerColor), Color.gray, "Color of the inside of a single key element when key is pressed.");
            KeyTextColor = config.Bind("General", nameof(KeyTextColor), Color.white, "Color of the text of a single key element.");
            KeyPressedTextColor = config.Bind("General", nameof(KeyPressedTextColor), Color.gray, "Color of the text of a single key element when key is pressed.");

            settingPage = TootTallySettingsManager.AddNewPage("Key Overlay", "Key Overlay", 40f, new Color(0, 0, 0, 0));

            settingPage.AddToggle("Horizontal Alignement", HorizontalAlignement);

            settingPage.AddSlider("Key Count Limit", 1f, 10f, KeyCountLimit, true);

            settingPage.AddSlider("Key Element Size", 4f, 32f, KeyElementSize, true);

            settingPage.AddSlider("Key Outline Size", 0f, 8f, KeyOutlineThiccness, true);

            settingPage.AddSlider("Beam Speed", 60f, 600f, BeamSpeed, true);

            settingPage.AddSlider("Beam Length", 30f, 600f, BeamLength, true);

            settingPage.AddLabel("Beam Color");
            settingPage.AddColorSliders("Beam Color", "Beam Color", BeamColor);

            settingPage.AddLabel("KeyOuterColor");
            settingPage.AddColorSliders("KeyOuterColor", "KeyOuterColor", KeyOuterColor);

            settingPage.AddLabel("KeyPressedOuterColor");
            settingPage.AddColorSliders("KeyPressedOuterColor", "KeyPressedOuterColor", KeyPressedOuterColor);

            settingPage.AddLabel("KeyInnerColor");
            settingPage.AddColorSliders("KeyInnerColor", "KeyInnerColor", KeyInnerColor);

            settingPage.AddLabel("KeyPressedInnerColor");
            settingPage.AddColorSliders("KeyPressedInnerColor", "KeyPressedInnerColor", KeyPressedInnerColor);

            settingPage.AddLabel("KeyTextColor");
            settingPage.AddColorSliders("KeyTextColor", "KeyTextColor", KeyTextColor);

            settingPage.AddLabel("KeyPressedTextColor");
            settingPage.AddColorSliders("KeyPressedTextColor", "KeyPressedTextColor", KeyPressedTextColor);

            TootTallySettings.Plugin.TryAddThunderstoreIconToPageButton(Instance.Info.Location, Name, settingPage);

            _harmony.PatchAll(typeof(KeyOverlayManager));
            LogInfo($"Module loaded!");
        }

        public void UnloadModule()
        {
            _harmony.UnpatchSelf();
            settingPage.Remove();
            LogInfo($"Module unloaded!");
        }

        public ConfigEntry<Color> BeamColor { get; set; }
        public ConfigEntry<Color> KeyOuterColor { get; set; }
        public ConfigEntry<Color> KeyPressedOuterColor { get; set; }
        public ConfigEntry<Color> KeyInnerColor { get; set; }
        public ConfigEntry<Color> KeyPressedInnerColor { get; set; }
        public ConfigEntry<Color> KeyTextColor { get; set; }
        public ConfigEntry<Color> KeyPressedTextColor { get; set; }
        public ConfigEntry<float> KeyElementSize { get; set; }
        public ConfigEntry<float> KeyOutlineThiccness { get; set; }
        public ConfigEntry<float> BeamSpeed { get; set; }
        public ConfigEntry<float> BeamLength { get; set; }
        public ConfigEntry<float> KeyCountLimit { get; set; }
        public ConfigEntry<bool> HorizontalAlignement { get; set; }

    }
}