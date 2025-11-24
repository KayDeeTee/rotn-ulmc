using System.Collections.Generic;
using MoonSharp.Interpreter;
using RhythmRift;
using RhythmRift.Enemies;
using Unity.Mathematics;
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
    public Dictionary<string, float> CurrentGridPos() => LuaManager.Vec2Dict( new Vector2(target.CurrentGridPosition.x, target.CurrentGridPosition.y ) );
    public Dictionary<string, float> TargetGridPos() => LuaManager.Vec2Dict( new Vector2(target.TargetGridPosition.x, target.TargetGridPosition.y ) );
    public Dictionary<string, float> CurrentPosition() => LuaManager.Vec3Dict( target.transform.localPosition );
    public int EnemyId => int.Parse(target.EnemyId);
    public bool IsWyrm => target.IsHoldNote;
    public void RecalculatePosition()
    {
        int2 CurrentGrid = target.CurrentGridPosition;
        int2 TargetGrid = target.TargetGridPosition;
        Vector3 CurrentGridWorldPos = GetAdjustedTileWorldPosition( target, CurrentGrid.x, CurrentGrid.y );
        Vector3 TargetGridWorldPos = GetAdjustedTileWorldPosition( target, TargetGrid.x, TargetGrid.y );
        target.CurrentGridWorldPosition = CurrentGridWorldPos;
        target.TargetWorldPosition = TargetGridWorldPos;

        if( IsWyrm ){
            if( target.IsBeingHeld ){
                target.transform.position = target.TargetWorldPosition;
            }
        }
    }
    public GameObject gameObject => target.gameObject;

    [MoonSharpHidden]
    public Vector3 GetAdjustedTileWorldPosition(RREnemy enemy, int xCoordinate, int yCoordinate)
    {
        if (RRStageControllerPatch.instance._gridView == null)
        {
            return Vector3.zero;
        }

        Vector3 tileWorldPositionFromGridPosition = RRStageControllerPatch.instance._gridView.GetTileWorldPositionFromGridPosition(xCoordinate, yCoordinate);
        float time = 1f - Mathf.Clamp01((float)yCoordinate / (float)RRStageControllerPatch.instance._gridView.NumRows);
        float num = enemy.ZOffsetDistanceScaleCurve.Evaluate(time);
        Vector3 basePositionOffset = enemy.BasePositionOffset;
        Vector3 vector = new Vector3(basePositionOffset.x, basePositionOffset.y, basePositionOffset.z * num);
        return tileWorldPositionFromGridPosition + vector;
    }


}
