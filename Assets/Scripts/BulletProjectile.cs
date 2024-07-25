using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;
    private Vector3 targetPosition;
    
    public void SetUp(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;

    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = 
            Vector3.Distance(transform.position, targetPosition);
        
        float moveSpeed = 200f;
        transform.position += moveDir * (moveSpeed * Time.deltaTime);
        
        float distanceAfterMoving = 
            Vector3.Distance(transform.position, targetPosition);

        //over shoot
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;
            
            trailRenderer.transform.parent = null; 
            
            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity);
        }

    }
}


