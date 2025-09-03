using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Choisit une animation de marche au hasard
/// </summary>
public class MovableRandomWalk : MonoBehaviour
{
    [System.Serializable]
    public class WalkAnimation
    {
        public RuntimeAnimatorController animation;
        public float minSpeed;
        public float maxSpeed;
        public bool canBeNegativeSpeed = true;
    }

    #region REFERENCES

    [SerializeField] public EditorReferences references = new EditorReferences();
    [System.Serializable]
    public class EditorReferences
    {
        public WalkAnimation[] walkAnimations;

        public float minXOffset;
        public float maxXOffset;

        public WalkAnimation GetRandomAnimation()
        {
            var x = Random.Range(0, walkAnimations.Length);
            return walkAnimations[x];
        }
    }

    #endregion

    private bool _isInitialized = false;
    private bool _isWalking = false;

    private void Start()
    {
        _isInitialized = true;
        _isWalking = false;

        SetRandomWalk();
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            return;
        }

        SetRandomWalk();
    }

    private void OnDisable()
    {
        _isWalking = false;
    }

    private void SetRandomWalk()
    {
        if (_isWalking)
            return;

        WalkAnimation randomAnimation = references.GetRandomAnimation();

        MovableAnimation ma = GetComponent<MovableAnimation>();
        MovableSplineAdapter msa = GetComponent<MovableSplineAdapter>();

        if (Random.value < 0.5f || randomAnimation.canBeNegativeSpeed == false)
            msa.SetSpeed(Random.Range(randomAnimation.minSpeed, randomAnimation.maxSpeed));
        else
            msa.SetSpeed(-Random.Range(randomAnimation.minSpeed, randomAnimation.maxSpeed));

        msa.SetXOffset(Random.Range(references.minXOffset, references.maxXOffset));

        ma.SetAnimation(randomAnimation.animation);

        _isWalking = true;
    }
}
