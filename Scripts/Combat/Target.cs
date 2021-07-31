using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordMan.Behaviors
{
    public abstract class Target : MonoBehaviour
    {
        [SerializeField] Transform _top = null, _middle = null, _bottom = null;
        [SerializeField] GameObject _canvas = null;



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