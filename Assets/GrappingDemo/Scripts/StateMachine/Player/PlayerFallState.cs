using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private static readonly int Fall = Animator.StringToHash("Fall");
    private const float CrossFadeFixedTime = 0.1f;
    private Vector3 momentum;
    public PlayerFallState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.InputManager.GrapplingEvent += OnGrappling;
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(Fall, CrossFadeFixedTime);
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);
        if (stateMachine.CharacterController.isGrounded)
        {
            ReturnToLocomotion();
        }
        FaceTarget();
    }

    public override void Exit()
    {
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
}
