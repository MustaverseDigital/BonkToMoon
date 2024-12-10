using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LeaderBoardItem : MonoBehaviour
    {
        public TMP_Text scoreText;
        public Image avatarImage;
        public TMP_Text walletIDText;
        public TMP_Text messageText;
        public Image leaderBoardImage;

        public void SetupBoardItem(RankData data)
        {
            scoreText.text = data.score.ToString();
            if (data.avatar != null)
            {
                avatarImage.sprite = data.avatar;
            }
            walletIDText.text = data.playerID;
            messageText.text = data.message ?? "Bonk to Moon!!!!";
        }
    }

    [System.Serializable]
    public class RankData
    {
        public string playerID;
        public int score;
        public Sprite avatar;
        public string message;
    }
}