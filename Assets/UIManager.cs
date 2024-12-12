using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class UIManager : MonoBehaviour
    {
        [Header("Menu Panel")] public Button trialModeButton;
        public Button rankModeButton;

        [Header("Game Panel")] public TMP_Text scoreText;
        public TMP_Text gameTimeText;
        public Image gameTimeImage;
        public Button gameExitButton;

        [Header("Rank Panel")] public Button rankBackButton;
        public LeaderBoardItem itemPrefab;
        public VerticalLayoutGroup itemGroup;
        public Sprite rank1, rank2, rank3;

        [Header("complete Panel")] public TMP_Text playerIDText;
        public Image avatarImage;
        public TMP_Text scoreResultText;
        public TMP_Text rankText;
        public Button leaderBoardButton;
        public Button tryTrialButton;
        public Button tryRankButton;
        public Button completeExitButton;

        [Header("Game Panel")] public RectTransform menuPanel;
        public RectTransform gamePanel;
        public RectTransform enterPanel;
        public RectTransform rankingPanel;
        public RectTransform completePanel;

        public void SetupLeaderBoard(List<RankData> rankingData)
        {
            //destory all child in itemGroup
            foreach (Transform child in itemGroup.transform)
            {
                Destroy(child.gameObject);
            }

            
            Sprite[] rankSprites = { rank1, rank2, rank3 };
            //re add all data 
            for (var index = 0; index < rankingData.Count; index++)
            {
                var data = rankingData[index];
                var leaderBoardItem = Instantiate(itemPrefab, itemGroup.transform);
                leaderBoardItem.SetupBoardItem(data);
                if (index < rankSprites.Length)
                {
                    leaderBoardItem.leaderBoardImage.sprite = rankSprites[index];
                }
            }
        }

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

        public void SetCompletePanelData(PlayerData data)
        {
            var playerID = data.playerID;
            var shortenedID = playerID[..4] + "..." + playerID[^4..];
            playerIDText.text = shortenedID;
            if (data.sprite != null)
            {
                avatarImage.sprite = data.sprite;
            }
            scoreResultText.text = data.score.ToString();
            rankText.text = data.rank < 0 ? "UnRanked" : data.rank.ToString();
        }
    }
}