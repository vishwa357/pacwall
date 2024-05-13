namespace pacwall
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// For in live progress ui and game over panel.
    /// </summary>
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

        /// <summary>
        /// Update progress of the game.
        /// </summary>
        /// <param name="n"></param>
        public void UpdateProgres(int n) {
            progressFill.fillAmount = n/100f;
        }

        /// <summary>
        /// Show gameover panel with a message
        /// </summary>
        /// <param name="msg"></param>
        public void Show(string msg) {
            msgtxt.text = msg;
            gamePanel.SetActive(true);
        }

        /// <summary>
        /// Hide gameover panel ()
        /// </summary>
        public void Deactivate() {
            gamePanel.SetActive(false);
        }
    }
}