using System;
using System.Collections.Generic;
using System.Globalization;
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
        TestLeaderboard();
    }

    private void SetupButton()
    {
        uiManager.trialModeButton.onClick.AddListener(GameStart);
        //need to cost bonk coin
        uiManager.rankModeButton.onClick.AddListener(GameStart);
        uiManager.gameExitButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
        uiManager.rankBackButton.onClick.AddListener(() => { uiManager.OpenPanel("Menu"); });
    }

    private void MenuStart()
    {
        uiManager.OpenPanel("Menu");
    }

    private void TestLeaderboard()
    {
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
    }

    private void GameStart()
    {
        uiManager.OpenPanel("Game");
        spawner.onBallLink += CalculateScore;
        _timer = gameTime;
        spawner.Spawn();
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
        cameraFollow.ModifyPositionY(bonkBall.transform.position.y);
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        uiManager.SetGameTime(_timer);
        if (_timer <= 0f)
        {
            GameOver();
            _timer = 0;
        }
    }

    private void GameOver()
    {
        //spawner.onBallLink -= CalculateScore;
    }
}