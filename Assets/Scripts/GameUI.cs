namespace pacwall
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameUI : MonoBehaviour
    {
        [SerializeField] GameObject gamePanel;
        [SerializeField] Button btnRestart;
        [SerializeField] Image progressFill;
        [SerializeField] TMP_Text msgtxt;
        
        public event Action onRestart;

        void Start() {
            btnRestart.onClick.AddListener(() => onRestart?.Invoke());
        }

        public void UpdateProgres(int n) {
            progressFill.fillAmount = n/100f;
        }

        public void Show(string msg) {
            msgtxt.text = msg;
            gamePanel.SetActive(true);
        }

        public void Deactivate() {
            gamePanel.SetActive(false);
        }
    }
}