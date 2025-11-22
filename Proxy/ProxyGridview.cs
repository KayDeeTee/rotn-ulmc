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
    public DynValue GetTileWorldPositionFromGridPosition(int x, int y)
    {
        Vector3 world_pos = target.GetTileWorldPositionFromGridPosition( x, y );
        return DynValue.NewTuple( [DynValue.NewNumber(world_pos.x), DynValue.NewNumber(world_pos.y), DynValue.NewNumber(world_pos.z)] );
    }
    public GameObject gameObject => target.gameObject;
}
