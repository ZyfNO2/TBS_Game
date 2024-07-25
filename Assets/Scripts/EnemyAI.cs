using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        //no more and end turn
                        TurnSystem.Instance.NextTurn();
                    }
                    //TurnSystem.Instance.NextTurn();
                }
                break;
            case State.Busy:
                break;
            
        }
        
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            
            timer = 2f;
            
        }
    }

    private void SetStateTakingTurn()
    {
        timer = .5f;
        state = State.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {

        
        //Debug.Log("Take Enemy AI Action");
        foreach (Unit enemyUnity in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnity, onEnemyAIActionComplete))
            {
                return true;
                
            }
        }

        return false;

    }

    private bool TryTakeEnemyAIAction(Unit enemyUnity, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (BaseAction baseAction in enemyUnity.GetBaseActionArray())
        {
            if (!enemyUnity.CanSpendActionPointToTakeAction(baseAction))
            {
                //cam not afford
                continue;
            }

            if (bestBaseAction == null)
            {
                // new here
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();

                bestBaseAction = baseAction;
            }
            else
            {
                //better ai value
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = baseAction.GetBestEnemyAIAction();

                    bestBaseAction = baseAction;
                }
            }
            

        }

        if (bestBaseAction != null && enemyUnity.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition,onEnemyAIActionComplete);

            return true;

        }
        else
        {
            return false;
        }
        
        // SpinAction spinAction = enemyUnity.GetSpinAction();
        //
        // GridPosition actionGridPosition = enemyUnity.GetGridPosition();
        //
        // if (!spinAction.IsValidActionGridPosition(actionGridPosition))
        // {
        //     return false;
        // }
        //
        // if (!enemyUnity.TrySpendActionPointsToTakeAction(spinAction))
        // {
        //     return false;
        // }
        //     
        // Debug.Log("Spin Action");
        //
        // spinAction.TakeAction(actionGridPosition,onEnemyAIActionComplete);
        // return true;

    }
    
    
    
    
    
    
    
    
}
