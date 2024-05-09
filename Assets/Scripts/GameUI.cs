namespace pacwall
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUI : MonoBehaviour
    {
        [SerializeField] GameObject gamePanel;
        [SerializeField] Button btnRestart;
        
        public event Action onRestart;

        void Start() {
            btnRestart.onClick.AddListener(() => onRestart?.Invoke());
        }

        public void Activate() {
            gamePanel.SetActive(true);
        }

        public void Deactivate() {
            gamePanel.SetActive(false);
        }
    }
}