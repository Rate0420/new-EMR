using UnityEngine;

// マウス入力で発射/消費アイテム使用イベントを疑似的に発火させる、テスト用の入力トリガー。
//
// 本来はメダル発射システムや消費アイテムのUIボタンなどから
// ItemTriggerEvents.OnMedalShot / OnConsumptionItem を呼ぶのが正しい形。
// それらの実装が済み次第、このスクリプトごと削除してよい。
public class DebugInputTrigger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ItemTriggerEvents.OnMedalShot?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ItemTriggerEvents.OnConsumptionItem?.Invoke();
        }
    }
}
