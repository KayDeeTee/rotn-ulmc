using System.Collections.Generic;
using MoonSharp.Interpreter;
using RhythmRift;
using RhythmRift.Enemies;
using Unity.Mathematics;
using UnityEngine;

namespace UIPlugin;

class ProxyGridview
{
    RRGridView target;
    [MoonSharpHidden]
    public ProxyGridview(RRGridView t)
    {
        target = t;
    }
    public RRTileView GetTile(int x, int y)
    {
        return target._tileViewsByGridPosition[ new int2( x,y ) ];
    }
    public RRArrowView GetArrow( int x)
    {
        return target._arrows[ x ];
    }
    public Dictionary<string, float> GetTileWorldPositionFromGridPosition(int x, int y)
    {
        Vector3 world_pos = target.GetTileWorldPositionFromGridPosition( x, y );
        return LuaManager.Vec3Dict( world_pos );
    }
    public GameObject gameObject => target.gameObject;
}
