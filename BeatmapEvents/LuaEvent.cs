namespace UIPlugin;


public class LuaEvent : CustomEvent {
    public override string Type => ".Lua";
    public string CustomType => DoesTypeMatch() ? GetMatchingType()[..^Type.Length] : "";

    public override string GetMatchingType() {
        foreach(var type in BeatmapEvent.type.Split()) {
            if(type.EndsWith(Type, System.StringComparison.InvariantCultureIgnoreCase)) {
                return type;
            }
        }

        return "";
    }
}
