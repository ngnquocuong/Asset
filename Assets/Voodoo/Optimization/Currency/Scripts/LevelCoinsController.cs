using System.Collections.Generic;
using Assets.GameOps.Features;
using UnityEngine;

public class LevelCoinsController : MonoBehaviour
{
    [SerializeField] private float m_ActiveCoinsPercentage = 0.45f;
    [SerializeField] List<GameObject> _coins;

    void Awake()
    {
        if (FeatureManager.Config.CurrencyV2)
        {
            Shuffle(_coins);

            int activeCoins = (int)(_coins.Count * m_ActiveCoinsPercentage);

            for (int i = 0; i < _coins.Count - activeCoins; i++)
            {
                _coins[i].SetActive(false);
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
        _coins = new List<GameObject>();

        foreach (Transform t in transform)
        {
            _coins.Add(t.gameObject);
        }
        Debug.Log("Refreshed");
    }

#if UNITY_EDITOR

    [ContextMenu("CoinsCount")]
    public void CoinsCount()
    {
        var c = FindObjectsOfType<PickableCurrencyAnimation>();

        var g = FindObjectsOfType<InGameGemAnimation>();

        Debug.Log("Coins: " + c.Length + "\n " + "Coins Groups: " + _coins.Count + "\n " + "Gems: " + g.Length);
    }
#endif
}
