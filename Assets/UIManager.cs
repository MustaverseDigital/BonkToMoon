using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        public TMP_Text scoreText;
        public TMP_Text gameTimeText;
        public Image gameTimeImage;
        public Button exitButton;


        public RectTransform menuPanel;
        public RectTransform gamePanel;
        public RectTransform enterPanel;
        public RectTransform rankingPanel;
        public RectTransform completePanel;


        public void OpenPanel(string panelName)
        {
            // 在顯示特定面板前，先關閉所有面板
            menuPanel.gameObject.SetActive(false);
            gamePanel.gameObject.SetActive(false);
            enterPanel.gameObject.SetActive(false);
            rankingPanel.gameObject.SetActive(false);
            completePanel.gameObject.SetActive(false);

            // 根據傳入的 panelName 開啟對應面板
            switch (panelName)
            {
                case "Menu":
                    menuPanel.gameObject.SetActive(true);
                    break;
                case "Game":
                    gamePanel.gameObject.SetActive(true);
                    break;
                case "Enter":
                    enterPanel.gameObject.SetActive(true);
                    break;
                case "Ranking":
                    rankingPanel.gameObject.SetActive(true);
                    break;
                case "Complete":
                    completePanel.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogWarning($"Panel '{panelName}' not recognized.");
                    break;
            }
        }

        public void SetGameTime(float time)
        {
            gameTimeText.text = $"{time:0.0}";
            gameTimeImage.fillAmount = time / 30f;
        }
        
    }
}