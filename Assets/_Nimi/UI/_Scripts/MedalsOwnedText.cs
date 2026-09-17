using TMPro;
using UnityEngine;
using EMR.Core;

namespace EMR.Medal.UI
{
    // 所持メダルを表示するためのクラス
    public class MedalsOwnedText : MonoBehaviour
    {
        [SerializeField] TMP_Text _medalsOwnedText; // 所持メダル数を表示するテキスト

        private GameState _gameState;


        private void Start()
        {
            _gameState = GameState.Instance;

            // 初期表示のために所持メダル数を設定
            SetMedalsOwnedCount(_gameState.OwnedModel.Count);

            // 所持メダルが更新されたときに呼ばれるイベントを登録
            _gameState.OwnedModel.OnCountChanged += SetMedalsOwnedCount;
        }

        private void OnDisable()
        {
            _gameState.OwnedModel.OnCountChanged -= SetMedalsOwnedCount;
        }


        // 所持メダル数をテキストに表示するメソッド
        private void SetMedalsOwnedCount(int medalOwnedCount)
        {
            if (medalOwnedCount == 0)
            {
                _medalsOwnedText.color = Color.red;
            }
            else
            {
                _medalsOwnedText.color = Color.white;
            }

            // テキストに所持メダル数を表示
            _medalsOwnedText.text = $"{medalOwnedCount.ToString("D5")}<size=8>枚";
        }
    }
}