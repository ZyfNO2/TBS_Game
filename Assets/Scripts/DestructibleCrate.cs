using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;
    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }


    public void Damage()
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);
        
        Destroy(gameObject);
        
        OnAnyDestroyed?.Invoke(this,EventArgs.Empty);
        
    }
    
    private void ApplyExplosionToChildren(Transform root,float explosionForce,
        Vector3 explosionPosition,float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                
                childRigidbody.AddExplosionForce(explosionForce,
                    explosionPosition,explosionRange);
            }
            
            ApplyExplosionToChildren(child,
                explosionForce, explosionPosition, explosionRange);
            
        }
    }
    

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    
    
}
