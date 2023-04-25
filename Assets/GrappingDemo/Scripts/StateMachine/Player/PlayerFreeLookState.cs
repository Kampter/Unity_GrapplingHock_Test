using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFreeLookState : PlayerBaseState
{
    private static readonly int FreeLookSpeed = Animator.StringToHash("FreeLookSpeed");
    private static readonly int FreeLookBlendTree = Animator.StringToHash("FreeLookBlendTree");
    private const float AnimatorDampTime = 0.1f;
    public PlayerFreeLookState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.InputManager.TargetEvent += OnTarget;
        stateMachine.InputManager.JumpEvent += OnJump;
        stateMachine.InputManager.GrapplingEvent += OnGrappling;
        stateMachine.Animator.Play(FreeLookBlendTree);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        stateMachine.transform.Translate(movement * deltaTime);

        float speed = stateMachine.FreeLookMovementSpeed;
        Move(movement * speed, deltaTime);

        if (stateMachine.InputManager.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeed, 0, AnimatorDampTime, deltaTime);
            return;
        }
        stateMachine.Animator.SetFloat(FreeLookSpeed, 1, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
        
    }

    public override void Exit()
    {
        stateMachine.InputManager.TargetEvent -= OnTarget;
        stateMachine.InputManager.JumpEvent -= OnJump;
        stateMachine.InputManager.GrapplingEvent -= OnGrappling;
    }

    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget())
        {
            return;
        }
        stateMachine.SwtichState(new PlayerTargetState(stateMachine));
    }
    
    private void OnJump()
    {
        stateMachine.SwtichState(new PlayerJumpState(stateMachine));
    }
    
    private void OnGrappling()
    {
        if (!stateMachine.Targeter.SelectTarget())
        {
            return;
        }
        stateMachine.SwtichState(new PlayerGrapplingState(stateMachine));
    }
    
    private Vector3 CalculateMovement()
    {
       Vector3 forward = stateMachine.MainCameraTransforms.forward;
       Vector3 right = stateMachine.MainCameraTransforms.right;

       forward.y = 0f;
       right.y = 0f;

       forward.Normalize();
       right.Normalize();

       Vector2 movementValue = stateMachine.InputManager.MovementValue;

       return forward * movementValue.y + right * movementValue.x;
    }
    
    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationSpeedSmooth);
    }
    
}
