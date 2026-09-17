using UnityEngine;
using EMR.Core;

namespace EMR.Medal.Hole
{
    public class MedalsOwnedPresenter : MonoBehaviour
    {
        [SerializeField] CollectionHole[] _collectionHole;

        void OnEnable()
        {
            foreach (var hole in _collectionHole)
            {
                hole.OnCollected += OnMedalCollected;
            }
        }

        void OnDisable()
        {
            foreach (var hole in _collectionHole)
            {
                hole.OnCollected -= OnMedalCollected;
            }
        }


        void OnMedalCollected(ICollectable collectable)
        {
            GameState.Instance.OwnedModel.AddMedal(collectable.Count);

            if (collectable.Info.Type == CollectableType.Ball)
            {
                Debug.Log("ボールが落下");
                GameState.Instance.RoundService.AddDroppedBalls(1);
            }
        }
    }
};