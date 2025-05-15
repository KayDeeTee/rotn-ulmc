using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using Shared;

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
        if (BuildInfoHelper.Instance.BuildId != "1.4.0-b20638")
        {
            Logger.LogInfo("Mod built for a previous version of the game, wait for an update or update this yourself.");
            return;
        }

        LuaManager.InitUserdata();
        Harmony.CreateAndPatchAll(typeof(UIPlugin));
        Harmony.CreateAndPatchAll(typeof(RRStageControllerPatch));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        // this throws an exception right now
        //Shared.BugSplatAccessor.Instance.BugSplat.ShouldPostException = ex => false;
    }
}
