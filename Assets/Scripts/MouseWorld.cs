using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private void Awake()
    {
        instance = this;
    }

    private static MouseWorld instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;
    private void Update()
    {
        transform.position = MouseWorld.GetPosition();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit,
            float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
    
    
    
    
    
}
