#pragma warning disable IDE1006

using MoonSharp.Interpreter;
using RhythmRift;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UIPlugin;


//should probably move sprite handling / caching into the lua context itself so 2 separate scripts loading images can't conflict
[MoonSharpUserData]
public class LuaContext
{
    [MoonSharpHidden]
    public Script script;
    [MoonSharpHidden]
    public RRStageController stageController;
    [MoonSharpHidden]
    public LuaContext(Script lua)
    {
        script = lua;
    }

    //
    //  Handles checking that the hook is actually set for this ctx, and that its actually a function
    //  Also handles basic error checking and reporting
    //
    public DynValue run_function(DynValue dv, object[] args)
    {
        if (dv.IsNil()) return DynValue.Nil;
        if (dv.Type != DataType.Function) return DynValue.Nil;
        try
        {
            return script.Call(dv, args);
        }
        catch (ScriptRuntimeException ex) { UIPlugin.Logger.LogError(string.Format("LUA ScriptRuntimeEx: {0}", ex.DecoratedMessage)); }
        catch (SyntaxErrorException ex) { UIPlugin.Logger.LogError(string.Format("LUA SyntaxErrorEx: {0}", ex.DecoratedMessage)); }
        //There should never be a syntax error here but doesn't hurt to check
        return DynValue.Nil;
    }

    //
    //  Lua hooks so you can do ctx.on_frame = func to have func() be called every frame
    //
    public DynValue on_post_init = DynValue.Nil;            //on_post_init()
    public DynValue on_frame = DynValue.Nil;                //on_frame()
    public DynValue on_beat = DynValue.Nil;                 //on_beat(int beat)
    public DynValue on_gain_vibe = DynValue.Nil;            //on_gain_vibe(float new_vibe)
    public DynValue on_vibe_activate = DynValue.Nil;        //on_vibe_activate(float vibe_amt)
    public DynValue on_vibe_deactivate = DynValue.Nil;      //on_vibe_deactivate(float vibe_amt)
    public DynValue on_player_death = DynValue.Nil;         //on_player_death()
    public DynValue on_player_hit = DynValue.Nil;           //on_player_hit(int id, int new_health, int track)
    public DynValue on_enemy_killed = DynValue.Nil;         //on_enemy_killed(string id, int track)

    //
    //  Game data to be passed to lua via ctx
    //
    public float current_time;
    public float previous_time;
    public float delta_time;
    public float current_beat;
    public float current_vibe;
    public int current_health;
    public bool in_vibe = false;
    public bool just_created = true;

    //
    //  Util functions for development
    //
    public void list_all_children()
    {
        list_all_children(stageController.gameObject.transform, "");
    }
    [MoonSharpHidden]
    public void list_all_children(Transform transform, string path)
    {
        UIPlugin.Logger.LogInfo(path + transform.name);
        foreach (Transform child in transform)
        {
            if (child == transform) continue;
            list_all_children(child, path + transform.name + "/");
        }
    }
    public void list_all_components(string path)
    {
        Transform t = stageController.transform.Find(path);
        Component[] components = t.gameObject.GetComponents(typeof(Component));
        foreach (Component component in components)
        {
            UIPlugin.Logger.LogInfo(component.ToString());
        }
    }

    //
    //  Functions for getting references to unity componenents
    //
    public TextMeshProUGUI get_tmpro(string path)
    {
        return stageController.transform.Find(path).GetComponent<TextMeshProUGUI>();
    }
    public Image get_image(string path)
    {
        return stageController.transform.Find(path).GetComponent<Image>();
    }
    public Transform get_transform(string path)
    {
        return stageController.transform.Find(path).GetComponent<Transform>();
    }
}
