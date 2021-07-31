using SwordMan.Behaviors;
using SwordMan.Managers;
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
        [SerializeField] Cinemachine.CinemachineInputProvider _inputProvider = null;
        [SerializeField] Cinemachine.CinemachineFreeLook _thirdPersonCamera = null;
        [SerializeField] Cinemachine.CinemachineImpulseSource _cinemachineImpulseSource = null;
        [SerializeField] Transform _leftFoot = null, _rightFoot = null;
        [SerializeField] Transform _cameraAimPoint = null;
        [SerializeField] GameObject _targetLockedVolume = null;

        [SerializeField] float _slowMoMultiplier = 0.25f;

        [SerializeField] Cinemachine.CinemachineTargetGroup _targetGroup = null;


        #region private variables

        float _lastLookDirection = 0f;
        #endregion

        #region instances
        AnimatorController _animatorController;
        MovementBehavior _movementBehavior;
        FightingBehavior _fightingBehavior;
        #endregion

        #region public properties

        public Transform InputSpace => _inputSpace;

        #endregion

        void Awake()
        {
            _animatorController = GetComponent<AnimatorController>();
            _movementBehavior = GetComponent<MovementBehavior>();
            _fightingBehavior = GetComponent<FightingBehavior>();


            if (InputManager.Instance.HideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

        void Update()
        {
            if (InputManager.Instance.MovementLocked) return;

            Time.timeScale = 1f;

            if (InputManager.Instance.MovementInput.magnitude == 0f)
                InputManager.Instance.IsRunning = false;
            else
                InputManager.Instance.MovementInput = Vector2.ClampMagnitude(InputManager.Instance.MovementInput, 1f);


            if (InputManager.Instance.TargetLocked)
            {
                UpdateLockState();
            }
            else
            {
                ClearLockState();
            }

            UpdateAnimator();

        }


        void ClearLockState()
        {
            _targetLockedVolume.SetActive(false);
            _thirdPersonCamera.gameObject.SetActive(true);
            _thirdPersonCamera.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

            if (_inputProvider != null)
                _inputProvider.enabled = true;

            _movementBehavior.Move();
        }

        void UpdateLockState()
        {
            if (_fightingBehavior.CurrentTarget != null)
            {



                if (InputManager.Instance.MovementInput.magnitude == 0f)
                    InputManager.Instance.IsMovingLeft = _lastLookDirection < 0f;
                else
                    _lastLookDirection = InputManager.Instance.MovementInput.x;

                Time.timeScale = _slowMoMultiplier;

                _targetLockedVolume.SetActive(true);

                if (_inputProvider != null)
                    _inputProvider.enabled = false;

                _thirdPersonCamera.gameObject.SetActive(false);
                _thirdPersonCamera.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.LockToTarget;


                if (_targetGroup.FindMember(_fightingBehavior.CurrentTarget.transform) < 0)
                    _targetGroup.AddMember(_fightingBehavior.CurrentTarget.transform, 1f, 0f);

                if (_targetGroup.FindMember(_cameraAimPoint) < 0)
                    _targetGroup.AddMember(_cameraAimPoint, 1f, 1f);

                _movementBehavior.MoveAround(_fightingBehavior.CurrentTarget.transform);
            }
            else
            {
                InputManager.Instance.TargetLocked = false;
            }
        }

        void UpdateAnimator()
        {
            _animatorController.Animator.SetFloat(_animatorController.InputMagAnimParam, InputManager.Instance.MovementInput.magnitude);
            _animatorController.Animator.SetFloat(_animatorController.InputXAnimParam, InputManager.Instance.MovementInput.x);
            _animatorController.Animator.SetFloat(_animatorController.InputYAnimParam, InputManager.Instance.MovementInput.y);

            _animatorController.Animator.SetFloat(_animatorController.HorizontalAnimParam, InputManager.Instance.MovementInput.x * (InputManager.Instance.IsRunning ? 2f : 1f));
            _animatorController.Animator.SetFloat(_animatorController.VerticalAnimParam, InputManager.Instance.MovementInput.y * (InputManager.Instance.IsRunning ? 2f : 1f));

            _animatorController.Animator.SetBool(_animatorController.TargetLockedAnimParam, InputManager.Instance.TargetLocked);
            _animatorController.Animator.SetBool(_animatorController.IsMovingLeftAnimParam, InputManager.Instance.IsMovingLeft);
        }

        #region CustomEvents / Public Methods

        public void StopRun()
        {
            InputManager.Instance.IsRunning = false;
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
            InputManager.Instance.MovementLocked = true;
            InputManager.Instance.MovementInput = Vector3.zero;
            //Erase velocity in the future
        }


        public void UnlockMovement()
        {
            InputManager.Instance.MovementLocked = false;
            Debug.Log("Unlock");
        }
    }

    #endregion

}