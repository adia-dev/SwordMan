using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordMan.Controllers
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {

        public Animator Animator { get; private set; }

        [SerializeField] float _animSpeedSmooth = 0.1f;


        public readonly int VelocityAnimParam = Animator.StringToHash("velocity");
        public readonly int InputMagAnimParam = Animator.StringToHash("inputMag");
        public readonly int JumpAnimParam = Animator.StringToHash("jump");
        public readonly int DodgeAnimParam = Animator.StringToHash("dodge");
        public readonly int InputXAnimParam = Animator.StringToHash("inputX");
        public readonly int InputYAnimParam = Animator.StringToHash("inputY");
        public readonly int HorizontalAnimParam = Animator.StringToHash("horizontal");
        public readonly int VerticalAnimParam = Animator.StringToHash("vertical");
        public readonly int TargetLockedAnimParam = Animator.StringToHash("targetLocked");
        public readonly int IsMovingLeftAnimParam = Animator.StringToHash("isMovingLeft");

        public float AnimSpeedSmooth => _animSpeedSmooth;

        void Awake()
        {
            Animator = GetComponent<Animator>();
        }


    }

}