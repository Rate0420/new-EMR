using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace EMR.PushPlate
{
    // 移動関連の処理
    public partial class PushPlateMover
    {
        // ループ制御用のCancellationTokenSource
        private CancellationTokenSource _loopCts;

        /// <summary>
        /// 動きを開始するメソッド
        /// </summary>
        private void StartMoveLoop()
        {
            // 既に動いている場合は二重起動を防ぐために一度止める
            StopMoveLoop();

            // 新しいCancellationTokenSourceを作成
            // GameObjectが破棄されたとき（OnDestroy）にも自動でキャンセルされるようにリンクさせる
            _loopCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            MoveLoopAsync(_loopCts.Token).Forget();
        }

        /// <summary>
        /// 動きを停止させるメソッド
        /// </summary>
        private void StopMoveLoop()
        {
            if (_loopCts != null)
            {
                _loopCts.Cancel();
                _loopCts.Dispose();
                _loopCts = null;
            }
        }


        private async UniTaskVoid MoveLoopAsync(CancellationToken cancellationToken)
        {
            // ループの開始前と各ステップで、キャンセル要求が来ていないかチェックする
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!CanMove())
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

                if (_direction > 0)
                {
                    await MoveToAsync(1f, cancellationToken);
                    await WaitAtPointAsync(_minPointWaitingTime, cancellationToken);
                }
                else
                {
                    await MoveToAsync(0f, cancellationToken);
                    await WaitAtPointAsync(_maxPointWaitingTime, cancellationToken);
                }
            }
        }

        private async UniTask MoveToAsync(float targetT, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!CanMove())
                {
                    await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
                    continue;
                }

                float delta = (_moveSpeed / _moveDistance) * Time.fixedDeltaTime;
                float nextT = _t + delta * _direction;

                _t = _direction > 0 ? Mathf.Min(nextT, targetT) : Mathf.Max(nextT, targetT);

                MovePlatform();

                if (Mathf.Approximately(_t, targetT))
                {
                    _direction *= -1;
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }


        // 指定した時間だけ待機する
        private async UniTask WaitAtPointAsync(float waitTime, CancellationToken cancellationToken)
        {
            if (waitTime <= 0f) return;

            // 待機時間を経過させる
            await UniTask.Delay(
                TimeSpan.FromSeconds(waitTime),
                DelayType.DeltaTime,
                PlayerLoopTiming.Update,
                cancellationToken);
        }

        private void MovePlatform()
        {
            float curvedT = EvaluateCurve(_t);
            Vector3 targetPosition = Vector3.Lerp(_startPosition, _endPosition, curvedT);

            _rigidbody.MovePosition(targetPosition);
        }

        private bool CanMove()
        {
            return _moveDistance > 0f && _moveSpeed > 0f;
        }
    }
}