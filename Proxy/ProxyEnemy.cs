using MoonSharp.Interpreter;
using RhythmRift.Enemies;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UIPlugin;

class ProxyEnemy
{
    RREnemy target;
    [MoonSharpHidden]
    public ProxyEnemy(RREnemy t)
    {
        target = t;
    }
    public int current_health => target.CurrentHealthValue;
    public DynValue current_grid_pos => DynValue.NewTuple([DynValue.NewNumber(target.CurrentGridPosition.x), DynValue.NewNumber(target.CurrentGridPosition.y)]);
    public DynValue target_grid_pos => DynValue.NewTuple([DynValue.NewNumber(target.TargetGridPosition.x), DynValue.NewNumber(target.TargetGridPosition.y)]);
    public int enemy_id => int.Parse(target.EnemyId);
    public bool is_wyrm => target.IsHoldNote;
}

