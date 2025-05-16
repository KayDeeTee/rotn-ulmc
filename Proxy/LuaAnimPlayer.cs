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
    public int AnimationFrame = 0;
    public float AnimationProgress = 0;
    public bool Playing = false;
    public float fps;
    public float ReferenceBPM = 120.0f;
    public bool ConstantSpeed = true; // if false modify delta time by beat length
    public bool Pausable = true; //if the animation pauses when the game is paused

    public bool SyncedToBeat = false; //if true don't use seconds current seconds is just current beat - start beat, and fps is frames per beat
    public float StartBeat = 0;

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
        CurrentFrame = -1;
        AnimationFrame = 0;
        AnimationProgress = 0.0f;
        CurrentAnim?.OnEnter?.Invoke(this);

        if (SyncedToBeat)
        {
            StartBeat = RRStageControllerPatch.instance.BeatmapPlayer.FmodTimeCapsule.TrueBeatNumber;
        }
    }
    public void PlaySync(string clipName, float subBeat)
    {
        Play(clipName);
        if (SyncedToBeat)
        {
            StartBeat = ((int)StartBeat) + subBeat;
        }
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
    public void Update(float deltaTime, float beatLength, bool paused)
    {
        if (!Playing) return;
        if (CurrentAnim == null) return;
        if (Pausable && paused) return;

        if (SyncedToBeat)
        {
            float CurrentBeat = RRStageControllerPatch.instance.BeatmapPlayer.FmodTimeCapsule.TrueBeatNumber;
            CurrentTime = CurrentBeat - StartBeat;
        }
        else
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
        }

        
        int cFrame = (int)(CurrentTime * fps);
        while (cFrame != CurrentFrame)
        {
            CurrentFrame++;
            if (CurrentFrame >= CurrentAnim.Duration)
            {
                if (!CurrentAnim.Loops)
                {
                    Playing = false;
                    CurrentAnim.OnFinish?.Invoke(this);
                    return;
                }
            }
            AnimationFrame = CurrentFrame % CurrentAnim.Duration;
            AnimationProgress = (float)AnimationFrame / (float)CurrentAnim.Duration;  
            CurrentAnim?.OnFrame?.Invoke(this);
        }
    }
}