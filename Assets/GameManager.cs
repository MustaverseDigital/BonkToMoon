using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BallSpawner spawner;
    public CameraFollow cameraFollow;
    public UIManager uiManager;
    public Slider slider;
    public float gameTime = 30f;


    private float _timer = 0f;
    
    private PlayerData _playerData = new();
    private bool _inGame = false;
    

    // singleton pattern
    public static GameManager instance;

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
    //todo need real player data link
    public void SetupPlayerData()
    {
        
    }

    private void SetupButton()
    {
        uiManager.trialModeButton.onClick.AddListener(GameStart);
        //todo cost bonk coin
        uiManager.rankModeButton.onClick.AddListener(GameStart);
        uiManager.gameExitButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
        uiManager.rankBackButton.onClick.AddListener(() => { uiManager.OpenPanel("Complete"); });
        uiManager.completeExitButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
        uiManager.leaderBoardButton.onClick.AddListener(RefreshLeaderboard);
        uiManager.tryTrialButton.onClick.AddListener(GameStart);
        //todo cost bonk coin
        uiManager.tryRankButton.onClick.AddListener(GameStart);
        
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

    private void GameStart()
    {
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
        _playerData.score = (int) finalScore;
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
        uiManager.SetCompletePanelData(_playerData);
        uiManager.OpenPanel("Complete");
        _inGame = false;
    }
}
[System.Serializable]
public class PlayerData
{
    public string playerID = "1231...0X34";
    public int score;
    public Sprite sprite;
    public int rank;
}