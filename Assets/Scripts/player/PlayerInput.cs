namespace pacwall.player
{
    using System;
    using Unity.VisualScripting;
    using UnityEngine;
    using static Unity.Mathematics.math;

    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] ButtonCustomTrigger btnLeft, btnUp, btnRight, btnDown;
        [SerializeField] Joystick joystick;

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

            Vector2 jd = joystick.Direction;
            Vector2 ajd = abs(jd);
            if(ajd.x > 0.5 || ajd.y > 0.5) {
                if(ajd.x > ajd.y) {
                    if(jd.x > 0)
                        onRight?.Invoke();
                    else
                        onLeft?.Invoke();
                }
                else {
                    if(jd.y > 0)
                        onUp?.Invoke();
                    else
                        onDown?.Invoke();
                }
            }
        }
    }
}