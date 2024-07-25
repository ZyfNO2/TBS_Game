using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;
    
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    
    private enum State
    {
        SwingSwordBeforeHit,
        SwingSwordAfterHit,
    }
    
    
    private int maxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        stateTimer -= Time.deltaTime;
        
        switch (state)
        {
            case State. SwingSwordBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition()
                                  - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward,aimDir, 
                    Time.deltaTime * rotateSpeed);
                break;
            
            case State.SwingSwordAfterHit:
                
                break;
            
            
        }
        
        if (stateTimer <= 0f)
        {
            NextState();
        }
        
    }
    private void NextState()
    {
        switch (state)
        {
            case State.SwingSwordBeforeHit:
                state = State.SwingSwordAfterHit;
                float afterHitStateTime = .5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(90);
                OnAnySwordHit?.Invoke(this,EventArgs.Empty);
                break;
            
            case State.SwingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this,EventArgs.Empty);
                ActionComplete();
                break;
            
            
        }
        
        
        //Debug.Log(state);
        
    }


    public override string GetActionName()
    {
        return "Sword"
            ;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        state = State.SwingSwordBeforeHit;
        float beforeHitStateTime = .7f;
        stateTimer = beforeHitStateTime;

        if (OnSwordActionStarted == null)
        {
            Debug.Log("Not Null");
        }
        OnSwordActionStarted?.Invoke(this,EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                
                //Debug.Log(testGridPosition);

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                // ReSharper disable once EqualExpressionComparison
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }
                
                
                
                validGridPositionList.Add(testGridPosition);
                
            }
        } 
        

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }
    
    
    
}
