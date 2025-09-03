using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InGameGemAnimation : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalRotate(transform.localEulerAngles + Vector3.up * 360, 6, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        transform.DOLocalMoveY(transform.localPosition.y - 0.3f, 3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
