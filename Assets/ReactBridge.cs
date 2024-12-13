using UnityEngine;

namespace DefaultNamespace
{
    public class ReactBridge : MonoBehaviour
    {
        public void sendAddress(string address) 
        {
            GameManager.instance.SetUser(address);
        }

        public void sendLeaderboard(string dataString)
        {
            GameManager.instance.SetLeaderBoardData(dataString);
        }

        public void startGameRank()
        {
            GameManager.instance.ContinueStartRank();
        }
    }
}