using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }
    
    private int turnNumber = 1;
    private bool isPlayerTurn = true;
    
    public event EventHandler OnTurnChanged;
    
    
    public void NextTurn()
    {
        turnNumber++;

        isPlayerTurn = !isPlayerTurn;
        
        OnTurnChanged?.Invoke(this,EventArgs.Empty);
        
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one TurnSystem" + transform + "-" + Instance );
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
    
    
    
}
