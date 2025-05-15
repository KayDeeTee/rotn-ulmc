using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RhythmRift.Enemies;
using Unity.Mathematics;

namespace UIPlugin;

public static class LuaManager
{
    //
    //  Create all the non-primitives you are allowed to send to a lua context
    //  RegisterAssembly does every class marked with [MoonSharpUserData]
    //  RegisterProxyType makes a proxy that replaces any attempt to return a class to lua, replaces it with a proxy that references it instead
    //
    public static void InitUserdata()
    {
        UserData.RegisterAssembly();
        UserData.RegisterProxyType<ProxyRectTransform, RectTransform>(r => new ProxyRectTransform(r));
        UserData.RegisterProxyType<ProxyTestMeshProUGUI, TextMeshProUGUI>(r => new ProxyTestMeshProUGUI(r));
        UserData.RegisterProxyType<ProxyImage, Image>(r => new ProxyImage(r));
        UserData.RegisterProxyType<ProxyEnemy, RREnemy>(r => new ProxyEnemy(r));
    }

    //
    //   
    //
    public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
    public static List<Script> scripts = new List<Script>();
    public static List<LuaContext> luaContexts = new List<LuaContext>();

    public static LuaOSD logOSD;

    //
    //  Reset and load all lua files found at song folder 
    //
    public static void Reset()
    {
        scripts.Clear();
        luaContexts.Clear();
        foreach (string key in Sprites.Keys)
        {
            UnityEngine.Object.Destroy(Sprites[key]);
        }
        Sprites = new Dictionary<string, Sprite>();
    }
    public static void LoadFile(string path)
    {
        Log(String.Format("Attempting to load lua at {0}", path));

        Script lua = new Script(MoonSharp.Interpreter.CoreModules.Preset_HardSandbox);
        lua.Options.ScriptLoader = new FileSystemScriptLoader();
        //Create Vars / Functions
        lua.Globals["log"] = (System.Object)Log;
        lua.Globals["load_texture"] = (System.Object)LoadTexture;
        lua.Globals["log_osd"] = (System.Object)LogOSD;
        lua.Globals["err_osd"] = (System.Object)LogOSDError;
        LuaContext ctx = new LuaContext(lua);
        ctx.stageController = RRStageControllerPatch.instance;
        lua.Globals["ctx"] = ctx;
        luaContexts.Add(ctx);

        try
        {
            lua.DoFile(path);

            DynValue v = lua.Globals.Get("Init");
            if (!v.IsNil())
            {
                if (v.Type == DataType.Function)
                {
                    lua.Call(v);
                }
            }
            scripts.Add(lua);
        }
        catch (ScriptRuntimeException ex)
        {
            string errorMessage = string.Format("LUA ScriptRuntimeEx: {0}", ex.DecoratedMessage);
            UIPlugin.Logger.LogError(errorMessage);
            logOSD.AddMessage(LuaOSDMessage.MessageLevel.Error, errorMessage, -1);
        }
        catch (SyntaxErrorException ex)
        {
            string errorMessage = string.Format("LUA SyntaxErrorEx: {0}", ex.DecoratedMessage);
            UIPlugin.Logger.LogError(errorMessage);
            logOSD.AddMessage(LuaOSDMessage.MessageLevel.Fatal, errorMessage, -1);
        }
    }
    public static void Load(string[] paths)
    {
        LuaOSD();
        foreach (string path in paths)
        {
            LoadFile(path);
        }
    }
    //
    // Create OSD for lua messages
    //
    public static void LuaOSD()
    {
        Transform rrui = RRStageControllerPatch.instance.GetComponent<Transform>();
        logOSD = new LuaOSD(rrui);
    }

    //
    // Utils for making vectors into something you can send to lua as a table
    // Vec3 becomes a table with an x, y, and z entry so you can access it the same way you would in c#
    //
    public static Dictionary<string, float> Vec3Dict(Vector3 v)
    {
        Dictionary<string, float> d = new Dictionary<string, float>();
        d["x"] = v.x;
        d["y"] = v.y;
        d["z"] = v.z;
        return d;
    }

    public static Dictionary<string, float> Vec2Dict(Vector2 v)
    {
        Dictionary<string, float> d = new Dictionary<string, float>();
        d["x"] = v.x;
        d["y"] = v.y;
        return d;
    }

    //
    //  Lua callbacks
    //
    private static void Log(string message)
    {
        UIPlugin.Logger.LogInfo(message);
    }

    private static void LogOSD(string message, float duration)
    {
        logOSD.AddMessage(LuaOSDMessage.MessageLevel.Debug, message, duration);
    }

    private static void LogOSDError(string message, float duration)
    {
        logOSD.AddMessage(LuaOSDMessage.MessageLevel.Error, message, duration);
    }


    //
    //  This probably should be async but it would be very awkward to communicate that with lua
    //
    private static void LoadTexture(string id, string path, int ppu, float x_pivot, float y_pivot)
    {
        string AssetPath = Path.Combine(RRStageControllerPatch.LuaPath, path);
        if (File.Exists(AssetPath))
        {
            byte[] bytes = File.ReadAllBytes(AssetPath);
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.LoadImage(bytes);
            Rect rect = new Rect(0, 0, tex.width, tex.height);
            Vector2 pivot = new Vector2(x_pivot, y_pivot);
            Sprites[id] = Sprite.Create(tex, rect, pivot, ppu);
        }
    }

}
