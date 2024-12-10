using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonkBall : MonoBehaviour
{
    float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos;
    private Vector2 endPos;
    public float LaunchPoint = 15;
    public float MinSwipDist = 0;
    private float BallVelocity = 0;
    private float BallSpeed = 0;
    public float MaxBallSpeed = 1;
    private Vector3 angle;
    private Vector3 newPosition, resetPos;
    private Rigidbody rb;
    public FixedJoint Joint;

    public Action OnLinkBall;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        resetPos = transform.position;
        ResetBall();
    }

    private void OnMouseDown()
    {
        startTime = Time.time;
        startPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        PickupBall();
    }

    private void OnMouseUp()
    {
        endTime = Time.time;
        endPos = Input.mousePosition;
        swipeDistance = (endPos - startPos).magnitude;
        swipeTime = endTime - startTime;
        if (swipeTime > 0.75f && swipeDistance > MinSwipDist)
        {
            // 拋球
            CalSpeed();
            Shoot();
            Invoke("NextBall", 2f);
        }
        else
            ResetBall();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (Joint == null)
            {
                Joint = gameObject.AddComponent<FixedJoint>();
                OnLinkBall?.Invoke();
            }

            Joint.connectedBody = col.rigidbody;
        }

        if (col.gameObject.CompareTag("Ground"))
        {
            Destroy(transform.root.gameObject, 2);
        }
    }

    void NextBall()
    {
        GameManager.instance.SpawnBall();
    }

    void ResetBall()
    {
        angle = Vector3.zero;
        endPos = Vector2.zero;
        startPos = Vector2.zero;
        BallSpeed = 0;
        startTime = 0;
        endTime = 0;
        swipeDistance = 0;
        swipeTime = 0;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        transform.position = resetPos;
    }

    void PickupBall()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane * LaunchPoint;
        newPosition = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = Vector3.Lerp(transform.position, newPosition, 80f * Time.deltaTime);
    }

    private void Shoot()
    {
        transform.parent = null;
        Vector2 swipeVector = endPos - startPos;
        var power = swipeVector.y / 100f;
        var launchVector = (Camera.main.transform.forward + new Vector3(0, 1, 0)) * power;
        // 使用攝影機方向和計算出的速度
        rb.AddForce(launchVector * BallSpeed, ForceMode.Impulse);
        rb.AddTorque(launchVector * (BallSpeed / 2), ForceMode.Impulse);
        rb.useGravity = true;
    }

    void CalSpeed()
    {
        if (swipeTime > 0)
            BallVelocity = swipeDistance / (swipeDistance - swipeTime);

        BallSpeed = BallVelocity;

        if (BallSpeed <= MaxBallSpeed)
        {
            BallSpeed = MaxBallSpeed;
        }

        swipeTime = 0;
    }
}