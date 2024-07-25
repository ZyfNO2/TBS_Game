using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    
    private List<Vector3> positionList;
    private int currentPositionIndex;
    
    private float moveSpeed = 4f;
    private float rotateSpeed = 16f;
    private readonly float stoppingDistance = .1f;
    
    
     //[SerializeField] private Animator unitAnimator;
     [SerializeField] private int maxMoveDistance = 9;
    
    // protected override void Awake()
    // {
    //     base.Awake();
    //     unit = GetComponent<Unit>();
    // }

    public override string GetActionName()
    {
        return "Move";
    }
    

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        transform.forward = Vector3.Lerp
            (transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        
        
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this,EventArgs.Empty);
                
                ActionComplete();
            }
            
        }
        
        
        
        
        
    }

    public override void TakeAction(GridPosition gridPosition,Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = 
            Pathfinding.Instance.FindPath(unit.GetGridPosition(), 
                gridPosition,out int pathLength);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.
                Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        
       
        OnStartMoving?.Invoke(this,EventArgs.Empty);
       
        ActionStart(onActionComplete);
        
    }
    
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition =  new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (testGridPosition == unitGridPosition)
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if (!Pathfinding.Instance.HasPath(unit.GetGridPosition(),testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.
                        GetPathLengh(unitGridPosition, testGridPosition) >
                    maxMoveDistance * pathfindingDistanceMultiplier)
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
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
    
    
}
