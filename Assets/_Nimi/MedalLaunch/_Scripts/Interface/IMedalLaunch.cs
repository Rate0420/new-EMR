using UnityEngine;

namespace EMR.Medal.Launch
{
    public interface IMedalLaunch
    {
        /// <summary>
        /// 発射物
        /// </summary>
        public Rigidbody LaunchPrefab { get; }

        /// <summary>
        /// 着陸までの推定残り時間
        /// </summary>
        public float TimeToLanding { get; }


        /// <summary>
        /// 発射する
        /// </summary>
        public void Launch();


        /// <summary>
        /// 発射物の設定
        /// </summary>
        /// <param name="prefab">Rigidbodyコンポーネントを含むGameObject</param>
        public void SetLaunchPrefab(Rigidbody prefab);

        /// <summary>
        /// 着陸までの残り時間を設定
        /// </summary>
        /// <param name="timeToLanding">着陸までの時間を秒単位で指定（小数可）。</param>
        public void SetTimeToLanding(float timeToLanding);
    }
}
