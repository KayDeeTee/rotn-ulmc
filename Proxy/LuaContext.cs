#pragma warning disable IDE1006

using MoonSharp.Interpreter;
using RhythmRift;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MoonSharp.Interpreter.Interop;

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
    //  Lua hooks so you can do ctx.on_frame.add(func) to have func() be called every frame
    //
    public Hook OnPostInit { get; }  = new();
    public Hook OnFrame { get; } = new();
    public Hook<int> OnBeat { get; } = new(); // args: beat
    public Hook<float> OnGainVibe { get; } = new(); // args: new_vibe
    public Hook<float> OnVibeActivate { get; } = new(); // args: vibe_amt
    public Hook<float> OnVibeDeactivate { get; } = new(); // args: vibe_amt
    public Hook OnPlayerDeath { get; } = new();
    public Hook<int, int, int> OnPlayerHit { get; } = new(); // args: id, new_health, track
    public Hook<string, int> OnEnemyKilled { get; } = new(); // args: id, track


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
