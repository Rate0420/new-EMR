using UnityEngine;

namespace EMR.Medal.Launch
{
    /// <summary>
    /// メダル発射に使う計算処理をまとめたクラス。
    /// MonoBehaviour、Input、Instantiate、Rigidbodyには依存しない。
    /// </summary>
    internal static class MedalLaunchMath
    {
        /// <summary>
        /// 入力範囲や発射範囲の中央を表す割合。
        /// カメラ未設定などで割合を計算できない場合に使用する。
        /// </summary>
        public const float DefaultRatio = 0.5f;

        /// <summary>
        /// 着地時間として許可する最小秒数。
        /// 0に近すぎる値で速度計算が破綻しないようにする。
        /// </summary>
        public const float MinimumTimeToLanding = 0.01f;

        /// <summary>
        /// 着地時間が最小値を下回らないように補正する。
        /// </summary>
        /// <param name="timeToLanding">補正前の着地時間。</param>
        /// <returns>最小値以上に補正された着地時間。</returns>
        public static float GetSafeTime(float timeToLanding)
        {
            return Mathf.Max(MinimumTimeToLanding, timeToLanding);
        }

        /// <summary>
        /// 2点間を0から1の割合で補間する。
        /// 割合が範囲外の場合は0から1に丸める。
        /// </summary>
        /// <param name="startPosition">割合0の位置。</param>
        /// <param name="endPosition">割合1の位置。</param>
        /// <param name="ratio">補間に使う割合。</param>
        /// <returns>2点間を補間した位置。</returns>
        public static Vector3 LerpClamped(Vector3 startPosition, Vector3 endPosition, float ratio)
        {
            // 入力値が範囲外でも、線分の外側へ出ないように制限する。
            return Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(ratio));
        }

        /// <summary>
        /// 指定した点が線分上のどの割合に対応するかを計算する。
        /// </summary>
        /// <param name="point">割合を調べたい点。</param>
        /// <param name="segmentStart">割合0になる線分の開始点。</param>
        /// <param name="segmentEnd">割合1になる線分の終了点。</param>
        /// <returns>線分開始を0、線分終了を1とした割合。</returns>
        public static float CalculateRatioOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            // 線分の向きと長さを表すベクトル。
            var segment = segmentEnd - segmentStart;

            // 開始点と終了点がほぼ同じ位置なら、左右を判定できないので中央扱いにする。
            if (segment.sqrMagnitude <= Mathf.Epsilon)
            {
                return DefaultRatio;
            }

            // 点から線分開始点へ向かうベクトルを線分方向へ射影する。
            // Dot(point - start, segment) / |segment|^2 で、線分上の割合を求められる。
            var ratio = Vector2.Dot(point - segmentStart, segment) / segment.sqrMagnitude;

            // 線分の外側をクリックした場合も、0から1の範囲に収める。
            return Mathf.Clamp01(ratio);
        }

        /// <summary>
        /// 指定時間後に着地点へ到達するための初速を計算する。
        /// </summary>
        /// <param name="startPosition">発射位置。</param>
        /// <param name="landingPosition">着地点。</param>
        /// <param name="timeToLanding">着地点へ到達するまでの秒数。</param>
        /// <param name="gravity">計算に使用する重力加速度。</param>
        /// <returns>Rigidbodyへ設定する初速。</returns>
        public static Vector3 CalculateBallisticInitialVelocity(Vector3 startPosition, Vector3 landingPosition, float timeToLanding, Vector3 gravity)
        {
            // 0除算や極端に大きい速度を避けるため、着地時間を最小値以上に丸める。
            var safeTime = GetSafeTime(timeToLanding);

            // 等加速度運動の式 p = p0 + v0*t + 1/2*g*t^2 を v0 について解く。
            // v0 = (p - p0 - 1/2*g*t^2) / t
            return (landingPosition - startPosition - gravity * (0.5f * safeTime * safeTime)) / safeTime;
        }

        /// <summary>
        /// 初速と経過時間から、弾道上の現在位置を計算する。
        /// </summary>
        /// <param name="startPosition">発射位置。</param>
        /// <param name="initialVelocity">発射時の初速。</param>
        /// <param name="elapsedTime">発射後の経過秒数。</param>
        /// <param name="gravity">計算に使用する重力加速度。</param>
        /// <returns>経過秒数時点の弾道上の位置。</returns>
        public static Vector3 EvaluateBallisticPosition(Vector3 startPosition, Vector3 initialVelocity, float elapsedTime, Vector3 gravity)
        {
            // 等加速度運動の式 p = p0 + v0*t + 1/2*g*t^2。
            return startPosition + initialVelocity * elapsedTime + gravity * (0.5f * elapsedTime * elapsedTime);
        }
    }
}