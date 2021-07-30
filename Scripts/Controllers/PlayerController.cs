using SwordMan.Behaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SwordMan.Controllers
{
    [RequireComponent(typeof(AnimatorController))]
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] Transform _inputSpace = null;
        [SerializeField] GameObject _thirdPersonCamera = null;
        [SerializeField] Transform _leftFoot = null, _rightFoot = null;
        [SerializeField] Transform _target;
        [SerializeField] Transform _cameraAimPoint = null;

        [SerializeField] bool _hideCursor = true;
        [SerializeField] bool _showDebug = false;
        [SerializeField] Cinemachine.CinemachineTargetGroup _targetGroup = null;


        #region private variables
        bool _isRunning = false;
        bool _lockMovement = false;
        bool _targetLocked = false;
        bool _isMovingLeft = false;
        float _lastLookDirection = 0f;
        Vector2 _movementInput;
        #endregion

        #region instances
        AnimatorController _animatorController;
        MovementBehavior _movementBehavior;
        #endregion

        #region public properties

        public Vector2 MovementInput => _movementInput;
        public bool MovementLocked => _lockMovement;
        public bool TargetLocked => _targetLocked;
        public bool IsMovingLeft => _isMovingLeft;
        public bool IsRunning => _isRunning;
        public Transform Target => _target;
        public Transform InputSpace => _inputSpace;

        #endregion

        void Awake()
        {
            _animatorController = GetComponent<AnimatorController>();
            _movementBehavior = GetComponent<MovementBehavior>();

            if (_hideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void Update()
        {
            if (_lockMovement) return;

            if (_movementInput.magnitude == 0f)
                _isRunning = false;

            HandleInputs();
            UpdateAnimator();

        }


        void HandleInputs()
        {
            _movementInput = Vector2.ClampMagnitude(_movementInput, 1f);

            if (_targetLocked && _target != null)
            {

                _thirdPersonCamera?.SetActive(false);
                _targetGroup.RemoveMember(null);
                _targetGroup.RemoveMember(_cameraAimPoint);

                if (_movementInput.magnitude == 0f)
                    _isMovingLeft = _lastLookDirection < 0f;
                else
                    _lastLookDirection = _movementInput.x;



                if (_targetGroup.FindMember(_target) < 0)
                    _targetGroup.AddMember(_target, 1f, 0f);

                if (_targetGroup.FindMember(_cameraAimPoint) < 0)
                    _targetGroup.AddMember(_cameraAimPoint, 10f, 1f);

                _movementBehavior.MoveAround(_target);

            }
            else
            {
                _thirdPersonCamera?.SetActive(true);
                _movementBehavior.Move();
            }

        }


        void UpdateAnimator()
        {
            _animatorController.Animator.SetFloat(_animatorController.InputMagAnimParam, _movementInput.magnitude);
            _animatorController.Animator.SetFloat(_animatorController.InputXAnimParam, _movementInput.x);
            _animatorController.Animator.SetFloat(_animatorController.InputYAnimParam, _movementInput.y);

            _animatorController.Animator.SetFloat(_animatorController.HorizontalAnimParam, _movementInput.x * (_isRunning ? 2f : 1f));
            _animatorController.Animator.SetFloat(_animatorController.VerticalAnimParam, _movementInput.y * (_isRunning ? 2f : 1f));

            _animatorController.Animator.SetBool(_animatorController.TargetLockedAnimParam, _targetLocked);
            _animatorController.Animator.SetBool(_animatorController.IsMovingLeftAnimParam, _isMovingLeft);

        }

        void OnDrawGizmos()
        {
            if (!_showDebug || _leftFoot == null || _rightFoot == null) return;


            Gizmos.DrawLine(_leftFoot.position, _rightFoot.position);

        }



        #region InputAction Events

        public void OnMove(InputAction.CallbackContext ctx)
        {
            _movementInput = ctx.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext ctx)
        {
            _isRunning = !_isRunning;
        }
        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _animatorController.Animator.SetTrigger(_animatorController.JumpAnimParam);
        }

        public void OnDodge(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _animatorController.Animator.SetTrigger(_animatorController.DodgeAnimParam);
        }

        public void OnLockTarget(InputAction.CallbackContext ctx)
        {
            if (ctx.started && _target != null)
                _targetLocked = !_targetLocked;
        }

        public void OnShowDebug(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _showDebug = !_showDebug;
        }

        #endregion

        #region CustomEvents / Public Methods

        public void StopRun()
        {
            _isRunning = false;
        }

        public void EnableRootMotion()
        {
            _animatorController.Animator.applyRootMotion = true;
        }


        public void DisableRootMotion()
        {
            _animatorController.Animator.applyRootMotion = false;
        }

        public void LockMovement()
        {
            _lockMovement = true;
            _movementInput = Vector3.zero;
            //Erase velocity in the future
        }


        public void UnlockMovement()
        {
            _lockMovement = false;
            Debug.Log("Unlock");
        }
    }

    #endregion

}