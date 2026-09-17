using UnityEngine;
using UnityEngine.Serialization;

using EMR.Core;

namespace EMR.Medal.Launch
{
    /// <summary>
    /// メダル発射の実行を担当するコンポーネント。
    /// 入力の取得、発射位置の決定、Prefab生成、Rigidbodyへの初速反映を行う。
    /// </summary>
    public partial class MedalLaunch : MonoBehaviour, IMedalLaunch
    {
        //発射時に生成するメダルPrefabのRigidbody。
        [Header("メダルの設定")]
        [SerializeField, FormerlySerializedAs("_medalPrefab")]
        private Rigidbody _medalRigidbodyPrefab;

        // 生成したメダルを子にするTransform。
        [SerializeField]
        private Transform _medalParentTransform;

        // マウス入力位置をスクリーン座標に変換するためのカメラ。
        [Header("入力の設定")]
        [SerializeField, FormerlySerializedAs("_inputCamera")]
        private Camera _mouseInputCamera;

        // 発射位置、発射範囲、生成時の回転の基準になるTransform。
        /// </summary>
        [Header("発射位置の設定")]
        [SerializeField, FormerlySerializedAs("_launchTransform")]
        private Transform _launchReferenceTransform;

        // 発射基準Transformから見た発射位置全体のローカルオフセット。
        [SerializeField, FormerlySerializedAs("_launchOffset")]
        private Vector3 _launchLocalOffset;

        // 発射範囲と生成回転に加えるローカル回転オフセット。
        [SerializeField, FormerlySerializedAs("_launchRotation")]
        private Vector3 _launchRotationOffset;

        // 発射してから着地点に到達するまでの秒数。
        [Header("着地時間の設定")]
        [SerializeField, Min(MedalLaunchMath.MinimumTimeToLanding)]
        private float _timeToLanding = 1f;

        // 発射可能範囲の開始位置。
        [Header("発射位置の制限")]
        [SerializeField, FormerlySerializedAs("_positionA")]
        private Vector3 _launchRangeStartLocalPosition = Vector3.left * 45f;

        // 発射可能範囲の終了位置。
        [SerializeField, FormerlySerializedAs("_positionB")]
        private Vector3 _launchRangeEndLocalPosition = Vector3.right * 45f;

        // マウス入力範囲全体に加えるワールド座標のオフセット。
        [Header("マウス入力範囲")]
        [SerializeField, FormerlySerializedAs("_offset")]
        private Vector3 _mouseInputWorldOffset;

        // カメラから見たマウス入力範囲の左端。
        [SerializeField, FormerlySerializedAs("_leftMaxPosition")]
        private Vector3 _mouseInputLeftWorldPosition = Vector3.right * -45f;

        // カメラから見たマウス入力範囲の右端。
        [SerializeField, FormerlySerializedAs("_rightMaxPosition")]
        private Vector3 _mouseInputRightWorldPosition = Vector3.right * 45f;

        // 選択中に発射範囲、入力範囲、弾道プレビューのGizmosを表示するか。
        [Header("デバッグ設定")]
        [SerializeField, FormerlySerializedAs("_isDrawGizmos")]
        private bool _drawGizmos = true;

        /// <summary>
        /// 現在設定されている発射Prefab。
        /// 外部からPrefab設定を確認するために公開する。
        /// </summary>
        public Rigidbody LaunchPrefab => _medalRigidbodyPrefab;

        /// <summary>
        /// 現在設定されている着地時間。
        /// 外部から発射パラメータを確認するために公開する。
        /// </summary>
        public float TimeToLanding => _timeToLanding;

        /// <summary>
        /// 計算に使う安全な着地時間。
        /// Inspector値が0以下になっても最小値以上に丸める。
        /// </summary>
        private float SafeTimeToLanding => MedalLaunchMath.GetSafeTime(_timeToLanding);

