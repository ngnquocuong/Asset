using Assets.GameOps.Features;
using UnityEngine;
using Voodoo.Optimisation.StickerCollections;
using Voodoo.Sauce.Internal.Analytics;

public class StickerPackSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _packPrefab;

    void Start()
    {
        // foreach (Transform child in transform)
        // {
        //     Instantiate(_packPrefab, child.position, Quaternion.identity, child);
        // }

        // return;

        if (!FeatureManager.Config.StickerCollections.IsEnabled || StickerCollectionsController.Instance==null)
        {
            return;
        }

        if (StickerCollectionsController.Instance.IsStickersPackLevel(ServiceLocator.GetGameCount()))
        {
            int childeren = transform.childCount;

            if (childeren > 0)
            {
                int spawnPoint = Random.Range(0, childeren);

                Instantiate(_packPrefab, transform.GetChild(spawnPoint).position, Quaternion.identity, transform.GetChild(spawnPoint));
            }
        }
        else
        {
            StickerCollectionsController.Instance.UpdateStickerPackSpawnCounter();
        }
    }
}
