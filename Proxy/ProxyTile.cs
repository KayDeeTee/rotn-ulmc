using MoonSharp.Interpreter;
using RhythmRift;
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
