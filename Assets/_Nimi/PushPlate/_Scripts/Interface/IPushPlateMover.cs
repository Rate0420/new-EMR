using UnityEngine;
using EMR.Utility;

namespace EMR.PushPlate
{
    public interface IPushPlateMover
    {
        /// <summary>
        /// 移動方向
        /// </summary>
        public MoveDirection MoveDirection { get; }

        /// <summary>
        /// 移動量
        /// </summary>
        public float MoveDistance { get; }

        /// <summary>
        /// 移動速度
        /// </summary>
        public float MoveSpeed { get; }


        /// <summary>
        /// 移動方向を変更する
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        public void SetMoveDirection(MoveDirection moveDirection);

        /// <summary>
        /// 移動量を変更する
        /// </summary>
        /// <param name="moveDistance">移動量</param>
        public void SetMoveDistance(float moveDistance);

        /// <summary>
        /// 移動速度を変更する
        /// </summary>
        /// <param name="speed">移動速度</param>
        public void SetMoveSpeed(float moveSpeed);
    }
}
