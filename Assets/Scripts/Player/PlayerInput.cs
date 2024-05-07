namespace pacwall.player
{
    using System;
    using UnityEngine;

    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] ButtonCustomTrigger btnLeft, btnUp, btnRight, btnDown;

        public event Action onLeft, onUp, onRight, onDown;

        void Start() {
            btnLeft.onTrigger += () => onLeft?.Invoke();
            btnUp.onTrigger += () => onUp?.Invoke();
            btnRight.onTrigger += () => onRight?.Invoke();
            btnDown.onTrigger += () => onDown?.Invoke();
        }

        void Update() {
            if(Input.GetKey(KeyCode.LeftArrow))
                onLeft?.Invoke();
            if(Input.GetKey(KeyCode.UpArrow))
                onUp?.Invoke();
            if(Input.GetKey(KeyCode.RightArrow))
                onRight?.Invoke();
            if(Input.GetKey(KeyCode.DownArrow))
                onDown?.Invoke();
        }
    }
}