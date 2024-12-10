using System;
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

        GameStart();
    }

    private void GameStart()
    {
        spawner.onBallLink += CalculateScore;
        _timer = gameTime;
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