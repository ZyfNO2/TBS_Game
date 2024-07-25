using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;
    void Start()
    {
       

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
            //ScreenShake.Instance.Shake(5f);
            
            
             // GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
             // GridPosition startGridPosition = new GridPosition(0,0);
             //
             // List<GridPosition> gridPositions = Pathfinding.Instance.
             //     FindPath(startGridPosition, mouseGridPosition,out int pathLength);
             //
             // for (int i = 0; i < gridPositions.Count -1; i++)
             // {
             //     Debug.DrawLine(
             //         LevelGrid.Instance.GetWorldPosition(gridPositions[i]),
             //         LevelGrid.Instance.GetWorldPosition(gridPositions[i + 1]),
             //         Color.white,
             //         10f);
             // }
        }
    }
}
