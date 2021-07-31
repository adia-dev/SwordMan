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
        [SerializeField] Color _detectionColor = Color.white;
        [SerializeField] Cinemachine.CinemachineImpulseSource _cinemachineImpulseSource;

        int _currentTargetIndex = 0;

        public List<Target> _targetsInRange = new List<Target>();
        public List<float> _targetsInRangeDot = new List<float>();
        public List<float> _targetsInRangeCross = new List<float>();
        public Target CurrentTarget => _currentTarget;

        PlayerController _playerController;
        AnimatorController _animatorController;
        [SerializeField] TMP_Text _nameText = null;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _animatorController = GetComponent<AnimatorController>();

            if (_targetsInRange.Count > 0)
                _currentTarget = _targetsInRange[0];

            if (_nameText != null)
                _nameText.text = gameObject.name;
        }

        // Update is called once per frame
        void Update()
        {
            if (_playerController == null)
            {
                return;
            }

            HandleTargets();

            if (_currentTarget == null) return;

            if (InputManager.Instance.Attack1)
            {
                Debug.Log("Attack 1");
                _animatorController.TriggerAttack2();
            }

            if (InputManager.Instance.Attack2)
            {
                _animatorController.TriggerAttack1();
                Debug.Log("Attack 2");
            }

            InputManager.Instance.Attack1 = false;
            InputManager.Instance.Attack2 = false;

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
                if (target != null && target != GetComponent<Target>())
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

            if (_targetsInRange.Count > 0 && !InputManager.Instance.TargetLocked)
            {
                int targetIndex = _targetsInRangeDot.IndexOf(_targetsInRangeDot.Max());
                _currentTarget = _targetsInRange[targetIndex];
            }
            else if (_targetsInRange.Count == 0)
            {
                _currentTarget = null;
            }
        }

        public bool SwitchTarget()
        {

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