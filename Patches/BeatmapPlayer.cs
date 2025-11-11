using HarmonyLib;
using RhythmRift;
using Shared.RhythmEngine;

namespace UIPlugin;

internal static class RRBeatmapPlayerPatch {
    [HarmonyPatch(typeof(RRBeatmapPlayer), nameof(RRBeatmapPlayer.ProcessBeatEvent))]
    [HarmonyPostfix]
    public static void ProcessBeatEvent(BeatmapEvent beatEvent) {
        if(CustomEvent.TryParse(beatEvent, out LuaEvent luaEvent)) {
            if(luaEvent.HasBeenProcessed()) {
                return;
            }
            luaEvent.FlagAsProcessed();

            foreach(var ctx in LuaManager.luaContexts) {
                ctx.GetEventHandler(luaEvent.CustomType).OnEvent.Invoke(luaEvent);
            }
        }
    }
}
