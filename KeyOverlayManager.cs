using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TootTallyCore.Graphics;
using UnityEngine.UI;
using UnityEngine;

namespace TootTallyKeyOverlay
{
    public static class KeyOverlayManager
    {
        private static KeyOverlayController _keyOverlayController;

        [HarmonyPatch(typeof(GameController), nameof(GameController.Start))]
        [HarmonyPostfix]
        public static void OnGameControllerStartSetupOverlay(GameController __instance)
        {
            if (__instance.freeplay) return;

            var uiCanvas = GameObject.Find("GameplayCanvas/UIHolder");
            var uiHolder = new GameObject("KeyOverlayUIHolder", typeof(RectTransform), typeof(GridLayoutGroup), typeof(KeyOverlayController));
            uiHolder.transform.SetParent(uiCanvas.transform);
            uiHolder.name = "KeyOverlayUIHolder";
            _keyOverlayController = uiHolder.GetComponent<KeyOverlayController>();
            _keyOverlayController.Init();
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.Update))]
        [HarmonyPostfix]
        public static void OnUpdateDetectKeyPressed(GameController __instance)
        {
            //Maybe add some stuff to disable / enable keypresses while pausing or something
        }
    }
}
