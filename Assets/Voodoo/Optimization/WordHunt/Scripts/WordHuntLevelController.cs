using System.Collections;
using System.Linq;
using Assets.GameOps.Features;
using hole.defaultgame;
using LibraryBen.Assembly.Miscellaneous.ExtensionMethods;
using RSG;
using UnityEngine;
using Voodoo.Optimization._Common;

namespace Voodoo.Optimization.WordHunt.Scripts {
    public class WordHuntLevelController : MonoBehaviour {
        [SerializeField] private WordHuntLetterView wordHuntLetterView;
        [SerializeField] private float letterMinimumScale = 2f;
        
        private WordHuntLetterView letterInstance;
        private Hole playerHole;
        private WordHuntPersistenceController persistenceController = new WordHuntPersistenceController();
        private bool isLetterCollected;
        
        private static float[] LetterSizePercentages = { 0.25f, 0.5f, 0.75f, 1f };

        private void Start() {
            var gameCount = ServiceLocator.GetGameCount();
            var config = ABTestLoader.Instance.wordHuntConfig;
            
            if (FeatureManager.Config.WordHunt.IsEnabled && 
                gameCount >= config.firstTimeAppearance &&
                gameCount % config.spawnLettersInLevelInterval == 0) {
                BaseGameManager.Instance.MapRotated += OnMapRotated;
            }
        }

        private void OnMapRotated() {
            BaseGameManager.Instance.MapRotated -= OnMapRotated;
            
            SpawnLetter();
            StartCoroutine(CheckIsLetterDestroyed());
        }

        private IEnumerator CheckIsLetterDestroyed() {
            while (!isLetterCollected) {
                if (letterInstance == null) {
                    SpawnLetter();
                }
                
                yield return new WaitForSeconds(2f);
            }
        }
        
        private void SpawnLetter() {
            var random = new System.Random(persistenceController.LastSeed);

            var spawnPoints = GetComponentsInChildren<WordHuntLetterSpawnPointModel>(true);
            var spawnPoint = spawnPoints.GetRandom(random.Next());
            var spawnPointMaxSize = spawnPoint.Size;
            var currentLetter = WordHuntController.Instance.CurrentLetter;

            letterInstance = Instantiate(wordHuntLetterView, spawnPoint.transform.position, Quaternion.identity, transform);
            letterInstance.transform.position = spawnPoint.Center;
            letterInstance.transform.forward = Vector3.forward;
            letterInstance.transform.rotation *= Quaternion.AngleAxis(90, Vector3.right);
            letterInstance.transform.rotation *= Quaternion.AngleAxis(Mathf.Lerp(-10, 10, (float) random.NextDouble()), Vector3.up);
            letterInstance.Collect += OnLetterCollected;

            Promise.Resolved()
                .Then(() => letterInstance.SetLetter(currentLetter))
                .Then(() => {
                    var letterSize = letterInstance.Size;
                    Debug.Log($"Letter size: {letterSize}");
                    var letterTargetPercentage = GetLetterCurrentTargetPercentage();
                    Debug.Log($"Letter target percentage: {letterTargetPercentage}");
                    
                    float maxScaleFactor;

                    if (letterSize.x >= letterSize.z) {
                        maxScaleFactor = spawnPointMaxSize.x / letterSize.x;
                    }
                    else {
                        maxScaleFactor = spawnPointMaxSize.z / letterSize.z;
                    }

                    var scaleFactor = Mathf.Max(letterMinimumScale, maxScaleFactor * letterTargetPercentage);
                    letterInstance.SetScaleTo(scaleFactor, false);
                    letterInstance.UpdateParticlesSize();
                });

            GetPlayerHoleAsync();
        }

        private float GetLetterCurrentTargetPercentage() {
            var index = persistenceController.LetterSizeIndex;

            if (index >= LetterSizePercentages.Length) {
                index = 0;
                persistenceController.LetterSizeOrderSeed++;
            }
            
            var random = new System.Random(persistenceController.LetterSizeOrderSeed);
            var orderedSizes = LetterSizePercentages.OrderBy(x => random.Next()).ToArray();

            persistenceController.LetterSizeIndex = index;
            persistenceController.Save();
            
            return orderedSizes[index];
        }

        private void GetPlayerHoleAsync() {
            Promise.Resolved()
                .WaitUntil(this, () => BaseGameManager.Instance != null && BaseGameManager.Instance.CurrentHole != null)
                .Then(() => playerHole = BaseGameManager.Instance.CurrentHole);
        }

        private void OnLetterCollected(CollectibleView collectible, Hole byHole) {
            if (playerHole == byHole) {
                isLetterCollected = true;
                
                WordHuntController.Instance.OnLetterCollected();

                collectible.Collect -= OnLetterCollected;
                //collectible.Destruct();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (playerHole == null || letterInstance == null) return;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(playerHole.transform.position, letterInstance.transform.position);
        }
        
        [ContextMenu("Order from big to small")]
        private void OrderFromBigToSmall() {
            var spawnPoints = GetComponentsInChildren<WordHuntLetterSpawnPointModel>(true);
            var orderedSpawnPoints = spawnPoints.OrderByDescending(sp => sp.Size.x * sp.Size.z).ToList();

            for (var i = 0; i < orderedSpawnPoints.Count; i++) {
                orderedSpawnPoints[i].transform.SetSiblingIndex(i);
            }
        }
#endif
    }
}