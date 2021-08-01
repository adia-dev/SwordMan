using SwordMan.Controllers;
using SwordMan.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordMan.Behaviors
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementBehavior : MonoBehaviour
    {

        [SerializeField] float _walkSpeed = 2f, _jogSpeed = 5f, _runSpeed = 7f;
        [SerializeField] float _speedSmoothTime = 0.1f, _turnSmoothTime = 0.15f;
        [SerializeField] float _gravityMultiplier = 1f;

        PlayerController _playerController;
        CharacterController _characterController;
        AnimatorController _animatorController;

        float _turnSmoothVelocity, _speedSmoothVelocity;
        float _velocityY;
        float _currentSpeed;

        public bool IsGrounded => _characterController.isGrounded;


        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            _animatorController = GetComponent<AnimatorController>();

        }

        // Update is called once per frame
        void Update()
        {
            if (IsGrounded)
            {
                _velocityY = 0f;
            }

            _animatorController.Animator.SetFloat(_animatorController.VelocityAnimParam, _currentSpeed);

        }


        public void Move()
        {
            if (InputManager.Instance.MovementInput.magnitude >= 0.2f)
            {
                float targetAngle = Mathf.Atan2(InputManager.Instance.MovementInput.x, InputManager.Instance.MovementInput.y) * Mathf.Rad2Deg;
                if (_playerController.InputSpace != null)
                    targetAngle += _playerController.InputSpace.eulerAngles.y;

                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

            }


            float targetSpeed = (InputManager.Instance.IsRunning ? _runSpeed : _walkSpeed) * InputManager.Instance.MovementInput.magnitude;

            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, _speedSmoothTime);

            _velocityY += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            Vector3 velocity = transform.forward * _currentSpeed + Vector3.up * _velocityY;

            _characterController.Move(velocity * Time.deltaTime);
        }

        public void MoveAround(Transform target)
        {
            if (target == null) return;



            Vector3 targetDirection = (target.transform.position - transform.position);
            targetDirection.y = 0f;
            targetDirection.Normalize();

            transform.rotation = Quaternion.LookRotation(targetDirection);

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            float targetSpeed = (InputManager.Instance.IsRunning ? _runSpeed : _walkSpeed) * InputManager.Instance.MovementInput.magnitude;

            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, targetSpeed, ref _speedSmoothVelocity, _speedSmoothTime);


            _velocityY += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            Vector3 velocity = (forward * InputManager.Instance.MovementInput.y + right * InputManager.Instance.MovementInput.x) * _currentSpeed + Vector3.up * _velocityY;

            _characterController.Move(velocity * Time.deltaTime);

        }
    }

}