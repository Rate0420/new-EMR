using UnityEngine;

public enum ItemType
{
    //バフアイテム
    [InspectorName("派手な帽子 ")] b_Hat,
    [InspectorName("有名ブランドの服")] b_Clothes,
    [InspectorName("高そうな腕時計")] b_Watch,
    [InspectorName("スタイリッシュなズボン")] b_Pants,
    [InspectorName("スポーツ店の靴")] b_Shoes,

    //消費アイテム
    [InspectorName("観覧車のチケット")]c_Wheel,
    [InspectorName("メダルウォール")] c_Wall,
    [InspectorName("フリーシューティング")] c_Shutar,
    [InspectorName("クエイクスタンプ")]c_stamp,

    // 選択肢ボール
    [InspectorName("選択肢ボール1")] s_Ball_1,
    [InspectorName("選択肢ボール2")] s_Ball_2
}
