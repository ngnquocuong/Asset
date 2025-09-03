using UnityEngine;
using DG.Tweening;

public class PickableCurrencyAnimation : MonoBehaviour
{
    void Start()
    {
        transform.DOKill();
        transform.DORotate(Vector3.down * 360, 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
}
