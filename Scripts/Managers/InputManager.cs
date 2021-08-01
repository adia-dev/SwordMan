using SwordMan.Behaviors;
using SwordMan.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SwordMan.Managers
{
    public class InputManager : MonoBehaviour
    {

        [SerializeField] bool _hideCursor = true;
        [SerializeField] bool _showDebug = false;

        static InputManager _instance = null;

        public static InputManager Instance => _instance;

        FightingBehavior _fightingBehavior = null;

        void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);

            _instance = this;
            _fightingBehavior = FindObjectOfType<PlayerController>().GetComponent<FightingBehavior>();

        }

        public Vector2 MovementInput { get; set; }
        public bool MovementLocked { get; set; }
        public bool DesireJump { get; set; }
        public bool Attack1 { get; set; }
        public bool Attack2 { get; set; }
        public bool Special1 { get; set; }
        public bool Special2 { get; set; }
        public bool DesireDodge { get; set; }
        public bool LockTarget { get; set; }
        public bool IsMovingLeft { get; set; }
        public bool IsRunning { get; set; }
        public bool ShowDebug { get; set; }
        public bool HideCursor => _hideCursor;

        #region InputAction Events


        public void OnMove(InputAction.CallbackContext ctx)
        {
            MovementInput = ctx.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext ctx)
        {
            IsRunning = !IsRunning;
        }
        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                DesireJump = true;
            if (ctx.canceled)
                DesireJump = false;
        }

        public void OnDodge(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                DesireDodge = true;
            if (ctx.canceled)
                DesireDodge = false;
        }

        public void OnLockTarget(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                LockTarget = true;

            Debug.Log("Lock Target");

            if (ctx.canceled)
                LockTarget = false;
        }

        public void OnSwitchTarget(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _fightingBehavior.SwitchTarget();

        }

        public void OnSwitchTargetLeft(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _fightingBehavior.SwitchTargetLeft();
        }

        public void OnSwitchTargetRight(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _fightingBehavior.SwitchTargetRight();
        }

        public void OnShowDebug(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                ShowDebug = !ShowDebug;
        }

        public void OnAttack1(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                Attack1 = true;
            if (ctx.canceled)
                Attack1 = false;
        }

        public void OnAttack2(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                Attack2 = true;
            if (ctx.canceled)
                Attack2 = false;
        }

        public void OnSpecial1(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                Special1 = true;
            if (ctx.canceled)
                Special1 = false;
        }

        public void OnSpecial2(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                Special2 = true;
            if (ctx.canceled)
                Special2 = false;
        }

        #endregion
    }

}