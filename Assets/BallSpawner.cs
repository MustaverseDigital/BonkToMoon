using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public BonkBall ballPrefab;
    public Transform ballSpawnPoint;
    private BonkBall currentBall;
    private List<BonkBall> _ballList = new();

    void Start()
    {
        currentBall = Instantiate(ballPrefab);
        currentBall.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        Spawn();
    }

    private void Spawn()
    {
        currentBall = Instantiate(ballPrefab);
        currentBall.transform.position = ballSpawnPoint.position;
        currentBall.OnLinkBall += CalculateScore;
        _ballList.Add(currentBall);
    }

    private void CalculateScore()
    {
        var bonkBall = GetTopBall();
        var positionY = bonkBall.transform.position.y;
        var count = _ballList.Count;
        Debug.Log($"ball Count : {count} higthest point {positionY}");
    }

    public BonkBall GetTopBall()
    {
        // 檢查清單是否為空
        if (_ballList.Count == 0)
        {
            return null;
        }

        // 移除列表中為 null 的物件
        _ballList.RemoveAll(x => !x && !x.Joint);

        // 找到最高的那顆球
        var topBall = _ballList.OrderByDescending(ball => ball.transform.position.y).FirstOrDefault();

        // 返回最高的球
        return topBall;
    }
}