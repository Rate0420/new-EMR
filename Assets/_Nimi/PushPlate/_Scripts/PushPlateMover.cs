using UnityEngine;
using EMR.Utility;
using EMR.Core;

namespace EMR.PushPlate
{
    [RequireComponent(typeof(Rigidbody))]
    public partial class PushPlateMover : MonoBehaviour, IPushPlateMover
    {
        [Header("Movement Settings")]
        [SerializeField] private MoveDirection _moveDirection = MoveDirection.PositiveZ;
        [SerializeField, Min(0f)] private float _moveDistance = 1.0f;
        [SerializeField, Min(0f)] private float _offset = 0.0f;
        [SerializeField, Min(0f)] private float _moveSpeed = 1.0f;
        [SerializeField, Min(0f)] private float _minPointWaitingTime = 0.3f;
        [SerializeField, Min(0f)] private float _maxPointWaitingTime = 0.3f;

        [SerializeField]
        private AnimationCurve _animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Debug")]
        [SerializeField] private bool _isActive = true;
        [SerializeField] private float _height = 0f;


        private Rigidbody _rigidbody;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private float _t;
        private int _direction = 1;


        /// <summary> 移動方向 </summary>
        public MoveDirection MoveDirection => _moveDirection;

        /// <summary> 移動量 </summary>
        public float MoveDistance => _moveDistance;

        /// <summary> 移動速度 </summary>
        public float MoveSpeed => _moveSpeed;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;

            CachePositions();
        }

        private void Start()
        {
            GameState.Instance.GamePause.OnPausedChange += PausedChenge;
            StartMoveLoop();
        }

        private void OnDisable()
        {
            GameState.Instance.GamePause.OnPausedChange -= PausedChenge;   
        }

        /// <summary>
        /// ポーズ中は動かないようにしたい
        /// </summary>
        private void PausedChenge()
        {
            if (GameState.Instance.GamePause.isPaused)
            {
                StartMoveLoop();
            }
            else
            {
                StopMoveLoop();
            }
        }


        /// <summary>
        /// 移動方向を変更する
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        public void SetMoveDirection(MoveDirection moveDirection)
        {
            _moveDirection = moveDirection;
        }

        /// <summary>
        /// 移動量を変更する
        /// </summary>
        /// <param name="moveDistance">移動量</param>
        public void SetMoveDistance(float moveDistance)
        {
            if (moveDistance < 0)
            {
                Debug.LogError($"'moveDistance'は0以上の値を代入してください。代入値:{moveDistance}", this);
                return;
            }

            _moveDistance = moveDistance;
        }

        /// <summary>
        /// 移動速度を変更する
        /// </summary>
        /// <param name="speed">移動速度</param>
        public void SetMoveSpeed(float moveSpeed)
        {
            if (moveSpeed < 0)
            {
                Debug.LogError($"'speed'の代入値は0以上にしてください。 代入値:{moveSpeed}", this);
                return;
            }

            _moveSpeed = moveSpeed;
        }
    }
}