using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UprightFreezers
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        public static ConfigEntry<float> freezerACost;
        public static ConfigEntry<float> freezerBCost;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            Harmony.DEBUG = true;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Plugin.Log = Logger;

            freezerACost = base.Config.Bind<float>("Cost", "FreezerA", 600f, "Cost of FREEZER A (1 wide)");
            freezerBCost = base.Config.Bind<float>("Cost", "FreezerB", 1200f, "Cost of FREEZER B (2 wide)");
        }

        public static Texture2D LoadTexture(string resourceName)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UprightFreezers.Resources." + resourceName + ".png");
            if (stream == null) {
                Log.LogError("Texture not found");
                return texture;
            }
            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData);
            texture.LoadImage(imageData);
            return texture;
        }
    }
}
