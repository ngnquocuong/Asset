using System.Collections;
using System.Collections.Generic;
using Assets.GameOps.Features;
using hole.defaultgame;
using UnityEngine;
using Voodoo.Sauce.Internal.Analytics;

public class PowerupsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _powerupPrefab;

    UIIndicator _uiIndicator;
    List<Transform> _list;
    Transform _lastTarget;

    IEnumerator Start()
    {
        if (FeatureManager.Config.UpgradablePowerups.IsEnabled && ServiceLocator.GetGameCount() >= ABTestLoader.Instance.upgradablePowerupsConfig.FirstTimeAppearance)
        {
            if (UpgradablePowerupsController.IsUpgradablePowerupLevel(ServiceLocator.GetGameCount()))
            {
                int childeren = transform.childCount;

                if (childeren > 0)
                {

                    if (ABTestLoader.Instance.upgradablePowerupsConfig.EnhanceVisibility)
                    {
                        int spawnPoint = Random.Range(0, childeren);
                        _list = new List<Transform>();
                        for (int i = Mathf.Clamp(ABTestLoader.Instance.upgradablePowerupsConfig.EnhanceVisibilityPowerupsCont, 1, childeren); i > 0; i--)
                        {
                            var powerup = Instantiate(_powerupPrefab, transform.GetChild(spawnPoint).position, Quaternion.identity, transform.GetChild(spawnPoint));
                            spawnPoint = (spawnPoint + 3) % childeren;
                            _list.Add(powerup.transform);
                        }

                        if (GameManager.Instance != null)
                        {
                            yield return new WaitUntil(() => GameManager.Instance.CurrentHole != null);
                            int target = -1;
                            float targetDistance = float.MaxValue;
                            for (int i = 0; i < _list.Count; i++)
                            {
                                Transform x = _list[i];
                                var distance = Vector3.Distance(GameManager.Instance.CurrentHole.transform.position, x.position);
                                if (distance < targetDistance)
                                {
                                    targetDistance = distance;
                                    target = i;
                                }
                            }

                            if (target != -1)
                            {
                                _uiIndicator = hole.defaultgame.UIManager.Instance.IndicatorsComponent.AddPowerupIndicator(_list[target]);
                                _lastTarget = _list[target];
                                StartCoroutine(UpdateIndicator());
                            }
                        }
                    }
                    else
                    {
                        int spawnPoint = Random.Range(0, childeren);

                        var powerup = Instantiate(_powerupPrefab, transform.GetChild(spawnPoint).position, Quaternion.identity, transform.GetChild(spawnPoint));

                        if (ABTestLoader.Instance.upgradablePowerupsConfig.EnhanceVisibility && hole.defaultgame.UIManager.Instance != null)
                        {
                            hole.defaultgame.UIManager.Instance.IndicatorsComponent.AddPowerupIndicator(powerup.transform);
                        }
                    }
                }
            }
            else
            {
                UpgradablePowerupsController.UpdateUpgradablePowerupSpawnCounter();
            }
        }
    }

    IEnumerator UpdateIndicator()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_uiIndicator != null && _list != null && _list.Count > 0)
            {
                int target = -1;
                float targetDistance = float.MaxValue;

                foreach (var x in _list.ToArray())
                    if (x == null)
                        _list.Remove(x);

                for (int i = 0; i < _list.Count; i++)
                {
                    Transform x = _list[i];
                    var distance = Vector3.Distance(GameManager.Instance.CurrentHole.transform.position, x.position);
                    if (distance < targetDistance)
                    {
                        targetDistance = distance;
                        target = i;
                    }
                }

                if (target != -1 && _list[target] != _lastTarget)
                {
                    _uiIndicator.SetPowerupTarget(_list[target]);
                    _lastTarget = _list[target];
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
