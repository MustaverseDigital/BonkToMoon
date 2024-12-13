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
            if (Input.GetKey(KeyCode.W))
            {
                _currentHeight += moveSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _currentHeight -= moveSpeed * Time.deltaTime;
            }

            _currentHeight = Mathf.Clamp(_currentHeight, 1f, 200f);

            if (Input.GetKey(KeyCode.A))
            {
                _currentAngle -= rotationSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _currentAngle += rotationSpeed * Time.deltaTime;
            }

            float radians = _currentAngle * Mathf.Deg2Rad;
            float x = centerPoint.x + _radius * Mathf.Cos(radians);
            float z = centerPoint.z + _radius * Mathf.Sin(radians);

            transform.position = Vector3.Lerp(transform.position, new Vector3(x, _currentHeight + 3, z), Time.deltaTime / 2f);

            transform.LookAt(new Vector3(centerPoint.x, _currentHeight - 3, centerPoint.z));
        }

        public void RotateCamera(float value)
        {
            _currentAngle += rotationSpeed * -value * Time.deltaTime;
        }

        public void ModifyPositionY(float positionY)
        {
            if (positionY > _currentHeight)
            {
                _currentHeight = positionY;
            }
        }

        public void ResetCamera()
        {
            _currentAngle = 0f;
            _currentHeight = 5f;
        }
    }
}