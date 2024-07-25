using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;

public class UnitAnimator : MonoBehaviour
{
 
    [SerializeField] private Animator animator;
    [FormerlySerializedAs("bulletProjecttilePrefab")] [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;
    
    private void Awake()
    {
        
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;

        }
        
        
    }

    private void Start()
    {
        EquipRifle();
    }


    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        Debug.Log("123");
        animator.SetTrigger("SwordSlash");
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }


    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking",true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking",false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletPorjectileTransform = 
            Instantiate(bulletProjectilePrefab,
            shootPointTransform.position,
            Quaternion.identity);
        BulletProjectile bulletProjectile = 
            bulletPorjectileTransform.
                GetComponent<BulletProjectile>();



        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y;
        
        //e.targetUnit.GetWorldPosition();
        bulletProjectile.SetUp(targetUnitShootAtPosition);
        
        
        
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }
    
    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
    
    
    
    
}
