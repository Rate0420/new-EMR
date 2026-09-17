using System;
using UnityEngine;
using EMR.Medal;
using EMR.Round;

namespace EMR.Core
{
    public class GameState : MonoBehaviour
    {
        public static GameState Instance { get; private set; }

        public MedalsOwnedModel OwnedModel { get; private set; }
        public MedalRefundNotifier RefundNotifier { get; private set; }
        public RoundManager RoundManager { get; private set; }
        public RoundProgressService RoundService { get; private set; }
        public GamePause GamePause { get; private set; }
        public GameLockManager GameLock { get; private set; }


        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// ゲーム全体のサービスを初期化する。
        /// Bootstrapper から一度だけ呼ぶ。
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException(
                    "GameState はすでに初期化されています。");
            }

            OwnedModel = new MedalsOwnedModel(30);
            RefundNotifier = new MedalRefundNotifier();
            RoundManager = new RoundManager(startRound: 1);
            RoundService = new RoundProgressService(RoundManager);

            GamePause = new GamePause();
            GameLock = new GameLockManager();

            IsInitialized = true;
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            Instance = null;
        }
    }
}