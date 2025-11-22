using MoonSharp.Interpreter;
using RhythmRift;
using RhythmRift.Enemies;
using Unity.Mathematics;
using UnityEngine;

namespace UIPlugin;

class ProxyTile
{
    RRTileView target;
    [MoonSharpHidden]
    public ProxyTile(RRTileView t)
    {
        target = t;
    }
    public GameObject gameObject => target.gameObject;
}
