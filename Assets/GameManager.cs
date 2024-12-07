using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    }

    public BallSpawner spawner;

    public void SpawnBall()
    {
        spawner.Spawn();
    }
}