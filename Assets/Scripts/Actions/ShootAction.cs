using System;
using System.Collections.Generic;
using UnityEngine;


public class ShootAction : BaseAction
{

    //public event EventHandler<Unit> OnShoot;
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;

    }
    
    
    
    private enum State
    {
        Aiming,
        Shooting,
        CoolOff,
    
    }

    [SerializeField] private LayerMask obstaclesLayerMask;
    
    private State state;
    private int maxShootDistance = 4;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;
    
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition()
                                  - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward,aimDir, 
                    Time.deltaTime * rotateSpeed);
                break;
            
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            
            case State.CoolOff:
                
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = .1f;
                stateTimer = shootingStateTime;
                break;
            
            case State.Shooting:
                state = State.CoolOff;
                float coolOffStateTime = .5f;
                stateTimer = coolOffStateTime;
                break;
            
            case State.CoolOff:
                ActionComplete();
                break;
        }
        
        
        //Debug.Log(state);
        
    }
    
    
    private void Shoot()
    {
        OnAnyShoot?.Invoke(this,new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
        });
        
        
        OnShoot?.Invoke(this,new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit,
        });
        
        
        
        targetUnit.Damage(40);
    }
    

    public override void TakeAction(GridPosition gridPosition,Action onActionComplete)
    {
        

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;
        
        ActionStart(onActionComplete);
        

    }

    public override string GetActionName()
    {
        return "Shoot";
    }
    
    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition untiGridPosition)
    {
        
        
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                
                if (testDistance > maxShootDistance)
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

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unit.GetWorldPosition(), targetUnit.GetWorldPosition()),
                        obstaclesLayerMask))
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
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt(
                (1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
    
    
    
}