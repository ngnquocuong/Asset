using System;
using System.Collections;
using System.Collections.Generic;
using Assets.GameOps.Features;
using DG.Tweening;
using Game.Maps;
using hole.defaultgame;
using UnityEngine;

public class InGamePickablePowerup : MonoBehaviour, IPickable
{
    public event Action<float> OnSizeChanged;

    [SerializeField] GameObject[] _powerUpGameObjects;
    [SerializeField] GameObject _rvIcon;

    [SerializeField] GameObject[] _groundParticle;

    private FallingObjectBehaviour _fallingObject;
    private PowerUpType _powerUpType;

    private readonly PowerUpType[] _powerups =
        {PowerUpType.Bigger, PowerUpType.Shrink, PowerUpType.Magnet, PowerUpType.Speed, PowerUpType.MoreTime};

    private bool CanBeInitialized => BaseGameManager.Instance && BaseGameManager.Instance.CurrentHole;

    private readonly float _maxSize = 15;
    private float _startSize => ABTestLoader.Instance.upgradablePowerupsConfig.EnhanceVisibility ? 1.25f : 1.5f;
    private readonly float _sizeMultiplier = .5f;
    private int _powerUpIndexOverride = -1;

    private void Start()
    {
        if (CanBeInitialized)
        {
            Initialize();
        }
        else
        {
            StartCoroutine(WaitUntilCanBeInitialized(Initialize));
        }
    }

    private IEnumerator WaitUntilCanBeInitialized(Action callback)
    {
        yield return new WaitUntil(() => CanBeInitialized);
        callback?.Invoke();
    }

    public void ScheduleDebugChangePowerUp(int index)
    {
        _powerUpIndexOverride = index;
    }
    
    public void DebugChangePowerup(int index)
    {
        if (index < 0 || index >= _powerups.Length)
            return;
        
        _powerUpType = _powerups[index - 1];

        foreach (var x in _powerUpGameObjects)
        {
            x.SetActive(false);
        }

        _powerUpGameObjects[index - 1].SetActive(true);
    }

    private void Initialize()
    {
        _fallingObject = GetComponent<FallingObjectBehaviour>();
        _fallingObject.Eaten += OnPackEaten;

        BaseGameManager.Instance.CurrentHole.SizeChanged += HandleOnSizeChanged;

        List<int> availablePowerups = new List<int>();
        for (int i = 0; i < ABTestLoader.Instance.upgradablePowerupsConfig.EnabledPowerups.Length; i++)
        {
            if (_powerups[i] == PowerUpType.Bigger && ABTestLoader.Instance.rebornBoostersConfig.Version > 1)
                continue;

            if (_powerups[i] == PowerUpType.MoreTime && !BaseGameManager.Instance.IsTimeBasedMode())
                continue;

            if (ABTestLoader.Instance.upgradablePowerupsConfig.EnabledPowerups[i] == 1)
                availablePowerups.Add(i);
        }

        var index = availablePowerups[UnityEngine.Random.Range(0, availablePowerups.Count)];
        // var index = 4;
        _powerUpType = _powerups[index];
        _powerUpGameObjects[index].SetActive(true);

        _rvIcon.SetActive(FeatureManager.Config.ShowFSOnPowerupPickup);

        if (ABTestLoader.Instance.upgradablePowerupsConfig.EnhanceVisibility)
        {
            _groundParticle[0].SetActive(false);
            _groundParticle[1].SetActive(true);
        }
        else
        {
            _groundParticle[0].SetActive(true);
            _groundParticle[1].SetActive(false);
        }

        transform.localScale = _startSize * Vector3.one;

        UpdateSize(BaseGameManager.Instance.CurrentHole.Size);
        
        if (_powerUpIndexOverride >= 0)
            DebugChangePowerup(_powerUpIndexOverride);
    }

    private void OnPackEaten(Hole hole)
    {
        if (FeatureManager.Config.ShowFSOnPowerupPickup)
        {
            VoodooSauce.ShowInterstitial(null, true, "in_game_powerup_" + _powerUpType);
        }

        SetPowerUp();
        UpgradablePowerupsController.PowerupCollected(_powerUpType, MapManager.Instance.CurrentMap.id);
    }

    private void SetPowerUp()
    {
        PowerUpManager.CurrentPowerUp = _powerUpType;

        switch (_powerUpType)
        {
            case PowerUpType.Bigger:
                if (FeatureManager.Config.UpgradablePowerups.IsEnabled && ServiceLocator.GetGameCount() >=
                    ABTestLoader.Instance.upgradablePowerupsConfig.FirstTimeAppearance)
                {
                    BaseGameManager.Instance.CurrentHole.SetLevel(BaseGameManager.Instance.CurrentHole.Level +
                                                                  UpgradablePowerupsController
                                                                      .GetPowerUp(PowerUpType.Bigger).GetCurrentStat());
                    BaseGameManager.Instance.CurrentHole.references.powerUpBigger.Initialize();
                }
                else
                {
                    BaseGameManager.Instance.CurrentHole.SetLevel(BaseGameManager.Instance.CurrentHole.Level + 1);
                }

                break;
            case PowerUpType.Magnet:
                BaseGameManager.Instance.CurrentHole.references.powerUpMagnet.Initialize();
                break;
            case PowerUpType.Shrink:
                BaseGameManager.Instance.CurrentHole.references.powerUpShrink.Initialize();
                break;
            case PowerUpType.Speed:
                BaseGameManager.Instance.CurrentHole.references.powerUpSpeed.Initialize();
                break;
            case PowerUpType.MoreTime:
                BaseGameManager.Instance.CurrentHole.references.powerupMoreTime.Initialize();
                break;
        }
    }

    private void OnDestroy()
    {
        // transform.DOKill();

        if (_fallingObject != null)
        {
            _fallingObject.Eaten -= OnPackEaten;
        }

        if (CanBeInitialized)
        {
            BaseGameManager.Instance.CurrentHole.SizeChanged -= HandleOnSizeChanged;
        }
    }

    private void HandleOnSizeChanged(float size)
    {
        UpdateSize(size, true);
    }

    private void UpdateSize(float _Size, bool _Animate = false)
    {
        float size = Mathf.Clamp(_Size * _sizeMultiplier, _startSize, _maxSize);
        if (_Animate == false)
        {
            transform.localScale = size * Vector3.one;
            OnSizeChanged?.Invoke(size);
            return;
        }

        transform.DOKill();
        transform.DOScale(size, 1f).SetEase(Ease.OutBack).OnComplete(() => { OnSizeChanged?.Invoke(size); });
    }
}