        /// <summary>
        /// コンポーネント追加時に呼ばれる。
        /// 初期状態では自身のTransformを発射基準にする。
        /// </summary>
        private void Reset()
        {
            _launchReferenceTransform = transform;
        }

        /// <summary>
        /// 実行開始時に呼ばれる。
        /// 発射基準Transformが未設定なら自身のTransformを補完する。
        /// </summary>
        private void Awake()
        {
            if (!_launchReferenceTransform)
            {
                _launchReferenceTransform = transform;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる。
        /// 左クリックされた瞬間にメダルを発射する。
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (GameState.Instance.OwnedModel.Count > 0 && !GameState.Instance.GamePause.isPaused)
                {
                    Launch();
                    GameState.Instance.OwnedModel.RemoveMedal();
                    GameState.Instance.RoundService.ConsumeMedals(1);
                }
            }
        }

        /// <summary>
        /// メダルPrefabを生成し、着地点に向かう初速をRigidbodyへ設定する。
        /// </summary>
        public void Launch()
        {
            if (!_medalRigidbodyPrefab)
            {
                Debug.LogWarning($"{nameof(MedalLaunch)}: Medal Prefab is not assigned.", this);
                return;
            }

            // クリック位置から、今回の発射に必要な位置、回転、初速をまとめて計算する。
            var shot = CreateLaunchShot(Input.mousePosition);

            // 計算済みの発射位置と回転でメダルを生成する。
            var medal = Instantiate(
                _medalRigidbodyPrefab,
                shot.Position,
                shot.Rotation,
                _medalParentTransform);

            // Unityのバージョン差を吸収しながらRigidbodyへ初速を反映する。
            SetRigidbodyVelocity(medal, shot.InitialVelocity);
        }

        /// <summary>
        /// 発射するメダルPrefabを外部から差し替える。
        /// </summary>
        /// <param name="prefab">新しく使用するメダルPrefabのRigidbody。</param>
        public void SetLaunchPrefab(Rigidbody prefab)
        {
            _medalRigidbodyPrefab = prefab;
        }

        /// <summary>
        /// 着地点に到達するまでの時間を外部から設定する。
        /// </summary>
        /// <param name="timeToLanding">着地までの秒数。</param>
        public void SetTimeToLanding(float timeToLanding)
        {
            _timeToLanding = timeToLanding;
        }

        /// <summary>
        /// 画面上の入力位置から、1回の発射に必要な値を作成する。
        /// </summary>
        /// <param name="screenPosition">マウスなどのスクリーン座標。</param>
        /// <returns>生成位置、生成回転、初速をまとめた発射データ。</returns>
        private LaunchShot CreateLaunchShot(Vector3 screenPosition)
        {
            // 入力範囲上の割合。左端なら0、右端なら1になる。
            var inputRatio = GetInputRatio(screenPosition);

            // 入力割合に対応する発射位置。
            var launchPosition = GetLaunchPosition(inputRatio);

            // 入力割合に対応する着地点。
            var landingPosition = GetLandingPosition(inputRatio);

            // 指定時間後に着地点へ届くように逆算した初速。
            var initialVelocity = MedalLaunchMath.CalculateBallisticInitialVelocity(
                launchPosition,
                landingPosition,
                SafeTimeToLanding,
                Physics.gravity);

            return new LaunchShot(launchPosition, GetLaunchRotation(), initialVelocity);
        }

