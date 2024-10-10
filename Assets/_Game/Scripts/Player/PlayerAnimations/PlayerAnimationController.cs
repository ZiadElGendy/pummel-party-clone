using System.Collections;
using System.Collections.Generic;
using PummelPartyClone;
using UnityEngine;

namespace PummelPartyClone
{
    public class PlayerAnimationController : MonoBehaviour
{
    // Components
    private Animator _animator;
    private PlayerAnimationState _currentState;

    // Hashes for the animator transitions
    private int _jumpHash;
    private int _runHash;
    private int _hitHash;
    private int _happyHash;

    void Awake()
    {
        // Get the components
        _animator = GetComponent<Animator>();

        // Hash the transition parameter names for faster access
        _jumpHash = Animator.StringToHash("JumpTrigger");
        _runHash = Animator.StringToHash("IsRunning");
        _hitHash = Animator.StringToHash("HitTrigger");
        _happyHash = Animator.StringToHash("HappyTrigger");
    }

    void OnEnable()
    {
        PlayerController.OnPlayerStateChanged += SetState;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerStateChanged -= SetState;
    }

    public void SetState(PlayerAnimationState newState)
    {
        switch (newState)
        {
            case PlayerAnimationState.Idle:
                _animator.SetBool(_runHash, false);
                break;
            case PlayerAnimationState.Running:
                _animator.SetBool(_runHash, true);
                break;
            case PlayerAnimationState.Jumping:
                _animator.SetTrigger(_jumpHash);
                break;
            case PlayerAnimationState.Hit:
                _animator.SetTrigger(_hitHash);
                break;
            case PlayerAnimationState.Happy:
                _animator.SetTrigger(_happyHash);
                break;
        }
        _currentState = newState;
    }
}
}
