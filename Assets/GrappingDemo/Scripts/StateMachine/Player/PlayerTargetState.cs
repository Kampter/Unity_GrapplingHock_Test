using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTargetState : PlayerBaseState
{
    private static readonly int TargetingBlendTree = Animator.StringToHash("TargetingBlendTree");
    private static readonly int TargetingForward = Animator.StringToHash("TargetingForward");
    private static readonly int TargetingRight = Animator.StringToHash("TargetingRight");
    public PlayerTargetState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputManager.TargetEvent += OnTarget;
        stateMachine.InputManager.JumpEvent += OnJump;
        stateMachine.InputManager.GrapplingEvent += OnGrappling;
        stateMachine.Animator.Play(TargetingBlendTree);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwtichState(new PlayerFreeLookState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
        UpdateAnimator(deltaTime);
        FaceTarget();
    }

    public override void Exit()
    {
        stateMachine.InputManager.TargetEvent -= OnTarget;
        stateMachine.InputManager.JumpEvent -= OnJump;
        stateMachine.InputManager.GrapplingEvent -= OnGrappling;
    }

    private void OnGrappling()
    {
        if (!stateMachine.Targeter.SelectTarget())
        {
            return;
        }
        stateMachine.SwtichState(new PlayerGrapplingState(stateMachine));
    }

    private void OnTarget()
    {
        stateMachine.Targeter.Cancel();
        stateMachine.SwtichState(new PlayerFreeLookState(stateMachine));
    }

    private void OnJump()
    {
        stateMachine.SwtichState(new PlayerJumpState(stateMachine));
    }
    
    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();
        var transform = stateMachine.CharacterController.transform;
        movement += transform.right * stateMachine.InputManager.MovementValue.x;
        movement += transform.forward * stateMachine.InputManager.MovementValue.y;
        return movement;
    }
    
    private void UpdateAnimator(float deltaTime)
    {
        float movementValueY = stateMachine.InputManager.MovementValue.y;
        if (movementValueY == 0)
        {
            stateMachine.Animator.SetFloat(TargetingForward, 0f, 0.1f, deltaTime);
        }
        else
        {
            stateMachine.Animator.SetFloat(TargetingForward, movementValueY > 0 ? 1f : -1f, 0.1f, deltaTime);
        }       
        float movementValueX = stateMachine.InputManager.MovementValue.x;
        if (movementValueX == 0)
        {
            stateMachine.Animator.SetFloat(TargetingRight, 0f, 0.1f, deltaTime);
        }
        else
        {
            stateMachine.Animator.SetFloat(TargetingRight, movementValueX > 0 ? 1f : -1f, 0.1f, deltaTime);
        }       
    }
}
