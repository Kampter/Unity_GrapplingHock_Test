using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }
    public event Action JumpEvent;
    public event Action TargetEvent;
    public event Action GrapplingEvent;
    private Controls controls;
    
    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    private void OnDestroy()
    {
        controls.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        if (JumpEvent != null)
        {
            JumpEvent.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        if (TargetEvent != null)
        {
            TargetEvent.Invoke();
        }
    }

    public void OnGrappling(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        if (GrapplingEvent != null)
        {
            GrapplingEvent.Invoke();
        }
    }
}
