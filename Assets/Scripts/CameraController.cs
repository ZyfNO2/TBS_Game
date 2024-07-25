using System;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_OFFSET = 2f;
    private const float MAX_FOLLOW_OFFSET = 12f;
    
    
    private Vector2 inputMoveDir = Vector2.zero;
    private Vector3 moveVector;
    private float moveSpeed = 10f;
    private Vector3 rotationVector;
    private float rotationSpeed = 100f;
    private float zoomAmount = 1f;
    private float zoomSpeed = 5f;
    private Vector3 targretFollowOffect;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineTransposer cinemachineTransposer;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targretFollowOffect = cinemachineTransposer.m_FollowOffset;

    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();


    }

    private void HandleMovement()
    {
        inputMoveDir = InputManager.Instance.GetCameraMoveVector();
        
        moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * (moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        rotationVector = Vector3.zero;

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();
        

        transform.eulerAngles += rotationVector * (rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        //Debug.Log(InputManager.Instance.GetCameraZoomAmount());
        //Vector3 followOffect = cinemachineTransposer.m_FollowOffset;
        float zoomIncreaseAmount = 1f;
        
        targretFollowOffect.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
        
        targretFollowOffect.y = Mathf.Clamp(targretFollowOffect.y, MIN_FOLLOW_OFFSET, MAX_FOLLOW_OFFSET);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset,targretFollowOffect,Time.deltaTime * zoomSpeed);
    }

    
    
    
}
