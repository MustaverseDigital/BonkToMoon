using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BallSpawner spawner;
    public CameraFollow cameraFollow;
    public Slider slider;

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


}