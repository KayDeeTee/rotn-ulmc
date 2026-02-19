using BepInEx;
using RiftOfTheNecroManager;

namespace UIPlugin;


[BepInPlugin(GUID, NAME, VERSION)]
[NecroManagerInfo(menuNameOverride: "UI Mod", customEventsNameOverride: "UIMod")]
public class UIPlugin : RiftPlugin
{
    public const string GUID = "rotn.katie.lua.ui_mod";
    public const string NAME = "UI Mod";
    public const string VERSION = "2.0.0";
    
    protected override void OnInit() {
        base.OnInit();
        LuaManager.InitUserdata();
    }
}
