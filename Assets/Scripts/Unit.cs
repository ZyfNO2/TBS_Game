using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    
    [SerializeField] private bool isEnemy;
    
    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    // private MoveAction moveAction;
    // private SpinAction spinAction;
    // private ShootAction shootAction;
    private BaseAction[] baseActionArray;

    private int actionPoints = 9;
    
 
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        // moveAction = GetComponent<MoveAction>();
        // spinAction = GetComponent<SpinAction>();
        // shootAction = GetComponent<ShootAction>();
        baseActionArray = GetComponents<BaseAction>();
    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition,this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this,EventArgs.Empty);
    }
    
    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovePosition(this,oldGridPosition,newGridPosition);
        }
    }

    public T GetAction<T>() where T: BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }

        return null;
    }
    
    // public MoveAction GetMoveAction()
    // {
    //     return moveAction;
    // }
    //
    // public SpinAction GetSpinAction()
    // {
    //     return spinAction;
    // }
    //
    // public ShootAction GetShootAction()
    // {
    //     return shootAction;
    // }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }
    
    

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoint(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoint(int amount)
    {
        actionPoints -= amount;
        
        OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())||
            (!IsEnemy())&&TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = ACTION_POINTS_MAX;
        
            OnAnyActionPointsChanged?.Invoke(this,EventArgs.Empty);
        }
        
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtPosition(gridPosition,this);
        Destroy(gameObject);
        
        OnAnyUnitDead?.Invoke(this,EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
    
    

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    
    
    
}