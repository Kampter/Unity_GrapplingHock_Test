using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private static readonly int Jump = Animator.StringToHash("Jump");
    private const float CrossFadeFixedTime = 0.1f;
    private Vector3 momentum;
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        
    }

    public override void Enter()
    {
        stateMachine.InputManager.GrapplingEvent += OnGrappling;
        stateMachine.ForceReceiver.Jump(stateMachine.JumpForce);
        momentum = stateMachine.CharacterController.velocity;
        momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(Jump, CrossFadeFixedTime);
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);
        if (stateMachine.CharacterController.velocity.y <= 0)
        {
            stateMachine.SwtichState(new PlayerFallState(stateMachine));
            return;
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
