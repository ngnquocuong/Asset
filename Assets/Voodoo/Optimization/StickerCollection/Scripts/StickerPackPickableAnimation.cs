using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StickerPackPickableAnimation : MonoBehaviour
{
    [SerializeField] private bool m_LaunchAtStart = true;

    private Sequence m_Sequence;

    void Start()
    {
        if (m_LaunchAtStart)
            Animate();
    }

    public void Animate()
    {
        m_Sequence?.Kill();
        m_Sequence = DOTween.Sequence();

        m_Sequence.Append(transform
            .DOLocalRotate(transform.localEulerAngles + Vector3.up * 360, 6, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental));
        m_Sequence.Join(transform.DOLocalMoveY(transform.localPosition.y - 0.3f, 3f).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo));
    }
}