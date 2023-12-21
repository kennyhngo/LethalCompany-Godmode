using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;
// using Godmode.Patches;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Godmode
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Ken.Godmode";
        private const string modName = "Toggleable God Mode";
        private const string modVersion = "1.0.0";
        
        private readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource mls;
        public static Plugin _instance;
        private static TextMeshProUGUI indicator;

        private static bool enableGod = false;
        private static ConfigEntry<string> cfgKeyBind;
        private static ConfigEntry<bool> showText;
        public static ConfigEntry<float> xOffset;
        public static ConfigEntry<float> yOffset;

        private void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Godmode loaded");

            _instance = this;
            enableGod = false;

            SceneManager.sceneLoaded += SceneLoad;
            cfgKeyBind = Config.Bind("General", "Key", "M", "Button to toggle god mode.");
            showText = Config.Bind("General", "Show Enable/Disable Text", true, "Show 'ON' or 'OFF' in the upper right corner of screen.");
            xOffset = Config.Bind("General", "X Offset", -0.08f, "Shifts the indicator text horizontally");
            yOffset = Config.Bind("General", "Y Offset", -0.025f, "Shifts the indicator text vertically");

            var KeyAction = new InputAction(binding: $"<Keyboard>/{cfgKeyBind.Value}");
            KeyAction.performed += OnKeyPressed;
            KeyAction.Enable();
            harmony.PatchAll(typeof(Plugin));
        }

        private void OnKeyPressed(InputAction.CallbackContext obj)
        {
            enableGod = !enableGod;
            mls.LogDebug($"God mode {(enableGod ? "enabled" : "disabled")}");
            indicator.text = UpdateIndicatorText();
            indicator.color = UpdateIndicatorColor();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
        [HarmonyPrefix]
        private static bool OverrideDeath()
        {
            return !enableGod;
        }
        private static string UpdateIndicatorText()
        {
            // return _player.health.ToString();
            return (enableGod ? "ON" : "OFF");
        }

        private static Color UpdateIndicatorColor()
        {
            return (enableGod ? Color.green : Color.red);
        }

        private static void SceneLoad(Scene sceneName, LoadSceneMode load)
        {
            if (showText.Value && sceneName.name == "SampleSceneRelay")
            {
                GameObject canvas = GameObject.Find("/Systems/UI/Canvas/Panel/GameObject/PlayerScreen");
                if (canvas != null )
                {
                    try
                    {

                        indicator = new GameObject("Godmode Indicator").AddComponent<TextMeshProUGUI>();
                        indicator.text = UpdateIndicatorText();
                        indicator.color = UpdateIndicatorColor();
                        indicator.rectTransform.SetParent(canvas.transform, false);
                        
                        indicator.rectTransform.anchorMax = new Vector2(xOffset.Value, 1); // x-axis
                        indicator.rectTransform.anchorMin = new Vector2(xOffset.Value, 1 - yOffset.Value); // y-axis
                        indicator.rectTransform.pivot = new Vector2(0, 1);
                        indicator.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
                        // indicator.rectTransform.anchoredPosition = new Vector2(-340, 220);

                        indicator.rectTransform.SetAsLastSibling();
                        indicator.fontSize = 12;
                        indicator.alignment = TextAlignmentOptions.Center;
                    }
                    catch (System.Exception ex)
                    {
                        mls.LogError("Error loading in SampleSceneRelay canvas.");
                        mls.LogError(ex.Message);
                    }
                } 
            }
        }
    }
}