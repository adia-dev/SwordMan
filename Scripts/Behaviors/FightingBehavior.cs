using SwordMan.Controllers;
using SwordMan.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SwordMan.Behaviors
{
    public class FightingBehavior : Target
    {

        [SerializeField] Target _currentTarget = null;
        [SerializeField] float _detectionRadius = 5f;
        [SerializeField] LayerMask _targetMask = -1;
        [SerializeField] Cinemachine.CinemachineImpulseSource _cinemachineImpulseSource;
        [SerializeField] BoxCollider _weaponCollider = null;

        int _currentTargetIndex = 0;
        bool _timeAltered = false;

        List<Target> _targetsInRange = new List<Target>();
        List<float> _targetsInRangeDot = new List<float>();
        List<float> _targetsInRangeCross = new List<float>();

        public Target CurrentTarget => _currentTarget;
        public bool TargetLocked { get; private set; }

        PlayerController _playerController;
        AnimatorController _animatorController;
        Animator _animator;
        [SerializeField] TMP_Text _nameText = null;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _animatorController = GetComponent<AnimatorController>();
            _animator = GetComponent<Animator>();

            if (_targetsInRange.Count > 0)
                _currentTarget = _targetsInRange[0];

            if (_nameText != null)
                _nameText.text = gameObject.name;
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerController == null || IsDead)
            {
                return;
            }

            HandleTargets();

            if (_currentTarget == null)
            {
                TargetLocked = false;
                return;
            }
            else if (InputManager.Instance.LockTarget)
            {
                TargetLocked = !TargetLocked;
                InputManager.Instance.LockTarget = false;
            }

            if (InputManager.Instance.Attack1)
            {
                _animatorController.TriggerAttack1();
            }

            if (InputManager.Instance.Attack2)
            {
                _animatorController.TriggerAttack2();
            }

            if (InputManager.Instance.Special1)
            {
                _animatorController.TriggerSpecial1();
            }

            if (InputManager.Instance.Special2)
            {
                _animatorController.TriggerSpecial2();
            }

            InputManager.Instance.Attack1 = false;
            InputManager.Instance.Attack2 = false;
            InputManager.Instance.Special1 = false;
            InputManager.Instance.Special2 = false;

        }

        public override void TakeDamage(int amount)
        {
            if (IsDead) return;

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
            if (_healthBar != null)
                _healthBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp((float)CurrentHealth / MaxHealth, 0f, 0.95f), 0.03f);

            _cinemachineImpulseSource.GenerateImpulse();
            StartCoroutine(HitStop(0.1f));

            if (IsDead)
            {
                _animator.SetTrigger("die");
                _currentTarget = null;
            }
            else
            {
                _animator.SetTrigger("hit");
            }
        }

        public override void Heal(int amount)
        {

            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            if (_healthBar != null)
                _healthBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp((float)CurrentHealth / MaxHealth, 0f, 0.95f), 0.03f);
        }

        void HandleTargets()
        {
            _targetsInRange.Clear();
            _targetsInRangeDot.Clear();
            _targetsInRangeCross.Clear();

            var cols = Physics.OverlapSphere(transform.position, _detectionRadius, _targetMask);

            foreach (var col in cols)
            {
                Target target = col.GetComponent<Target>();
                if (target != null && target != GetComponent<Target>() && !target.IsDead)
                {
                    if (!_targetsInRange.Contains(target))
                    {
                        _targetsInRange.Add(target);
                        Vector3 targetDirection = (target.transform.position - transform.position);
                        targetDirection.y = 0f;
                        targetDirection.Normalize();
                        _targetsInRangeDot.Add(Vector3.Dot(transform.forward, targetDirection));
                        _targetsInRangeCross.Add(Vector3.Cross(transform.forward, targetDirection).y);
                    }
                }
            }

            if (_targetsInRange.Count > 0 && !TargetLocked)
            {
                int targetIndex = _targetsInRangeDot.IndexOf(_targetsInRangeDot.Max());
                _currentTarget = _targetsInRange[targetIndex];
                _cinemachineImpulseSource.GenerateImpulse();
            }
            else if (_targetsInRange.Count == 0)
            {
                _currentTarget = null;
            }
        }


        IEnumerator HitStop(float hitStopTime)
        {
            if (!_timeAltered)
            {
                _timeAltered = true;

                float timer = 0f;
                Time.timeScale = 0f;
                while (timer <= hitStopTime)
                {
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                Time.timeScale = 1f;
                _timeAltered = false;
                yield return null;
            }

        }

        //Events
        public bool SwitchTarget()
        {
            if (_currentTarget == null || !TargetLocked) return false;

            if (_targetsInRange.Count == 0)
            {
                _currentTarget = null;
                return false;
            }

            _currentTargetIndex = (_currentTargetIndex + 1) % _targetsInRange.Count;
            _currentTarget = _targetsInRange[_currentTargetIndex];
            _cinemachineImpulseSource.GenerateImpulse();

            return true;
        }

        public bool SwitchTargetRight()
        {

            if (_currentTarget == null || !TargetLocked) return false;

            float minCrossY = 1f;
            int targetIndex = -1;

            for (int i = 0; i < _targetsInRange.Count; i++)
            {
                if (_targetsInRange[i] == _currentTarget) continue;
                if (_targetsInRangeCross[i] < 0f) continue;

                if (_targetsInRangeCross[i] < minCrossY)
                {
                    minCrossY = _targetsInRangeCross[i];
                    targetIndex = i;
                }
            }

            if (targetIndex != -1)
            {
                _currentTargetIndex = targetIndex;
                _currentTarget = _targetsInRange[targetIndex];
                _cinemachineImpulseSource.GenerateImpulse();
                return true;
            }

            return false;
        }

        public bool SwitchTargetLeft()
        {
            if (_currentTarget == null || !TargetLocked) return false;

            float maxCrossY = -1f;
            int targetIndex = -1;

            for (int i = 0; i < _targetsInRange.Count; i++)
            {
                if (_targetsInRange[i] == _currentTarget) continue;
                if (_targetsInRangeCross[i] > 0f) continue;

                if (_targetsInRangeCross[i] > maxCrossY)
                {
                    maxCrossY = _targetsInRangeCross[i];
                    targetIndex = i;
                }
            }

            if (targetIndex != -1)
            {
                _currentTargetIndex = targetIndex;
                _currentTarget = _targetsInRange[targetIndex];
                _cinemachineImpulseSource.GenerateImpulse();
                return true;
            }

            return false;
        }

        public void EnableWeaponCollider()
        {
            if (_weaponCollider != null)
                _weaponCollider.enabled = true;
        }

        public void DisableWeaponCollider()
        {
            if (_weaponCollider != null)
                _weaponCollider.enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _targetsInRange.Count > 0 ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            foreach (var target in _targetsInRange)
            {
                if (target == _currentTarget)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
    }

}