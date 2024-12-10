using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    private BonkBall currentBall;
    public List<BonkBall> ballList = new();

    public Action onBallLink;

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        var ballObj = Instantiate(ballPrefab);
        ballObj.transform.position = ballSpawnPoint.position;
        ballObj.transform.parent = ballSpawnPoint;
        ballObj.transform.localRotation = Quaternion.identity;
        currentBall = ballObj.GetComponentInChildren<BonkBall>();
        currentBall.OnLinkBall += OnLinkBall;
        ballList.Add(currentBall);
    }

    private void OnLinkBall()
    {
        onBallLink?.Invoke();
        currentBall.OnLinkBall -= OnLinkBall;
    }


    public BonkBall GetTopBall()
    {
        // 檢查清單是否為空
        if (ballList.Count == 0)
        {
            return null;
        }

        // 移除列表中為 null 的物件
        ballList.RemoveAll(x => !x && !x.Joint);

        // 找到最高的那顆球
        var topBall = ballList.OrderByDescending(ball => ball.transform.position.y).FirstOrDefault();

        // 返回最高的球
        return topBall;
    }
}