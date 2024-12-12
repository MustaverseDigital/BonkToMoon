using System.Collections.Generic;
using System.Runtime.InteropServices;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BallSpawner spawner;
    public CameraFollow cameraFollow;
    public UIManager uiManager;
    public Slider slider;
    public float gameTime = 30f;


    private float _timer = 0f;
    
    public PlayerData playerData = new();
    private bool _inGame = false;
    private List<RankData> _rankData = new();
    

    // singleton pattern
    public static GameManager instance;
    
    public void SetUser(string address)
    {
        playerData.playerID = address;
    }

    public void SetLeaderBoardData(string dataString)
    {
        var json = "{\"data\":" + dataString + "}";

        var wrapper = JsonUtility.FromJson<BoardDataWrapper>(json);

        if (wrapper != null && wrapper.data != null)
        {
            foreach (var player in wrapper.data)
            {
                _rankData.Add(new RankData
                {
                    playerID = player.address,
                    score = player.score,
                    message = player.name
                });
            }
        }
        else
        {
            Debug.LogWarning($"Failed to parse leaderboard data. {json}");
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        SetupButton();
        MenuStart();
    }

    private void SetupButton()
    {
        uiManager.trialModeButton.onClick.AddListener(() => GameStart(false));
        uiManager.rankModeButton.onClick.AddListener(() => GameStart(true));
        uiManager.gameExitButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
        uiManager.rankBackButton.onClick.AddListener(() => { uiManager.OpenPanel("Complete"); });
        uiManager.completeExitButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
        uiManager.leaderBoardButton.onClick.AddListener(RefreshLeaderboard);
        uiManager.tryTrialButton.onClick.AddListener(() => GameStart(false));
        uiManager.tryRankButton.onClick.AddListener(() => GameStart(true));
        
    }

    private void MenuStart()
    {
        uiManager.OpenPanel("Menu");
    }

    private void RefreshLeaderboard()
    {
        //todo TestData need to change to real one 
        uiManager.SetupLeaderBoard(new List<RankData>
        {
            new()
            {
                playerID = "1",
                score = 1000,
            },
            new()
            {
                playerID = "2",
                score = 500,
            },
            new()
            {
                playerID = "3",
                score = 100,
            },
        });
        uiManager.OpenPanel("Ranking");
    }

    private void GameStart(bool ranking)
    {
        if (ranking)
        {
            SendMessageToReact("StartRank");
        }
        
        uiManager.OpenPanel("Game");
        spawner.onBallLink += CalculateScore;
        _timer = gameTime;
        spawner.Spawn();
        _inGame = true;
    }


    public void SpawnBall()
    {
        spawner.Spawn();
    }

    public void ResetSlider()
    {
        slider.value = 0.5f;
    }

    public void OnSliderTouchDown()
    {
        cameraFollow.RotateCamera(slider.value - 0.5f);
    }

    private void CalculateScore()
    {
        var bonkBall = spawner.GetTopBall();
        var positionY = bonkBall.transform.position.y + 5;
        var count = spawner.ballList.Count;
        var baseScore = count * positionY;
        var timeFactor = gameTime - _timer;
        var finalScore = baseScore * timeFactor;
        uiManager.scoreText.text = $"{finalScore:0}";
        playerData.score = (int) finalScore;
        cameraFollow.ModifyPositionY(bonkBall.transform.position.y);
    }

    private void Update()
    {
        if (!_inGame) return;
        _timer -= Time.deltaTime;
        uiManager.SetGameTime(_timer);
        if (_timer <= 0f)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        spawner.onBallLink -= CalculateScore;
        spawner.ResetAllBall();
        uiManager.SetCompletePanelData(playerData);
        uiManager.OpenPanel("Complete");
        _inGame = false;

        var boardData = new PlayerBoardData();
        boardData.player = playerData.playerID;
        boardData.score = playerData.score;
        var jsonData = JsonUtility.ToJson(boardData);
        SendMessageToReact($"EndGame{jsonData}");
    }

    [DllImport("__Internal")]
    private static extern void SendMessageToReact(string message);
    
}
[System.Serializable]
public class PlayerData
{
    public string playerID = "1231...0X34";
    public int score;
    public Sprite sprite;
    public int rank;
}
[System.Serializable]
public class PlayerBoardData
{
    public string player;
    public int score;
}


[System.Serializable]
public class BoardData
{
    public string name;
    public int score;
    public string address;
}

[System.Serializable]
public class BoardDataWrapper
{
    public BoardData[] data;
}