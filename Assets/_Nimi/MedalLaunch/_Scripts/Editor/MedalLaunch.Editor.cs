#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EMR.Medal.Launch
{
    /// <summary>
    /// MedalLaunchのEditor専用表示を担当するpartialクラス。
    /// 発射範囲、入力範囲、着地点、弾道プレビューをGizmosで描画する。
    /// </summary>
    public partial class MedalLaunch
    {
        /// <summary>
        /// GameObjectが選択されている間、Sceneビューにデバッグ表示を描画する。
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos)
            {
                return;
            }

            // Sceneビューに描画する発射可能範囲。
            GetLaunchRange(out var launchStartPosition, out var launchEndPosition);

            // Sceneビューに描画するマウス入力範囲。
            GetMouseInputRange(out var mouseInputLeftPosition, out var mouseInputRightPosition);

            // プレビューに使う入力割合。非再生時は中央、再生時は現在のマウス位置を使う。
            var inputRatio = GetPreviewInputRatio();

            // 入力割合に対応する発射位置。
            var launchPosition = MedalLaunchMath.LerpClamped(
                launchStartPosition,
                launchEndPosition,
                inputRatio);

            // 入力割合に対応する着地点。
            var landingPosition = MedalLaunchMath.LerpClamped(
                mouseInputLeftPosition,
                mouseInputRightPosition,
                inputRatio);

            DrawLaunchRangeGizmos(launchStartPosition, launchEndPosition);
            DrawMouseInputRangeGizmos(mouseInputLeftPosition, mouseInputRightPosition, landingPosition);
            DrawLaunchPreviewGizmos(launchPosition, landingPosition);
            DrawLandingGizmos(launchPosition, landingPosition);
        }

        /// <summary>
        /// Gizmosプレビューに使う入力割合を取得する。
        /// </summary>
        /// <returns>非再生時は中央、再生時は現在のマウス入力割合。</returns>
        private float GetPreviewInputRatio()
        {
            return Application.isPlaying
                ? GetInputRatio(Input.mousePosition)
                : MedalLaunchMath.DefaultRatio;
        }

        /// <summary>
        /// 発射可能範囲の開始点、終了点、線分を描画する。
        /// </summary>
        /// <param name="startPosition">発射可能範囲の開始位置。</param>
        /// <param name="endPosition">発射可能範囲の終了位置。</param>
        private void DrawLaunchRangeGizmos(Vector3 startPosition, Vector3 endPosition)
        {
            // 発射範囲の開始位置。
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(startPosition, 0.25f);

            // 発射範囲の終了位置。
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPosition, 0.25f);

            // 開始位置と終了位置を結ぶ発射可能範囲。
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPosition, endPosition);
        }

        /// <summary>
        /// マウス入力範囲と、現在の入力割合に対応する着地点を描画する。
        /// </summary>
        /// <param name="leftPosition">入力範囲の左端。</param>
        /// <param name="rightPosition">入力範囲の右端。</param>
        /// <param name="currentPosition">現在の入力割合に対応する位置。</param>
        private void DrawMouseInputRangeGizmos(
            Vector3 leftPosition,
            Vector3 rightPosition,
            Vector3 currentPosition)
        {
            // 入力範囲の両端と線分。
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(leftPosition, 0.25f);
            Gizmos.DrawSphere(rightPosition, 0.25f);
            Gizmos.DrawLine(leftPosition, rightPosition);

            // 現在の入力割合に対応する着地点。
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(currentPosition, 0.2f);
            DrawWireDisc(currentPosition, Vector3.up, 0.45f, Color.white);
        }

        /// <summary>
        /// 発射位置、発射方向、着地点との対応線を描画する。
        /// </summary>
        /// <param name="launchPosition">発射位置。</param>
        /// <param name="landingPosition">着地点。</param>
        private void DrawLaunchPreviewGizmos(Vector3 launchPosition, Vector3 landingPosition)
        {
            // 発射方向を求めるため、プレビュー用にも実際と同じ初速を計算する。
            var initialVelocity = MedalLaunchMath.CalculateBallisticInitialVelocity(
                launchPosition,
                landingPosition,
                SafeTimeToLanding,
                Physics.gravity);

            // 着地点と発射位置の対応を示す線。
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(landingPosition, launchPosition);

            // 発射位置。
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(launchPosition, 0.35f);

            if (initialVelocity.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            // 初速ベクトルを正規化した発射方向。
            var launchDirection = initialVelocity.normalized;

            // 発射方向をSceneビュー上で確認するための線と円。
            Gizmos.DrawRay(launchPosition, launchDirection * 3f);
            DrawWireDisc(launchPosition, launchDirection, 0.55f, Color.green);
        }

        /// <summary>
        /// 着地点と、発射位置から着地点までの弾道プレビューを描画する。
        /// </summary>
        /// <param name="launchPosition">発射位置。</param>
        /// <param name="landingPosition">着地点。</param>
        private void DrawLandingGizmos(Vector3 launchPosition, Vector3 landingPosition)
        {
            // 着地点。
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(landingPosition, 0.3f);
            DrawWireDisc(landingPosition, Vector3.up, 0.65f, Color.magenta);

            // 弾道を何分割して描画するか。
            const int trajectorySegmentCount = 24;

            // 直前に描画した弾道上の位置。
            var previousPosition = launchPosition;

            for (var i = 1; i <= trajectorySegmentCount; i++)
            {
                // 0から1で表した弾道上の進行割合。
                var normalizedTime = i / (float)trajectorySegmentCount;

                // 今回描画する弾道上の位置。
                var nextPosition = EvaluateTrajectoryPosition(
                    launchPosition,
                    landingPosition,
                    normalizedTime);

                // 直前の位置と今回の位置を線で結び、弾道を作る。
                Gizmos.DrawLine(previousPosition, nextPosition);
                previousPosition = nextPosition;

                // 一定間隔で点を置き、弾道の時間方向の流れを見やすくする。
                if (i % 6 == 0)
                {
                    Gizmos.DrawSphere(nextPosition, 0.12f);
                }
            }
        }

        /// <summary>
        /// 指定した中心、法線、半径でワイヤー円を描画する。
        /// </summary>
        /// <param name="center">円の中心位置。</param>
        /// <param name="normal">円の向き。ほぼゼロの場合は上向きに補正する。</param>
        /// <param name="radius">円の半径。</param>
        /// <param name="color">描画色。</param>
        private static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            Handles.color = color;
            Handles.DrawWireDisc(
                center,
                normal.sqrMagnitude <= Mathf.Epsilon ? Vector3.up : normal.normalized,
                radius);
        }
    }
}
#endif
