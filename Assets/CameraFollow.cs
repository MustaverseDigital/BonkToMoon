using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class CameraFollow : MonoBehaviour
    {
        private BallSpawner _ballSpawner;

        [SerializeField] private float rotationSpeed = 50f; // 左右旋轉速度
        [SerializeField] private float moveSpeed = 5f; // 高度控制速度
        [SerializeField] private Vector3 centerPoint = Vector3.zero; // 圓心位置 (默認為 (0, 0, 0))

        private float _currentAngle = 0f; // 當前的旋轉角度
        private float _currentHeight; // 當前的相機高度
        private float _radius; // 旋轉半徑

        private void Start()
        {
            _ballSpawner = FindObjectOfType<BallSpawner>();

            // 根據初始位置計算半徑
            Vector3 offset = transform.position - centerPoint;
            _radius = new Vector2(offset.x, offset.z).magnitude;

            // 初始化當前高度為相機的初始高度
            _currentHeight = transform.position.y;

            // 初始化角度：計算初始位置與圓心的角度
            _currentAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;

            Debug.Log($"Initial radius: {_radius}, Initial height: {_currentHeight}");
        }

        private void LateUpdate()
        {
            // 控制高度 (W/S)
            if (Input.GetKey(KeyCode.W))
            {
                _currentHeight += moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _currentHeight -= moveSpeed * Time.deltaTime;
            }

            // 限制高度範圍
            _currentHeight = Mathf.Clamp(_currentHeight, 1f, 20f);

            // 控制旋轉 (A/D)
            if (Input.GetKey(KeyCode.A))
            {
                _currentAngle -= rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _currentAngle += rotationSpeed * Time.deltaTime;
            }

            // 計算相機的位置
            float radians = _currentAngle * Mathf.Deg2Rad;
            float x = centerPoint.x + _radius * Mathf.Cos(radians);
            float z = centerPoint.z + _radius * Mathf.Sin(radians);

            // 設定相機位置
            transform.position = new Vector3(x, _currentHeight, z);

            // 設定相機始終面向中心點
            transform.LookAt(new Vector3(centerPoint.x, _currentHeight, centerPoint.z));
        }

        public void RotateCamera(float value)
        {
            _currentAngle += rotationSpeed * value * Time.deltaTime;
        }
    }
}