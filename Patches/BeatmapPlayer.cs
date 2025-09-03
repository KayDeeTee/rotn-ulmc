using HarmonyLib;
using RhythmRift;
using Shared.RhythmEngine;
using UnityEngine.UIElements;

namespace UIPlugin;

internal static class RRBeatmapPlayerPatch {
    [HarmonyPatch(typeof(RRBeatmapPlayer), nameof(RRBeatmapPlayer.ProcessBeatEvent))]
    [HarmonyPostfix]
    public static void ProcessBeatEvent(BeatmapEvent beatEvent) {
        if(CustomEvent.TryParse(beatEvent, out SetPortraitEvent setPortraitEvent)) {
            // TODO: actually handle this
            UIPlugin.Logger.LogWarning($"Loading {setPortraitEvent.Name} {setPortraitEvent.IsHero}");
        } else if(CustomEvent.TryParse(beatEvent, out LuaEvent luaEvent)) {
            foreach(var ctx in LuaManager.luaContexts) {
                ctx.GetEventHandler(luaEvent.CustomType).OnEvent.Invoke(luaEvent);
            }
        }
    }
}
