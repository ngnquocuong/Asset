using System.Collections.Generic;
using Assets.GameOps.Features;
using UnityEngine;
using Voodoo.Optimisation.Chestroom;
using Voodoo.Optimization._Common;
using Voodoo.Sauce.Internal.Analytics;

namespace Voodoo.Optimization.ChestRoom.InGame
{
    public class ChestRoomKeySpawner : MonoBehaviour
    {
        [SerializeField] private ChestRoomKey _keyPrefab;
        [SerializeField] private ChestRoomKeySpawnPoint _spawnPointPrefab;

        private List<ChestRoomKey> _keys = new List<ChestRoomKey>();

        public ChestRoomKeySpawnPoint SpawnPointPrefab => _spawnPointPrefab;

        private void Start()
        {
            if (!FeatureManager.Config.Chestroom.IsEnabled)
            {
                return;
            }

            if (ChestroomController.Instance.IsChestroomKeyLevel(ServiceLocator.GetGameCount()))
            {

                var spawnPoints = GetComponentsInChildren<ChestRoomKeySpawnPoint>();
                int start = Random.Range(0, 2);

                for (int i = start; i < spawnPoints.Length; i += 2)
                {
                    var spawnPoint = spawnPoints[i];
                    var key = Instantiate(_keyPrefab, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
                    key.Collect += OnKeyCollected;
                    _keys.Add(key);
                }
            }
            else
            {
                ChestroomController.Instance.UpdateChestroomKeySpawnCounter();
            }
        }

        private void OnKeyCollected(CollectibleView collectedKey, Hole hole)
        {
            collectedKey.Collect -= OnKeyCollected;

            foreach (var key in _keys)
            {
                if (key == collectedKey)
                    continue;

                key.gameObject.SetActive(false);
            }
        }

        public void DestroyAllKeys()
        {
            foreach (var key in _keys)
            {
                if(!key)
                    continue;
                key.gameObject.SetActive(false);
            }
        }
    }
}