        /// <summary>
        /// スクリーン座標がマウス入力範囲のどの割合にあるかを取得する。
        /// </summary>
        /// <param name="screenPosition">判定したいスクリーン座標。</param>
        /// <returns>入力範囲の左端を0、右端を1とした割合。</returns>
        private float GetInputRatio(Vector3 screenPosition)
        {
            // 入力範囲のワールド座標をスクリーン座標に変換するためのカメラ。
            var camera = GetMouseInputCamera();
            if (!camera)
            {
                return MedalLaunchMath.DefaultRatio;
            }

            // マウス入力範囲のワールド座標上の左端と右端。
            GetMouseInputRange(out var leftWorldPosition, out var rightWorldPosition);

            // 入力範囲の両端をカメラから見たスクリーン座標へ変換する。
            var leftScreenPosition = camera.WorldToScreenPoint(leftWorldPosition);
            var rightScreenPosition = camera.WorldToScreenPoint(rightWorldPosition);

            // zが0以下の場合、その点はカメラの裏側にあるため正しい割合を計算しない。
            if (leftScreenPosition.z <= 0f || rightScreenPosition.z <= 0f)
            {
                return MedalLaunchMath.DefaultRatio;
            }

            // スクリーン上の入力範囲線分に、マウス位置を射影して割合を求める。
            return MedalLaunchMath.CalculateRatioOnSegment(
                new Vector2(screenPosition.x, screenPosition.y),
                new Vector2(leftScreenPosition.x, leftScreenPosition.y),
                new Vector2(rightScreenPosition.x, rightScreenPosition.y));
        }

        /// <summary>
        /// 入力割合に対応する発射位置を取得する。
        /// </summary>
        /// <param name="inputRatio">発射範囲の開始を0、終了を1とした割合。</param>
        /// <returns>ワールド座標の発射位置。</returns>
        private Vector3 GetLaunchPosition(float inputRatio)
        {
            // 発射可能範囲のワールド座標上の開始位置と終了位置。
            GetLaunchRange(out var startPosition, out var endPosition);

            // 割合を0から1に制限してから、発射範囲上の位置を補間する。
            return MedalLaunchMath.LerpClamped(startPosition, endPosition, inputRatio);
        }

        /// <summary>
        /// 入力割合に対応する着地点を取得する。
        /// </summary>
        /// <param name="inputRatio">入力範囲の左端を0、右端を1とした割合。</param>
        /// <returns>ワールド座標の着地点。</returns>
        private Vector3 GetLandingPosition(float inputRatio)
        {
            // マウス入力範囲のワールド座標上の左端と右端。
            GetMouseInputRange(out var leftPosition, out var rightPosition);

            // 割合を0から1に制限してから、入力範囲上の位置を補間する。
            return MedalLaunchMath.LerpClamped(leftPosition, rightPosition, inputRatio);
        }

        /// <summary>
        /// 弾道プレビュー用に、発射から着地までの途中位置を計算する。
        /// </summary>
        /// <param name="startPosition">発射位置。</param>
        /// <param name="landingPosition">着地点。</param>
        /// <param name="normalizedTime">0から1で表した発射後の進行割合。</param>
        /// <returns>指定された進行割合での弾道上の位置。</returns>
        private Vector3 EvaluateTrajectoryPosition(Vector3 startPosition, Vector3 landingPosition, float normalizedTime)
        {
            // 実際の弾道計算に使う秒数。
            var duration = SafeTimeToLanding;

            // 0から1の進行割合を、実際の経過秒数へ変換する。
            var elapsedTime = Mathf.Clamp01(normalizedTime) * duration;

            // この発射位置から着地点へ到達するための初速。
            var initialVelocity = MedalLaunchMath.CalculateBallisticInitialVelocity(
                startPosition,
                landingPosition,
                duration,
                Physics.gravity);

            // 等加速度運動の式で、経過秒数時点の位置を求める。
            return MedalLaunchMath.EvaluateBallisticPosition(
                startPosition,
                initialVelocity,
                elapsedTime,
                Physics.gravity);
        }

