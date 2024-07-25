using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    
    
    
    private CinemachineImpulseSource cinemachineImpulseSource;

    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one ScreenShake" + transform + "-" + Instance );
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        
        
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    

    public void Shake(float intenity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intenity);
    }
    
    
}
