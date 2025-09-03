using System.Collections.Generic;
using HarmonyLib;
using RhythmRift;
using static RhythmRift.RREnemyController;

namespace UIPlugin;

internal static class RREnemyControllerPatch
{
    public static RREnemyController instance;
    [HarmonyPatch(typeof(RREnemyController), "Initialize")]
    [HarmonyPostfix]
    public static void Init(RREnemyController __instance)
    {
        instance = __instance;
        foreach (LuaContext luaContext in LuaManager.luaContexts)
        {
            luaContext.enemyController = instance;
        }
    }
    //
    //  Call enemy hit hook
    //
    [HarmonyPatch(typeof(RREnemyController), "AttackEnemiesAtPositions")]
    [HarmonyPostfix]
    public static void AttackEnemies(RREnemyController __instance, List<EnemyHitData> __result)
    {
        foreach (EnemyHitData enemyHitData in __result)
        {
            int track = enemyHitData.Enemy.CurrentGridPosition.x;
            if (enemyHitData.Enemy.CurrentGridPosition.y != 0) track = enemyHitData.Enemy.TargetGridPosition.x;
            foreach (LuaContext ctx in LuaManager.luaContexts)
            {
                ctx.OnEnemyHit.Invoke(enemyHitData.Enemy, enemyHitData.RatingPercent, enemyHitData.TargetBeat, track);
            }
        }
    }
}