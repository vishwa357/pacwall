namespace pacwall.player {
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    class ButtonCustomTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        bool _isDown = false;
        public event Action onTrigger;

        void Update () {
            if(_isDown)
                onTrigger?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log(gameObject.name + " down");
            _isDown = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            Debug.Log(gameObject.name + " up");
            _isDown = false;
        }
    }
}