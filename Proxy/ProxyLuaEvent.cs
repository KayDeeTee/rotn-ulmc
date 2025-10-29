using MoonSharp.Interpreter;
using UnityEngine;

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

    public string GetString(string key) => target.GetString(key);
    public bool? GetBool(string key) => target.GetBool(key);
    public int? GetInt(string key) => target.GetInt(key);
    public float? GetFloat(string key) => target.GetFloat(key);
    public Color? GetColor(string key) => target.GetColor(key);
}
