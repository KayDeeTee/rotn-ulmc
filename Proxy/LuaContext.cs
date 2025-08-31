#pragma warning disable IDE1006

using MoonSharp.Interpreter;
using RhythmRift;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MoonSharp.Interpreter.Interop;
using RhythmRift.Enemies;
using System.Collections.Generic;

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
    public RREnemyController enemyController;
    [MoonSharpHidden]
    public LuaContext(Script lua)
    {
        script = lua;
        AnimationPlayers = [];
    }
    //
    //  Lua hooks so you can do ctx.on_frame.add(func) to have func() be called every frame
    //
    public Hook OnPostInit { get; } = new();
    public Hook OnFrame { get; } = new();
    public Hook<int> OnBeat { get; } = new(); // args: beat
    public Hook<float> OnGainVibe { get; } = new(); // args: new_vibe
    public Hook<float> OnVibeActivate { get; } = new(); // args: vibe_amt
    public Hook<float> OnVibeDeactivate { get; } = new(); // args: vibe_amt
    public Hook OnPlayerDeath { get; } = new();
    public Hook<int, int, int> OnPlayerHit { get; } = new(); // args: id, new_health, track
    public Hook<string, int> OnEnemyKilled { get; } = new(); // args: id, track
    public Hook<RREnemy, int, float, int> OnEnemyHit { get; } = new(); //args: enemy, rating_percent, target_beat, track

    //
    //  Game data to be passed to lua via ctx
    //
    public float currentTime;
    public float previousTime;
    public float deltaTime;
    public float currentBeat;
    public float currentVibe;
    public int currentHealth;
    public bool inVibe = false;
    public bool justCreated = true;

    //
    //  Util functions for development
    //
    public void ListAllChildren()
    {
        ListAllChildren(stageController.gameObject.transform, "");
    }
    [MoonSharpHidden]
    public void ListAllChildren(Transform transform, string path)
    {
        UIPlugin.Logger.LogInfo(path + transform.name);
        foreach (Transform child in transform)
        {
            if (child == transform) continue;
            ListAllChildren(child, path + transform.name + "/");
        }
    }
    public void ListAllComponents(string path)
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
    public TextMeshProUGUI GetTmpro(string path)
    {
        return GetTransform(path)?.GetComponent<TextMeshProUGUI>();
    }
    public Image GetImage(string path)
    {
        return GetTransform(path)?.GetComponent<Image>();
    }
    public Transform GetTransform(string path)
    {
        return stageController.transform.Find(path);
    }

    //
    // Enemies
    //
    public List<RREnemy> GetActiveEnemies()
    {
        if (enemyController == null) return new List<RREnemy>();
        return enemyController._activeEnemies;
    }

    //
    //  Animation
    //
    [MoonSharpHidden]
    public HashSet<LuaAnimPlayer> AnimationPlayers;
    public LuaAnimPlayer CreateAnimPlayer()
    {
        LuaAnimPlayer animPlayer = new LuaAnimPlayer();
        AnimationPlayers.Add(animPlayer);
        return animPlayer;
    }
}
