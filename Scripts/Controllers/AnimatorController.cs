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
        public readonly int JumpAnimParam = Animator.StringToHash("jump");
        public readonly int DodgeAnimParam = Animator.StringToHash("dodge");



        public readonly int InputMagAnimParam = Animator.StringToHash("inputMag");
        public readonly int InputXAnimParam = Animator.StringToHash("inputX");
        public readonly int InputYAnimParam = Animator.StringToHash("inputY");
        public readonly int HorizontalAnimParam = Animator.StringToHash("horizontal");
        public readonly int VerticalAnimParam = Animator.StringToHash("vertical");

        public readonly int IsMovingLeftAnimParam = Animator.StringToHash("isMovingLeft");
        public readonly int TargetLockedAnimParam = Animator.StringToHash("targetLocked");

        public readonly int Attack1AnimParam = Animator.StringToHash("attack1");
        public readonly int Attack2AnimParam = Animator.StringToHash("attack2");
        public readonly int Special1AnimParam = Animator.StringToHash("special1");
        public readonly int Special2AnimParam = Animator.StringToHash("special2");

        public readonly int DieAnimParam = Animator.StringToHash("die");
        public readonly int HitAnimParam = Animator.StringToHash("hit");
        public float AnimSpeedSmooth => _animSpeedSmooth;

        void Awake()
        {
            Animator = GetComponent<Animator>();
        }


        void Update()
        {

        }

        public void TriggerAttack1() => Animator.SetTrigger(Attack1AnimParam);
        public void TriggerAttack2() => Animator.SetTrigger(Attack2AnimParam);
        public void TriggerSpecial1() => Animator.SetTrigger(Special1AnimParam);
        public void TriggerSpecial2() => Animator.SetTrigger(Special2AnimParam);
        public void Die() => Animator.SetTrigger(DieAnimParam);
        public void Hit() => Animator.SetTrigger(HitAnimParam);

    }

}