using UnityEngine;
using EMR.Round;
using EMR.Core;
using NaughtyAttributes;

namespace EMR.Bootstrap
{
    public class RoundProgressInitializer : MonoBehaviour
    {
        [SerializeField, Expandable] private RoundProgressionSettings _roundProgressionSettings;

        private RoundManager _manager;
        private RoundProgressService _service;


        private void Start()
        {
            _manager = GameState.Instance.RoundManager;
            _service = GameState.Instance.RoundService;

            _service.OnRoundAdvanced += OnRoundAdvanced;

            // 最初のラウンドの設定を登録
            OnRoundAdvanced(_manager.CurrentRound);
        }

        private void OnDestroy()
        {
            _service.OnRoundAdvanced -= OnRoundAdvanced;
        }

        private void OnRoundAdvanced(int round)
        {
            RoundRequirement requirement =
                _roundProgressionSettings.GetRequirement(round);

            _service.SetRequirement(requirement);
        }
    }
}