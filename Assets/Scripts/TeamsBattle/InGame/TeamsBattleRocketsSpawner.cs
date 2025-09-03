using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Oxygen;
using TeamsBattle.Events;
using Random = UnityEngine.Random;

namespace TeamsBattle
{
    public class TeamsBattleRocketsSpawner : MonoBehaviour
    {
        [SerializeField] private TeamsBattleRocketCollectable _rocketPrefab;
        [SerializeField] private TeamsBattleFadingLabel _fadingLabelPrefab;
        
        private Dictionary<Transform, TeamsBattleRocketCollectable> _spawners = new ();
        private int _spawnedAmount = 0;
        private Transform _lastCollectedSpawner;
        private float _cooldown = 0;
        private int _totalCollected = 0;

        private void Awake()
        {
            if (!ABTestLoader.Instance.TeamsBattleConfig.isEnabled)
            {
                Destroy(gameObject);
                return;
            }
            
            if (TeamsBattleManager.Instance == null)
            {
                Destroy(gameObject);
                return;
            }

            if (!TeamsBattleManager.IsInitialized)
            {
                Destroy(gameObject);
                return;
            }
            
            if (!TeamsBattleManager.Progress.IsBattleActive || 
                TeamsBattleManager.Progress.IsRegistrationActive || 
                !TeamsBattleManager.Progress.IsPlayerParticipating)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            var spawnersContainer = FindObjectOfType<PowerupsSpawner>().transform;
            for (var i = 0; i < spawnersContainer.childCount; i++)
                _spawners.Add(spawnersContainer.GetChild(i), null);
            
            for (var i = 0; i < TeamsBattleManager.Config.CollectiblesInGameAmount; i++)
                SpawnRocket(GetEmptySpawners()[Random.Range(0, GetEmptySpawners().Count)]);
            
            _cooldown = Random.Range(TeamsBattleManager.Config.CollectibleInGameCooldownMin, TeamsBattleManager.Config.CollectibleInGameCooldownMax);
        }

        private void Update()
        {
            if (_cooldown > 0)
                _cooldown -= Time.deltaTime;
            
            if (_cooldown > 0)
                return;
            
            if (_spawnedAmount >= TeamsBattleManager.Config.CollectiblesInGameMaxSpawned)
                return;
            
            var neededAmount = TeamsBattleManager.Config.CollectiblesInGameAmount - _spawners.Values.Count(x => x != null);
            if (neededAmount <= 0)
                return;
            
            var emptySpawners = GetEmptySpawners();
            if (emptySpawners == null || emptySpawners.Count == 0)
                return;
            
            SpawnRocket(emptySpawners[Random.Range(0, emptySpawners.Count)]);
            _cooldown = Random.Range(TeamsBattleManager.Config.CollectibleInGameCooldownMin, TeamsBattleManager.Config.CollectibleInGameCooldownMax);
        }
        
        private void SpawnRocket(Transform spawner)
        {
            if (_spawnedAmount >= TeamsBattleManager.Config.CollectiblesInGameMaxSpawned)
                return;
            
            var rocket = Instantiate(_rocketPrefab, spawner.position, Quaternion.identity, spawner);
            _spawners[spawner] = rocket;
            var size = Random.Range(TeamsBattleManager.Config.CollectibleInGameMinSize, TeamsBattleManager.Config.CollectibleInGameMaxSize);
            rocket.Setup(this, size);
            
            _spawnedAmount++;
            _lastCollectedSpawner = null;
        }
        
        private List<Transform> GetEmptySpawners()
        {
            return _spawners.Where(x => x.Value == null && x.Key != _lastCollectedSpawner).Select(x => x.Key).ToList();
        }
        
        public void RocketCollected(TeamsBattleRocketCollectable rocket)
        {
            TeamsBattleManager.Progress.AddPlayerPoints(TeamsBattleManager.Config.CollectibleInGameReward.quantity, null, false);
            _totalCollected += TeamsBattleManager.Config.CollectibleInGameReward.quantity;
            
            var spawner = _spawners.First(x => x.Value == rocket).Key;
            _spawners[spawner] = null;
            _lastCollectedSpawner = spawner;
            
            var fadingLabel = Instantiate(_fadingLabelPrefab, rocket.transform.position, Quaternion.identity);
            fadingLabel.transform.localScale = BaseGameManager.Instance.CurrentHole.references.BuffsHolder.localScale;
            fadingLabel.Setup(TeamsBattleManager.Config.CollectibleInGameReward.quantity);
            
            GlobalBus.Send(new OnInGameCollectibleCollected(TeamsBattleManager.Config.CollectibleInGameReward, _totalCollected));
        }
    }
}