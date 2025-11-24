using System.Collections.Generic;
using MoonSharp.Interpreter;
using RhythmRift;
using RhythmRift.Enemies;
using Unity.Mathematics;
using UnityEngine;

namespace UIPlugin;

class ProxyGameObject
{
    GameObject target;
    [MoonSharpHidden]
    public ProxyGameObject(GameObject t)
    {
        target = t;
    }
    public float PositionX => target.transform.localPosition.x;
    public Dictionary<string, float> GetPosition() => LuaManager.Vec3Dict(target.transform.localPosition);
    public void SetPosition(float x, float y, float z)
    {
        target.transform.localPosition = new Vector3(x, y, z);
    }

    public Dictionary<string, float> GetScale() => LuaManager.Vec3Dict(target.transform.localScale);
    public void SetScale(float x, float y, float z)
    {
        target.transform.localScale = new Vector3(x, y, z);
    }

    public Dictionary<string, float> GetEuler() => LuaManager.Vec3Dict( target.transform.localEulerAngles );
    public void SetEuler(float x, float y, float z)
    {
        target.transform.localEulerAngles = new Vector3(x, y, z);
    }

    public Dictionary<string, float> GetRotation() => LuaManager.QuaternionDict( target.transform.localRotation );
    public void SetRotation(float x, float y, float z, float w)
    {
        target.transform.localRotation = new Quaternion(x, y, z, w);
    }

}
