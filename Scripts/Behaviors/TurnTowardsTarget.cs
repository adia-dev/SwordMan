using SwordMan.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordMan.Behaviors
{
    public class TurnTowardsTarget : MonoBehaviour
    {

        [SerializeField] Transform _target = null;
        [SerializeField] bool _targetPlayer = false;
        [SerializeField] bool _invert = false;

        void Awake()
        {
            if (_targetPlayer)
            {
                _target = FindObjectOfType<PlayerController>().transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_target != null)
            {
                Vector3 targetDirection;

                if (_invert)
                    targetDirection = (transform.position - _target.transform.position);
                else
                    targetDirection = (_target.transform.position - transform.position);

                targetDirection.y = 0f;
                targetDirection.Normalize();

                transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }
    }

}