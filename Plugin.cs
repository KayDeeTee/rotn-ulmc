using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using Shared;
using System.Linq;
using PluginConfig = UIPlugin.Config;

namespace UIPlugin;


[BepInPlugin(GUID, NAME, VERSION)]
public class UIPlugin : BaseUnityPlugin
{
    public const string GUID = "rotn.katie.lua.ui_mod";
    public const string NAME = "UI Mod";
    public const string VERSION = "0.1.0.0";

    public const string ALLOWED_VERSIONS = "1.10.0 1.8.0 1.7.1 1.7.0";
    public static string[] AllowedVersions => ALLOWED_VERSIONS.Split(' ');


    internal static new ManualLogSource Logger;
    internal static UIPlugin instance;

    internal void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        instance = this;

        PluginConfig.Bind(Config);

        var gameVersion = BuildInfoHelper.Instance.BuildId.Split('-')[0];
        if(!AllowedVersions.Contains(gameVersion) && !PluginConfig.VersionControl.DisableVersionCheck) {
            Logger.LogFatal($"The current version of the game is not compatible with this plugin. Please update the game or the mod to the correct version. The current mod version is v{VERSION} and the current game version is {gameVersion}. Allowed game versions: {string.Join(", ", AllowedVersions)}");
            return;
        }

        LuaManager.InitUserdata();
        Harmony.CreateAndPatchAll(typeof(UIPlugin));
        Harmony.CreateAndPatchAll(typeof(RREnemyControllerPatch));
        Harmony.CreateAndPatchAll(typeof(RRStageControllerPatch));
        Harmony.CreateAndPatchAll(typeof(RRBeatmapPlayerPatch));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        // this throws an exception right now
        //Shared.BugSplatAccessor.Instance.BugSplat.ShouldPostException = ex => false;
    }
}
