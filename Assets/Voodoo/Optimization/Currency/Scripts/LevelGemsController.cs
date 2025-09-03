using System.Collections.Generic;
using Assets.GameOps.Features;
using UnityEngine;

public class LevelGemsController : MonoBehaviour
{
    [SerializeField] List<GameObject> _gems;

    void Awake()
    {
        if (ABTestLoader.Instance.rebornMissionsConfig.Version==1 && FeatureManager.Config.WorldsShop && ServiceLocator.GetGameCount() >= ABTestLoader.Instance.worldsShopConfig.FirstTimeAppearance && ServiceLocator.GetGameCount() >= ABTestLoader.Instance.worldsShopConfig.FirstGemsAppearance)
        {
            Shuffle(_gems);

            int activeCoins = (int)(_gems.Count * ABTestLoader.Instance.worldsShopConfig.ActiveGemsPercentage);

            for (int i = 0; i < _gems.Count - activeCoins; i++)
            {
                _gems[i].SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        _gems = new List<GameObject>();

        foreach (Transform t in transform)
        {
            _gems.Add(t.gameObject);
        }
        Debug.Log("Refreshed");
    }
}
