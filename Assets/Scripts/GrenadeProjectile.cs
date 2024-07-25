using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private GameObject grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    
    private Vector3 targetPosition;
    private Action onGrenadeBehaviorComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    
    

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDir * (moveSpeed * Time.deltaTime);


        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance/totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
        
        float reachedTargetDistance = .1f;
        
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            //Debug.Log("123");
            float damageRaduis = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRaduis);
            
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
                
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                { 
                    destructibleCrate.Damage();
                }
                
                
            }

            trailRenderer.transform.parent = null;
            OnAnyGrenadeExploded?.Invoke(this,EventArgs.Empty);
            Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            onGrenadeBehaviorComplete();
            Destroy(gameObject);
            
            
        }
        
    }


    public void Setup(GridPosition targetGridPosition,Action onGrenadeBehaviorComplete)
    {
        this.onGrenadeBehaviorComplete = onGrenadeBehaviorComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, targetPosition);

    }
}