        /// <summary>
        /// 発射可能範囲の開始位置と終了位置をワールド座標で取得する。
        /// </summary>
        /// <param name="startPosition">発射可能範囲の開始位置。</param>
        /// <param name="endPosition">発射可能範囲の終了位置。</param>
        private void GetLaunchRange(out Vector3 startPosition, out Vector3 endPosition)
        {
            // 発射範囲の基準になるTransform。
            var referenceTransform = GetLaunchReferenceTransform();

            // 発射範囲のローカル座標に加える回転オフセット。
            var rotationOffset = Quaternion.Euler(_launchRotationOffset);

            // ローカルオフセットと回転済みの範囲開始位置を、基準Transformからワールド座標へ変換する。
            startPosition = referenceTransform.TransformPoint(
                _launchLocalOffset + rotationOffset * _launchRangeStartLocalPosition);

            // ローカルオフセットと回転済みの範囲終了位置を、基準Transformからワールド座標へ変換する。
            endPosition = referenceTransform.TransformPoint(
                _launchLocalOffset + rotationOffset * _launchRangeEndLocalPosition);
        }

        /// <summary>
        /// マウス入力範囲の左端と右端をワールド座標で取得する。
        /// </summary>
        /// <param name="leftPosition">入力範囲の左端。</param>
        /// <param name="rightPosition">入力範囲の右端。</param>
        private void GetMouseInputRange(out Vector3 leftPosition, out Vector3 rightPosition)
        {
            // 入力範囲全体のオフセットを左端に加えたワールド座標。
            leftPosition = _mouseInputWorldOffset + _mouseInputLeftWorldPosition;

            // 入力範囲全体のオフセットを右端に加えたワールド座標。
            rightPosition = _mouseInputWorldOffset + _mouseInputRightWorldPosition;
        }

        /// <summary>
        /// メダル生成時に使う回転を取得する。
        /// </summary>
        /// <returns>発射基準Transformの回転に、回転オフセットを加えたQuaternion。</returns>
        private Quaternion GetLaunchRotation()
        {
            return GetLaunchReferenceTransform().rotation * Quaternion.Euler(_launchRotationOffset);
        }

        /// <summary>
        /// 発射位置と発射回転の基準になるTransformを取得する。
        /// </summary>
        /// <returns>設定済みの基準Transform。未設定の場合は自身のTransform。</returns>
        private Transform GetLaunchReferenceTransform()
        {
            return _launchReferenceTransform ? _launchReferenceTransform : transform;
        }

        /// <summary>
        /// マウス入力範囲をスクリーン座標へ変換するためのカメラを取得する。
        /// </summary>
        /// <returns>設定済みの入力カメラ。未設定の場合はCamera.main。</returns>
        private Camera GetMouseInputCamera()
        {
            return _mouseInputCamera ? _mouseInputCamera : Camera.main;
        }

        /// <summary>
        /// Unityのバージョン差を吸収してRigidbodyへ速度を設定する。
        /// </summary>
        /// <param name="targetRigidbody">速度を設定する対象のRigidbody。</param>
        /// <param name="velocity">設定する初速。</param>
        private static void SetRigidbodyVelocity(Rigidbody targetRigidbody, Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            targetRigidbody.linearVelocity = velocity;
#else
            targetRigidbody.velocity = velocity;
#endif
        }

        /// <summary>
        /// 1回の発射に必要な生成位置、生成回転、初速をまとめた値。
        /// </summary>
        private readonly struct LaunchShot
        {
            /// <summary>
            /// 発射データを作成する。
            /// </summary>
            /// <param name="position">メダルを生成するワールド座標。</param>
            /// <param name="rotation">メダルを生成する回転。</param>
            /// <param name="initialVelocity">Rigidbodyへ設定する初速。</param>
            public LaunchShot(Vector3 position, Quaternion rotation, Vector3 initialVelocity)
            {
                Position = position;
                Rotation = rotation;
                InitialVelocity = initialVelocity;
            }

            /// <summary>
            /// メダルを生成するワールド座標。
            /// </summary>
            public Vector3 Position { get; }

            /// <summary>
            /// メダルを生成する回転。
            /// </summary>
            public Quaternion Rotation { get; }

            /// <summary>
            /// Rigidbodyへ設定する初速。
            /// </summary>
            public Vector3 InitialVelocity { get; }
        }
    }
}
