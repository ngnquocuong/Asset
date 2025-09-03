using System;
using System.Collections;
using DG.Tweening;
using Game.Maps;
using UnityEngine;
using Voodoo.Optimisation.StickerCollections;

public class InGameStickerPack : MonoBehaviour
{

    private FallingObjectBehaviour _fallingObject;

    private bool CanBeInitialized => BaseGameManager.Instance && BaseGameManager.Instance.CurrentHole;

    private float _maxSize = 20;
    private float _startSize = 3;
    private float _sizeMultiplier = .5f;

    Rigidbody _rigidbody;



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

    private void Initialize()
    {
        _fallingObject = GetComponent<FallingObjectBehaviour>();
        _fallingObject.Eaten += OnPackEaten;

        BaseGameManager.Instance.CurrentHole.SizeChanged += OnSizeChanged;

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnPackEaten(Hole hole)
    {
        StickerCollectionInGameUI.Instance.Show();
        StickerCollectionsController.Instance.CollectStickersPack(MapManager.Instance.CurrentMap.id);
        _rigidbody.DOMoveY(-20, 1);
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
            BaseGameManager.Instance.CurrentHole.SizeChanged -= OnSizeChanged;
        }
    }

    private void OnSizeChanged(float size)
    {
        transform.DOKill();
        transform.DOScale(Mathf.Clamp(size * _sizeMultiplier, 3, _maxSize), 1f).SetEase(Ease.OutBack);
    }
}
