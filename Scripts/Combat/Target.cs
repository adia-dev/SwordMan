using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordMan.Behaviors
{
    public abstract class Target : MonoBehaviour
    {
        [SerializeField] Transform _top = null, _middle = null, _bottom = null;
        [SerializeField] GameObject _canvas = null;
        [SerializeField] public Image _healthBar = null, _energyBar = null;
        public int MaxHealth = 1500;
        public int CurrentHealth;

        public bool IsDead => CurrentHealth <= 0;

        void Start()
        {
            CurrentHealth = MaxHealth;
            if (_healthBar != null)
                _healthBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp((float)CurrentHealth / MaxHealth, 0f, 0.95f), 0.03f);
        }


        public abstract void TakeDamage(int amount);
        public abstract void Heal(int amount);


        public void ToggleCanvas(bool state)
        {
            _canvas.SetActive(state);
        }

        public Transform GetTargetPoint()
        {
            if (_top != null)
                return _top;
            if (_middle != null)
                return _middle;
            if (_bottom != null)
                return _bottom;

            return transform;
        }

        public Transform GetTopAim() => _top != null ? _top : transform;
        public Transform GetMiddleAim() => _middle != null ? _middle : transform;
        public Transform GetBottomAim() => _bottom != null ? _bottom : transform;
    }

}