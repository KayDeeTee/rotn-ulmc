using MoonSharp.Interpreter;

namespace UIPlugin;

[MoonSharpUserData]
public class LuaAnimClip
{
    public int Duration = 1; //in frames
    public bool Loops = false;
    public Hook<LuaAnimPlayer> OnEnter { get; } = new();
    public Hook<LuaAnimPlayer> OnFrame { get; } = new();
    public Hook<LuaAnimPlayer> OnExit { get; } = new();
    public Hook<LuaAnimPlayer> OnFinish { get; } = new();
}