using MoonSharp.Interpreter;


namespace UIPlugin;


[MoonSharpUserData]
public class LuaEventHandler {
    public Hook<LuaEvent> OnPreload { get; } = new();
    public Hook<LuaEvent> OnEvent { get; } = new();
    public Hook<LuaEvent> OnSkip { get; } = new();
}