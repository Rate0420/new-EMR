using UnityEngine;

namespace EMR.Medal.Refund.l
{
    public class MedalLanded : MonoBehaviour
    {
        // 一回着地したらもう着地しないようにするためのフラグ
        public bool hasLanded = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (hasLanded) return; // すでに着地している場合は何もしない
            ItemTriggerEvents.OnMedalLanded?.Invoke(this.gameObject); // メダルが地面に着地したことを通知
            hasLanded = true; // 着地したフラグを立てる
        }
    }
}

