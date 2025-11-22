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

    public DynValue GetPosition => DynValue.NewTuple([DynValue.NewNumber( target.transform.localPosition.x), DynValue.NewNumber( target.transform.localPosition.y), DynValue.NewNumber( target.transform.localPosition.z)  ]);
    public void SetPosition(float x, float y, float z)
    {
        target.transform.localPosition = new Vector3(x, y, z);
    }

    public DynValue GetScale => DynValue.NewTuple([DynValue.NewNumber( target.transform.localScale.x), DynValue.NewNumber( target.transform.localScale.y), DynValue.NewNumber( target.transform.localScale.z)  ]);
    public void SetScale(float x, float y, float z)
    {
        target.transform.localScale = new Vector3(x, y, z);
    }

    public DynValue GetEuler => DynValue.NewTuple([DynValue.NewNumber( target.transform.localEulerAngles.x), DynValue.NewNumber( target.transform.localEulerAngles.y), DynValue.NewNumber( target.transform.localEulerAngles.z)  ]);
    public void SetEuler(float x, float y, float z)
    {
        target.transform.localEulerAngles = new Vector3(x, y, z);
    }

    public DynValue GetRotation => DynValue.NewTuple([DynValue.NewNumber( target.transform.localRotation.x), DynValue.NewNumber( target.transform.localRotation.y), DynValue.NewNumber( target.transform.localRotation.z), DynValue.NewNumber( target.transform.localRotation.w)  ]);
    public void SetRotation(float x, float y, float z, float w)
    {
        target.transform.localRotation = new Quaternion(x, y, z, w);
    }

}
