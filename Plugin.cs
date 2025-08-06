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
            PositionAlignment = config.Bind("General", nameof(PositionAlignment), KeyOverlayUIHolder.UIAlignment.BottomRight, "Rough position of the overlay on the screen.");
            PosXOffset = config.Bind("General", nameof(PosXOffset), 0f, "X position offset for the overlay.");
            PosYOffset = config.Bind("General", nameof(PosYOffset), 0f, "Y position offset for the overlay.");

            KeyCountLimit = config.Bind("General", nameof(KeyCountLimit), 4f, "Limit of keys displayed at the same time. Limited to 10 because on average, humans have less than 10 fingers.");
            KeyElementSize = config.Bind("General", nameof(KeyElementSize), 18f, "Size in pixels of a single key element.");
            KeyOutlineThiccness = config.Bind("General", nameof(KeyOutlineThiccness), 2f, "Size in pixels of a single key element.");
            HorizontalAlignment = config.Bind("General", nameof(HorizontalAlignment), false, "Switch to horizontal layout.");

            BeamDirection = config.Bind("General", nameof(BeamDirection), KeyOverlayUIHolder.BeamDirection.Left, "Rough position of the overlay on the screen.");
            BeamSpeed = config.Bind("General", nameof(BeamSpeed), 200f, "Speed of the beam when pressing a key. Frame dependent and default at 200fps");
            BeamLength = config.Bind("General", nameof(BeamLength), 150f, "Length of the beam when pressing a key. Default 150px");
            BeamColor = config.Bind("General", nameof(BeamColor), Color.gray, "Color of the beam when pressing a key.");
            KeyOuterColor = config.Bind("General", nameof(KeyOuterColor), Color.white, "Color of the outline of a single key element.");
            KeyPressedOuterColor = config.Bind("General", nameof(KeyPressedOuterColor), Color.gray, "Color of the outline of a single key element when key is pressed.");
            KeyInnerColor = config.Bind("General", nameof(KeyInnerColor), Color.black, "Color of the inside of a single key element.");
            KeyPressedInnerColor = config.Bind("General", nameof(KeyPressedInnerColor), Color.gray, "Color of the inside of a single key element when key is pressed.");
            KeyTextColor = config.Bind("General", nameof(KeyTextColor), Color.white, "Color of the text of a single key element.");
            KeyPressedTextColor = config.Bind("General", nameof(KeyPressedTextColor), Color.gray, "Color of the text of a single key element when key is pressed.");

            settingPage = TootTallySettingsManager.AddNewPage(new KeyOverlaySettingsPage());

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
        public ConfigEntry<KeyOverlayUIHolder.UIAlignment> PositionAlignment { get; set; }

        public ConfigEntry<float> PosXOffset { get; set; }
        public ConfigEntry<float> PosYOffset { get; set; }

        public ConfigEntry<KeyOverlayUIHolder.BeamDirection> BeamDirection { get; set; }
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
        public ConfigEntry<bool> HorizontalAlignment { get; set; }

    }
}