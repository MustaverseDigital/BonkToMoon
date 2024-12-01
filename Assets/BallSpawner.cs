using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public BonkBall ballPrefab;
    private BonkBall currentBall;

    void Start()
    {
        currentBall = Instantiate(ballPrefab);
        currentBall.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BonkBall ball = Instantiate(ballPrefab);
            currentBall = ball;
            ball.transform.position = transform.position;
        }
    }
}
