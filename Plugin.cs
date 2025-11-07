using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using Shared;
using System.Linq;

namespace UIPlugin;

[BepInPlugin("rotn.katie.lua.ui_mod", "UI Mod", "0.1.0.0")]
public class UIPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static UIPlugin instance;
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        instance = this;

        Logger.LogInfo(String.Format("BuildVer: {0}", BuildInfoHelper.Instance.BuildId));
        string[] versions = ["1.7.0", "1.7.1", "1.8.0", "1.9.0", "1.10.0", "1.11.0"];
        if (!versions.Contains(BuildInfoHelper.Instance.BuildId.Split('-')[0]))
        {
            Logger.LogInfo("Mod built for a previous version of the game, wait for an update or update this yourself.");
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
