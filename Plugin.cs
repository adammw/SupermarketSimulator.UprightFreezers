using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace UprightFreezers
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            Harmony.DEBUG = true;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Plugin.Log = Logger;
        }
    }
}
