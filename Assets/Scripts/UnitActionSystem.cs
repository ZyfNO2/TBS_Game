using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class UnitActionSystem : MonoBehaviour
{
    
    public static UnitActionSystem Instance { get; private set; }
    private bool isBusy;
    private BaseAction selectedAction;
    
    
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;
    
    
    
    [SerializeField]private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one UnityActionSystem" + transform + "-" + Instance );
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }


    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        
        if (TryHandleUnitSelection())
        {
            return;
            
        }
        
        
        HandleSelectedAction();
        
        
    }




    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                return;
            }
            
            
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition,ClearBusy);

            OnActionStarted?.Invoke(this,EventArgs.Empty);
            
            // switch (selectedAction)
            // {
            //     case MoveAction moveAction:
            //         if (moveAction.IsValidActionGridPosition(mouseGridPosition))
            //         {
            //             
            //             selectedUnit.GetMoveAction().Move(mouseGridPosition,ClearBusy);
            //         }
            //         break;
            //     case SpinAction spinAction:
            //         SetBusy();
            //         
            //         spinAction.Spin(ClearBusy);
            //         break;
            //         
                    
                    
        }

    }
    
    
    
    private void SetBusy()
    {
        isBusy = true;
        
        OnBusyChanged?.Invoke(this,isBusy);
        
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this,isBusy);
    }
    
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if(Physics.Raycast(ray, out RaycastHit raycastHit,
                   float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {

                    if (unit == selectedUnit)
                    {
                        //allready been selected
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        //click an emeny
                        return false;
                    }
                    
                    
                    
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        
        
        

        return false;
    }


    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        
        SetSelectedAction(unit.GetAction<MoveAction>());
        
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

    }


    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        
    }
    

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
    
    
    
}
