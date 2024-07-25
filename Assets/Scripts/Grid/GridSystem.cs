using UnityEngine;
using System;

public class GridSystem<TGridObject> 
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectsArray;
    
    
    
    public GridSystem(int width,int height,float cellSize,
        Func<GridSystem<TGridObject>,GridPosition,TGridObject> creatGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectsArray = new TGridObject[width, height];
        
        
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Debug.DrawLine(GetWorldPosition(x,z),GetWorldPosition(x,z) + Vector3.right*.2f,Color.white,1000);
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectsArray[x, z] = creatGridObject(this, gridPosition);

            }
        }
        
    }


    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize));

    }


    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                Transform debugTransfrom =  
                    GameObject.
                    Instantiate(debugPrefab,GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject =  debugTransfrom.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
        
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
    
    
    
}
