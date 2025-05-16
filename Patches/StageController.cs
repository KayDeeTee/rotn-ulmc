using System.IO;
using System.Linq;
using HarmonyLib;
using RhythmRift;
using Shared.RhythmEngine;
using Shared.SceneLoading.Payloads;

namespace UIPlugin;

internal static class RRStageControllerPatch
{
    public static string LuaPath = "";
    public static RRStageController instance;

    //
    //  Resets lua state then lists all lua files at <songfolder>/UI/ and parses them with LuaManager
    //
    [HarmonyPatch(typeof(RRStageController), "UnpackScenePayload")]
    [HarmonyPostfix]
    public static void UnpackScene(RRStageController __instance, ScenePayload currentScenePayload)
    {
        instance = __instance;
        LuaManager.Reset();

        if (currentScenePayload is not RRDynamicScenePayload payload)
        {
            return;
        }

        UIPlugin.Logger.LogInfo(currentScenePayload.GetLevelId());

        LuaPath = Path.Combine(Path.GetDirectoryName(payload.GetBeatmapFileName()), "UI");

        if (!Directory.Exists(LuaPath))
        {
            UIPlugin.Logger.LogInfo("No lua folder found.");
            return;
        }

        var scripts = Directory.EnumerateFiles(LuaPath, "*.lua", SearchOption.AllDirectories);
        var to_load = scripts.ToArray();
        LuaManager.Load(to_load);
    }

    //
    //  Calls the lua hook on_frame, and if first frame ctx exists runs lua hook on_post_init
    //
    [HarmonyPatch(typeof(RRStageController), "Update")]
    [HarmonyPostfix]
    public static void OnUpdate(RRStageController __instance)
    {
        bool paused = __instance._isPaused;
        FmodTimeCapsule fmod = __instance.BeatmapPlayer.FmodTimeCapsule;
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.previous_time = ctx.current_time;
            ctx.current_time = fmod.Time;
            ctx.delta_time = fmod.DeltaTime;
            ctx.current_beat = fmod.TrueBeatNumber;
            ctx.in_vibe = __instance._isVibePowerActive;
            ctx.current_health = __instance.PlayerHealth;
            ctx.current_vibe = __instance._currentVibePower;

            if (ctx.just_created)
            {
                ctx.just_created = false;
                ctx.OnPostInit.Invoke();
            }
            ctx.OnFrame.Invoke();

            foreach (LuaAnimPlayer animPlayer in ctx.AnimationPlayers)
            {
                animPlayer.Update(ctx.delta_time, fmod.BeatLengthInSeconds, paused);
            }
        }

        LuaManager.logOSD.Update(fmod.DeltaTime);
    }

    //
    //  Calls the lua hook on_beat
    //
    [HarmonyPatch(typeof(RRStageController), "HandleBeatUpdate")]
    [HarmonyPostfix]
    public static void OnBeat(RRStageController __instance)
    {
        FmodTimeCapsule fmod = __instance.BeatmapPlayer.FmodTimeCapsule;
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnBeat.Invoke(fmod.CurrentBeatNumber);
        }
    }

    //
    //  Calls the lua hook on_gain_vibe
    //
    [HarmonyPatch(typeof(RRStageController), "VibeChainSuccess")]
    [HarmonyPostfix]
    public static void VibeChainSuccess(RRStageController __instance)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnGainVibe.Invoke(__instance._currentVibePower);
        }
    }

    //
    //  Calls the lua hook on_vibe_activate
    //
    [HarmonyPatch(typeof(RRStageController), "ActivateVibePower")]
    [HarmonyPostfix]
    public static void ActivateVibePower(RRStageController __instance)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnVibeActivate.Invoke(__instance._currentVibePower);
        }
    }

    //
    //  Calls the lua hook on_vibe_deactivate
    //
    [HarmonyPatch(typeof(RRStageController), "DeactivateVibePower")]
    [HarmonyPostfix]
    public static void DeactivateVibePower(RRStageController __instance)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnVibeDeactivate.Invoke(__instance._currentVibePower);
        }
    }

    //
    //  Calls the lua hook on_player_death
    //
    [HarmonyPatch(typeof(RRStageController), "HandlePlayerDefeat")]
    [HarmonyPostfix]
    public static void HandlePlayerDefeat(RRStageController __instance)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnPlayerDeath.Invoke();
        }
    }

    //
    //  Calls the lua hook on_player_hit
    //
    [HarmonyPatch(typeof(RRStageController), "HandleEnemyAttack")]
    [HarmonyPostfix]
    public static void HandleEnemyAttack(RRStageController __instance, Unity.Mathematics.int2 attackLocation, int enemyTypeId)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnPlayerHit.Invoke(enemyTypeId, __instance.PlayerHealth, attackLocation.x);
        }
    }

    //
    //  Calls the lua hook on_enemy_killed
    //
    [HarmonyPatch(typeof(RRStageController), "HandleEnemySlain")]
    [HarmonyPostfix]
    public static void HandleEnemySlain(RRStageController __instance, IRREnemyDataAccessor slainEnemy)
    {
        foreach (LuaContext ctx in LuaManager.luaContexts)
        {
            ctx.OnEnemyKilled.Invoke(slainEnemy.DisplayName, slainEnemy.CurrentGridPosition.x);
        }
    }
}