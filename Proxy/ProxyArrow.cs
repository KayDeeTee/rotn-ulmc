using MoonSharp.Interpreter;
using RhythmRift;
using RhythmRift.Enemies;
using Unity.Mathematics;
using UnityEngine;

namespace UIPlugin;

class ProxyArrow
{
    RRArrowView target;
    [MoonSharpHidden]
    public ProxyArrow(RRArrowView t)
    {
        target = t;
    }
    public GameObject gameObject => target.gameObject;
}
