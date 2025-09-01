using MoonSharp.Interpreter;
using RhythmRift.Enemies;
using UnityEngine;

namespace UIPlugin;

class ProxyEnemy
{
    RREnemy target;
    [MoonSharpHidden]
    public ProxyEnemy(RREnemy t)
    {
        target = t;
    }
    public int CurrentHealth => target.CurrentHealthValue;
    public DynValue CurrentGridPos => DynValue.NewTuple([DynValue.NewNumber(target.CurrentGridPosition.x), DynValue.NewNumber(target.CurrentGridPosition.y)]);
    public DynValue TargetGridPos => DynValue.NewTuple([DynValue.NewNumber(target.TargetGridPosition.x), DynValue.NewNumber(target.TargetGridPosition.y)]);
    public DynValue CurrentPosition => DynValue.NewTuple([DynValue.NewNumber( ((Component)target).transform.position.x), DynValue.NewNumber( ((Component)target).transform.position.y), DynValue.NewNumber( ((Component)target).transform.position.z)  ]);
    public int EnemyId => int.Parse(target.EnemyId);
    public bool IsWyrm => target.IsHoldNote;
}
