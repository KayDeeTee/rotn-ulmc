using System.Threading.Tasks;
using RiftOfTheNecroManager.BeatmapEvents;
using RiftOfTheNecroManager.Patches;

namespace UIPlugin;


[CustomEvent(TYPE, CustomEventMatchMode.ContravariantStrict, CustomEventFlags.SkipBeat0)]
public class LuaEvent : CustomEvent {
    public const string TYPE = "Lua";
    public string CustomType => Type[..^(TYPE.Length + 1)];
    
    public override async Task Preload(StageState stage) {
        foreach(var ctx in LuaManager.luaContexts) {
            ctx.GetEventHandler(CustomType).OnPreload.Invoke(this);
        }
    }
    
    public override void Process(StageState stage) {
        foreach(var ctx in LuaManager.luaContexts) {
            ctx.GetEventHandler(CustomType).OnEvent.Invoke(this);
        }
    }
    
    public override void Skip(StageState stage) {
        foreach(var ctx in LuaManager.luaContexts) {
            if(BeatmapEvent.startBeatNumber <= stage.StartBeat) {
                ctx.GetEventHandler(CustomType).OnSkip.Invoke(this);
            }
        }
    }
}
