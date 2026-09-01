using System.Collections.Generic;
using MoonSharp.Interpreter;
using RhythmRift.Enemies;
using Shared.RhythmEngine;
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
    public bool IsWrappingAroundGrid => target._isWrappingAroundGrid;
    public bool IsBeingTeleported => target.IsBeingTeleported;
    public void RecalculatePosition()
    {
        if( RRStageControllerPatch.instance == null || RRStageControllerPatch.instance._gridView == null ){
            return;
        }

        int2 currentGrid = target.CurrentGridPosition;
        int2 targetGrid = target.TargetGridPosition;
        Vector3 currentGridWorldPos = GetAdjustedTileWorldPosition( target, currentGrid.x, currentGrid.y );
        Vector3 targetGridWorldPos = GetAdjustedTileWorldPosition( target, targetGrid.x, targetGrid.y );

        // Keep stock wrap endpoints; writing on-grid world pos here sticks leave/enter anims.
        if( target._isWrappingAroundGrid ){
            RefreshWrappingWorldPositions( currentGridWorldPos, targetGridWorldPos );
        } else {
            target.CurrentGridWorldPosition = currentGridWorldPos;
            target.TargetWorldPosition = targetGridWorldPos;
        }

        SyncTransformToMovementProgress();

        if( IsWyrm ){
            if( target.IsBeingHeld ){
                target.transform.position = target.TargetWorldPosition;
            }
        }
    }
    public GameObject gameObject => target.gameObject;

    [MoonSharpHidden]
    void RefreshWrappingWorldPositions(Vector3 currentAdj, Vector3 targetAdj)
    {
        Vector2 baseDist = RRStageControllerPatch.instance._gridView.GetBaseDistanceBetweenTiles();

        // Target.x < Current.x => wrapped past right (wrapHorizontalSign = -1)
        // Target.x > Current.x => wrapped past left (wrapHorizontalSign = 1)
        int wrapHorizontalSign;
        if( target.TargetGridPosition.x < target.CurrentGridPosition.x ){
            wrapHorizontalSign = -1;
        } else if( target.TargetGridPosition.x > target.CurrentGridPosition.x ){
            wrapHorizontalSign = 1;
        } else {
            wrapHorizontalSign = target.TargetWorldPosition.x >= target.CurrentGridWorldPosition.x ? -1 : 1;
        }

        Vector3 offGridContinue = new Vector3(
            currentAdj.x - baseDist.x * wrapHorizontalSign,
            targetAdj.y,
            targetAdj.z );
        Vector3 queuedOffGrid = new Vector3(
            targetAdj.x + baseDist.x * wrapHorizontalSign,
            currentAdj.y,
            currentAdj.z );

        target._queuedOffGridWorldPosition = queuedOffGrid;
        target._queuedTargetWorldPosition = targetAdj;

        if( !target._hasPerformedGridWrapAround ){
            target.CurrentGridWorldPosition = currentAdj;
            target.TargetWorldPosition = offGridContinue;
        } else {
            target.CurrentGridWorldPosition = queuedOffGrid;
            target.TargetWorldPosition = targetAdj;
        }
    }

    [MoonSharpHidden]
    void SyncTransformToMovementProgress()
    {
        bool idleOnTile = target.CurrentGridPosition.Equals(target.TargetGridPosition)
            && !target._isWrappingAroundGrid
            && !target.IsBeingTeleported
            && !target.IsPerformingSpecialActionMovement
            && !target.IsPerformingHitMovement
            && !target.IsSnappingToActionRow;

        if( idleOnTile ){
            target.transform.position = target.CurrentGridWorldPosition;
            return;
        }

        var stage = RRStageControllerPatch.instance;
        var player = stage != null ? stage.BeatmapPlayer : null;
        if( player == null ){
            target.transform.position = target.TargetWorldPosition;
            return;
        }

        FmodTimeCapsule fmod = player.FmodTimeCapsule;
        float lerp;
        if( target.IsPerformingSpecialActionMovement && target._specialActionMoveCurve != null ){
            float durationSec = target.SpecialMoveActionDurationInBeats * fmod.BeatLengthInSeconds;
            float t = durationSec > 0f ? Mathf.Clamp01(target._timeSpentInSpecialActionMove / durationSec) : 1f;
            lerp = target._specialActionMoveCurve.Evaluate(t);
        } else {
            float progress = target.GetNormalizedProgressToNextMove(fmod);
            lerp = target._movementCurve != null ? target._movementCurve.Evaluate(progress) : progress;
        }

        target.transform.position = Vector3.Lerp( target.CurrentGridWorldPosition, target.TargetWorldPosition, lerp );
    }

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
