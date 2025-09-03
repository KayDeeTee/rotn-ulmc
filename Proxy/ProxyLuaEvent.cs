using MoonSharp.Interpreter;

namespace UIPlugin;

class ProxyLuaEvent
{
    LuaEvent target;
    [MoonSharpHidden]
    public ProxyLuaEvent(LuaEvent t)
    {
        target = t;
    }
    public string Type => target.CustomType;
}
