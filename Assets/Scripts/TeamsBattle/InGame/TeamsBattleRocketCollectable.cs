using UnityEngine;

namespace TeamsBattle
{
    public class TeamsBattleRocketCollectable : MonoBehaviour
    {
        [SerializeField] private Transform _sizeTransform;
        
        private FallingObjectBehaviour _fallingObject;
        private TeamsBattleRocketsSpawner _spawner;

        public void Setup(TeamsBattleRocketsSpawner spawner, int size)
        {
            _fallingObject = GetComponent<FallingObjectBehaviour>();
            if (_fallingObject != null)
                _fallingObject.Eaten += OnCollected;
            
            _spawner = spawner;
            _fallingObject.FallingObject.SizeRequired = size;
            _sizeTransform.localScale = Vector3.one * size;
        }

        private void OnDestroy()
        {
            if (_fallingObject != null)
                _fallingObject.Eaten -= OnCollected;
        }

        private void OnCollected(Hole hole)
        {
            _spawner.RocketCollected(this);
            Destroy(gameObject);
        }
    }
}