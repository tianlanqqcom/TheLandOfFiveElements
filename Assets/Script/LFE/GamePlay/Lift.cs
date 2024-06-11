using System.Collections;
using UnityEngine;

namespace Script.LFE.GamePlay
{
    public class Lift : MonoBehaviour
    {
        public Transform pointA; // 电梯的起始位置
        public Transform pointB; // 电梯的终止位置
        public float speed = 1.0f; // 电梯移动的速度
        public float waitTime = 2.0f; // 电梯在每个位置等待的时间
        public AudioClip liftArrive;

        private Vector3 _targetPosition; // 目标位置
        private bool _movingToB = true; // 当前移动方向
        private AudioSource _audioSource;


        private void Start()
        {
            // 初始化目标位置为pointB
            _targetPosition = pointB.position;
            _audioSource = gameObject.GetComponent<AudioSource>();
            // 开始移动协程
            StartCoroutine(MoveElevator());
        }

        private IEnumerator MoveElevator()
        {
            while (true)
            {
                // 移动电梯
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);

                // 检查是否到达目标位置
                if (transform.position == _targetPosition)
                {
                    // Play audio
                    if (liftArrive)
                    {
                        _audioSource?.PlayOneShot(liftArrive);
                    }
                    
                    // 等待指定的时间
                    yield return new WaitForSeconds(waitTime);

                    // 切换目标位置
                    _targetPosition = _movingToB ? pointA.position : pointB.position;

                    // 切换移动方向
                    _movingToB = !_movingToB;
                }

                // 等待下一帧
                yield return null;
            }
        }
    }
}