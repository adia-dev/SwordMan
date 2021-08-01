using SwordMan.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordMan.Behaviors
{
    public class Weapon : MonoBehaviour
    {

        [SerializeField] Transform _owner = null;


        private void Awake()
        {
            if (_owner = null)
                _owner = FindObjectOfType<PlayerController>().transform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var target = collision.collider.GetComponent<Target>();

            if (target != null && collision.collider.name != "Player" && !target.IsDead)
            {
                Debug.Log("Touched : " + target.name);
                target.TakeDamage(450);
            }
        }

    }

}