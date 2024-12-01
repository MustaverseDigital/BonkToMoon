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
    public float MaxBallSpeed = 50;
    private Vector3 angle;
    private Vector3 newPosition, resetPos;
    private Rigidbody rb;
    public FixedJoint Joint;

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

        if (swipeTime < 0.75f && swipeDistance > 30f)
        {
            //throw ball
            CalSpeed();
            CalAngle();
            rb.AddForce(new Vector3((angle.x * BallSpeed), (angle.y * BallSpeed) * 2.5f, (-angle.y * BallSpeed) * 2));
            rb.useGravity = true;
            // Invoke("ResetBall", 2f);
        }
        else
            ResetBall();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Joint = gameObject.AddComponent<FixedJoint>();
            Joint.connectedBody = col.rigidbody;
        }
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
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, 80f * Time.deltaTime);
    }

    private void CalAngle()
    {
        angle = Camera.main.ScreenToWorldPoint(new Vector3(endPos.x, endPos.y + 100f, (Camera.main.nearClipPlane + 5)));
    }

    void CalSpeed()
    {
        if (swipeTime > 0)
            BallVelocity = swipeDistance / (swipeDistance - swipeTime);

        BallSpeed = BallVelocity * 40;

        if (BallSpeed <= MaxBallSpeed)
        {
            BallSpeed = MaxBallSpeed;
        }
        swipeTime = 0;
    }
}