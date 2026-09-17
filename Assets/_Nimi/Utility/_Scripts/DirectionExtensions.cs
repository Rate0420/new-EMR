using UnityEngine;

namespace EMR.Utility
{
    public enum MoveDirection
    {
        PositiveX,
        PositiveY,
        PositiveZ,
        NegativeX,
        NegativeY,
        NegativeZ
    }

    // PushPlateMoverで使用するユーティリティクラス
    public static class DirectionExtensions
    {
        public static Vector3 ToVector3(this MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.PositiveX => Vector3.right,
                MoveDirection.PositiveY => Vector3.up,
                MoveDirection.PositiveZ => Vector3.forward,
                MoveDirection.NegativeX => Vector3.left,
                MoveDirection.NegativeY => Vector3.down,
                MoveDirection.NegativeZ => Vector3.back,
                _ => Vector3.zero
            };
        }
    }
}
