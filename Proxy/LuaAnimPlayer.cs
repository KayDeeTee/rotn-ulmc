using System.Collections.Generic;
using MoonSharp.Interpreter;
using UIPlugin;
using UnityEngine;

namespace UIPlugin;

[MoonSharpUserData]
public class LuaAnimPlayer
{
    public LuaAnimPlayer()
    {
        Objects = new Dictionary<string, RectTransform>();
        AnimBank = new Dictionary<string, LuaAnimClip>();
        Playing = false;        
    }
    [MoonSharpHidden]
    LuaAnimClip CurrentAnim;
    public float CurrentTime;
    public int CurrentFrame;
    public bool Playing = false;
    public float fps;
    public float ReferenceBPM = 120.0f;
    public bool ConstantSpeed = true; // if false modify delta time by beat length

    [MoonSharpHidden]
    Dictionary<string, LuaAnimClip> AnimBank;
    [MoonSharpHidden]
    Dictionary<string, RectTransform> Objects;

    public LuaAnimClip CreateNewClip(string key)
    {
        LuaAnimClip newClip = new LuaAnimClip();
        AnimBank[key] = newClip;
        return newClip;
    }
    
    public void AddObject(string key, RectTransform obj)
    {
        Objects[key] = obj;
    }
    public RectTransform GetObject(string key) {
        return Objects[key];
    }

    public void Play(string clipName)
    {
        Playing = true;
        CurrentAnim?.OnExit?.Invoke(this);
        CurrentAnim = AnimBank[clipName];
        CurrentTime = 0;
        CurrentFrame = 0;
        CurrentAnim?.OnEnter?.Invoke(this);
    }
    public void Stop()
    {
        Playing = false;
        CurrentTime = 0;
        CurrentFrame = 0;
    }

    public void Pause()
    {
        Playing = false;
    }

    public void Resume()
    {
        if( CurrentAnim != null ) Playing = true;
    }

    [MoonSharpHidden]
    public void Update(float deltaTime, float beatLength)
    {
        if (ConstantSpeed)
        {
            CurrentTime += deltaTime;
        }
        else
        {
            float currentBpm = 60 / beatLength;
            float ratio = ReferenceBPM / currentBpm;
            CurrentTime += deltaTime * ratio;
        }
        
        int cFrame = (int)(CurrentTime * fps);
        if (cFrame != CurrentFrame)
        {
            CurrentFrame = cFrame;
            if (CurrentFrame >= CurrentAnim?.Duration)
            {
                if (CurrentAnim.Loops)
                {

                    CurrentFrame %= CurrentAnim.Duration;
                }
                else
                {
                    Playing = false;
                    CurrentAnim?.OnFinish?.Invoke(this);
                    return;
                }
            }
            CurrentAnim?.OnFrame?.Invoke(this);
        }
    }
}